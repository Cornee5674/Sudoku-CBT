using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

namespace SudokuCBT
{
    class Program
    {
        static int debugRow = 0;
        static int debugColumn = 0;

        static void Main(string[] args)
        {
            SudokuCBT sudokuCBT = createSudokuCBT(args);
            
            //Stopwatch for diagnostics
            Stopwatch s = new();
            s.Start();
            
            //Run the chronological backtracking algorithm on the loaded sudoku and print the solution
            doCBT(sudokuCBT, true);
            s.Stop();

            sudokuCBT.printSudoku();

            //Print the elapsed time
            Console.WriteLine("Elapsed time: " + s.ElapsedMilliseconds.ToString() + " ms");

            //BenchmarkSudoku(args, false);

            Console.ReadKey();
        }

        static long BenchmarkSudoku(string[] args, bool useForwardChecking)
        {
            int runXTimes = 50;
            long totalTime = 0;
            for (int i = 0; i < runXTimes; i++)
            {
                SudokuCBT sudoku = createSudokuCBT(args);
                Stopwatch s = new();
                s.Start();
                doCBT(sudoku, useForwardChecking);
                s.Stop();
                Console.WriteLine((i + 1) + ": " + s.ElapsedMilliseconds.ToString());
                totalTime += s.ElapsedMilliseconds;
            }
            long averageMS = totalTime / 50;
            Console.WriteLine("Average elapsed time, over 50 runs: " + averageMS);
            return averageMS;
        }

        static SudokuCBT createSudokuCBT(string[] args)
        {
            // Creating the Sudoku
            string text = File.ReadAllText("Arguments.txt");
            string[] textArgs = text.Split(" ");
            SudokuCBT sudokuCBT;
            if (args.Length > 0)
            {
                sudokuCBT = new SudokuCBT(convertToInt(args), true);
            }else
            {
                sudokuCBT = new SudokuCBT(convertToInt(textArgs), false);
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

        static void doCBT(SudokuCBT originalSudoku, bool useForwardChecking)
        {
            SudokuCBT currentSudoku = originalSudoku.Clone();
            
            //Make the sudoku node consistent
            currentSudoku.NodeConsistency();

            //Solve the sudoku and return when solved
            //If no solution is found we indicate this
            if (solve(currentSudoku, 0, 0, useForwardChecking))
                return;
            else
                Console.WriteLine("No solution found");
        }

        //Recursive function that solves the sudoku
        static bool solve(SudokuCBT sudoku, int x, int y, bool useForwardChecking)
        {
            //If all squares are filled in we can return true meaning the sudoku is solved
            if(x == 9 && y == 8)
                return true;
            //If we've searched an entire row we go down 1 row
            if (x == 9)
            {
                y++;
                x = 0;
            }

            //If the square is already filled in, search the next one
            //If this returns false it means we need to go back a step and check the next successor
            //If true is returned it means we have encoutered a solution and we go back up the entire chain
            if (sudoku.sudokuField[x,y] != 0)
                return solve(sudoku, x+1, y, useForwardChecking);

            //Get the domain of the current field
            List<int> domain = sudoku.domainField[x, y];
            //Create an empty list which will store the indices we have already tested on this field
            List<int> testedIndices = new();
            int currentIndex;
            while (domain.Count > 0)
            {
                //Get the index we will be checking for and add this to the list of checked indices
                currentIndex = domain[0];
                testedIndices.Add(currentIndex);
                
                //We give the field we are checking for the current index
                sudoku.sudokuField[x, y] = currentIndex;
                //If this is a partial solution we continue
                if (sudoku.partialSolution())
                {
                    ////Once we have found a partial solution we do a forward check
                    ////If this check is false it means there is no actual possible solution possible
                    ////with this partial solution, thus we return false
                    //if (!sudoku.forwardChecking(x, y))
                    //{
                    //    sudoku.sudokuField[x, y] = 0;
                    //    //return false;
                    //}
                    //else
                    //{
                    //    //If we have reached this point it means that this partial solution is good to expand upon
                    //    //We make use of recursion to check the next field
                    //    //If true is returned by the function it means that we have found a solution
                    //    //Therefore this function returns true as well
                    //    if (solve(sudoku, x + 1, y))
                    //        return true;
                    //}

                    if (solve(sudoku, x + 1, y, useForwardChecking))
                    {
                        return true;
                    }

                    
                }
                //If this is not a partial solution we revert our change
                sudoku.sudokuField[x, y] = 0;

                //Update the domain list with all the possible values that have not been checked yet
                //We do this by reading the domain and subtracting all the indices we have already tested
                domain = sudoku.domainField[x, y].Except(testedIndices).ToList();
            }

            //If this function never returned "true" it means that this is not the good path
            //and we take a step back
            return false;
        }
        
    }
}