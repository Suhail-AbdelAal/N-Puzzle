﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace N_Puzzle
{
    public class A_star
    {
        public State current;
        private bool hamming;
        private bool manhattan;

        public A_star(State initial)
        {
            current = (State)initial.Clone();
        }

        public void solve()
        {
            var pq = new PriorityQueue<State, int>();
            var visited = new HashSet<string>();
            var timer = new Stopwatch();

            visited.Add(current.sb.ToString());

            timer.Start();
            while (!(current.isGoal()))
            {
                // Gettign the legal moves of the blank square
                List<char> moves = current.getMoves();

                // Each move is a child node 
                for (int i = 0; i < moves.Count; i++)
                {
                    // Generating a child 
                    int[,] newPuzzle = current.getnewPuzzle(moves[i]);
                    State newChild = new State(newPuzzle, current);

                    //if the child is visited we skip adding it's heuristic values to the priority queue
                    if (visited.Contains(newChild.sb.ToString())) 
                        continue;


                    current.addChild(newChild);
                    if (State.isHamming)
                        pq.Enqueue(newChild, newChild.hamming());
                    if (State.isManhattan)
                        pq.Enqueue(newChild, newChild.manhattan());

                }
                current = pq.Dequeue(); // Assigning the node with the minimum heuristic score to the current node
                visited.Add(current.sb.ToString()); // setting the current node as visited

            }
            timer.Stop();
            Console.WriteLine("Time in sceonds: " + timer.Elapsed + " s");
            Console.WriteLine("Time in Milliseconds: " + timer.ElapsedMilliseconds + " ms");
        }

        public void printNumOfSteps()
        {
            Console.WriteLine("# Steps: "+ current.getDepth());
            Console.WriteLine();
        }
        public void printSteps(State it)
        {
            if (it.getParent() == null)
                return;

            printSteps(it.getParent());

            it.display();
            Console.WriteLine();
        }


    }
}