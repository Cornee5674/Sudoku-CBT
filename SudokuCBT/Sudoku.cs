using System;
namespace SudokuCBT
{
    public class Sudoku
    {
        int blockRows = 3;
        int blockColumns = 3;

        public SudokuBlock[,] field;

        public Sudoku(int[] list, bool print)
        {
            // We create a new empty sudokublock for every row and column
            field = new SudokuBlock[blockRows, blockColumns];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    field[i, j] = new SudokuBlock();
                }
            }
            // We initialise the values in the sudokublocks using the read intlist
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        field[i, j].block[row, 0] = list[((row * 9) + (j * 3)) + (i * 27)];
                        field[i, j].block[row, 1] = list[(((row * 9) + 1) + (j * 3)) + (i * 27)];
                        field[i, j].block[row, 2] = list[(((row * 9) + 2) + (j * 3)) + (i * 27)];
                    }
                }
            }
            if (print)
            {
                Console.WriteLine("Empty sudoku: ");
                printSudoku();
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

        public SudokuBlock(int[,] field, int[,] mask) //Used to generate new blocks from old ones
        {
            this.block = field;
            this.mask = mask;
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