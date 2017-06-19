using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fill_a_pix
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FillAPix puzzle;
            using (var stream = File.OpenRead(@".\puzzles\easy1.txt"))
                puzzle = FillAPix.FromStream(stream);

            Console.WriteLine(puzzle);

            Console.ReadKey();
        }
    }
}
