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
    public class Main_Menu_Scene : Scene
    {
        private readonly Main_Menu_Scene_Layer Main_Menu_Scene__SCENE_LAYER;
        
        public Main_Menu_Scene(CellGame game) 
            : base(game)
        {
            Main_Menu_Scene__SCENE_LAYER = new Main_Menu_Scene_Layer(this);
            
            AddLayer(Main_Menu_Scene__SCENE_LAYER);
        }
    }
}
