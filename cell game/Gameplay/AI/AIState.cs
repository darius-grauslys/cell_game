using isometricgame.GameEngine.Systems;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay.AI
{
    public enum AIDirective
    {
        Grow,
        Attack
    }

    public class AIState
    {
        private Player player;
        private Level gameLevel;
        private LevelAnalysis levelAnalysis;

        public AIDirective aiDirective;
        public IntegerPosition closestNthPosition = new IntegerPosition(-1,-1);

        public float EconWeight => 
            ((player.cellCount / gameLevel.MAX_CELLS) + 
            ((player.normalCellPlacementCount + 1) / (gameLevel.maxNormalEcon + 1)) +
            ((player.jumperCellPlacementCount + 1) / (gameLevel.maxJumperEcon + 1)))
            / 3;

        private readonly float maxDist;
        
        public AIState(Level gameLevel, Player player)
        {
            this.gameLevel = gameLevel;
            this.player = player;
            levelAnalysis = new LevelAnalysis(gameLevel);
            maxDist = Dist(new IntegerPosition(gameLevel.width, gameLevel.height));
        }

        public float GetProximityWeight(IntegerPosition aiPos, int n = 0, int target = 0, bool playableEnemyPosition = false) => (maxDist - GetNthClosestEnemyDistance(aiPos, n, target, playableEnemyPosition)) / maxDist;

        public int GetNthClosestEnemyDistance(IntegerPosition aiPosition, int n, int target = 0, bool playableEnemyPosition = false)
        {
            IntegerPosition disVec = new IntegerPosition(gameLevel.width, gameLevel.height);
            IntegerPosition deltaVec = disVec;

            Func<int, int, bool> playableChecker;
            playableChecker = (x, y) => 
            {
                bool ret = false;
                levelAnalysis.IsAllValidNeighborhood_1(x, y,
                    (vx, vy) =>
                        ret = true,
                    (vx, vy) => levelAnalysis.map[vx, vy] == 0
                    );
                return ret;
            };

            Predicate<int> targetChecker;
            if (target > 0)
                targetChecker = (id) => id == target;
            else
                targetChecker = (id) => id > 0 && id != player.id;

            Func<int, int, bool> checker;
            if (playableEnemyPosition)
            {
                checker = (x, y) =>
                {
                    return targetChecker(levelAnalysis.map[x, y]) && playableChecker(x, y);
                };
            }
            else
            {
                checker = (x, y) => targetChecker(levelAnalysis.map[x, y]);
            }

            int dist = gameLevel.width * gameLevel.height;
            List<int> dists = new List<int> { dist };
            List<IntegerPosition> positions = new List<IntegerPosition>() { new IntegerPosition(-1,-1) };

            levelAnalysis.GetMap();
            levelAnalysis.ForArea((x, y) => 
            {
                if (checker(x,y))
                {
                    deltaVec = new IntegerPosition(Math.Abs(aiPosition.X - x), Math.Abs(aiPosition.Y - y));
                    dist = Dist(deltaVec);

                    for (int i = 0; i < dists.Count; i++)
                    {
                        if (dist < dists[i])
                        {
                            dists.Insert(i, dist);
                            positions.Insert(i, new IntegerPosition(x, y));
                            break;
                        }
                        else if (i + 1 == dists.Count)
                        {
                            dists.Add(dist);
                            positions.Add(new IntegerPosition(x, y));
                            break;
                        }
                    }
                }
            });

            dists.RemoveAt(dists.Count-1);
            positions.RemoveAt(positions.Count-1);

            if (n >= dists.Count)
                return -1;
            else
            {
                closestNthPosition = positions[n];
                return dists[n];
            }
        }

        public static int Dist(IntegerPosition pos1)
        {
            return (int)(Math.Sqrt((pos1.X * pos1.X) + (pos1.Y * pos1.Y)));
        }

        public static int Dist(IntegerPosition pos1, IntegerPosition pos2)
        {
            int dx = pos2.X - pos1.X, dy = pos2.Y - pos1.Y;
            return (int)(Math.Sqrt((dx * dx) + (dy * dy)));
        }
    }
}
