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
    public class GameLogic
    {
        // Metoda rozpoczynająca grę
        public static void Game_Start()
        {
            // Wybór rozmiaru planszy
            string mapSize = "Wybierz rozmiar planszy";
            string[] choice = { "10x10", "15x12", "20x20", "30x30" };
            MenuOn.Menu sizeOfMap = new MenuOn.Menu(mapSize, choice);
            int maps = sizeOfMap.Run();
            int mapY = 10;
            int mapX = 10;

            switch (maps)
            {
                // Ustawienie wymiarów planszy w zależności od wyboru 
                case 0:
                    mapY = 10; //100
                    mapX = 10;
                    break;

                case 1:
                    mapY = 15; //180
                    mapX = 12;
                    break;

                case 2:
                    mapY = 20; //400
                    mapX = 20;
                    break;

                case 3:
                    mapY = 30; //900
                    mapX = 30;
                    break;
            }
            // Wybór ilości postaci
            string charPrompt = "Wybierz ilość postaci: ";
            string[] quantity = { "Jedna", "Dwie", "Trzy", "Cztery" };
            MenuOn.Menu charMenu = new MenuOn.Menu(charPrompt, quantity);
            int chars = charMenu.Run();
            int character_quantity = 1;

            switch (chars)
            {
                case 0:
                    character_quantity = 1;
                    break;

                case 1:
                    character_quantity = 2;
                    break;

                case 2:
                    character_quantity = 3;
                    break;

                case 3:
                    character_quantity = 4;
                    break;
            }
            List<Characters> characters = new List<Characters>();
            Console.Clear();
            // Inicjalizacja liczników postaci i przeciwników
            Database.aliveComputerCharactersCount = character_quantity;
            Database.alivePlayerCharactersCount = character_quantity;
            Database.enemiesdefeated = character_quantity;
            // Wybór klasy dla każdej postaci
            for (int i = 0; i < character_quantity; i++)
            {
                Console.WriteLine($"Wybierz klasę dla postaci nr {i + 1}:");
                Console.WriteLine("1. Medyk\n2. Zwiadowca\n3. Obrońca\n4. Psionik\n5. Strzelec");
                string charClass = $"Wybierz klasę dla postaci nr {i + 1}: ";
                string[] classes = { "Medyk", "Zwiadowca", "Obrońca", "Psionik", "Strzelec" };
                MenuOn.Menu charClassMenu = new MenuOn.Menu(charClass, classes);
                int characterType = charClassMenu.Run();

                string server = "localhost";
                string database = "fighting_chess_v3";
                string username = "root";
                string passwd = "";

                string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

                try
                {
                    // Pobranie danych o parametrach postaci z bazy danych
                    using (MySqlConnection conn = new MySqlConnection(constring))
                    {
                        conn.Open();

                        string db_moves = @$"SELECT Moves FROM character_stats WHERE id={characterType}";
                        int par_moves = GetIntFromQuery(conn, db_moves);
                        string db_hp = @$"SELECT Hp FROM character_stats WHERE id={characterType}";
                        int par_hp = GetIntFromQuery(conn, db_hp);
                        string db_damage = @$"SELECT Damage FROM character_stats WHERE id={characterType}";
                        int par_damage = GetIntFromQuery(conn, db_damage);
                        string db_ammo = @$"SELECT Ammo FROM character_stats WHERE id={characterType}";
                        int par_ammo = GetIntFromQuery(conn, db_ammo);
                        string db_shield = @$"SELECT Shield FROM character_stats WHERE id={characterType}";
                        int par_shield = GetIntFromQuery(conn, db_shield);

                        // Dodanie postaci do listy
                        switch (characterType)
                        {
                            case 0:
                                characters.Add(new Apothecary(0, i, (char)('1' + i), par_moves, par_hp, par_damage, par_ammo, par_shield));
                                break;
                            case 1:
                                characters.Add(new Scout(0, i, (char)('1' + i), par_moves, par_hp, par_damage, par_ammo, par_shield));
                                break;
                            case 2:
                                characters.Add(new Guard(0, i, (char)('1' + i), par_moves, par_hp, par_damage, par_ammo, par_shield));
                                break;
                            case 3:
                                characters.Add(new Psionic(0, i, (char)('1' + i), par_moves, par_hp, par_damage, par_ammo, par_shield));
                                break;
                            case 4:
                                characters.Add(new Gunner(0, i, (char)('1' + i), par_moves, par_hp, par_damage, par_ammo, par_shield));
                                break;
                        }
                    }
                    // Metoda pomocnicza pobierająca wartość int z zapytania do bazy danych
                    int GetIntFromQuery(MySqlConnection conn, string query)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            object result = cmd.ExecuteScalar();
                            return result != null ? Convert.ToInt32(result) : 0;
                        }
                    }

                }
                catch (MySqlException)
                {
                    WriteLine("Coś poszło nie tak należy zrestartować aplikacje. Wciśnij dowolny przycisk, aby kontynuować");
                    ReadLine();
                }
            }
            // Zapisanie czasu rozpoczęcia gry
            Database.startTime = DateTime.Now;
            // Dodanie przeciwników
            for (int j = 0; j < character_quantity; j++)
            {
                int randomX = new Random().Next(0, mapX - 1);
                int randomY = new Random().Next((mapY - 1) / 2, mapY - 1);
                characters.Add(new ComputerCharacters(randomX, randomY, (char)('A' + j), 6, 7, 3, 3, 2));
            }
            // Wygenerowanie przeszkód na planszy
            List<Tuple<int, int>> obstacles = GenerateRandomObstacles(mapX - 4, mapY - 4, (mapY * mapX) / 20, characters);
            Movement movement = new Movement(mapY, mapX, characters, obstacles);
        }
        // Metoda generująca losowe przeszkody na planszy
        private static List<Tuple<int, int>> GenerateRandomObstacles(int height, int width, int obstacleCount, List<Characters> characters)
        {
            List<Tuple<int, int>> obstacles = new List<Tuple<int, int>>();
            Random rnd = new Random();
            for (int i = 0; i < obstacleCount; i++)
            {
                int obstacleX = rnd.Next(width);
                int obstacleY = rnd.Next(height);
                // Sprawdzenie czy wylosowane współrzędne nie pokrywają się z położeniem postaci
                while (characters.Any(c => c.PositionX == obstacleX && c.PositionY == obstacleY))
                {
                    obstacleX = rnd.Next(width);
                    obstacleY = rnd.Next(height);
                }
                obstacles.Add(new Tuple<int, int>(obstacleX, obstacleY));
            }
            return obstacles;
        }
        // Metoda kończąca grę
        public static void EndGame()
        {
            // Obliczenie czasu trwania sesji
            TimeSpan sessionDuration = Database.endTime - Database.startTime;
            string sessionDurationSTR = sessionDuration.ToString(@"hh\:mm\:ss");

            Console.Clear();
            // Połączenie z bazą danych
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string passwd = "";

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";
            string query = @"UPDATE halloffame SET GamesPlayed = GamesPlayed + 1, EnemiesDefeated = EnemiesDefeated + @enemiesDefeated, BestTime = CASE WHEN @sessionDuration < BestTime OR BestTime = '00:00:00' THEN @sessionDuration ELSE BestTime END, TotalTimeSpent = ADDTIME(TotalTimeSpent, @sessionDuration) WHERE User_ID = @userId;";

            try
            {
                // Wykonanie zapytania do bazy danych
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Dodanie parametrów do zapytania
                        cmd.Parameters.AddWithValue("@userId", Database.Session.UserID);
                        cmd.Parameters.AddWithValue("@sessionDuration", sessionDurationSTR);
                        cmd.Parameters.AddWithValue("@enemiesDefeated", Database.enemiesdefeated);


                        int resaults = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Zaktualizowano {resaults} rekordów.");
                        Console.WriteLine("Przeszedłeś mapę !!!");
                        Console.WriteLine($"Czas trwania sesji: {sessionDuration}");
                        Console.WriteLine($"Czas trwania sesji STR: {sessionDurationSTR}");
                        Console.WriteLine($"Zabito : {Database.enemiesdefeated} wrogów");
                        Console.WriteLine($@"Wciśnij dowolny przycisk, aby kontynuować");
                        Console.ReadKey();
                    }
                }
            }
            // Obsługa błędów połączenia z bazą danych
            catch (MySqlException ex)
            {
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
            Console.Clear();
        }
        // Metoda zamykająca program
        public static void Exit()
        {

            Console.WriteLine("Dzięki za wsparcie!");
            Environment.Exit(0);
        }
    }
}
