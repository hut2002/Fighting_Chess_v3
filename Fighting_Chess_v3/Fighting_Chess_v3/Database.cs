using MySql.Data.MySqlClient;
using System;
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
    // Klasa zawierająca statyczne pola i metody dostępu do danych gry
    public static class Database
    {
        public static int aliveComputerCharactersCount;             // Liczba żywych postaci kontrolowanych przez komputer
        public static int alivePlayerCharactersCount;               // Liczba żywych postaci kontrolowanych przez gracza
        public static DateTime startTime;                           // Czas rozpoczęcia gry
        public static DateTime endTime;                             // Czas zakończenia gry
        public static int enemiesdefeated;                          // Liczba pokonanych przeciwników
        public static string adminname;                             // Nazwa administratora
        public static string adminpasswd;                           // Hasło administratora
        // Wewnętrzna klasa sesji użytkownika przechowująca informacje o zalogowanym użytkowniku
        internal static class Session
        {
            public static long UserID { get; set; }
            public static bool IsLoggedIn { get; set; } = false;
        }
        // Metoda umożliwiająca aktualizację statystyk postaci
        public static void UpdateCharacterStatsMenu()
        {
            // Wybór opcji aktualizacji statystyk dla danej klasy postaci
            string prompt = "Wybierz opcję:";
            string[] options = { "Medyk", "Zwiadowca", "Obrońca", "Psionik", "Strzelec", "Powrót do menu" };
            MenuOn.Menu updateMenu = new MenuOn.Menu(prompt, options);
            int selectedIndex = updateMenu.Run();
            // Wybór opcji w menu aktualizacji statystyk postaci
            switch (selectedIndex)
            {
                case 0:
                    UpdateCharacterStats(0); // Aktualizacja statystyk Medyka
                    break;
                case 1:
                    UpdateCharacterStats(1); // Aktualizacja statystyk Zwiadowcy
                    break;
                case 2:
                    UpdateCharacterStats(2); // Aktualizacja statystyk Obrońcy
                    break;
                case 3:
                    UpdateCharacterStats(3); // Aktualizacja statystyk Psionika
                    break;
                case 4:
                    UpdateCharacterStats(4); // Aktualizacja statystyk Strzelca
                    break;
                case 5:
                    MenuOn.AdminPanelMenu(); // Powrót do menu głównego
                    break;
            }
        }
        // Metoda aktualizująca statystyki dla danej klasy postaci
        public static void UpdateCharacterStats(int characterId)
        {
            // Dane do połączenia z bazą danych
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = adminname;
            string passwd = adminpasswd;

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                // Połączenie z bazą danych
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    // Zapytanie SQL o aktualne statystyki postaci
                    string query = $"SELECT Moves, Hp, Damage, Ammo, Shield FROM character_stats WHERE Id = {characterId}";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Odczytanie aktualnych statystyk postaci
                        int moves = Convert.ToInt32(reader["Moves"]);
                        int hp = Convert.ToInt32(reader["Hp"]);
                        int damage = Convert.ToInt32(reader["Damage"]);
                        int ammo = Convert.ToInt32(reader["Ammo"]);
                        int shield = Convert.ToInt32(reader["Shield"]);

                        reader.Close();

                        // Wyświetlenie obecnych statystyk postaci
                        Console.WriteLine($"Obecne statystyki postaci (Id = {characterId}):");
                        Console.WriteLine($"1. Ruchy: {moves}");
                        Console.WriteLine($"2. Hp: {hp}");
                        Console.WriteLine($"3. Obrażenia: {damage}");
                        Console.WriteLine($"4. Ammo: {ammo}");
                        Console.WriteLine($"5. Tarcza: {shield}\n");
                        // Wprowadzenie nowych wartości statystyk postaci
                        Console.WriteLine("Wprowadź nowe wartości statystyk:");

                        Console.Write("1. Ruchy: ");
                        int newMoves = Convert.ToInt32(Console.ReadLine());

                        Console.Write("2. Hp: ");
                        int newHp = Convert.ToInt32(Console.ReadLine());

                        Console.Write("3. Obrażenia: ");
                        int newDamage = Convert.ToInt32(Console.ReadLine());

                        Console.Write("4. Ammo: ");
                        int newAmmo = Convert.ToInt32(Console.ReadLine());

                        Console.Write("5. Tarcza: ");
                        int newShield = Convert.ToInt32(Console.ReadLine());

                        // Zapytanie SQL aktualizujące statystyki postaci
                        string updateQuery = $"UPDATE character_stats SET Moves = {newMoves}, Hp = {newHp}, Damage = {newDamage}, Ammo = {newAmmo}, Shield = {newShield} WHERE Id = {characterId}";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            reader.Close();
                            Console.WriteLine("Dane postaci zostały pomyślnie zaktualizowane.");
                            Console.ReadKey();
                            MenuOn.AdminPanelMenu();
                        }
                        else
                        {
                            reader.Close();
                            Console.WriteLine("Nie udało się zaktualizować danych postaci.");
                            Console.ReadKey();
                            MenuOn.AdminPanelMenu();
                        }
                    }
                    else
                    {
                        reader.Close();
                        Console.WriteLine($"Nie znaleziono postaci o Id = {characterId}.");
                        Console.ReadKey();
                        MenuOn.AdminPanelMenu();
                    }
                }
            }
            // Metoda umożliwiająca aktualizację danych użytkownika
            catch (MySqlException ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas aktualizacji danych postaci: {ex.Message}");
                Console.ReadKey();
                MenuOn.AdminPanelMenu();
            }
        }
        // Metoda umożliwiająca aktualizację danych użytkownika
        public static void UpdateUserData()
        {
            // Wprowadzenie nazwy użytkownika do aktualizacji danych
            Console.WriteLine("\nPodaj nazwę urzytkownika do którego chcesz zaktualizować: ");
            string uname = Console.ReadLine();
            // Dane do połączenia z bazą danych
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = adminname;
            string passwd = adminpasswd;

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                // Dane do połączenia z bazą danych
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    // Zapytanie SQL o aktualne dane użytkownika
                    string query = $"SELECT Name, Password, E_Mail FROM users WHERE Name = '{uname}'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Odczytanie aktualnych danych użytkownika
                        string name = reader["Name"].ToString();
                        string password = reader["Password"].ToString();
                        string email = reader["E_Mail"].ToString();

                        reader.Close();
                        // Wyświetlenie obecnych danych użytkownika
                        Console.WriteLine($"Obecne dane użytkownika (Nazwa użytkownika: {uname}):");
                        Console.WriteLine($"1. Imie: {name}");
                        Console.WriteLine($"2. Hasło: {password}");
                        Console.WriteLine($"3. E-Mail: {email}");
                        // Wprowadzenie nowych danych użytkownika
                        Console.WriteLine("\nWprowadź nowe dane użytkownika:");

                        Console.Write("1. Imie: ");
                        string newName = Console.ReadLine();

                        Console.Write("2. Hasło: ");
                        string newPassword = Console.ReadLine();

                        Console.Write("3. E-Mail: ");
                        string newEmail = Console.ReadLine();

                        // Aktualizacja danych w bazie danych
                        string updateQuery = $"UPDATE users SET Name = '{newName}', Password = '{newPassword}', E_Mail = '{newEmail}' WHERE Name = '{newName}'";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Dane użytkownika zostały pomyślnie zaktualizowane.");
                            Console.ReadKey();
                            MenuOn.AdminPanelMenu();

                        }
                        else
                        {
                            Console.WriteLine("Nie udało się zaktualizować danych użytkownika.");
                            Console.ReadKey();
                            MenuOn.AdminPanelMenu();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Nie znaleziono użytkownika o nazwie = {uname}.");
                        Console.ReadKey();
                        MenuOn.AdminPanelMenu();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas aktualizacji danych użytkownika: {ex.Message}");
                Console.ReadKey();
                MenuOn.AdminPanelMenu();
            }
        }
        // Metoda umożliwiająca logowanie administratora
        public static void AdminPanel()
        {
            // Wprowadzenie danych logowania administratora
            Console.Clear();
            Console.WriteLine("Podaj nazwę użytkownika:");
            string name = Console.ReadLine();
            adminname = name;
            Console.WriteLine("Podaj hasło:");
            string password = ReadPassword();
            adminpasswd = password;

            // Sprawdzenie poprawności danych logowania
            if (CheckAdminPanel(name, password))
            {
                Console.WriteLine("Logowanie powiodło się.");
                Console.ReadKey();
                Console.Clear();
                MenuOn.AdminPanelMenu();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Logowanie nie powiodło się.");
                Console.ReadKey();
                Console.Clear();
            }
            // Metoda do wprowadzania hasła bez wyświetlania jego treści
            static string ReadPassword()
            {
                string password = string.Empty;
                ConsoleKeyInfo key;
                // Pętla wprowadzania hasła z ukrywaniem jego treści
                do
                {
                    key = ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password += key.KeyChar;
                        Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Write("\b \b");
                    }
                } while (key.Key != ConsoleKey.Enter);
                WriteLine("");
                return password;
            }
        }
        // Metoda sprawdzająca poprawność danych logowania administratora
        public static bool CheckAdminPanel(string name, string password)
        {
            string server = "localhost";
            string database = "mysql";
            string username = name;
            string passwd = password;

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                // Połączenie z bazą danych
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    // Zapytanie SQL sprawdzające uprawnienia administratora
                    string query = $"SELECT COUNT(*) FROM mysql.db WHERE user = @username AND Grant_priv = 'Y';";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", name);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Wprowadzono błędne dane");
                Console.ReadKey();
                return false;
            }
        }
        // Metoda umożliwiająca rejestrację nowego użytkownika
        public static void Register()
        {
            Clear();
            WriteLine("Podaj nazwę użytkownika:");
            string name = ReadLine();
            WriteLine("Podaj hasło:");
            string password = ReadPassword();
            WriteLine("Podaj E-Mail:");
            string mail = ReadLine();
            // Sprawdzenie poprawności danych rejestracji
            if (CheckRegister(name, password, mail))
            {
                WriteLine("Rejestracja powiodła się. Naciśnij dowolny przycisk aby kontynuować.");
                ReadLine();
                MenuOn.Main_Menu();
            }
            else
            {
                Clear();
                WriteLine("Rejestracja nie powiodła się, podałeś niespójne  dane. Spróbuj ponownie.");
                Register();
            }
            // Metoda do wprowadzania hasła bez wyświetlania jego treści
            static string ReadPassword()
            {
                string password = string.Empty;
                ConsoleKeyInfo key;

                do
                {
                    key = ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password += key.KeyChar;
                        Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Write("\b \b");
                    }
                } while (key.Key != ConsoleKey.Enter);
                WriteLine("");
                return password;
            }
        }

        public static bool CheckRegister(string name, string password, string mail)
        {
            // Połączenie z bazą danych
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string passwd = "";

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";
            // Otwarcie połączenia z bazą danych
            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    // Rozpoczęcie transakcji
                    var transaction = conn.BeginTransaction();
                    // Wstawienie nowego użytkownika do tabeli Users
                    var query = new MySqlCommand("INSERT INTO Users (Name, Password, E_mail) VALUES (@name, @password, @mail);", conn, transaction);
                    query.Parameters.AddWithValue("@name", name);
                    query.Parameters.AddWithValue("@password", password);
                    query.Parameters.AddWithValue("@mail", mail);
                    query.ExecuteNonQuery();
                    // Pobranie identyfikatora nowego użytkownika
                    long userId = query.LastInsertedId;
                    // Wstawienie rekordu do tabeli HallOfFame dla nowego użytkownika
                    var hallOfFameCommand = new MySqlCommand("INSERT INTO HallOfFame (User_ID, TotalTimeSpent, EnemiesDefeated, GamesPlayed) VALUES (@userId, '00:00:00', 0, 0);", conn, transaction);
                    hallOfFameCommand.Parameters.AddWithValue("@userId", userId);
                    hallOfFameCommand.ExecuteNonQuery();
                    // Zatwierdzenie transakcji
                    transaction.Commit();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
                return false;
            }
        }


        public static void Login()
        {
            Console.Clear();
            Console.WriteLine("Podaj nazwę użytkownika:");
            string name = Console.ReadLine();
            Console.WriteLine("Podaj hasło:");
            string password = ReadPassword();

            // Sprawdzenie poprawności danych logowania
            if (CheckLogin(name, password))
            {
                Console.WriteLine("Logowanie powiodło się.");
                Console.ReadKey();
                Console.Clear();
                GameLogic.Game_Start();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Logowanie nie powiodło się.");
                Console.ReadKey();
                Console.Clear();

                MenuOn.Main_Menu();


            }
            // Funkcja do wczytywania hasła z konsoli
            static string ReadPassword()
            {
                string password = string.Empty;
                ConsoleKeyInfo key;

                do
                {
                    key = ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password += key.KeyChar;
                        Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Write("\b \b");
                    }
                } while (key.Key != ConsoleKey.Enter);
                WriteLine("");
                return password;
            }
        }

        public static bool CheckLogin(string name, string password)
        {
            // Połączenie z bazą danych
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string passwd = "";

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                // Otwarcie połączenia z bazą 
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    // Zapytanie sprawdzające poprawność danych logowania
                    string query = "SELECT User_ID, Password FROM Users WHERE Name = @name;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    // Wykonanie zapytania
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string retrievedPassword = reader["Password"].ToString();
                            int userID = Convert.ToInt32(reader["User_ID"]);
                            // Sprawdzenie czy hasło zgadza się z tym w bazie danych
                            if (retrievedPassword == password)
                            {
                                // Przypisanie ID użytkownika i ustawienie flagi zalogowania
                                Session.UserID = userID;
                                Session.IsLoggedIn = true;
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Obsługa błędu w przypadku problemu z bazą 
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
                return false;
            }
        }

        public static void HallofFame()
        {
            // Połączenie z bazą 
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string password = "";

            string connectionString = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={password};";

            try
            {
                // Otwarcie połączenia z bazą danych
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Zapytanie pobierające dane z Salii Chwały
                    string query = @" SELECT u.Name, h.TotalTimeSpent, h.EnemiesDefeated, h.GamesPlayed, h.BestTime FROM HallOfFame h JOIN Users u ON h.User_ID = u.User_ID ORDER BY h.GamesPlayed DESC, h.EnemiesDefeated DESC, h.TotalTimeSpent ASC;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    // Wykonanie zapytania
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.Clear();
                        Console.WriteLine("Sala Chwały:");
                        Console.WriteLine("Nazwa | Łączny czas w grze | Pokonani przeciwnicy | Zagrane gry | Najlepszy czas");
                        Console.WriteLine(new string('-', 80));
                        // Wyświetlenie wyników
                        while (reader.Read())
                        {
                            string name = reader["Name"].ToString();
                            string totalTimeSpent = reader["TotalTimeSpent"].ToString();
                            int enemiesDefeated = reader.GetInt32("EnemiesDefeated");
                            int gamesPlayed = reader.GetInt32("GamesPlayed");
                            string bestTime = reader["BestTime"].ToString();

                            Console.WriteLine($"{name,5} | {totalTimeSpent,17} | {enemiesDefeated,16} | {gamesPlayed,12} | {bestTime,9}");
                        }

                        Console.WriteLine("Wciśnij ENTER aby zamknąć");
                        Console.ReadLine();
                        Console.Clear();
                        MenuOn.Main_Menu();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Obsługa błędu w przypadku problemu z bazą danych
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
            }
        }
    }
}
