using System;
namespace SudokuCBT
{
    public class Sudoku
    {
        int blockRows = 3;
        int blockColumns = 3;


        public SudokuBlock[,] field;
        // We create an identical structure but with domains and constraints
        public DomainConstraintBlock[,] dcField;

        public Sudoku(int[] list, bool print)
        {
            // We create a new empty sudokublock for every row and column
            field = new SudokuBlock[blockRows, blockColumns];

            // Initializing the sudoku and constraints/domains
            dcField = new DomainConstraintBlock[blockRows, blockColumns];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    field[i, j] = new SudokuBlock();
                    dcField[i, j] = new DomainConstraintBlock();
                }
            }

            // We initialise the values in the sudokublocks using the read intlist
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        // If the given number is not 0, change the constraint list in that spot to only that number since it is fixed
                        int nr1 = list[(row * 9) + (j * 3) + (i * 27)];
                        if (nr1 != 0) dcField[i, j].dc[row, 0].AddConstraintsIfGivenNumber(nr1);
                        field[i, j].block[row, 0] = nr1;

                        int nr2 = list[(row * 9) + 1 + (j * 3) + (i * 27)];
                        if (nr2 != 0) dcField[i, j].dc[row, 1].AddConstraintsIfGivenNumber(nr2);
                        field[i, j].block[row, 1] = nr2;

                        int nr3 = list[(row * 9) + 2 + (j * 3) + (i * 27)];
                        if (nr3 != 0) dcField[i, j].dc[row, 2].AddConstraintsIfGivenNumber(nr3);
                        field[i, j].block[row, 2] = nr3;
                    }
                }
            }
            if (print)
            {
                Console.WriteLine("Empty sudoku: ");
                printSudoku();
            }
            checkPartialSolution();
        }

        public bool checkPartialSolution()
        {
            // We use generic variable names in the for loops, since they are for different usecases, depending on if you check a row, column or block.
            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < 1; y++)
                {
                    // 3 dictionaries, one to store the counters of the rows, one for the columns and one for the blocks
                    Dictionary<int, int> tempDictColumn = new Dictionary<int, int>();
                    Dictionary<int, int> tempDictRow = new Dictionary<int, int>();
                    Dictionary<int, int> tempDictBlock = new Dictionary<int, int>();
                    for (int n = 0; n < 3; n++)
                    {
                        for (int m = 0; m < 3; m++)
                        {
                            // Check the columns
                            int nrColumn = field[n, x].block[m, y];
                            AddToDict(nrColumn, tempDictColumn);

                            // Check the rows
                            int nrRow = field[x, n].block[y, m];
                            AddToDict(nrRow, tempDictRow);

                            // Check the columns
                            int nrBlock = field[x, y].block[n, m];
                            AddToDict(nrBlock, tempDictBlock);
                        }
                    }

                    if (checkDictValue2(tempDictColumn) || checkDictValue2(tempDictRow) || checkDictValue2(tempDictBlock)) return false;
                }
            }
            return true;
        }

        private bool checkDictValue2(Dictionary<int, int> dict)
        {
            // Here we check if any key in the dictionary has a value of 2 or greater, if so, the sudoku is not a partial solution
            foreach(var item in dict)
            {
                if (item.Value > 1 && item.Key != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddToDict(int nr, Dictionary<int, int> dict)
        {
            // This dictionary is used as a counter, so if the dict contains a given number, we increase the count of this number.
            // If not, we add it to the dictionary
            if (dict.ContainsKey(nr))
            {
                dict[nr]++;
            }else
            {
                dict.Add(nr, 1);
            }
        }

        // Debug function to print all constraint lists of a block
        public void PrintConstraint(int row, int block)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.WriteLine("Constraint list:");
                    dcField[row, block].dc[i, j].PrintConstraintList();
                    //Console.WriteLine("Domain list:");
                    //dcField[row, block].dc[i, j].PrintDomainList();
                }
            }
            Console.WriteLine("-----------------------");
        }

        public void NodeConsistency()
        {
            // Remove constraints (from fixed numbers in the same row, column or block)
            RemoveConstraints();
            // Update the domains on all values
            UpdateDomains();
        }

        public void RemoveConstraints()
        {
            // For every spot in sudoku (seems like a wild nested for, but only runs 81 calculations (3 * 3 * 3 * 3))
            for (int rowBlock = 0; rowBlock < 3; rowBlock++)
            {
                for (int columnBlock = 0; columnBlock < 3; columnBlock++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        for (int column = 0; column < 3; column++)
                        {
                            int nr = field[rowBlock, columnBlock].block[row, column];
                            if (nr != 0)
                            {
                                RemoveConstraintsBlock(nr, rowBlock, columnBlock, row, column);
                                RemoveConstraintsRow(nr, rowBlock, row, columnBlock, column);
                                RemoveConstraintsColumn(nr, columnBlock, column, rowBlock, row);
                            }
                        }
                    }
                }
            }
        }

        public void RemoveConstraintsColumn(int nr, int columnBlock, int column, int exceptRowBlock, int exceptRow)
        {
            // For every spot in given column
            for (int rowBlock = 0; rowBlock < 3; rowBlock++)
            {
                // Remove found number from the constraint list, except if it has the index of the spot of the found number
                for (int row = 0; row < 3; row++)
                {
                    if (exceptRowBlock == rowBlock && exceptRow == row) continue;
                    else dcField[rowBlock, columnBlock].dc[row, column].RemoveConstraint(nr);
                }
            }
        }

        public void RemoveConstraintsRow(int nr, int rowBlock, int row, int exceptColumnBlock, int exceptColumn)
        {
            // For every spot in given row
            for (int columnBlock = 0; columnBlock < 3; columnBlock++)
            {
                for (int column = 0; column < 3; column++)
                {
                    // Remove found number from the constraint list, except if it has the index of the spot of the found number
                    if (exceptColumnBlock == columnBlock && exceptColumn == column) continue;
                    else dcField[rowBlock, columnBlock].dc[row, column].RemoveConstraint(nr);
                }
            }
        }

        public void RemoveConstraintsBlock(int nr, int rowBlock, int columnBlock, int exceptRow, int exceptColumn)
        {
            // For every spot in the given block
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    // Remove the found number from the constraint list, except if it has the index of the spot of the found number
                    if (exceptRow == row && exceptColumn == column) continue;
                    else dcField[rowBlock, columnBlock].dc[row, column].RemoveConstraint(nr);
                }
            }
        }

        public void UpdateDomains()
        {
            // For every spot in sudoku (seems like a wild nested for, but only runs 81 calculations (3 * 3 * 3 * 3))
            for (int rowBlock = 0; rowBlock < 3; rowBlock++)
            {
                for (int columnBlock = 0; columnBlock < 3; columnBlock++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        for (int column = 0; column < 3; column++)
                        {
                            // Updates the domain given the constraint list
                            dcField[rowBlock, columnBlock].dc[row, column].IntersectConstraintWithDomain();
                        }
                    }
                }
            }
        }


        public void printSudoku()
        {
            string stringBuild = "+---------------------------------------+\n";
            stringBuild += "|+-----------++-----------++-----------+|\n";
            // For every row of 3 blocks
            for (int x = 0; x < 3; x++)
            {
                // For every row in a block
                for (int j = 0; j < 3; j++)
                {
                    // For every block in the row of 3 blocks
                    for (int i = 0; i < 3; i++)
                    {
                        // Add the xth, ith block, in the jth row in that block, to the string
                        stringBuild += field[x, i].printBlock(j);
                        if (i == 2) stringBuild += "||\n";
                    }
                    stringBuild += "|+-----------++-----------++-----------+|\n";
                }
                if (x != 2) stringBuild += "|+-----------++-----------++-----------+|\n";
                else stringBuild += "+---------------------------------------+";
            }

            Console.WriteLine(stringBuild);
        }
    }

    public class DomainConstraintBlock
    {
        // Create a 2d array (3x3) identical to the sudokublock structure
        public DomainsAndConstraints[,] dc;
        int rows = 3;
        int columns = 3;

        public DomainConstraintBlock()
        {
            // Create a new domain and constraints class for every value
            dc = new DomainsAndConstraints[rows, columns];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    dc[i, j] = new DomainsAndConstraints();
                }
            }
        }
    }

    public class SudokuBlock
    {
        int rows = 3;
        int columns = 3;
        public int[,] block;
        // 1 is locked, 0 is unlocked in the mask array. Used to check if a number can be swapped
        int[,] mask;
        Random rnd;

        public SudokuBlock()
        {
            block = new int[rows, columns];
            mask = new int[rows, columns];
            rnd = new Random();

            // Creating of all the initial values in this block
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    block[i, j] = 0;
                    mask[i, j] = 0;
                }
            }
        }


        // Printing blocks
        internal string printBlock(int row)
        {
            // Debug printer function
            string toPrint = "|| ";
            for (int i = 0; i < columns; i++)
            {
                int x = block[row, i];
                if (x == 0) toPrint += "  ";
                else toPrint += x + " ";
                //toPrint += block[row, i] + " ";
                if (i != columns - 1)
                {
                    toPrint += "| ";
                }
            }
            return toPrint;
        }

        public void printBlock()
        {
            // Debug printer function
            string wholeString = "";
            for (int i = 0; i < 3; i++)
            {
                wholeString += printBlock(i) + "\n";
            }
            Console.WriteLine(wholeString);
        }
    }
}