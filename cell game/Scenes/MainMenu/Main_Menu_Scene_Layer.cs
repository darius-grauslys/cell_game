using cell_game.UI;
using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using OpenTK.Input;

namespace cell_game.Scenes
{
    public class Main_Menu_Scene_Layer : Scene_Layer
    {
        private readonly string Main_Menu_Scene_Layer__Title_String_Field = "Cells";
        private readonly string Main_Menu_Scene_Layer__Selection_Hint_String_Field 
            = "Use the up and down arrows to select, and enter to choose.";
        private readonly string Main_Menu_Scene_Layer__Credits_String_Field 
            = "A game by Darius Grauslys";
        private TextSelect Main_Menu_Scene_Layer__Text_Select;

        private readonly TextDisplayer Cell_Game__TEXT_DISPLAYER__Reference;

        private readonly SceneManagementService Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference;

        private readonly InputHandler Cell_Game__INPUTHANDLER__Reference;
        
        private readonly CellGame CELL_GAME__Reference;

        public Main_Menu_Scene_Layer
            (Scene sceneLayerParentScene) : base(sceneLayerParentScene)
        {
            CELL_GAME__Reference = sceneLayerParentScene.Game as CellGame;

            Cell_Game__TEXT_DISPLAYER__Reference = CELL_GAME__Reference.Game__Text_Displayer;
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference = CELL_GAME__Reference.Game__Scene_Management_Service;
            Cell_Game__INPUTHANDLER__Reference = CELL_GAME__Reference.Cell_Game__Input_Handler;
            
            Main_Menu_Scene_Layer__Text_Select = new TextSelect(new TextOption[]
            {
                new TextOption("New Game", EnterScene_GameCreation),
                new TextOption("How to Play", EnterScene_GameTutorial),
                new TextOption("Quit Game", QuitGame)
            });
        }
        
        private void EnterScene_GameCreation()
        {
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference.SetScene(CellGame.SCENE_TAG__GAME_CREATION_MENU);
        }

        private void EnterScene_GameTutorial()
        {
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference.SetScene(CellGame.SCENE_TAG__TUTORIAL_SCENE);
        }

        private void QuitGame()
        {
            CELL_GAME__Reference.Close();
        }

        protected override void Handle_Update__Scene_Layer(FrameArgument e)
        {
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Up.ToString()))
            {
                Main_Menu_Scene_Layer__Text_Select.OffsetIndex(-1);
            }
            else if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Down.ToString()))
            {
                Main_Menu_Scene_Layer__Text_Select.OffsetIndex(1);
            }
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.Enter.ToString()))
            {
                Main_Menu_Scene_Layer__Text_Select.SelectOption();
            }
            
            base.Handle_Update__Scene_Layer(e);
        }

        protected override void Handle_Render__Scene_Layer(RenderService renderService, FrameArgument e)
        {
            Cell_Game__TEXT_DISPLAYER__Reference.DrawText
            (
                renderService,
                Main_Menu_Scene_Layer__Title_String_Field,
                "font", 
                -18 * Main_Menu_Scene_Layer__Title_String_Field.Length / 2, 
                200
            );
            
            Cell_Game__TEXT_DISPLAYER__Reference.DrawText
            (
                renderService, 
                Main_Menu_Scene_Layer__Selection_Hint_String_Field, 
                "font", 
                -18 * Main_Menu_Scene_Layer__Selection_Hint_String_Field.Length / 2, 
                100
            );

            Main_Menu_Scene_Layer__Text_Select.Draw(renderService, Cell_Game__TEXT_DISPLAYER__Reference, 0, 0);

            Cell_Game__TEXT_DISPLAYER__Reference.DrawText
            (
                renderService,
                Main_Menu_Scene_Layer__Credits_String_Field, 
                "font", 
                550 - (Main_Menu_Scene_Layer__Credits_String_Field.Length * 18), 
                -420
            );
            
            base.Handle_Render__Scene_Layer(renderService, e);
        }
    }
}