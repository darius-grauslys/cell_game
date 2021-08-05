using System.Collections.Generic;
using cell_game.Gameplay;
using cell_game.Gameplay.AI;
using cell_game.UI;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using OpenTK.Input;

namespace cell_game.Scenes
{
    public class Game_Creation_Scene_Layer : Scene_Layer
    {
        private const int MAX_PLAYERS = 6;

        private TextSelect Game_Creation_Scene_Layer__Text_Select;
        private TextDisplayer Cell_Game__TEXT_DISPLAYER__Reference;
        
        private InputHandler Cell_Game__INPUTHANDLER__Reference;

        private SceneManagementService Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference { get; set; }

        private Game_Scene GAME_SCENE__Reference;
        
        private int Game_Creation_Scene_Layer__Human_Player_Count = 1;
        private int Game_Creation_Scene_Layer__AI_Player_Count = 1;
        private MapType Game_Creation_Scene_Layer__Map_Type;

        private readonly string 
            Game_Creation_Scene_Layer__Human_String_Field = "Human Player Count: ", 
            Game_Creation_Scene_Layer__AI_String_Field = "AI Player Count: ", 
            Game_Creation_Scene_Layer__Map_Type_String_Field = "Map Size: ", 
            Game_Creation_Scene_Layer__Begin_String_Field = "Begin";
        private readonly string[] Game_Creation_Scene_Layer__Name_String_Fields 
            = new string[] { "blue", "red", "green", "purple", "orange", "pink" };
        private readonly string[] Game_Creation_Scene_Layer__Size_String_Fields 
            = new string[] { "20x10", "40x20", "70x40" };


        public Game_Creation_Scene_Layer
            (Scene sceneLayerParentScene) 
            : base(sceneLayerParentScene)
        {
            Game_Creation_Scene_Layer__Text_Select = new TextSelect(new TextOption[]
            {
                new TextOption("Human Player Count: 1", TickHumanPlayerCount),
                new TextOption("AI Player Count: 1", TickAIPlayerCount),
                new TextOption("Map Size: 20x10", TickMapSize),
                new TextOption("Begin", BeginGame)
            });

            CellGame cellGame = sceneLayerParentScene.Game as CellGame;
            
            Cell_Game__TEXT_DISPLAYER__Reference = cellGame.Get_System__Game<TextDisplayer>();

            Cell_Game__INPUTHANDLER__Reference = cellGame.Cell_Game__Input_Handler;
            
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference = cellGame.Get_System__Game<SceneManagementService>();

            GAME_SCENE__Reference = 
                Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference
                    .GetScene("gameScene") as Game_Scene;
        }

        private void BeginGame()
        {
            List<Player> players = new List<Player>(
                Game_Creation_Scene_Layer__Human_Player_Count + Game_Creation_Scene_Layer__AI_Player_Count
                );

            for (int i = 0; i < Game_Creation_Scene_Layer__Human_Player_Count; i++)
                players.Add
                (
                    new Player
                    (
                        Game_Creation_Scene_Layer__Name_String_Fields[i], (uint)i+1
                    )
                );
            
            for (int i = 0; i < Game_Creation_Scene_Layer__AI_Player_Count; i++)
                players.Add
                (
                    new Player
                    (
                        Game_Creation_Scene_Layer__Name_String_Fields[i + Game_Creation_Scene_Layer__Human_Player_Count], 
                        (uint)(i + Game_Creation_Scene_Layer__Human_Player_Count+1), 
                        new RandomAI()
                    )
                );

            int width, height;

            switch (Game_Creation_Scene_Layer__Map_Type)
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

            GAME_SCENE__Reference.Set__Level_Data__Game_Scene(players, width, height);
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference.SetScene(CellGame.SCENE_TAG__GAME_SCENE);
        }

        private void TickMapSize()
        {
            Game_Creation_Scene_Layer__Text_Select.Options[2].option = 
                Game_Creation_Scene_Layer__Map_Type_String_Field 
                + Game_Creation_Scene_Layer__Size_String_Fields[(int)Game_Creation_Scene_Layer__Map_Type];
        }

        private void TickAIPlayerCount()
        {
            Game_Creation_Scene_Layer__Text_Select.Options[1].option = 
                Game_Creation_Scene_Layer__AI_String_Field + Game_Creation_Scene_Layer__AI_Player_Count;
        }

        private void TickHumanPlayerCount()
        {
            Game_Creation_Scene_Layer__Text_Select.Options[0].option = 
                Game_Creation_Scene_Layer__Human_String_Field + Game_Creation_Scene_Layer__Human_Player_Count;
        }

        private void OffsetPlayerCount(int offset, int opposing, ref int count)
        {
            if (count + offset + opposing <= MAX_PLAYERS && count + offset >= 0)
                count += offset;
        }

        protected override void Handle_Update__Scene_Layer(FrameArgument e)
        {
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Up.ToString()))
            {
                Game_Creation_Scene_Layer__Text_Select.OffsetIndex(-1);
            }
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Down.ToString()))
            {
                Game_Creation_Scene_Layer__Text_Select.OffsetIndex(1);
            }
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Left.ToString()))
            {
                switch (Game_Creation_Scene_Layer__Text_Select.Index)
                {
                    case 0:
                        OffsetPlayerCount(-1, Game_Creation_Scene_Layer__AI_Player_Count, ref Game_Creation_Scene_Layer__Human_Player_Count);
                        TickHumanPlayerCount();
                        break;
                    case 1:
                        OffsetPlayerCount(-1, Game_Creation_Scene_Layer__Human_Player_Count, ref Game_Creation_Scene_Layer__AI_Player_Count);
                        TickAIPlayerCount();
                        break;
                    case 2:
                        Game_Creation_Scene_Layer__Map_Type = (MapType)(((int)Game_Creation_Scene_Layer__Map_Type + 2) % 3);
                        TickMapSize();
                        break;
                }
            }
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Right.ToString()))
            {
                switch (Game_Creation_Scene_Layer__Text_Select.Index)
                {
                    case 0:
                        OffsetPlayerCount(1, Game_Creation_Scene_Layer__AI_Player_Count, ref Game_Creation_Scene_Layer__Human_Player_Count);
                        TickHumanPlayerCount();
                        break;
                    case 1:
                        OffsetPlayerCount(1, Game_Creation_Scene_Layer__Human_Player_Count, ref Game_Creation_Scene_Layer__AI_Player_Count);
                        TickAIPlayerCount();
                        break;
                    case 2:
                        Game_Creation_Scene_Layer__Map_Type = (MapType)(((int)Game_Creation_Scene_Layer__Map_Type + 1) % 3);
                        TickMapSize();
                        break;
                }
            }

            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Enter.ToString()))
            {
                Game_Creation_Scene_Layer__Text_Select.SelectOption();
            }
            
            base.Handle_Update__Scene_Layer(e);
        }

        protected override void Handle_Render__Scene_Layer(RenderService renderService, FrameArgument e)
        {
            Game_Creation_Scene_Layer__Text_Select.Draw(renderService, Cell_Game__TEXT_DISPLAYER__Reference, 0, 0);
            
            base.Handle_Render__Scene_Layer(renderService, e);
        }
    }
}