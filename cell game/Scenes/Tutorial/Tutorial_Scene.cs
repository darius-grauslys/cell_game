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
    public class Tutorial_Scene : Scene
    {
        private readonly Tutorial_Scene_Layer Tutorial_Scene__SCENE_LAYER;
        
        public Tutorial_Scene(Game game) : base(game)
        {
            Tutorial_Scene__SCENE_LAYER = new Tutorial_Scene_Layer(this);
            
            AddLayer(Tutorial_Scene__SCENE_LAYER);
        }
    }
}
