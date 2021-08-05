using System;
using System.Collections.Generic;
using cell_game.Gameplay;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Rendering;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems;
using isometricgame.GameEngine.Systems.Input;
using isometricgame.GameEngine.Systems.Rendering;
using isometricgame.GameEngine.Tools;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;

namespace cell_game.Scenes
{
    public class Game_Scene_Layer : Scene_Layer
    {
        internal readonly CellGame CELL_GAME__Reference;
        private readonly InputHandler Cell_Game__INPUTHANDLER__Reference;
        private readonly SceneManagementService Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference; 
        
        private readonly Selector Game_Scene_Layer__SELECTOR;
        private Level_Data Game_Scene_Layer__Level_Data;
        private TextDisplayer Cell_Game__TEXT_DISPLAYER__Reference;
        
        private int old_turn = -1;
        private Timer endGameTimer = new Timer();
        
        public Game_Scene_Layer(Game_Scene sceneLayerParentScene) 
            : base(sceneLayerParentScene)
        {
            CELL_GAME__Reference = sceneLayerParentScene.Game as CellGame;
            Cell_Game__INPUTHANDLER__Reference = CELL_GAME__Reference.Cell_Game__Input_Handler;
            Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference =
                CELL_GAME__Reference.Get_System__Game<SceneManagementService>();
            
            Cell_Game__TEXT_DISPLAYER__Reference = CELL_GAME__Reference.Get_System__Game<TextDisplayer>();

            Game_Scene_Layer__Level_Data = new Level_Data(5, 5, new List<Player>(), new IntegerPosition(0, 0));
            
            Add__Scene_Object__Scene_Layer(Game_Scene_Layer__SELECTOR = new Selector(this, Game_Scene_Layer__Level_Data));
        }
        
        public void Set__Level_Data__Game_Scene_Layer(List<Player> players, int width, int height)
        {
            int offsetX = width / 2 * -16;
            int offsetY = (height / 2 * -16) - 96;
            Game_Scene_Layer__Level_Data = new Level_Data(width, height, players, new IntegerPosition(offsetX, offsetY));
            for (int i = 0; i < players.Count; i++)
                if (players[i].aiControlled)
                    players[i].playerAi.SetLevel(Game_Scene_Layer__Level_Data, players[i]);
            Game_Scene_Layer__SELECTOR.SetLevel(Game_Scene_Layer__Level_Data);
            endGameTimer.Set();
            old_turn = -1;
        }

        protected override void Handle_Gained_Focus__Scene_Layer()
        {
            if(!Game_Scene_Layer__Level_Data.gameStarted)
                Game_Scene_Layer__Level_Data.StartTurn();
            base.Handle_Gained_Focus__Scene_Layer();
        }

        protected override void Handle_Update__Scene_Layer(FrameArgument e)
        {
            if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState("Escape"))
                Game_Scene_Layer__Level_Data.gameOver = true;

            if (!Game_Scene_Layer__Level_Data.gameOver)
            {
                if (Game_Scene_Layer__Level_Data.gameStarted)
                {
                    if (Game_Scene_Layer__Level_Data.activePlayer.turnOver)
                        Game_Scene_Layer__Level_Data.StartTurn();

                    if (old_turn != Game_Scene_Layer__Level_Data.turnIndex)
                    {
                        old_turn = Game_Scene_Layer__Level_Data.turnIndex;
                        if (Game_Scene_Layer__Level_Data.activePlayer.aiControlled)
                        {
                            Game_Scene_Layer__SELECTOR.ReleasePlayerControl();
                            DetermineAI();
                        }
                        else
                            Game_Scene_Layer__SELECTOR.GivePlayerControl();
                    }

                    if (Game_Scene_Layer__Level_Data.activePlayer.TotalMoves == 0) //player turn over.
                    {
                        Game_Scene_Layer__Level_Data.PerformCaptures(); //check for bonus moves - check integ
                        if (Game_Scene_Layer__Level_Data.activePlayer.TotalMoves == 0) //if bonus moves - check ai, otherwise continue to player.
                        {
                            Game_Scene_Layer__Level_Data.activePlayer.turnOver = true;
                            return;
                        }
                        else if (Game_Scene_Layer__Level_Data.activePlayer.aiControlled)
                        {
                            DetermineAI();
                        }
                    }
                    else
                    {
                        Game_Scene_Layer__Level_Data.PlayerCompetitiveIntegrityCheck();
                        if (Game_Scene_Layer__Level_Data.activePlayer.aiControlled && !Game_Scene_Layer__Level_Data.activePlayer.turnOver)
                        {
                            DetermineAI();
                        }
                    }
                }
            }
            else
            {
                endGameTimer.Increase_DeltaTime((float)e.DeltaTime);
                if (endGameTimer.Finished)
                {
                    Cell_Game__SCENE_MANAGEMENT_SERVICE__Reference.SetScene
                        (
                        CellGame.SCENE_TAG__MAIN_MENU
                        );
                }
            }
            
            base.Handle_Update__Scene_Layer(e);
        }
        
        private void DetermineAI()
        {
            GameAction[] moves = Game_Scene_Layer__Level_Data.activePlayer.playerAi.DetermineActions();
            
            Game_Scene_Layer__SELECTOR.SetAIMoves(moves);
        }

        protected override void Handle_Render__Scene_Layer(RenderService renderService, FrameArgument e)
        {
            if (Game_Scene_Layer__Level_Data.gameStarted)
            {
                Game_Scene_Layer__Level_Data
                    .ForEachCell
                        (
                        (int x, int y, ref RenderUnit c) => renderService
                            .DrawSprite
                            (
                                ref c, 
                                x * 16 + Game_Scene_Layer__Level_Data.offset.X, 
                                y * 16 + Game_Scene_Layer__Level_Data.offset.Y
                            )
                        );

                string turntext;
                string scoreboard = "";

                if (!Game_Scene_Layer__Level_Data.gameOver)
                {
                    turntext = String.Format(
                        "{0}'s turn.\n" +
                        "Normal Cells: {1}\n" +
                        "Jumper Cells: {2}\n" +
                        "Placement Type: {3}\n" +
                        "{4}",

                        Game_Scene_Layer__Level_Data.activePlayer.name,
                        Game_Scene_Layer__Level_Data.activePlayer.normalCellPlacementCount,
                        Game_Scene_Layer__Level_Data.activePlayer.jumperCellPlacementCount,
                        ((Game_Scene_Layer__SELECTOR.placingNormals) ? "Normals" : "Jumpers"),
                        (Game_Scene_Layer__SELECTOR.placementSuccess) ? "" :
                        "Failure to place cell. Try again."
                        );
                }
                else
                {
                    turntext = "Game Over." 
                               + (
                                   (Game_Scene_Layer__Level_Data.winningPlayer != null)
                                       ? " " + Game_Scene_Layer__Level_Data.winningPlayer.name + " wins." : "");
                }

                List<Player> sortedPlayers = new List<Player>() { Game_Scene_Layer__Level_Data.playerRoster[0] };

                for (int i = 1; i < Game_Scene_Layer__Level_Data.playerRoster.Count; i++)
                {
                    for (int j = 0; j < sortedPlayers.Count; j++)
                    {
                        if (Game_Scene_Layer__Level_Data.playerRoster[i].cellCount > sortedPlayers[j].cellCount)
                        {
                            sortedPlayers.Insert(j, Game_Scene_Layer__Level_Data.playerRoster[i]);
                            break;
                        }
                        else if (j + 1 == sortedPlayers.Count)
                        {
                            sortedPlayers.Add(Game_Scene_Layer__Level_Data.playerRoster[i]);
                            break;
                        }
                    }
                }

                if (Cell_Game__INPUTHANDLER__Reference.EvaluateSwitchState("S"))
                {
                    for (int i = 0; i < sortedPlayers.Count; i++)
                    {
                        scoreboard += String.Format("{0} - Cells {1}", sortedPlayers[i].name, sortedPlayers[i].cellCount);
                        if (sortedPlayers[i].turnOver)
                            scoreboard += " - Done";
                        scoreboard += '\n';
                    }
                }
                else
                {
                    scoreboard = "S to toggle scoreboard.\nT to toggle cell type.\nEsc to quit.";
                }

                Cell_Game__TEXT_DISPLAYER__Reference.DrawText(renderService, turntext, "font", -580, 400);
                Cell_Game__TEXT_DISPLAYER__Reference.DrawText(renderService, scoreboard, "font", 100, 400);
            }
            
            base.Handle_Render__Scene_Layer(renderService, e);
        }
    }
}