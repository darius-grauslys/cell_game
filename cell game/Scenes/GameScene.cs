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

namespace cell_game.Scenes
{
    public class GameScene : ChildScene
    {
        TextDisplayer textDisplayer;
        private Level gameLevel;
        Selector selector;

        private int old_turn = -1;

        private ControlScene controlScene;

        private Timer endGameTimer;

        public GameScene(Game game, ControlScene controlScene) 
            : base(game)
        {
            this.controlScene = controlScene;
            textDisplayer = game.GetSystem<TextDisplayer>();

            gameLevel = new Level(5, 5, new List<Player>(), new IntegerPosition(0, 0));

            DynamicSceneObjects.Add(selector = new Selector(this, gameLevel));
            
            InputHandler.DeclareKeySwitch(OpenTK.Input.Key.S);
            InputHandler.DeclareKeySwitch(OpenTK.Input.Key.Escape);

            endGameTimer = new Timer(3);
        }

        public void SetLevel(List<Player> players, int width, int height)
        {
            int offsetX = width / 2 * -16;
            int offsetY = (height / 2 * -16) - 96;
            gameLevel = new Level(width, height, players, new IntegerPosition(offsetX, offsetY));
            for (int i = 0; i < players.Count; i++)
                if (players[i].aiControlled)
                    players[i].playerAi.SetLevel(gameLevel, players[i]);
            selector.SetLevel(gameLevel);
            gameLevel.StartTurn();
            endGameTimer.Set();
            old_turn = -1;
        }

        public override void UpdateFrame(FrameArgument e)
        {
            if (InputHandler.Keyboard_SwitchState_BoolReset(OpenTK.Input.Key.Escape))
                gameLevel.gameOver = true;

            if (!gameLevel.gameOver)
            {
                if (gameLevel.gameStarted)
                {
                    if (gameLevel.activePlayer.turnOver)
                        gameLevel.StartTurn();

                    if (old_turn != gameLevel.turnIndex)
                    {
                        old_turn = gameLevel.turnIndex;
                        if (gameLevel.activePlayer.aiControlled)
                        {
                            selector.ReleasePlayerControl();
                            DetermineAI();
                        }
                        else
                            selector.GivePlayerControl();
                    }

                    if (gameLevel.activePlayer.TotalMoves == 0) //player turn over.
                    {
                        gameLevel.PerformCaptures(); //check for bonus moves - check integ
                        if (gameLevel.activePlayer.TotalMoves == 0) //if bonus moves - check ai, otherwise continue to player.
                        {
                            gameLevel.activePlayer.turnOver = true;
                            return;
                        }
                        else if (gameLevel.activePlayer.aiControlled)
                        {
                            DetermineAI();
                        }
                    }
                    else
                    {
                        gameLevel.PlayerCompetitiveIntegrityCheck();
                        if (gameLevel.activePlayer.aiControlled && !gameLevel.activePlayer.turnOver)
                        {
                            DetermineAI();
                        }
                    }
                }
            }
            else
            {
                endGameTimer.DeltaTime((float)e.DeltaTime);
                if (endGameTimer.Finished)
                    controlScene.TransitionScene(0);
            }

            base.UpdateFrame(e);
        }

        private void DetermineAI()
        {
            GameAction[] moves = gameLevel.activePlayer.playerAi.DetermineActions();
            
            selector.SetAIMoves(moves);
        }

        public override void RenderFrame(RenderService renderService, FrameArgument e)
        {
            if (gameLevel.gameStarted)
            {
                gameLevel.ForEachCell((int x, int y, ref RenderUnit c) => DrawSprite(renderService, ref c));

                string turntext;
                string scoreboard = "";

                if (!gameLevel.gameOver)
                {
                    turntext = String.Format(
                        "{0}'s turn.\n" +
                        "Normal Cells: {1}\n" +
                        "Jumper Cells: {2}\n" +
                        "Placement Type: {3}\n" +
                        "{4}",

                        gameLevel.activePlayer.name,
                        gameLevel.activePlayer.normalCellPlacementCount,
                        gameLevel.activePlayer.jumperCellPlacementCount,
                        ((selector.placingNormals) ? "Normals" : "Jumpers"),
                        (selector.placementSuccess) ? "" :
                        "Failure to place cell. Try again."
                        );
                }
                else
                {
                    turntext = "Game Over." + ((gameLevel.winningPlayer != null) ? " " + gameLevel.winningPlayer.name + " wins." : "");
                }

                List<Player> sortedPlayers = new List<Player>() { gameLevel.playerRoster[0] };

                for (int i = 1; i < gameLevel.playerRoster.Count; i++)
                {
                    for (int j = 0; j < sortedPlayers.Count; j++)
                    {
                        if (gameLevel.playerRoster[i].cellCount > sortedPlayers[j].cellCount)
                        {
                            sortedPlayers.Insert(j, gameLevel.playerRoster[i]);
                            break;
                        }
                        else if (j + 1 == sortedPlayers.Count)
                        {
                            sortedPlayers.Add(gameLevel.playerRoster[i]);
                            break;
                        }
                    }
                }

                if (InputHandler.Keyboard_SwitchState_Bool(OpenTK.Input.Key.S))
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

                textDisplayer.DrawText(renderService, turntext, "font", -580, 400);
                textDisplayer.DrawText(renderService, scoreboard, "font", 100, 400);
            }

            base.RenderFrame(renderService, e);
        }
    }
}
