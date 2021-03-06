using isometricgame.GameEngine.WorldSpace.ChunkSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Gameplay
{
    public enum GameActions
    {
        PlaceNormal,
        PlaceJumper,
        EndTurn
    }

    public struct GameAction
    {
        public readonly GameActions gameAction;
        public readonly IntegerPosition position;

        public GameAction(GameActions gameAction, IntegerPosition position)
        {
            this.gameAction = gameAction;
            this.position = position;
        }
    }
}
