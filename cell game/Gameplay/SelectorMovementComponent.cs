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
using isometricgame.GameEngine.Scenes.Components;

namespace cell_game.Gameplay
{
    public class SelectorMovementComponent : GameObject_Component
    {
        internal InputHandler Cell_Game__InputHandler__Reference { get; set; }
        
        private Transform_Component Attached_GameObject__Transform_Component__Reference { get; set; }
        
        private int limitX, limitY;
        private int offsetX, offsetY;
        private int x=0, y=0;

        public int X => x;
        public int Y => y;

        private double timeDelay = 0.1;
        private double timeCounter;

        private bool enabled = true;
        public void Toggle(bool state)
            => enabled = state;
        
        public SelectorMovementComponent(InputHandler inputHandler, int offsetX, int offsetY, int limitX, int limitY)
        {
            this.Cell_Game__InputHandler__Reference = inputHandler;
            SetBounds(offsetX, offsetY, limitX, limitY);
        }

        public void SetBounds(int offsetX, int offsetY, int limitX, int limitY)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.limitX = limitX;
            this.limitY = limitY;
        }

        protected override void Handle_Attach_To__GameObject__Component()
        {
            Attached_GameObject__Transform_Component__Reference
                = Component__Attached_GameObject.Get__Component__GameObject<Transform_Component>();

            base.Handle_Attach_To__GameObject__Component();
        }

        protected override void Handle__Update__Component(FrameArgument args)
        {
            if (Cell_Game__InputHandler__Reference.Keyboard_UpDown == null || !enabled)
                return;
            KeyboardState keyboard = Cell_Game__InputHandler__Reference.Keyboard_UpDown.Keyboard;

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
            base.Handle__Update__Component(args);
        }

        public void SetPositionByGridIndex(int x, int y)
        {
            Attached_GameObject__Transform_Component__Reference.Position 
                = new Vector3((x + offsetX) * 16, (y + offsetY) * 16, 0);
        }
    }
}
