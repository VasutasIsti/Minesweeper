﻿// made by VasutasIsti in 2023
// it's a little project, just for fun and learning
// if you Can't understand some of the notes, it's maybe because it's in hungarian... without the accent dots... good luck googling it.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Minesweeper
{
    internal class Program
    {
        public static Board board;
        public static int size = 12;
        public static float difficulty = .15f;          // it's the percentage of bombs on the board.
        public static bool inGame = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my little Minesweeper Clone\nIt's in an early stage, so my programing skills");
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
            inGame = true;
            board = new Board(size, difficulty);
            board.InitializeCells();
            board.VisualizeBoard("normal");
            int[] firstGuess = FirstGuess();
            board.PlaceBombs(firstGuess);
            board.SetNeighbourCounts();
            board.grid[firstGuess[0], firstGuess[1]].isVisited= true;
            if (board.grid[firstGuess[0], firstGuess[1]].neighbours == 0)
            { 
                VisitNeighbours(firstGuess[0], firstGuess[1]); 
            }
            board.VisualizeBoard("normal");
        }

        static int[] FirstGuess()
        {
            int[] solution = new int[2];
            int row = RequestCoordFromPlayer(true);
            int col = RequestCoordFromPlayer(false);
            solution[0] = row;
            solution[1] = col;
            return solution;
        }

        static List<Cell> cells = new List<Cell>();

        static void GuessCordinate()
        {
            int row = RequestCoordFromPlayer(true);
            int col = RequestCoordFromPlayer(false);
            if (board.grid[row, col].isVisited)
            {
                board.VisualizeBoard("normal");
                Console.WriteLine("Az adott koordinátát már megvizsgálta!");
                return;
            }
            else if (board.grid[row, col].isBomb)
            {
                board.VisualizeBoard("fail");
                inGame = false;
                return;
            }
            else if (board.grid[row, col].neighbours > 0)
            {
                board.grid[row, col].isVisited = true;
                board.VisualizeBoard("normal");
                return;
            }
            else    // ha ures cellara nyomtunk, akkor az osszes szomszédos ures cella minden szomszedjat meg kell jeleniteni.
            {
                board.grid[row, col].isVisited = true;
                cells.Clear();
                VisitNeighbours(row, col);
                board.VisualizeBoard("normal");
                return;
            }
        }
        public static void VisitNeighbours(int row, int col)
        {
            for (int i = row-1; i <= row+1; i++)
            {
                for (int j = col-1; j <= col+1; j++)
                {
                    if (i==row && j==col) {continue;}
                    try
                    {
                        if (!cells.Contains(board.grid[i, j]) && !board.grid[i, j].isVisited)
                        {
                            if (board.grid[i, j].neighbours == 0)
                            {
                                cells.Add(board.grid[i, j]);
                                board.grid[i, j].isVisited = true;
                                VisitNeighbours(i, j);
                            }
                            else if (board.grid[i,j].neighbours>0)
                            {
                                cells.Add(board.grid[i, j]);
                                board.grid[i, j].isVisited = true;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    { continue; }
                }
            }
        }

        static int RequestCoordFromPlayer(bool isRow)
        {
            bool wrongAnswer = true;
            int coord = 0;
            while (wrongAnswer)
            {
                try // row
                {
                    if (isRow) { Console.Write("Adja meg a sor számát: "); }
                    else { Console.Write("Adja meg a kiválasztott oszlop számát: "); }
                    coord = Convert.ToInt32(Console.ReadLine()) - 1;
                    if (coord >= size) { continue; }
                }
                catch (Exception)
                {
                    continue;
                }
                wrongAnswer = false;
            }
            return coord;
        }
    }

    class Cell
    {
        public bool isVisited;
        public bool isBomb;
        public int neighbours;
        public bool isFlagged = false;

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
        public double difficulty;                        // 5 - 25 % kozott ajanlott, utobbi mar nagyon suru
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

        public void VisualizeBoard(string mode)
        {
            string coordinates = " ";
            string borderLine = " +";
            for (int i = 1; i <= this.size; i++)
            {
                coordinates += " " + (i % 10).ToString();
                borderLine += "-+";
            }
            Console.WriteLine(coordinates+"\n"+borderLine);
            for (int i = 0; i < this.size; i++)
            {
                string line = ((i + 1) % 10).ToString() + "|";
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
                line += ((i + 1) % 10).ToString();
                Console.WriteLine(line);
                Console.WriteLine(borderLine);
            }
            Console.WriteLine(coordinates);
        }
    }
}
