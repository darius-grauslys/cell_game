using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Scenes
{
    public class ChildScene : Scene
    {
        protected readonly InputHandler InputHandler;
        
        public ChildScene(Game game) 
            : base(game)
        {
            InputHandler = game.GetSystem<InputSystem>().RegisterHandler(InputType.Keyboard_UpDown);
        }

        public void EnterScene()
        {
            InputHandler.Enabled = true;
        }

        public void ExitScene()
        {
            InputHandler.Enabled = false;
        }
    }
}
