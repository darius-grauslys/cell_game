using cell_game.Gameplay;
using cell_game.Gameplay.AI;
using cell_game.UI;
using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Scenes
{
    public enum MapType
    {
        Small=0,  //20x10
        Medium=1, //30x20
        Large=2   //40x30
    }

    public class GameCreationScene : ChildScene
    {
        private static readonly int MAX_PLAYERS = 6;

        TextSelect textSelect;
        TextDisplayer textDisplayer;

        int humanPlayers = 1;
        int aiPlayers = 1;
        MapType mapType;

        string humanField = "Human Player Count: ", aiField = "AI Player Count: ", mapField = "Map Size: ", beginField = "Begin";
        string[] names = new string[] { "blue", "red", "green", "purple", "orange", "pink" };
        string[] sizes = new string[] { "20x10", "40x20", "70x40" };

        ControlScene controlScene;

        public GameCreationScene(Game game, ControlScene controlScene) 
            : base(game)
        {
            this.controlScene = controlScene;
            textSelect = new TextSelect(new TextOption[]
            {
                new TextOption("Human Player Count: 1", TickHumanPlayerCount),
                new TextOption("AI Player Count: 1", TickAIPlayerCount),
                new TextOption("Map Size: 20x10", TickMapSize),
                new TextOption("Begin", BeginGame)
            });
            
            textDisplayer = game.GetSystem<TextDisplayer>();

            InputHandler.DeclareKeySwitch(Key.Up);
            InputHandler.DeclareKeySwitch(Key.Down);
            InputHandler.DeclareKeySwitch(Key.Left);
            InputHandler.DeclareKeySwitch(Key.Right);
            InputHandler.DeclareKeySwitch(Key.Enter);
        }

        private void BeginGame()
        {
            List<Player> players = new List<Player>(humanPlayers + aiPlayers);

            for (int i = 0; i < humanPlayers; i++)
                players.Add(new Player(names[i], i+1));
            for (int i = 0; i < aiPlayers; i++)
                players.Add(new Player(names[i + humanPlayers], i + humanPlayers+1, new RandomAI()));

            int width, height;

            switch (mapType)
            {
                case MapType.Small:
                default:
                    width = 20;
                    height = 10;
                    break;
                case MapType.Medium:
                    width = 40;
                    height = 20;
                    break;
                case MapType.Large:
                    width = 70;
                    height = 40;
                    break;
            }

            controlScene.BeginGame(players, width, height);
        }

        private void TickMapSize()
        {
            textSelect.Options[2].option = mapField + sizes[(int)mapType];
        }

        private void TickAIPlayerCount()
        {
            textSelect.Options[1].option = aiField + aiPlayers;
        }

        private void TickHumanPlayerCount()
        {
            textSelect.Options[0].option = humanField + humanPlayers;
        }

        public override void UpdateFrame(FrameArgument e)
        {
            if (InputHandler.Keyboard_SwitchState_BoolReset(Key.Up))
            {
                textSelect.OffsetIndex(-1);
            }
            if (InputHandler.Keyboard_SwitchState_BoolReset(Key.Down))
            {
                textSelect.OffsetIndex(1);
            }
            if (InputHandler.Keyboard_SwitchState_BoolReset(Key.Left))
            {
                switch (textSelect.Index)
                {
                    case 0:
                        OffsetPlayerCount(-1, aiPlayers, ref humanPlayers);
                        TickHumanPlayerCount();
                        break;
                    case 1:
                        OffsetPlayerCount(-1, humanPlayers, ref aiPlayers);
                        TickAIPlayerCount();
                        break;
                    case 2:
                        mapType = (MapType)(((int)mapType + 2) % 3);
                        TickMapSize();
                        break;
                }
            }
            if (InputHandler.Keyboard_SwitchState_BoolReset(Key.Right))
            {
                switch (textSelect.Index)
                {
                    case 0:
                        OffsetPlayerCount(1, aiPlayers, ref humanPlayers);
                        TickHumanPlayerCount();
                        break;
                    case 1:
                        OffsetPlayerCount(1, humanPlayers, ref aiPlayers);
                        TickAIPlayerCount();
                        break;
                    case 2:
                        mapType = (MapType)(((int)mapType + 1) % 3);
                        TickMapSize();
                        break;
                }
            }

            if (InputHandler.Keyboard_SwitchState_BoolResetFree(Key.Enter))
            {
                textSelect.SelectOption();
            }

            base.UpdateFrame(e);
        }

        public override void RenderFrame(RenderService renderService, FrameArgument e)
        {
            textSelect.Draw(renderService, textDisplayer, 0, 0);

            base.RenderFrame(renderService, e);
        }

        private void OffsetPlayerCount(int offset, int opposing, ref int count)
        {
            if (count + offset + opposing <= MAX_PLAYERS && count + offset >= 0)
                count += offset;
        }
    }
}
