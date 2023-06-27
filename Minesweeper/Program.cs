// made by VasutasIsti in 2023
// it's a little project, just for fun and learning
// if you Can't understand some of the notes, it's maybe because it's in hungarian... without the accent dots... good luck googling it.

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

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
            Console.WriteLine("Welcome to my ");
            bool stay = true;
            while (stay)
            {
                StartPlay();
                while (inGame)
                {
                    GuessCordinate();
                }
                Console.WriteLine("Game Over! (New Game starts in 5 seconds.)");
                Thread.Sleep(5000);
            }

            Console.ReadKey();
        }

        static void StartPlay()
        {
            board = new Board(size, difficulty);
            board.InitializeCells();
            board.VisalizeBoard("normal");
            board.PlaceBombs(FirstGuess());
            board.SetNeighbourCounts();
            board.VisalizeBoard("normal");
        }

        static int[] FirstGuess()
        {
            int[] solution = new int[2];
            int row = RequestRowFromPlayer();
            int col = RequestColFromPlayer();
            board.grid[row, col].isVisited = true;
            solution[0] = row;
            solution[1] = col;
            return solution;
        }

        static void GuessCordinate()
        {
            int row = RequestRowFromPlayer();
            int col = RequestColFromPlayer();
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
                board.grid[row, col].isVisited = true;
                board.VisalizeBoard("normal");
                return;
            }
            else    // ha ures cellara nyomtunk, akkor az osszes szomszédos ures cella minden szomszedjat meg kell jeleniteni.
            {
                board.grid[row, col].isVisited = true;
                // TODO
                board.VisalizeBoard("normal");
                return;
            }
        }

        static int RequestRowFromPlayer()
        {
            bool wrongAnswer = true;
            int row = 0;
            while (wrongAnswer)
            {
                try // row
                {
                    Console.Write("Adja meg a kiválasztott sor számát: ");
                    row = Convert.ToInt32(Console.ReadLine()) - 1;
                    if (row >= size) { continue; }
                }
                catch (Exception)
                {
                    continue;
                }
                wrongAnswer = false;
            }
            return row;
        }

        static int RequestColFromPlayer()
        {
            bool wrongAnswer = true;
            int col = 0;
            while (wrongAnswer)
            {
                try // col
                {
                    Console.Write("Adja meg a kiválasztott oszlop számát: ");
                    col = Convert.ToInt32(Console.ReadLine()) - 1;
                    if (col >= size) { continue; }
                }
                catch (Exception)
                {
                    continue;
                }
                wrongAnswer = false;
            }
            return col;
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
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    this.grid[i, j] = new Cell();
                }
            }
        }

        public void PlaceBombs(int[] startCoords)
        {
            int bombs = Convert.ToInt32(Math.Round((this.size * this.size) * this.difficulty));
            Random rnd = new Random();
            int row = 0, col = 0;
            for (int i = 0; i < bombs; i++)
            {
                row = rnd.Next(this.size);
                col = rnd.Next(this.size);
                if (!this.grid[row, col].isBomb && StartCoord(row, col, startCoords))
                {
                    this.grid[row, col].Bomb();
                }
                else
                {
                    i -=1;
                }
            }
        }

        public bool StartCoord(int row, int col, int[] startCoords)
        {
            if (row != startCoords[0] && col != startCoords[1]) { return true; }
            if (row != startCoords[0] && col == startCoords[1]) { return true; }
            if (row == startCoords[0] && col != startCoords[1]) { return true; }
            return false;
        }

        public void SetNeighbourCounts()
        {
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
