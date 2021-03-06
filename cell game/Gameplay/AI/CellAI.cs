using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay.AI
{
    public abstract class CellAI
    {
        protected Level gameLevel;
        protected LevelAnalysis levelAnalysis;

        protected AIState aiState;

        public void SetLevel(Level gameLevel, Player player)
        {
            this.gameLevel = gameLevel;
            levelAnalysis = new LevelAnalysis(gameLevel);
            aiState = new AIState(gameLevel, player);
        }

        public abstract GameAction[] DetermineActions();

        public static IntegerPosition GetValidRandomPosition(Level gameLevel, int range=1)
        {
            IntegerPosition ret = new IntegerPosition();

            bool isValid = false;
            while (!isValid && gameLevel.RemainingCells > 0 && !gameLevel.activePlayer.turnOver)
            {
                ret = GetRandomPosition(gameLevel);
                isValid = gameLevel.IsValidPosition(ret.X, ret.Y, range);
            }

            return ret;
        }

        public static IntegerPosition GetRandomPosition(Level gameLevel)
        {
            return new IntegerPosition(gameLevel.rand.Next(gameLevel.width), gameLevel.rand.Next(gameLevel.height));
        }
    }
}
