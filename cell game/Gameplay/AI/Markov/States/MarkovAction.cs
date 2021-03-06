using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay.AI.Markov.States
{
    public abstract class MarkovAction
    {
        protected AIState aiState;
        protected MarkovAction[] childStates;
        
        public MarkovAction(AIState aiState, MarkovAction[] childActions)
        {
            this.aiState = aiState;
            this.childStates = childActions;
        }

        public abstract MarkovAction GetNextAction();

        public abstract GameAction GetGameAction();
    }
}
