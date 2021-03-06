using isometricgame.GameEngine.Rendering;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay
{
    public class Level
    {
        public delegate void CellOperation(int x, int y, ref RenderUnit unit);

        public RenderStructure structure;
        public List<Player> playerRoster;
        public Player activePlayer;
        public Player winningPlayer;
        public int turnIndex = -1;

        public int width, height;

        public int MAX_CELLS => width * height;

        public Random rand;

        public bool gameOver = false;
        public bool gameStarted = false;

        private LevelAnalysis levelAnalysis;
        private int playerCount = 0;
        private int finishedPlayerCount = 0;
        public int FinishedPlayerCount => finishedPlayerCount;

        private int remainingCells = 0;
        public int RemainingCells => remainingCells;

        public readonly int maxNormalEcon = 4, maxJumperEcon = 1;

        public readonly IntegerPosition offset;

        public Level(int width, int height, List<Player> players, IntegerPosition offset)
        {
            this.offset = offset;

            this.width = width;
            this.height = height;

            this.playerRoster = players;
            playerCount = players.Count;

            rand = new Random();
            structure = new RenderStructure(width, height);
            foreach (Player player in playerRoster)
            {
                if (player.aiControlled)
                    player.playerAi.SetLevel(this, player);
            }

            for (int i = 0; i < playerRoster.Count; i++)
            {
                placeCell(rand.Next(width), rand.Next(height), i + 1);
            }
            
            remainingCells = width * height;

            ForEachCell((int x, int y, ref RenderUnit c) => { c.Position = new Vector3(x * 16, y * 16, 0) + new Vector3(offset.X, offset.Y, 0); });
            levelAnalysis = new LevelAnalysis(this);
        }

        public void StartTurn()
        {
            if (!gameStarted)
                gameStarted = true;

            if (finishedPlayerCount == playerCount)
            {
                gameOver = true;

                winningPlayer = playerRoster[0];
                foreach (Player player in playerRoster)
                    if (winningPlayer.cellCount < player.cellCount)
                        winningPlayer = player;

                return;
            }

            if (activePlayer != null)
            {
                activePlayer.turnOver = false;
                if (activePlayer.normalCellPlacementCount > maxNormalEcon)
                    activePlayer.normalCellPlacementCount = maxNormalEcon;
                if (activePlayer.jumperCellPlacementCount > maxJumperEcon)
                    activePlayer.jumperCellPlacementCount = maxJumperEcon;
            }

            turnIndex = (turnIndex + 1) % playerRoster.Count;
            
            activePlayer = playerRoster[turnIndex];

            if (!activePlayer.finishedInGame)
            {
                activePlayer.hadBonusMoves = false;

                SetCellNum(ref activePlayer.normalCellPlacementCount, -1);

                PlayerCompetitiveIntegrityCheck();
            }
        }

        public bool Inbounds(int x, int y) => x > -1 && y > -1 && x < width && y < height;

        public void PerformCaptures()
        {
            IntegerPosition[] fillSpaces = levelAnalysis.FindFillSpaces(turnIndex + 1);

            int bonusJumper = 0;
            bool bonusNormals = false;

            for (int i = 0; i < fillSpaces.Length; i++)
            {
                int pixelValue = structure.StructuralUnits[fillSpaces[i].Y][fillSpaces[i].X].Id;
                if (pixelValue == 0)
                    bonusJumper++;
                else if (pixelValue != turnIndex + 1)
                    bonusNormals = true;
                placeCell(fillSpaces[i].X, fillSpaces[i].Y, activePlayer.id);
            }

            if (activePlayer.hadBonusMoves)
                return;
            else
                activePlayer.hadBonusMoves = true;

            PlayerCompetitiveIntegrityCheck();
            if (activePlayer.turnOver)
                return;

            bonusJumper /= 9;
            activePlayer.jumperCellPlacementCount = bonusJumper;
            if (bonusNormals)
                SetCellNum(ref activePlayer.normalCellPlacementCount, -1);
        }
        
        public void PlayerCompetitiveIntegrityCheck()
        {
            if (activePlayer.finishedInGame)
                return;
            activePlayer.availableMoves = levelAnalysis.GetNumberOfPlayableMoves(turnIndex + 1);
            if (0 == activePlayer.availableMoves)
            {
                activePlayer.finishedInGame = true;
                finishedPlayerCount++;
            }
        }

        public void ForEachCell(CellOperation operation)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    operation(x, y, ref structure.StructuralUnits[y][x]);
        }

        public bool PlaceCell(int x, int y, int playerId, int range = 1)
        {
            bool validplace = IsValidPosition(x, y, range);

            if (!validplace)
                return validplace;
            
            placeCell(x, y, playerId);
            return validplace;
        }

        public bool IsValidPosition(int x, int y, int range=1)
        {
            if (structure.StructuralUnits[y][x].Id != 0)
                return false;
            if (range == 0)
                return true;

            int lowerBoundX = (x >= range) ? -range : 0;
            int upperBoundX = (x < width - range) ? range : 0;
            int lowerBoundY = (y >= range) ? -range : 0;
            int upperBoundY = (y < height - range) ? range : 0;
            
            for (int lx = x + lowerBoundX; lx <= x + upperBoundX; lx++)
            {
                for (int ly = y + lowerBoundY; ly <= y + upperBoundY; ly++)
                {
                    if (structure.StructuralUnits[ly][lx].Id == turnIndex + 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void placeCell(int x, int y, int playerId)
        {
            playerRoster[playerId-1].cellCount++;
            int id = structure.StructuralUnits[y][x].Id;
            if (id > 0 && id != turnIndex + 1)
                playerRoster[id-1].cellCount--;
            else if (id == 0)
                remainingCells--;
            structure.StructuralUnits[y][x].Id = playerId;
        }

        private void SetCellNum(ref int num, int count)
        {
            if (0 <= count)
            {
                num = count;
                return;
            }
            int a = rand.Next(6) + 1;
            int b = rand.Next(6) + 1;
            bool flop;
            flop = (a < b) && count == -1;
            num = (flop) ? a : b;
        }
    }
}
