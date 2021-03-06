using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using isometricgame.GameEngine.Tools;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Scenes
{
    public class TutorialScene : ChildScene
    {
        private TextDisplayer textDisplayer;
        
        private Timer timer;
        private ControlScene controlScene;

        private string tutorialText_1 = 
            "Each player begins the game in random positions.\n" +
            "At the beginning of each turn 2 dice are rolled \n" +
            "with the lowest number rolled being the number  \n" +
            "of cells the active player can place that turn. \n" +
            "Cells can only be placed adjacent to existing   \n" +
            "cells that player controls. Use the arrow keys  \n" +
            "to place cells. After all available cells have  \n" +
            "been placed positions encircled by cells are    \n" +
            "captured and bonus moves are given.\n\n" +
            "Press any key to continue tutorial...";

        private string tutorialText_2 =
            "Capturing any number of  \n" +
            "enemy cells grants an additional dual dice roll \n" +
            "of bonus moves. Additionally, every 9 empty     \n" +
            "squares that is captured grants 1 jumper cell.   \n" +
            "A jumper cell can be placed up to two cells away\n" +
            "from an existing cell. To switch between jumper \n" +
            "and normal cells press T.\n\n" +
            "Press any key to finish the tutorial...";

        private string displayText;

        bool page2 = false;

        public TutorialScene(Game game, ControlScene controlScene) : base(game)
        {
            this.controlScene = controlScene;
            textDisplayer = game.GetSystem<TextDisplayer>();
            timer = new Timer(1.5f);
            timer.Set();

            displayText = tutorialText_1;
        }

        public override void UpdateFrame(FrameArgument e)
        {
            timer.DeltaTime((float)e.DeltaTime);
            if (timer.Finished)
            {
                if (InputHandler.Keyboard_UpDown != null)
                {
                    KeyboardState keyboard = InputHandler.Keyboard_UpDown.Keyboard;
                    if (keyboard.IsAnyKeyDown)
                    {
                        timer.Set();
                        if (page2)
                        {
                            displayText = tutorialText_1;
                            page2 = false;
                            controlScene.TransitionScene(0);
                        }
                        else
                        {
                            displayText = tutorialText_2;
                            page2 = true;
                        }
                    }

                }
            }
            base.UpdateFrame(e);
        }

        public override void RenderFrame(RenderService renderService, FrameArgument e)
        {
            textDisplayer.DrawText(renderService, displayText, "font", -580, 400);

            base.RenderFrame(renderService, e);
        }
    }
}
