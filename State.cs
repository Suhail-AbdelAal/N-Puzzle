﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Puzzle
{
    public class State
    {
        private static int SIZE;

        public int[,] puzzle;
        private State parent;
        private List<State> children;
        private KeyValuePair<int, int> zeroPos;
        private char lastMove;
        private int depth;
        private int hammingDistance;
        private int manhattanDistance;
             
        public State(int[,] tiles, State? parent = null)
        {
            SIZE = tiles.GetLength(0);

            this.puzzle = new int[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    this.puzzle[i, j] = tiles[i, j];
            
            this.parent = parent;
            this.children = new List<State>();
            this.lastMove = '0';
            if (this.parent == null)
            {
                depth = 0;
                for (int i = 0; i < SIZE; i++)
                    for (int j = 0; j < SIZE; j++)
                        if (tiles[i, j] == 0)
                            zeroPos = new KeyValuePair<int, int>(i, j);
            }
            else
            {
                depth = this.parent.getDepth() + 1;
                zeroPos = this.parent.getZeroPos();
                switch (this.parent.getLastMove())
                {
                    case 'U':
                        zeroPos = new KeyValuePair<int, int> (zeroPos.Key - 1, zeroPos.Value);
                        break;
                    case 'D':
                        zeroPos = new KeyValuePair<int, int>(zeroPos.Key + 1, zeroPos.Value);
                        break;
                    case 'R':
                        zeroPos = new KeyValuePair<int, int>(zeroPos.Key, zeroPos.Value + 1);
                        break;
                    case 'L':
                        zeroPos = new KeyValuePair<int, int>(zeroPos.Key, zeroPos.Value - 1);
                        break;
                }
            }
            hammingDistance = hamming();
            manhattanDistance = manhattan();
        }

        public void setDepth(int depth)
        {
            this.depth = depth;
        }

        public void setZeroPos(int i, int j)
        {
            zeroPos = new KeyValuePair<int, int>(i, j);
        }

        public int getDepth()
        {
            return depth;
        }

        public State getParent()
        {
            return parent;
        }

        public KeyValuePair<int, int> getZeroPos()
        {
            return zeroPos;
        }

        public int at(int i, int j)
        {
            return puzzle[i, j];
        }

        public static string getStringPuzzle(int[,] p)
        {
            string puzz = "";
            for (int i=  0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                    puzz += p[i, j].ToString();
            }
            return puzz;
        }


        public bool isSolvable()
        {
            // Copying the 2D puzzle into 1D array
            int[] temp = new int[SIZE * SIZE];
            int ind = 0;
            for (int i = 0; i < SIZE; i++) 
                for (int j = 0; j < SIZE; j++)
                    temp[ind++] = puzzle[i, j]; 

            // Calculate number of inverstions
            int inverstions = 0;
            for (int i = 0; i < SIZE * SIZE - 1; i++)
            {   
                for (int j = i + 1;j < SIZE * SIZE; j++)
                {
                    // Ignoring comparision with the blank square
                    if (temp[i] == 0 || temp[j] == 0)
                        continue;

                    // Checking inverstion pairs
                    /*if (temp[i] > temp[j] && temp[i] != 0 && temp[j] != 0)
                        ++inverstions;*/

                    if (temp[i] > temp[j])
                        ++inverstions;

                    /*  if (puzzle[j, i] > 0 && puzzle[j, i] > puzzle[i, j])
                          ++inverstions;*/
                }
            }

            if (SIZE % 2 != 0)
                return (inverstions % 2 == 0);

            int blankRow = getZeroRow();

            // For all 'blacnkRow' Even and 'inverstions' Odd is solvable, and vice versa
            if (blankRow % 2 == 0 && inverstions % 2 != 0)
                return true;

            if (blankRow % 2 != 0 && inverstions % 2 == 0)
                return true;

            // Any other case is not solvable
            return false;
        }

        // Getting the row position of the blank square starting from the bottom
        private int getZeroRow()
        {
            for (int i = SIZE - 1; i >= 0; i--)
                for (int j = SIZE - 1; j >= 0; j--)
                    if (puzzle[i, j] == 0)
                        return  - i;

            return -1; // Impossible case
        }

        private int manhattan()
        {
            int manhattanSum = 0;
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (puzzle[i, j] == 0) continue;
                    int goalRow = (puzzle[i, j] - 1) / SIZE;
                    int goalCol = (puzzle[i, j] - 1) % SIZE;
                    manhattanSum += Math.Abs(goalRow - i) + Math.Abs(goalCol - j);
                }
            }
            return manhattanSum + depth;
        }

        public int getManhattanDist()
        {
            return manhattanDistance;
        }

        private int hamming()
        {
            int hammingSum = 0;
            int factor = (puzzle[0, 0] == 0) ? 0 : 1;
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (puzzle[i, j] == 0) continue;
                    int pos = (i * SIZE) + j + factor;
                    if (puzzle[i, j] != pos)
                        hammingSum++;
                }
            }
            return hammingSum + depth;
        }

        public int getHammingDist()
        {
            return hammingDistance;
        }

        public bool isGoal()
        {
            return (manhattanDistance - depth == 0);
        }

        public int[,] getnewPuzzle(char direction)
        {
            int[,] newPuzzle = new int[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    newPuzzle[i, j] = puzzle[i, j];

            int x = getZeroPos().Key;
            int y = getZeroPos().Value;

            switch (direction)
            {
                // Swapping Up
                case 'U':
                    {
                        newPuzzle[x, y] = newPuzzle[x - 1, y];
                        newPuzzle[x - 1, y] = 0;
                    }
                    break;
                // Swapping Down
                case 'D':
                    {
                        newPuzzle[x, y] = newPuzzle[x + 1, y];
                        newPuzzle[x + 1, y] = 0;
                    }
                    break;
                // Swapping Right
                case 'R':
                    {
                        newPuzzle[x, y] = newPuzzle[x, y + 1];
                        newPuzzle[x, y + 1] = 0;
                    }
                    break;
                // Swapping Left
                case 'L':
                    {
                        newPuzzle[x, y] = newPuzzle[x, y - 1];
                        newPuzzle[x, y - 1] = 0;
                    }
                    break;
            }
            lastMove = direction;
            return newPuzzle;
        }

        public void addChild(State child)
        {
            children.Add(child);
        }

        public State getChild(int i)
        {
            return children[i];
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public List<State> getChildren()
        {
            return children;
        }

        public List<char> getMoves()
        {
            List<char> moves = new List<char>();
            int i = zeroPos.Key;
            int j = zeroPos.Value;

            if (i - 1 >= 0)
                moves.Add('U');

            if (i + 1 < SIZE)
                moves.Add('D');

            if (j - 1 >= 0)
                moves.Add('L');

            if (j + 1 < SIZE)
                moves.Add('R');

            return moves;
        }
        
        public char getLastMove()
        {
            return lastMove;
        }

        public void setLastMove(char lastMove)
        {
            this.lastMove = lastMove;
        }

        public void display()
        {
            Console.WriteLine("# " + this.depth);
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                    Console.Write(puzzle[i, j] + " ");
                Console.WriteLine();
            }
        }

        public static int size()
        {
            return SIZE;
        }
    }
}