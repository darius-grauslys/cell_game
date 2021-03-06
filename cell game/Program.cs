using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            CellGame game = new CellGame(
                dir,
                Path.Combine(dir, "Assets\\")
                );
            game.Run();
        }
    }
}
