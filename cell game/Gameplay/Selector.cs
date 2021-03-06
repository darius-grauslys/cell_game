using isometricgame.GameEngine;
using isometricgame.GameEngine.Components.Rendering;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay
{
    public class Selector : GameObject
    {
        private bool playerControlled = true;
        private Level gameLevel;
        SelectorMovementComponent movement;
        InputHandler inputHandler;
        public bool placingNormals = true;

        public bool placementSuccess = true;

        private double timeCounter, timeDelay = 0.1, aiDelay = 0.01;

        private GameAction[] aiMoves = new GameAction[0];
        private int moveIndex = -1;

        public Selector(Scene scene, Level gameLevel) 
            : base(scene, new Vector3(0,0,0))
        {
            SpriteComponent = new SpriteComponent();
            SpriteComponent.SetSprite(scene.Game.GetSystem<SpriteLibrary>().GetSprite("selector"));

            inputHandler = scene.Game.GetSystem<InputSystem>().RegisterHandler(InputType.Keyboard_UpDown);
            inputHandler.DeclareKeySwitch(Key.T);

            AddComponent(movement = new SelectorMovementComponent(inputHandler, 0, 0, 5, 5));
            SetLevel(gameLevel);
        }

        public void SetLevel(Level gameLevel)
        {
            this.gameLevel = gameLevel;

            int offsetX = gameLevel.offset.X / 16;
            int offsetY = gameLevel.offset.Y / 16;

            int limitX = gameLevel.width;
            int limitY = gameLevel.height;

            Position = new Vector3(offsetX * 16, offsetY * 16, 0);
            movement.SetBounds(offsetX, offsetY, limitX, limitY);
        }

        public override void OnUpdate(FrameArgument args)
        {
            if (gameLevel.gameStarted)
            {
                if (timeCounter > 0)
                {
                    timeCounter -= args.DeltaTime;
                }

                if (timeCounter <= 0)
                {
                    timeCounter = timeDelay;
                    if (playerControlled)
                    {
                        if (inputHandler.Keyboard_UpDown != null)
                        {
                            KeyboardState keyboard = inputHandler.Keyboard_UpDown.Keyboard;

                            placingNormals = !inputHandler.Keyboard_SwitchState_Bool(Key.T);

                            bool canPlace = true;

                            if (placingNormals)
                                canPlace = (gameLevel.activePlayer.normalCellPlacementCount > 0);
                            else
                                canPlace = (gameLevel.activePlayer.jumperCellPlacementCount > 0);

                            if (canPlace && keyboard.IsKeyDown(Key.Space) && gameLevel.activePlayer.TotalMoves > 0)
                            {
                                int range = (placingNormals) ? 1 : 3;
                                placementSuccess = gameLevel.PlaceCell(movement.X, movement.Y, gameLevel.activePlayer.id, range);

                                if (placementSuccess)
                                {
                                    if (placingNormals)
                                        gameLevel.activePlayer.normalCellPlacementCount--;
                                    else
                                        gameLevel.activePlayer.jumperCellPlacementCount--;
                                }
                            }
                        }
                    }
                    else //AI moves
                    {
                        if (moveIndex > -1 && moveIndex < aiMoves.Length)
                        {
                            if (aiMoves[moveIndex].gameAction == GameActions.EndTurn)
                            {
                                gameLevel.activePlayer.turnOver = true;
                                return;
                            }

                            movement.SetPositionByGridIndex(aiMoves[moveIndex].position.X, aiMoves[moveIndex].position.Y);

                            placingNormals = aiMoves[moveIndex].gameAction == GameActions.PlaceNormal;
                            int range = (placingNormals) ? 1 : 3;
                            bool success = gameLevel.PlaceCell(aiMoves[moveIndex].position.X, aiMoves[moveIndex].position.Y, gameLevel.activePlayer.id, range);

                            if (success)
                            {
                                if (placingNormals)
                                    gameLevel.activePlayer.normalCellPlacementCount--;
                                else
                                    gameLevel.activePlayer.jumperCellPlacementCount--;
                                moveIndex++;
                            }
                        }
                        else
                        {
                            gameLevel.activePlayer.turnOver = true;
                        }
                    }
                }
            }

            base.OnUpdate(args);
        }

        public void SetAIMoves(GameAction[] actions)
        {
            aiMoves = actions;
            moveIndex = 0;
        }

        public void ReleasePlayerControl()
        {
            playerControlled = false;
            timeDelay = aiDelay;
            movement.Toggle(playerControlled);
        }

        public void GivePlayerControl()
        {
            playerControlled = true;
            timeDelay = 0.1;
            movement.Toggle(playerControlled);
        }
    }
}
