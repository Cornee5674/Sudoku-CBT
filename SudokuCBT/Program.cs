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
            Sudoku sudoku = createSudoku(args, true);

            //sudoku.PrintConstraint(debugRow, debugColumn);

            // Make sudoku node consistent (knoopconsistent)
            sudoku.NodeConsistency();


            //sudoku.PrintConstraint(debugRow, debugColumn);

            Console.ReadKey();
        }

        static Sudoku createSudoku(string[] args, bool print)
        {
            // Creating the Sudoku
            string text = File.ReadAllText("Arguments.txt");
            string[] textArgs = text.Split(" ");
            Sudoku sudoku;
            if (args.Length > 0)
            {
                sudoku = new Sudoku(convertToInt(args), print);
            }
            else
            {
                sudoku = new Sudoku(convertToInt(textArgs), print);
            }
            return sudoku;
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