using isometricgame.GameEngine.Rendering;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay
{
    enum IndexCheckType
    {
        Any,
        Negative,
        Zero
    }

    public class LevelAnalysis
    {
        public int[,] map;
        int width, height;
        Level_Data gameLevelData;

        public LevelAnalysis(Level_Data gameLevelData)
        {
            this.gameLevelData = gameLevelData;
            map = new int[gameLevelData.width, gameLevelData.height];
            width = gameLevelData.width;
            height = gameLevelData.height;
        }

        public void GetMap()
        {
            gameLevelData.ForEachCell((int x, int y, ref RenderUnit c) => map[x, y] = (int)c.Id);
        }

        public int GetNumberOfPlayableMoves(int playerValue, int range = 1)
        {
            gameLevelData.ForEachCell((int x, int y, ref RenderUnit c) => map[x, y] = (int)c.Id);
            
            int cellCount = 0;
            int moveCount = 0;

            ForArea_Breakable((x, y) => 
            {
                if (map[x, y] != playerValue)
                    return true;
                cellCount++;

                IsValidNeighborhood(x, y, range, (dx, dy) => { moveCount++; }, (dx, dy) => map[dx, dy] == 0);

                //IsAllValidAdjacents(x, y, (dx, dy) => ret = true, (dx, dy) => map[dx, dy] == 0);
                //if (!ret)
                //    IsAllValidDiagonals(x, y, (dx, dy) => ret = true, (dx, dy) => map[dx, dy] == 0);
                
                if (cellCount == gameLevelData.activePlayer.cellCount)
                    return false;
                return true;
            });

            return moveCount;
        }

        public IntegerPosition[] FindFillSpaces(int playerValue)
        {
            gameLevelData.ForEachCell((int x, int y, ref RenderUnit c) => map[x, y] = (int)c.Id);

            Func<int, int, bool> checker = (px, py) => map[px, py] == playerValue;
            Action<int, int> drawer_step_1 = (px, py) => map[px, py] = -1;
            Action<int, int> drawer_step_2 = (px, py) => { if (map[px,py] == -1) map[px, py] = -2; };

            //if (playerValue == 1)
            //    printmap();

            ForArea((xi, yi) =>
            {
                if (map[xi, yi] == playerValue)
                {
                    //horizontals
                    if (xi > 0)
                        DrawLine(xi - 1, xi, -1, (x) => checker(x, yi), (x) => drawer_step_1(x, yi));
                    if (xi < width - 1)
                        DrawLine(xi + 1, width - xi - 1, 1, (x) => checker(x, yi), (x) => drawer_step_1(x, yi));

                    //verticals
                    if (yi > 0)
                        DrawLine(yi - 1, yi, -1, (y) => checker(xi, y), (y) => drawer_step_2(xi, y));
                    if (yi < height - 1)
                        DrawLine(yi + 1, height - yi - 1, 1, (y) => checker(xi, y), (y) => drawer_step_2(xi, y));
                }
            });

            //if (playerValue == 1)
            //    printmap();
            
            List<IntegerPosition> positions = new List<IntegerPosition>();

            ForArea((xi, yi) =>
            {
                if (map[xi, yi] == -2)
                    positions.Add(new IntegerPosition(xi, yi));
            });

            //find invalid positions.
            List<IntegerPosition> invalidPositions = new List<IntegerPosition>();
            List<IntegerPosition> checkedPositions = new List<IntegerPosition>();

            Func<int, int, bool> condition = (dx, dy) => map[dx, dy] == -2 || map[dx, dy] == playerValue;

            for (int i = 0; i < positions.Count; i++)
                if (!IsAllValidAdjacents(positions[i].X, positions[i].Y, (hx, hy) => { }, condition)) //empty handle.
                    invalidPositions.Add(positions[i]);

            while (invalidPositions.Count > 0)
            {
                positions.Remove(invalidPositions[0]);

                IsAllValidAdjacents(invalidPositions[0].X, invalidPositions[0].Y, (hx, hy) => 
                {
                    IntegerPosition pos = new IntegerPosition(hx, hy);
                    if (positions.Contains(pos) && !checkedPositions.Contains(pos))
                    {
                        invalidPositions.Add(pos);
                        checkedPositions.Add(pos);
                    }
                },
                condition
                );

                invalidPositions.RemoveAt(0);
            }

            return positions.ToArray();
        }

        /// <summary>
        /// Returns true after all conditionals if there is at least one true condition.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <param name="validHandle"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool IsValidNeighborhood(int x, int y, int range, Action<int, int> validHandle, Func<int, int, bool> condition)
        {
            bool ret = false;

            if (range == 1)
            {
                Func<int, int, bool> wrapperCondition = (bx, by) => { bool cond = condition(bx, by); ret = ret || cond; return cond; };
                IsAllValidNeighborhood_1(x, y, validHandle, wrapperCondition);
                return ret;
            }
            
            int lowerX = (x - range > 0) ? x - range : 0;
            int lowerY = (y - range > 0) ? y - range : 0;
            int upperX = (x + range < width - 1) ? x + range : width - 1;
            int upperY = (y + range < height - 1) ? y + range : height - 1;

            for (int lx = lowerX; lx < upperX; lx++)
            {
                for (int ly = lowerY; ly < upperY; ly++)
                {
                    if (lx != x && ly != y)
                        if (ret = ret || condition(lx, ly))
                            validHandle(lx, ly);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns true if all conditions met. Use condition for one case basis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="validHandle"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool IsAllValidNeighborhood_1(int x, int y, Action<int, int> validHandle, Func<int, int, bool> condition)
        {
            bool ret = IsAllValidDiagonals(x, y, validHandle, condition);
            ret = IsAllValidAdjacents(x, y, validHandle, condition) && ret;

            return ret;
        }

        /// <summary>
        /// Returns true if all conditions met. Utilize condition for one case basis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="validHandle"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool IsAllValidDiagonals(int x, int y, Action<int, int> validHandle, Func<int, int, bool> condition)
        {
            bool ret = true;

            if (x > 0)
            {
                if (y > 0)
                {
                    ret = IsValidAdjacent(x - 1, y - 1, validHandle, condition) && ret;
                }
                if (y < height - 1)
                {
                    ret = IsValidAdjacent(x - 1, y + 1, validHandle, condition) && ret;
                }
            }
            if (x < width - 1)
            {
                if (y > 0)
                {
                    ret = IsValidAdjacent(x + 1, y - 1, validHandle, condition) && ret;
                }
                if (y < height - 1)
                {
                    ret = IsValidAdjacent(x + 1, y + 1, validHandle, condition) && ret;
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns true if all conditions met. Use condition for one case basis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="validHandle"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool IsAllValidAdjacents(int x, int y, Action<int,int> validHandle, Func<int,int,bool> condition)
        {
            bool ret = true;

            if (x > 0)
            {
                ret = IsValidAdjacent(x - 1, y, validHandle, condition);
            }
            if (x < width - 1)
            {
                ret = IsValidAdjacent(x + 1, y, validHandle, condition) && ret;
            }
            if (y > 0)
            {
                ret = IsValidAdjacent(x, y - 1, validHandle, condition) && ret;
            }
            if (y < height - 1)
            {
                ret = IsValidAdjacent(x, y + 1, validHandle, condition) && ret;
            }
            return ret;
        }

        /// <summary>
        /// validHandle invokes when condition is met.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="validHandle"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private bool IsValidAdjacent(int dx, int dy, Action<int,int> validHandle, Func<int,int,bool> condition)
        {
            //bool ret = map[dx, dy] == -2 || map[dx, dy] == playerValue;
            bool ret = condition(dx, dy);

            if (ret)
                validHandle(dx, dy);

            return ret;
        }

        /// <summary>
        /// breaks on a false condition.
        /// </summary>
        /// <param name="conditionalAction"></param>
        public void ForArea_Breakable(Func<int, int, bool> conditionalAction)
        {
            for (int yi = 0; yi < height; yi++)
            {
                for (int xi = 0; xi < width; xi++)
                {
                    if (!conditionalAction(xi, yi))
                        return;
                }
            }
        }

        public void ForArea(Action<int, int> action)
        {
            for (int yi = 0; yi < height; yi++)
            {
                for (int xi = 0; xi < width; xi++)
                {
                    action(xi, yi);
                }
            }
        } 

        private void DrawLine(int start, int length, int stepScalar, Predicate<int> checker, Action<int> drawer)
        {
            bool draw = false;
            int index = start;

            for(int i=0;i<length;i++)
            {
                if (draw = checker(index + (i * stepScalar)))
                {
                    index += ((i-1) * stepScalar);
                    break;
                }
            }
            
            if (draw)
            {
                int drawLength = (int)Math.Abs(start - (index + stepScalar));
                int drawIndex = start;
                for (int i = 0; i < drawLength; i++)
                {
                    drawer(drawIndex + (i * stepScalar));
                }
            }
        }

        private void printmap()
        {
            ForArea((x, y) =>
            {
                int val = map[x, height - y - 1];
                string valString = val.ToString();
                if (valString.Length < 2)
                    valString += ' ';
                if ((x + (y * width)) % width == 0)
                    Console.WriteLine();
                Console.Write(String.Format("[{0}]", (val == 0) ? ".." : valString));
            });
            Console.WriteLine("\n");
        }
    }
}
