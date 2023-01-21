using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuCBT
{
    class Program
    {
        static int debugRow = 0;
        static int debugColumn = 0;

        static void Main(string[] args)
        {
            SudokuCBT sudokuCBT = createSudokuCBT(args);

            Console.ReadKey();
        }

        static SudokuCBT createSudokuCBT(string[] args)
        {
            // Creating the Sudoku
            string text = File.ReadAllText("Arguments.txt");
            string[] textArgs = text.Split(" ");
            SudokuCBT sudokuCBT;
            if (args.Length > 0)
            {
                sudokuCBT = new SudokuCBT(convertToInt(args));
            }else
            {
                sudokuCBT = new SudokuCBT(convertToInt(textArgs));
            }
            return sudokuCBT;
        }

        static int[] convertToInt(string[] args)
        {
            // Helper function to convert a list of strings to a list of ints
            int[] toInts = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                toInts[i] = int.Parse(args[i]);
            }
            return toInts;
        }
    }
}