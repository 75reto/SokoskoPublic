using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Sokoban_Programm
{
    public class Level
    {
        public string leveName;
        public char[,] grille;
        public int width;
        public int height;
        

        public void Load(string levelname)
        {

            string[] lines = File.ReadAllLines(levelname);

            foreach (var line in lines)
            {

                if (line.Trim()[0] == '#')
                {
                    height++;
                    if (width < line.Length)
                        width = line.Length;
                }
            }

            grille = new char[width, height];

            for (int y = 0; y < lines.Count(); y++)
            {
                if (lines[y].Trim()[0] == '#')
                {
                    for (int x = 0; x < lines[y].Length; x++)
                        grille[x, y] = lines[y][x];
                }
            }
        }

    }
}
