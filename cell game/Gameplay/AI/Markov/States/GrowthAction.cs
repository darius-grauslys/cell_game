using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay.AI.Markov.States
{
    public class GrowthAction : MarkovAction
    {
        public GrowthAction(AIState aiState, MarkovAction[] childActions) 
            : base(aiState, childActions)
        {
        }

        public override GameAction GetGameAction()
        {
            throw new NotImplementedException();
        }

        public override MarkovAction GetNextAction()
        {
            throw new NotImplementedException();
        }
    }
}
