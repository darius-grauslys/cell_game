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

namespace cell_game
{
    public class CellGame : Game
    {
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

            TextDisplayer.LoadFont("font", SpriteLibrary.GetSprite("font"));

            //List<Player> players = new List<Player>()
            //{
            //    new Player("blue", 1, new RandomAI(6)),
            //    new Player("red", 2, new RandomAI(1)),
            //    new Player("green", 3, new RandomAI(2)),
            //    new Player("purple", 4, new RandomAI(3)),
            //    new Player("orange", 5, new RandomAI(4)),
            //    new Player("pink", 6)
            //};
            
            //SetScene(new GameScene(this, players));

            SetScene(new ControlScene(this));

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
            SpriteLibrary.RecordSprite(
                AssetProvider.ExtractSpriteSheet(
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
