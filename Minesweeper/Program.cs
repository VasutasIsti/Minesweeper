using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Minesweeper
{
    internal class Program
    {
        public static Board board;
        public static int size = 30;
        public static float difficulty = .15f;
        public static bool inGame = true;

        static void Main(string[] args)
        {
            StartPlay();
            while (inGame)
            {
                GuessCordinate();
            }
            Console.WriteLine("Game Over! (Press Enter to Exit)");

            Console.ReadKey();
        }

        static void StartPlay()
        {
            board = new Board(size, difficulty);
            board.InitializeCells();
            board.PlaceBombs();
            board.SetNeighbourCounts();
            board.VisalizeBoard("normal");
        }

        static void GuessCordinate()
        {
            Console.Write("Adja meg a kiválasztott sor számát: ");
            int row = Convert.ToInt32(Console.ReadLine());
            Console.Write("Adja meg a kiválasztott oszlop számát: ");
            int col = Convert.ToInt32(Console.ReadLine());
            if (board.grid[row, col].isVisited)
            {
                board.VisalizeBoard("normal");
                Console.WriteLine("Az adott koordinátát már megvizsgálta!");
                return;
            }
            else if (board.grid[row, col].isBomb)
            {
                board.VisalizeBoard("fail");
                inGame = false;
                return;
            }
            else if (board.grid[row, col].neighbours > 0)
            {
                // TODO
            }
            else    // ha ures cellara nyomtunk, akkor az osszes szomszédos ures cella minden szomszedjat meg kell jeleniteni.
            {
                // TODO
            }

        }
    }

    class Cell
    {
        public bool isVisited;
        public bool isBomb;
        public int neighbours;


        public Cell()
        {
            this.isVisited = false;
            this.isBomb = false;
            this.neighbours = 0;
        }
        public void Bomb()
        {
            this.isBomb = true;
            this.neighbours = -1;
        }
        public void SetNeighbourCount(int neighbours)
        {
            this.neighbours = neighbours;
        }
    }

    class Board
    {
        public int size;
        public Cell[,] grid;
        public double difficulty;                        // 10 - 90 % kozott
        public Board(int size, float difficulty)
        {
            this.size = size;
            this.grid = new Cell[size, size];
            this.difficulty = difficulty;
        }

        public void InitializeCells()
        {
            //Console.WriteLine("Board Initializing...");
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    this.grid[i, j] = new Cell();
                }
            }
            //Console.WriteLine("Board Init complete.");
        }

        public void PlaceBombs()
        {
            //Console.Write("Planting bombs...");
            int bombs = Convert.ToInt32(Math.Round((this.size * this.size) * this.difficulty));
            //Console.WriteLine("({0})", bombs.ToString());
            Random rnd = new Random();
            int row = 0, col = 0;
            for (int i = 0; i < bombs; i++)
            {
                row = rnd.Next(this.size);
                col = rnd.Next(this.size);
                if (!this.grid[row, col].isBomb)
                {
                    this.grid[row, col].Bomb();
                    //Console.WriteLine("{0}. bomb ({1}, {2})", i.ToString(), row.ToString(), col.ToString());
                }
                else
                {
                    i -=1;
                }
            }
            //Console.WriteLine("Bombs have been planted.");
        }

        public void SetNeighbourCounts()
        {
            //Console.WriteLine("Setting up board...");
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (!this.grid[i, j].isBomb)
                    {
                        this.grid[i, j].SetNeighbourCount(CountNeighbours(i,j));
                    }
                }
            }
            //Console.WriteLine("Board setup complete.");
        }

        public int CountNeighbours(int row, int col)
        {
            int count = 0;
            for (int i = row-1; i <= row+1; i++)
            {
                for (int j = col-1; j <= col+1; j++)
                {
                    if (i == row && j == col) { continue; }
                    try
                    {
                        if (this.grid[i, j].isBomb)
                        { count++; }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }

            return count;
        }

        public void VisalizeBoard(string mode)
        {
            string borderLine = "+";
            for (int i = 0; i < this.size; i++)
            {
                borderLine += "-+";
            }
            Console.WriteLine(borderLine);
            for (int i = 0; i < this.size; i++)
            {
                string line = "|";
                for (int j = 0; j < this.size; j++)
                {
                    switch (mode)
                    {
                        case "all":
                            if (this.grid[i,j].isBomb) { line += "*|"; }
                            else if (this.grid[i, j].neighbours > 0) { line += this.grid[i, j].neighbours.ToString() + "|"; }
                            else { line += " |"; }
                            break;
                        case "fail":
                            if (this.grid[i, j].isBomb) { line += "X|"; }
                            else if (this.grid[i, j].neighbours > 0) { line += this.grid[i, j].neighbours.ToString() + "|"; }
                            else { line += " |"; }
                            break;
                        default:
                            if (!this.grid[i, j].isVisited) { line += "?|"; }
                            else if (this.grid[i, j].isBomb) { line += "*|"; }
                            else if (this.grid[i, j].neighbours > 0) { line += this.grid[i, j].neighbours.ToString() + "|"; }
                            else { line += " |"; }
                            break;
                    }
                }
                Console.WriteLine(line);
                Console.WriteLine(borderLine);
            }
        }
    }
}
