﻿using System;
using System.Linq;

namespace SudokuCBT
{
	public class SudokuCBT
	{
		// The actual sudoku
		public int[,] sudokuField;
		// For every spot in the sudoku is a domain list
		public List<int>[,] domainField;

		// All the constraints, with the key of the dictionary being (indexrow1, indexcolumn1), (indexrow2, indexcolumn2)
		// And the value (value1, value2)
		public Dictionary<((int, int), (int, int)), List<(int, int)>> constraints;

		int rows = 9;
		int columns = 9;

		public SudokuCBT(int[] list)
		{
			// Initialize sudoku
			sudokuField = new int[rows, columns];

			// Fill the sudoku field with the numbers from the given int list
			for (int i = 0; i < list.Length; i++)
			{
				int columnNr = i % 9;
				int rowNr = i / 9;
				sudokuField[rowNr, columnNr] = list[i];
			}

            // Initialize domain field
            domainField = new List<int>[rows, columns];
            initializeDomainFields();

			// Initializing constraints
            constraints = new Dictionary<((int, int), (int, int)), List<(int, int)>>();
			initializeConstraints();

            Console.WriteLine("Empty sudoku:");
			printSudoku();
			printConstraints();
		}

		// Use these 3 functions to get the lists of indices that lie in the same row/block/column
        private List<(int, int)> blockIndices(int x, int y)
        {
            List<(int, int)> indices = new List<(int, int)>();
            List<int> tempxList = returnTempList(x);
            List<int> tempyList = returnTempList(y);

            for (int i = 0; i < tempxList.Count; i++)
            {
                for (int j = 0; j < tempyList.Count; j++)
                {
                    indices.Add((tempxList[i], tempyList[j]));
                }
            }
            return indices;
        }

		private List<(int, int)> rowIndices(int x, int y)
		{
			List<(int, int)> indices = new List<(int, int)>();
			for (int i = 0; i < 9; i++)
			{
				indices.Add((x, i));
			}
			return indices;
		}

		private List<(int, int)> columnIndices(int x, int y)
		{
			List<(int, int)> indices = new List<(int, int)>();
			for (int i = 0; i < 9; i++)
			{
				indices.Add((i, y));
			}
			return indices;
		}

        private void printConstraints()
		{
            foreach (var item in constraints)
            {
				string x = "";
				foreach (var item2 in item.Value)
				{
					x += item2 + " : ";
				}
                Console.Write("Constraint: " + item.Key + "        Value: " + x + "\n");
            }
        }

        private void initializeConstraints()
		{
			// This function creates all the C0, 0 - C8, 8 constraints
			// It initially sets all the constraints to (1,1) - (9,9) except if it is an assigned number
			// If that is the case, the only constraint will be (n, n)
			createSelfConstraints();

			// We remove all reflexive constraints that are created by the assigned numbers in this function
			removeConstraintsFromAssignedNumbers();
		}

		private void removeConstraintsFromAssignedNumbers()
		{
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					int currentNr = sudokuField[i, j];
					if (currentNr != 0)
					{
						// If the number in this field is not 0, we remove constraints in the same row, column and block

						for (int y = 0; y < 9; y++)
						{
							// Remove constraints in same row
							if (y == j) continue;
							constraints[((i, y), (i, y))].Remove((currentNr, currentNr));
						}
						for (int x = 0; x < 9; x++)
						{
							// Remove constraints in same column
							if (x == i) continue;
							constraints[((x, j), (x, j))].Remove((currentNr, currentNr));
						}

						// Get the list of indices of the block where the current number resides
						List<(int, int)> indicesSameBlock = blockIndices(i, j);
						foreach (var item in indicesSameBlock)
						{
							// For every index in this block, remove constraints
							if (item.Item1 == i && item.Item2 == j) continue;
							constraints[((item.Item1, item.Item2), (item.Item1, item.Item2))].Remove((currentNr, currentNr));
						}
					}
				}
			}
		}

		private List<int> returnTempList(int x)
		{
			List<int> tempxList = new List<int>();
            if (x == 0 || x == 1 || x == 2)
            {
                tempxList.Add(0);
                tempxList.Add(1);
                tempxList.Add(2);
            }
            else if (x == 3 || x == 4 || x == 5)
            {
				tempxList.Add(3);
				tempxList.Add(4);
				tempxList.Add(5);
            }else if (x == 6 || x == 7 || x == 8)
			{
				tempxList.Add(6);
				tempxList.Add(7);
				tempxList.Add(8);
			}
			return tempxList;
        }

		private void createSelfConstraints()
		{
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // If the number is not an assigned number, add this constraint to the constraints dictionary with as only value this assigned number
                    int currentNr = sudokuField[i, j];
                    if (currentNr != 0)
                    {
                        constraints.Add(((i, j), (i, j)), new List<(int, int)> { (currentNr, currentNr) });
                    }
                    else
                    {
                        // If not, the constraint Ci, i can be 1-9 initially, so we add this constraint with the values (1, 1) - (9, 9)
                        for (int n = 1; n <= 9; n++)
                        {
                            bool t = constraints.ContainsKey(((i, j), (i, j)));
                            // Quick check if the key already exists in the dictionary (and thus the list of values it can be)
                            if (t)
                            {
                                constraints[((i, j), (i, j))].Add((n, n));
                            }
                            else
                            {
                                constraints.Add(((i, j), (i, j)), new List<(int, int)> { (n, n) });
                            }
                        }
                    }
                }
            }
        }

		private void initializeDomainFields()
		{
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
					// Creating a list of ints as domain for a specific spot in the sudoku, and giving that list the values 1-9 as initial domain
                    domainField[i, j] = new List<int>();
					for (int n = 1; n <= 9; n++)
					{
						domainField[i, j].Add(n);
					}
                }
            }
        }


		public void printSudoku()
		{
			// Printing the sudoku by using fancy semantics
            string stringBuild = "+---------------------------------------+\n";
			for (int i = 0; i < 9; i++)
			{
                stringBuild += "|+-----------++-----------++-----------+|\n";
				if (i == 3 || i == 6) stringBuild += "|+-----------++-----------++-----------+|\n";
                stringBuild += "||";
				for (int j = 0; j < 9; j++)
				{
					int nr = sudokuField[i, j];
					if (nr != 0) stringBuild += " " + nr;
					else stringBuild += "  ";
					stringBuild += " |";
                    if (j == 2 || j == 5 || j == 8) stringBuild += "|";
					if (j == 8) stringBuild += "\n";
				}
			}
            stringBuild += "|+-----------++-----------++-----------+|\n";
            stringBuild += "+---------------------------------------+";
            Console.WriteLine(stringBuild);
        }
	}
}

