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
            sudokuCBT.NodeConsistency();

            //sudokuCBT.printConstraints();
            forwardChecking(sudokuCBT, 0, 2);

            Console.WriteLine(sudokuCBT.partialSolution());

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

        static bool forwardChecking(SudokuCBT sudoku, int newRow, int newCol)
        {
            //get value of newly assigned variable
            int newValue = sudoku.sudokuField[newRow, newCol];

            //create list with all indices to be check
            List<(int, int)> indices = sudoku.rowIndices(newRow, newCol);
            indices.AddRange(sudoku.columnIndices(newRow, newCol));
            indices.AddRange(sudoku.blockIndices(newRow, newCol));

            //remove duplicates and indices of newly set variable itself
            indices = indices.Distinct().ToList();
            indices.Remove((newRow, newCol));

            //iterate indices
            for (int i = 0; i < indices.Count; i++)
            {
                //check domain of current variable
                List<int> curDomain = sudoku.domainField[indices[i].Item1, indices[i].Item2];

                //if domain would become empty by removing set value, return false
                if (curDomain.Contains(newValue) & (curDomain.Count == 1))
                    return false;
            }

            //if none of the domains would become empty, return true
            return true;
        }
    }
}