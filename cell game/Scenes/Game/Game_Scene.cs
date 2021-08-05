using cell_game.Gameplay;
using cell_game.Gameplay.AI;
using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Rendering;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using isometricgame.GameEngine.Tools;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isometricgame.GameEngine.Systems;

namespace cell_game.Scenes
{
    public class Game_Scene : Scene
    {
        private readonly InputHandler Cell_Game__InputHandler_Reference;
        private readonly SceneManagementService Cell_Game__Scene_Management_Service__Reference;

        private readonly Game_Scene_Layer Game_Scene__SCENE_LAYER;

        public void Set__Level_Data__Game_Scene(List<Player> players, int width, int height)
            => Game_Scene__SCENE_LAYER.Set__Level_Data__Game_Scene_Layer(players, width, height);
        
        public Game_Scene(CellGame game) 
            : base(game)
        {
            Game_Scene__SCENE_LAYER = new Game_Scene_Layer(this);
            
            AddLayer(Game_Scene__SCENE_LAYER);
            
            Cell_Game__InputHandler_Reference = game.Cell_Game__Input_Handler;
        }
    }
}
