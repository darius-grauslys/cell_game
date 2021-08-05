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
        protected Level_Data gameLevelData;
        protected LevelAnalysis levelAnalysis;

        protected AIState aiState;

        public void SetLevel(Level_Data gameLevelData, Player player)
        {
            this.gameLevelData = gameLevelData;
            levelAnalysis = new LevelAnalysis(gameLevelData);
            aiState = new AIState(gameLevelData, player);
        }

        public abstract GameAction[] DetermineActions();

        public static IntegerPosition GetValidRandomPosition(Level_Data gameLevelData, int range=1)
        {
            IntegerPosition ret = new IntegerPosition();

            bool isValid = false;
            while (!isValid && gameLevelData.RemainingCells > 0 && !gameLevelData.activePlayer.turnOver)
            {
                ret = GetRandomPosition(gameLevelData);
                isValid = gameLevelData.IsValidPosition(ret.X, ret.Y, range);
            }

            return ret;
        }

        public static IntegerPosition GetRandomPosition(Level_Data gameLevelData)
        {
            return new IntegerPosition(gameLevelData.rand.Next(gameLevelData.width), gameLevelData.rand.Next(gameLevelData.height));
        }
    }
}
