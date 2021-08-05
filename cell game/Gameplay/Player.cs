using cell_game.Gameplay.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay
{
    public class Player
    {
        public int normalCellPlacementCount = 0;
        public int jumperCellPlacementCount = 0;
        public int TotalMoves => normalCellPlacementCount + jumperCellPlacementCount;
        public bool hadBonusMoves = false;

        public int cellCount = 0;
        public int availableMoves = 0;
        public bool aiControlled = true;
        public bool turnOver = false;
        public bool finishedInGame = false;
        public readonly uint id;

        public readonly string name;

        public CellAI playerAi;

        public Player(string name, uint id, CellAI playerAi = null)
        {
            this.name = name;
            this.id = id;
            this.aiControlled = playerAi != null;
            this.playerAi = playerAi;
        }
    }
}
