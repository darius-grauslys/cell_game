using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using isometricgame.GameEngine.Tools;
using OpenTK.Input;

namespace cell_game.Scenes
{
    public class Tutorial_Scene_Layer : Scene_Layer
    {
        private readonly TextDisplayer Cell_Game__TEXT_DISPLAYER__Reference;
        
        private readonly SceneManagementService Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference;

        private readonly InputHandler Cell_Game__INPUTHANDLER_Reference;
        
        private Timer Tutorial_Scene_Layer__Timer;
        
        private string Tutorial_Scene_Layer__Page_One = 
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

        private string Tutorial_Scene_Layer__Page_Two =
            "Capturing any number of  \n" +
            "enemy cells grants an additional dual dice roll \n" +
            "of bonus moves. Additionally, every 9 empty     \n" +
            "squares that is captured grants 1 jumper cell.   \n" +
            "A jumper cell can be placed up to two cells away\n" +
            "from an existing cell. To switch between jumper \n" +
            "and normal cells press T.\n\n" +
            "Press any key to finish the tutorial...";

        private string Tutorial_Scene_Layer__Display_Text;

        private bool Tutorial_Scene_Layer__Is_On_Page_Two = false;
        
        public Tutorial_Scene_Layer
            (Scene sceneLayerParentScene) 
            : base(sceneLayerParentScene)
        {
            CellGame cellGame = sceneLayerParentScene.Game as CellGame;

            Cell_Game__TEXT_DISPLAYER__Reference = cellGame.Game__Text_Displayer;
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference = cellGame.Game__Scene_Management_Service;
            Cell_Game__INPUTHANDLER_Reference = cellGame.Cell_Game__Input_Handler;
            
            Tutorial_Scene_Layer__Timer = new Timer(1.5f);
            Tutorial_Scene_Layer__Timer.Set();

            Tutorial_Scene_Layer__Display_Text = Tutorial_Scene_Layer__Page_One;
        }

        protected override void Handle_Update__Scene_Layer(FrameArgument e)
        {
            
            Tutorial_Scene_Layer__Timer.Increase_DeltaTime((float)e.DeltaTime);
            if (Tutorial_Scene_Layer__Timer.Finished)
            {
                if (Cell_Game__INPUTHANDLER_Reference.Keyboard_UpDown != null)
                {
                    KeyboardState keyboard = Cell_Game__INPUTHANDLER_Reference.Keyboard_UpDown.Keyboard;
                    if (keyboard.IsAnyKeyDown)
                    {
                        Tutorial_Scene_Layer__Timer.Set();
                        if (Tutorial_Scene_Layer__Is_On_Page_Two)
                        {
                            Tutorial_Scene_Layer__Display_Text = Tutorial_Scene_Layer__Page_One;
                            Tutorial_Scene_Layer__Is_On_Page_Two = false;
                            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference.SetScene(CellGame.SCENE_TAG__MAIN_MENU);
                        }
                        else
                        {
                            Tutorial_Scene_Layer__Display_Text = Tutorial_Scene_Layer__Page_Two;
                            Tutorial_Scene_Layer__Is_On_Page_Two = true;
                        }
                    }

                }
            }
            
            base.Handle_Update__Scene_Layer(e);
        }

        protected override void Handle_Render__Scene_Layer(RenderService renderService, FrameArgument e)
        {
            Cell_Game__TEXT_DISPLAYER__Reference.DrawText(renderService, Tutorial_Scene_Layer__Display_Text, "font", -580, 400);
            
            base.Handle_Render__Scene_Layer(renderService, e);
        }
    }
}