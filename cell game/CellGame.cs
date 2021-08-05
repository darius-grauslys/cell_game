using cell_game.Gameplay;
using cell_game.Gameplay.AI;
using cell_game.Scenes;
using isometricgame.GameEngine;
using isometricgame.GameEngine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isometricgame.GameEngine.Systems.Input;
using OpenTK.Input;

namespace cell_game
{
    public class CellGame : Game
    {
        public const string SCENE_TAG__MAIN_MENU = "mainMenuScene";
        public const string SCENE_TAG__GAME_CREATION_MENU = "gameCreationMenuScene";
        public const string SCENE_TAG__GAME_SCENE = "gameScene";
        public const string SCENE_TAG__TUTORIAL_SCENE = "tutorialScene";
        
        public InputHandler Cell_Game__Input_Handler { get; private set; }
        
        public CellGame(string GAME_DIR = "", string GAME_DIR_ASSETS = "", string GAME_DIR_WORLDS = "") 
            : base(1200, 900, "Cell Game", GAME_DIR, GAME_DIR_ASSETS, GAME_DIR_WORLDS)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadSprite("empty.png", "empty cell", 16, 16);
            LoadSprite("blue cell.png", "blue cell", 16, 16);
            LoadSprite("red cell.png", "red cell", 16, 16);
            LoadSprite("green cell.png", "green cell", 16, 16);
            LoadSprite("purple cell.png", "purple cell", 16, 16);
            LoadSprite("orange cell.png", "orange cell", 16, 16);
            LoadSprite("pink cell.png", "pink cell", 16, 16);
            LoadSprite("selector.png", "selector", 16, 16);
            LoadSprite("gamefont.png", "font", 18, 28);

            Cell_Game__Input_Handler = Game__Input_System.RegisterHandler(InputType.Keyboard_UpDown);
            Cell_Game__Input_Handler.DeclareSwitch(Key.Up.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.Down.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.Left.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.Right.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.Enter.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.S.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.Escape.ToString());
            Cell_Game__Input_Handler.DeclareSwitch(Key.T.ToString());
            
            Game__Text_Displayer.LoadFont("font", (int)Game__Sprite_Library.GetSpriteID("font"));

            Game__Scene_Management_Service.AddScene(SCENE_TAG__MAIN_MENU, new Main_Menu_Scene(this));
            Game__Scene_Management_Service.AddScene(SCENE_TAG__GAME_SCENE, new Game_Scene(this));
            Game__Scene_Management_Service.AddScene(SCENE_TAG__GAME_CREATION_MENU, new Game_Creation_Scene(this));;
            Game__Scene_Management_Service.AddScene(SCENE_TAG__TUTORIAL_SCENE, new Tutorial_Scene(this));

            Game__Scene_Management_Service.SetScene(SCENE_TAG__MAIN_MENU);
            
            base.OnLoad(e);
        }

        private void LoadSprite(
            string assetName,
            string name = null,
            int width = -1,
            int height = -1,
            float offsetX = 0,
            float offsetY = 0,
            float r = 0,
            float g = 0,
            float b = 0,
            float a = 0f)
        {
            Game__Sprite_Library.RecordSprite(
                Game__Asset_Provider.ExtractSpriteSheet(
                    Path.Combine(GAME_DIRECTORY_ASSETS, assetName),
                    name,
                    width,
                    height,
                    offsetX,
                    offsetY,
                    r,
                    g,
                    b,
                    a));
        }
    }
}
