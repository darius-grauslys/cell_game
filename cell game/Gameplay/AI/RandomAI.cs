using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isometricgame.GameEngine.Rendering;
using isometricgame.GameEngine.WorldSpace.ChunkSpace;

namespace cell_game.Gameplay.AI
{
    public class RandomAI : CellAI
    {
        private float aggressionWeight = 0.90f;
        private float econWeight = 0.90f;
        private int vendetta;

        private Random random = new Random();

        public RandomAI(int vendetta=0)
        {
            this.vendetta = vendetta;
            aggressionWeight = (random.Next(50) / 100f) + 0.2f;
            econWeight = random.Next(100) / 100f;
        }

        public override GameAction[] DetermineActions()
        {
            IntegerPosition biasedPosition = GetRandomPosition(gameLevel);
            float vendettaAttackWeight = aiState.GetProximityWeight(biasedPosition,0,vendetta,true);

            float attackWeight = vendettaAttackWeight;
            for (int i = 0; i < gameLevel.playerRoster.Count; i++)
            {
                if (!gameLevel.playerRoster[i].turnOver && gameLevel.playerRoster[i] != gameLevel.activePlayer)
                {
                    attackWeight = aiState.GetProximityWeight(biasedPosition, 0, gameLevel.playerRoster[i].id, true);
                    if (attackWeight > vendettaAttackWeight)
                    {
                        vendetta = gameLevel.playerRoster[i].id;
                        vendettaAttackWeight = attackWeight;
                    }
                }
            }

            if (vendettaAttackWeight > aggressionWeight)
                biasedPosition = aiState.closestNthPosition;
            else if (aiState.EconWeight < econWeight)
                biasedPosition = GetValidRandomPosition(gameLevel,4);

            List<IntegerPosition> normalPositions = getPositions(gameLevel.activePlayer.normalCellPlacementCount, 1, biasedPosition);
            List<IntegerPosition> jumperPosition = getPositions(gameLevel.activePlayer.jumperCellPlacementCount, 3, biasedPosition);

            List<GameAction> actions = new List<GameAction>();
            for (int i = 0; i < normalPositions.Count; i++)
                actions.Add(new GameAction(GameActions.PlaceNormal, normalPositions[i]));
            for (int i = 0; i < jumperPosition.Count; i++)
                actions.Add(new GameAction(GameActions.PlaceJumper, jumperPosition[i]));

            if (actions.Count == 0)
                actions.Add(new GameAction(GameActions.EndTurn, new IntegerPosition(0,0)));

            return actions.ToArray();
        }

        private List<IntegerPosition> getPositions(int count, int range, IntegerPosition targetPosition)
        {
            List<IntegerPosition> validPlacements = new List<IntegerPosition>();

            levelAnalysis.GetMap();

            levelAnalysis.ForArea((x, y) => 
            {
                if (levelAnalysis.map[x, y] == gameLevel.activePlayer.id)
                {
                    levelAnalysis.IsValidNeighborhood(x, y, range, (dx, dy) =>
                        {
                            IntegerPosition pos = new IntegerPosition(dx, dy);
                            if (!validPlacements.Contains(pos))
                                validPlacements.Add(pos);
                        },
                        (dx, dy) => levelAnalysis.map[dx, dy] == 0
                    );
                }
            });

            if (validPlacements.Count == 0)
                return validPlacements;

            int dist = AIState.Dist(validPlacements[0], targetPosition);
            List<int> dists = new List<int>() { dist };
            List<IntegerPosition> sortedByDistance = new List<IntegerPosition>() { validPlacements[0] };

            for (int i = 0; i < validPlacements.Count; i++)
            {
                dist = AIState.Dist(validPlacements[i], targetPosition);
                for (int j = 0; j < sortedByDistance.Count; j++)
                {
                    if (dist < dists[j])
                    {
                        sortedByDistance.Insert(j, validPlacements[i]);
                        dists.Insert(j, dist);
                        break;
                    }
                    else if (j + 1 == sortedByDistance.Count)
                    {
                        sortedByDistance.Add(validPlacements[i]);
                        dists.Add(dist);
                        break;
                    }
                }
            }

            int counter = sortedByDistance.Count - count;
            if (counter < 1)
            {
                return sortedByDistance;
            }
            while (counter > 0 && sortedByDistance.Count > 0)
            {
                counter--;

                sortedByDistance.RemoveAt(sortedByDistance.Count-1);
            }
            return sortedByDistance;
        }
    }
}
