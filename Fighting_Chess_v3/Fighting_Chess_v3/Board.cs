using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using static System.Console;
using NAudio.Wave;

namespace Fighting_Chess_v3
{
    // Klasa reprezentująca szachownicę.
    public class Board
    {
        public int Height { get; set; }        // Wysokość szachownicy.
        public int Width { get; set; }        // Szerokość szachownicy.
        public List<Characters> Characters { get; set; }        // Szerokość szachownicy.
        public List<Tuple<int, int>> Obstacles { get; set; }        // Szerokość szachownicy.
        public int HighlightedCharacterId { get; set; }        // Identyfikator zaznaczonej postaci.
        // Identyfikator zaznaczonej postaci.
        public Board(int height, int width, List<Characters> characters, List<Tuple<int, int>> obstacles, int highlightedCharacterId)
        {
            Height = height;
            Width = width;
            Characters = characters;
            Obstacles = obstacles;
            HighlightedCharacterId = highlightedCharacterId;
            // Iteracja po każdym wierszu szachownicy.
            for (int i = 0; i < height; i++)
            {
                // Iteracja po każdej kolumnie szachownicy.
                for (int x = 0; x < width; x++)
                {
                    bool playerPresent = false;
                    bool obstaclePresent = false;
                    // Sprawdzenie czy na danej pozycji znajduje się postać.
                    foreach (var character in Characters)
                    {
                        if (i == character.PositionY && x == character.PositionX)
                        {
                            // Zmiana koloru tekstu w zależności od typu postaci.
                            Console.ForegroundColor = character is ComputerCharacters ? ConsoleColor.Red : ConsoleColor.Green;
                            // Zaznaczenie postaci na planszy.
                            if (highlightedCharacterId != -1 && highlightedCharacterId == Array.IndexOf(Characters.ToArray(), character))
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                            }
                            // Wyświetlenie symbolu postaci.
                            if (character is not ComputerCharacters)
                            {
                                Console.Write($"[{character.Symbol}]");
                            }
                            else
                            {
                                Console.Write($"[{character.Symbol}]");
                            }

                            playerPresent = true;
                            Console.ResetColor();
                            break;
                        }
                    }
                    // Sprawdzenie czy na danej pozycji znajduje się przeszkoda.
                    foreach (var obstacle in Obstacles)
                    {
                        if (x == obstacle.Item1 && i == obstacle.Item2)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("[#]");
                            obstaclePresent = true;
                            Console.ResetColor();
                            break;
                        }
                    }
                    // Wyświetlenie pustego pola, jeśli nie ma postaci ani przeszkody.
                    if (!playerPresent && !obstaclePresent)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[ ]");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
