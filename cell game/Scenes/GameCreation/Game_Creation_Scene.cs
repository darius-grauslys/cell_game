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
using isometricgame.GameEngine.Systems;

namespace cell_game.Scenes
{
    public enum MapType
    {
        Small=0,  //20x10
        Medium=1, //30x20
        Large=2   //40x30
    }

    public class Game_Creation_Scene : Scene
    {
        private readonly Game_Creation_Scene_Layer Game_Creation_Scene__SCENE_LAYER;
        
        public Game_Creation_Scene(Game game) 
            : base(game)
        {
            Game_Creation_Scene__SCENE_LAYER = new Game_Creation_Scene_Layer(this);
            
            AddLayer(Game_Creation_Scene__SCENE_LAYER);
        }
    }
}
