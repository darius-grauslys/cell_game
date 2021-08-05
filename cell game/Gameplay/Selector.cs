using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using OpenTK;
using OpenTK.Input;
using cell_game.Scenes;

namespace cell_game.Gameplay
{
    public class Selector : World_Space_GameObject
    {
        private bool playerControlled = true;
        private Level_Data gameLevelData;
        SelectorMovementComponent movement;
        InputHandler Cell_Game__INPUTHANDLER__Reference;
        public bool placingNormals = true;

        public bool placementSuccess = true;

        private double timeCounter, timeDelay = 0.1, aiDelay = 0.01;

        private GameAction[] aiMoves = new GameAction[0];
        private int moveIndex = -1;

        public Selector(Game_Scene_Layer sceneLayer, Level_Data gameLevelData) 
            : 
            base
            (
                sceneLayer, 
                new Vector3(0,0,0), 
                new SelectorMovementComponent(null, 0, 0, 5, 5)
            )
        {
            GameObject_World__Sprite_Render.Set__Sprite__Sprite_Render
            (
                sceneLayer.Scene_Layer__Game.Get_System__Game<SpriteLibrary>().ExtractRenderUnit("selector")
            );

            Cell_Game__INPUTHANDLER__Reference = sceneLayer.CELL_GAME__Reference.Cell_Game__Input_Handler;

            movement = Get__Component__GameObject<SelectorMovementComponent>();
            movement.Cell_Game__InputHandler__Reference = Cell_Game__INPUTHANDLER__Reference;
            
            SetLevel(gameLevelData);
        }

        public void SetLevel(Level_Data gameLevelData)
        {
            this.gameLevelData = gameLevelData;

            int offsetX = gameLevelData.offset.X / 16;
            int offsetY = gameLevelData.offset.Y / 16;

            int limitX = gameLevelData.width;
            int limitY = gameLevelData.height;

            Position = new Vector3(offsetX * 16, offsetY * 16, 0);
            movement.SetBounds(offsetX, offsetY, limitX, limitY);
        }

        public override void OnUpdate(FrameArgument args)
        {
            if (gameLevelData.gameStarted)
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
                        if (Cell_Game__INPUTHANDLER__Reference.Keyboard_UpDown != null)
                        {
                            KeyboardState keyboard = Cell_Game__INPUTHANDLER__Reference.Keyboard_UpDown.Keyboard;

                            placingNormals = !Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState(Key.T.ToString());

                            bool canPlace = true;

                            if (placingNormals)
                                canPlace = (gameLevelData.activePlayer.normalCellPlacementCount > 0);
                            else
                                canPlace = (gameLevelData.activePlayer.jumperCellPlacementCount > 0);

                            if (canPlace && keyboard.IsKeyDown(Key.Space) && gameLevelData.activePlayer.TotalMoves > 0)
                            {
                                int range = (placingNormals) ? 1 : 3;
                                placementSuccess = gameLevelData.PlaceCell(movement.X, movement.Y, gameLevelData.activePlayer.id, range);

                                if (placementSuccess)
                                {
                                    if (placingNormals)
                                        gameLevelData.activePlayer.normalCellPlacementCount--;
                                    else
                                        gameLevelData.activePlayer.jumperCellPlacementCount--;
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
                                gameLevelData.activePlayer.turnOver = true;
                                return;
                            }

                            movement.SetPositionByGridIndex(aiMoves[moveIndex].position.X, aiMoves[moveIndex].position.Y);

                            placingNormals = aiMoves[moveIndex].gameAction == GameActions.PlaceNormal;
                            int range = (placingNormals) ? 1 : 3;
                            bool success = gameLevelData.PlaceCell(aiMoves[moveIndex].position.X, aiMoves[moveIndex].position.Y, gameLevelData.activePlayer.id, range);

                            if (success)
                            {
                                if (placingNormals)
                                    gameLevelData.activePlayer.normalCellPlacementCount--;
                                else
                                    gameLevelData.activePlayer.jumperCellPlacementCount--;
                                moveIndex++;
                            }
                        }
                        else
                        {
                            gameLevelData.activePlayer.turnOver = true;
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
