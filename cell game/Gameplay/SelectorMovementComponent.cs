using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Systems.Input;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay
{
    public class SelectorMovementComponent : GameComponent
    {
        InputHandler inputHandler;

        private int limitX, limitY;
        private int offsetX, offsetY;
        private int x=0, y=0;

        public int X => x;
        public int Y => y;

        private double timeDelay = 0.1;
        private double timeCounter;

        public SelectorMovementComponent(InputHandler inputHandler, int offsetX, int offsetY, int limitX, int limitY)
        {
            this.inputHandler = inputHandler;
            SetBounds(offsetX, offsetY, limitX, limitY);
        }

        public void SetBounds(int offsetX, int offsetY, int limitX, int limitY)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.limitX = limitX;
            this.limitY = limitY;
        }

        protected override void OnUpdate(FrameArgument args)
        {
            if (inputHandler.Keyboard_UpDown == null)
                return;
            KeyboardState keyboard = inputHandler.Keyboard_UpDown.Keyboard;

            if (timeCounter > 0)
            {
                timeCounter -= args.DeltaTime;
                return;
            }
            if (keyboard.IsAnyKeyDown)
                timeCounter = timeDelay;

            if (keyboard.IsKeyDown(Key.Up))
            {
                if (y < limitY-1)
                    y++;
            }
            if (keyboard.IsKeyDown(Key.Down))
            {
                if (y > 0)
                    y--;
            }
            if (keyboard.IsKeyDown(Key.Right))
            {
                if (x < limitX-1)
                    x++;
            }
            if (keyboard.IsKeyDown(Key.Left))
            {
                if (x > 0)
                    x--;
            }

            SetPositionByGridIndex(x, y);
            base.OnUpdate(args);
        }

        public void SetPositionByGridIndex(int x, int y)
        {
            ParentObject.Position = new Vector3((x + offsetX) * 16, (y + offsetY) * 16, 0);
        }
    }
}
