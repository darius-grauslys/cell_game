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
    public class MainMenuScene : ChildScene
    {
        private readonly string title = "Cells";
        private readonly string selectionHint = "Use the up and down arrows to select, and enter to choose.";
        private readonly string credit = "A game by Darius Grauslys";
        private TextSelect textSelect;

        private TextDisplayer textDisplayer;

        private ControlScene controlScene;

        private Game game;

        public MainMenuScene(Game game, ControlScene controlScene) 
            : base(game)
        {
            this.game = game;
            this.controlScene = controlScene;

            textSelect = new TextSelect(new TextOption[]
            {
                new TextOption("New Game", EnterScene_GameCreation),
                new TextOption("How to Play", EnterScene_GameTutorial),
                new TextOption("Quit Game", QuitGame)
            });

            textDisplayer = game.GetSystem<TextDisplayer>();

            InputHandler.DeclareKeySwitch(Key.Up);
            InputHandler.DeclareKeySwitch(Key.Down);
            InputHandler.DeclareKeySwitch(Key.Enter);
        }

        public override void UpdateFrame(FrameArgument e)
        {
            if (InputHandler.Keyboard_SwitchState_BoolReset(Key.Up))
            {
                textSelect.OffsetIndex(-1);
            }
            else if (InputHandler.Keyboard_SwitchState_BoolReset(Key.Down))
            {
                textSelect.OffsetIndex(1);
            }
            if (InputHandler.Keyboard_SwitchState_BoolResetFree(Key.Enter))
            {
                textSelect.SelectOption();
            }

            base.UpdateFrame(e);
        }

        public override void RenderFrame(RenderService renderService, FrameArgument e)
        {
            textDisplayer.DrawText(renderService, title, "font", -18 * title.Length / 2, 200);
            textDisplayer.DrawText(renderService, selectionHint, "font", -18 * selectionHint.Length / 2, 100);

            textSelect.Draw(renderService, textDisplayer, 0, 0);

            textDisplayer.DrawText(renderService, credit, "font", 550 - (credit.Length * 18), -420);

            base.RenderFrame(renderService, e);
        }

        private void EnterScene_GameCreation()
        {
            controlScene.TransitionScene(1);
        }

        private void EnterScene_GameTutorial()
        {
            controlScene.TransitionScene(2);
        }

        private void QuitGame()
        {
            game.Close();
        }
    }
}
