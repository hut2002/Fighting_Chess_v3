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
    internal class Program
    {
        static int aliveComputerCharactersCount;
        static int alivePlayerCharactersCount;
        static DateTime startTime;
        static DateTime endTime;
        static int enemiesdefeated;
        static string adminname;
        static string adminpasswd;

        internal static class Session
        {
            public static long UserID { get; set; }
            public static bool IsLoggedIn { get; set; } = false;
        }

        static void Main(string[] args)
        {
            Music();
        }

        static void Main_Menu()
        {
            string gamePrompt = "Wybierz opcję: ";
            string[] options = { "Logowanie", "Zacznij grę bez logowania", "Tablica Chwały", "Zarejestruj się", "Panel Administracyjny", "Instrukcja Gry", "Wyjdź" };
            Menu charMenu = new Menu(gamePrompt, options);
            int selectedIndex = charMenu.Run();

            Console.CursorVisible = false;

            switch (selectedIndex)
            {
                case 0:
                    Login();
                    break;
                case 1:
                    Game_Start();
                    break;
                case 2:
                    HallofFame();
                    break;
                case 3:
                    Register();
                    break;
                case 4:
                    AdminPanel();
                    break;
                case 5:
                    ShowInstructions();
                    break;
                case 6:
                    Exit();
                    break;
            }
        }

        static void ShowInstructions()
        {
            Console.Clear();
            Console.WriteLine("Instrukcja gry - Fighting Chess");
            Console.WriteLine();
            Console.WriteLine("Wprowadzenie:");
            Console.WriteLine("Gra 'Fighting Chess' to strategiczna gra turowa, w której gracze kontrolują różne postacie na planszy, starając się pokonać wrogów. Każda postać ma unikalne zdolności i statystyki, które wpływają na jej ruchy i zdolności bojowe.");
            Console.WriteLine();
            Console.WriteLine("Cel gry:");
            Console.WriteLine("- Pokonaj wszystkie postacie kontrolowane przez komputer.");
            Console.WriteLine("- Przeżyj, kontrolując swoje postacie, aż wszystkie postacie wroga zostaną pokonane.");
            Console.WriteLine();
            Console.WriteLine("Rozgrywka:");
            Console.WriteLine("Przygotowanie do gry:");
            Console.WriteLine("1. Plansza: Gra odbywa się na planszy o określonej wysokości i szerokości.");
            Console.WriteLine("2. Postacie: Każda postać ma swoje statystyki, takie jak punkty ruchu (Moves), punkty życia (Hp), obrażenia (Damage), amunicję (Ammo) i tarczę (Shield).");
            Console.WriteLine("3. Przeszkody: Na planszy znajdują się przeszkody, które blokują ruch postaci.");
            Console.WriteLine();
            Console.WriteLine("Postacie:");
            Console.WriteLine("- Apothecary: Leczy sojuszników, ma zdolność przywracania życia.");
            Console.WriteLine("- Gunner: Specjalizuje się w strzelaniu, ma większą amunicję.");
            Console.WriteLine("- Psionic: Posiada zdolności psioniczne, które mogą uszkodzić tarcze wrogów.");
            Console.WriteLine("- Guard: Ma wysoką tarczę i może chronić sojuszników.");
            Console.WriteLine("- Scout: Bardzo szybki, ale ma niski poziom tarczy.");
            Console.WriteLine("- ComputerCharacters: Postacie kontrolowane przez komputer.");
            Console.WriteLine();
            Console.WriteLine("Klawisze sterowania:");
            Console.WriteLine("- Strzałki: Przesuwanie postaci (Góra, Dół, Lewo, Prawo).");
            Console.WriteLine("- Spacja: Aktywowanie specjalnych zdolności postaci.");
            Console.WriteLine("- Q: Strzał, jeśli postać ma wystarczająco dużo ruchów i amunicji.");
            Console.WriteLine("- R: Przeładowanie broni (wymaga 2 punktów ruchu).");
            Console.WriteLine("- L: Zaznaczenie wroga.");
            Console.WriteLine("- Cyfry (1-9): Wybór postaci do ruchu.");
            Console.WriteLine("- Escape: Pauza lub zakończenie gry.");
            Console.WriteLine();
            Console.WriteLine("Tura gracza:");
            Console.WriteLine("1. Regeneracja: Na początku każdej tury, postacie regenerują swoje punkty ruchu i tarcze.");
            Console.WriteLine("2. Ruch i akcje: Gracz wybiera postać i wykonuje ruchy, korzystając ze strzałek, oraz może wykonywać akcje, takie jak strzelanie, przeładowanie lub użycie zdolności specjalnych.");
            Console.WriteLine("3. Kolizje: Postacie nie mogą przechodzić przez inne postacie ani przeszkody. Kolizje są sprawdzane przed każdym ruchem.");
            Console.WriteLine();
            Console.WriteLine("Tura komputera:");
            Console.WriteLine("Po zakończeniu tury gracza, komputer kontroluje swoje postacie, poruszając się w stronę najbliższych wrogów i atakując ich.");
            Console.WriteLine();
            Console.WriteLine("Zdolności specjalne:");
            Console.WriteLine("- Apothecary: Może leczyć sojuszników w zasięgu.");
            Console.WriteLine("- Psionic: Może uszkadzać tarcze wrogów.");
            Console.WriteLine("- Guard: Może przekazywać tarczę sojusznikom.");
            Console.WriteLine();
            Console.WriteLine("Warunki zakończenia gry:");
            Console.WriteLine("- Gra kończy się, gdy wszystkie postacie jednego z graczy zostaną pokonane.");
            Console.WriteLine("- Jeśli graczowi zabraknie postaci, wyświetlany jest komunikat 'Przegrana'.");
            Console.WriteLine("- Jeśli wszystkie postacie komputera zostaną pokonane, wyświetlany jest komunikat 'Wygrana'.");
            Console.WriteLine();
            Console.WriteLine("Funkcje dodatkowe:");
            Console.WriteLine("- Zapis gry: Gra może być zapisana i wczytana w późniejszym czasie.");
            Console.WriteLine("- Tablica wyników: Po zakończeniu gry, wyniki mogą być zapisane w bazie danych, a najlepsze czasy i liczba pokonanych wrogów mogą być wyświetlane na tablicy wyników.");
            Console.WriteLine();
            Console.WriteLine("Przykładowa sesja gry:");
            Console.WriteLine("1. Gracz uruchamia grę i wybiera postać do ruchu.");
            Console.WriteLine("2. Gracz porusza postacią za pomocą strzałek.");
            Console.WriteLine("3. Gracz wykonuje akcję, taką jak strzelanie do wroga.");
            Console.WriteLine("4. Komputer wykonuje swoje ruchy i akcje.");
            Console.WriteLine("5. Gra trwa, dopóki wszystkie postacie jednego z graczy nie zostaną pokonane.");
            Console.WriteLine();
            Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić do menu głównego...");
            Console.ReadKey();
            Main_Menu();
        }

        static void AdminPanelMenu()
        {
            string prompt = "Wybierz opcję:";
            string[] options = { "Aktualizuj statystyki postaci", "Aktualizuj dane o użytkownikach", "Powrót do menu głównego" };
            Menu updateMenu = new Menu(prompt, options);
            int selectedIndex = updateMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    UpdateCharacterStatsMenu();
                    break;
                case 1:
                    UpdateUserData();
                    break;
                case 2:
                    Main_Menu();
                    break;
            }
        }

        static void UpdateCharacterStatsMenu()
        {
            string prompt = "Wybierz opcję:";
            string[] options = {"Medyk", "Zwiadowca", "Obrońca", "Psionik", "Strzelec", "Powrót do menu"};
            Menu updateMenu = new Menu(prompt, options);
            int selectedIndex = updateMenu.Run();

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
                    AdminPanelMenu(); // Powrót do menu głównego
                    break;
            }
        }

        static void UpdateCharacterStats(int characterId)
        {
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = adminname;
            string passwd = adminpasswd;

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();

                    string query = $"SELECT Moves, Hp, Damage, Ammo, Shield FROM character_stats WHERE Id = {characterId}";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) 
                    {
                        int moves = Convert.ToInt32(reader["Moves"]);
                        int hp = Convert.ToInt32(reader["Hp"]);
                        int damage = Convert.ToInt32(reader["Damage"]);
                        int ammo = Convert.ToInt32(reader["Ammo"]);
                        int shield = Convert.ToInt32(reader["Shield"]);

                        reader.Close();


                        Console.WriteLine($"Obecne statystyki postaci (Id = {characterId}):");
                        Console.WriteLine($"1. Ruchy: {moves}");
                        Console.WriteLine($"2. Hp: {hp}");
                        Console.WriteLine($"3. Obrażenia: {damage}");
                        Console.WriteLine($"4. Ammo: {ammo}");
                        Console.WriteLine($"5. Tarcza: {shield}\n");

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
                        

                        string updateQuery = $"UPDATE character_stats SET Moves = {newMoves}, Hp = {newHp}, Damage = {newDamage}, Ammo = {newAmmo}, Shield = {newShield} WHERE Id = {characterId}";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            reader.Close();
                            Console.WriteLine("Dane postaci zostały pomyślnie zaktualizowane.");
                            Console.ReadKey();
                            AdminPanelMenu();
                        }
                        else
                        {
                            reader.Close();
                            Console.WriteLine("Nie udało się zaktualizować danych postaci.");
                            Console.ReadKey();
                            AdminPanelMenu();
                        }
                    }
                    else
                    {
                        reader.Close();
                        Console.WriteLine($"Nie znaleziono postaci o Id = {characterId}.");
                        Console.ReadKey();
                        AdminPanelMenu();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas aktualizacji danych postaci: {ex.Message}");
                Console.ReadKey();
                AdminPanelMenu();
            }
        }


        static void UpdateUserData()
        {
            Console.WriteLine("\nPodaj nazwę urzytkownika do którego chcesz zaktualizować: ");
            string uname = Console.ReadLine();

            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = adminname;
            string passwd = adminpasswd;

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();

                    string query = $"SELECT Name, Password, E_Mail FROM users WHERE Name = '{uname}'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string name = reader["Name"].ToString();
                        string password = reader["Password"].ToString();
                        string email = reader["E_Mail"].ToString();

                        reader.Close();

                        Console.WriteLine($"Obecne dane użytkownika (Nazwa użytkownika: {uname}):");
                        Console.WriteLine($"1. Imie: {name}");
                        Console.WriteLine($"2. Hasło: {password}");
                        Console.WriteLine($"3. E-Mail: {email}");

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
                            AdminPanelMenu();

                        }
                        else
                        {
                            Console.WriteLine("Nie udało się zaktualizować danych użytkownika.");
                            Console.ReadKey();
                            AdminPanelMenu();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Nie znaleziono użytkownika o nazwie = {uname}.");
                        Console.ReadKey();
                        AdminPanelMenu();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas aktualizacji danych użytkownika: {ex.Message}");
                Console.ReadKey();
                AdminPanelMenu();
            }
        }

        static void AdminPanel()
        {
            Console.Clear();
            Console.WriteLine("Podaj nazwę użytkownika:");
            string name = Console.ReadLine();
            adminname = name;
            Console.WriteLine("Podaj hasło:");
            string password = ReadPassword();
            adminpasswd = password;


            if (CheckAdminPanel(name, password))
            {
                Console.WriteLine("Logowanie powiodło się.");
                Console.ReadKey();
                Console.Clear();
                AdminPanelMenu();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Logowanie nie powiodło się.");
                Console.ReadKey();
                Console.Clear();     
            }
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

        static bool CheckAdminPanel(string name, string password)
        {
            string server = "localhost";
            string database = "mysql";
            string username = name;
            string passwd = password;

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();

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

        static void Music()
        {
            {
                try
                {
                    // Ścieżka do pliku muzycznego
                    string audioFilePath = @"C:\Users\hutek\source\repos\Fighting_Chess_v3\Fighting_Chess_v3\2020-03-22_-_A_Bit_Of_Hope_-_David_Fesliyan.mp3";

                    using (var audioFile = new AudioFileReader(audioFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        Main_Menu();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                }
            }
        }

        class Config
        {
            public string AudioFilePath { get; set; }
        }

        static void Register()
        {
            Clear();
            WriteLine("Podaj nazwę użytkownika:");
            string name = ReadLine();
            WriteLine("Podaj hasło:");
            string password = ReadPassword();
            WriteLine("Podaj E-Mail:");
            string mail = ReadLine();

            if (CheckRegister(name, password, mail))
            {
                WriteLine("Rejestracja powiodła się. Naciśnij dowolny przycisk aby kontynuować.");
                ReadLine();
                Main_Menu();
            }
            else
            {
                Clear();
                WriteLine("Rejestracja nie powiodła się, podałeś niespójne  dane. Spróbuj ponownie.");
                Register();
            }
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

        static bool CheckRegister(string name, string password, string mail)
        {
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string passwd = "";

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();

                    var transaction = conn.BeginTransaction();

                    var query = new MySqlCommand("INSERT INTO Users (Name, Password, E_mail) VALUES (@name, @password, @mail);", conn, transaction);
                    query.Parameters.AddWithValue("@name", name);
                    query.Parameters.AddWithValue("@password", password);
                    query.Parameters.AddWithValue("@mail", mail);
                    query.ExecuteNonQuery();

                    long userId = query.LastInsertedId;

                    var hallOfFameCommand = new MySqlCommand("INSERT INTO HallOfFame (User_ID, TotalTimeSpent, EnemiesDefeated, GamesPlayed) VALUES (@userId, '00:00:00', 0, 0);", conn, transaction);
                    hallOfFameCommand.Parameters.AddWithValue("@userId", userId);
                    hallOfFameCommand.ExecuteNonQuery();

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
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("Podaj nazwę użytkownika:");
            string name = Console.ReadLine();
            Console.WriteLine("Podaj hasło:");
            string password = ReadPassword();


            if (CheckLogin(name, password))
            {
                Console.WriteLine("Logowanie powiodło się.");
                Console.ReadKey();
                Console.Clear();
                Game_Start();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Logowanie nie powiodło się.");
                Console.ReadKey();
                Console.Clear();

                Main_Menu();


            }
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

        static bool CheckLogin(string name, string password)
        {


            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string passwd = "";

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();

                    string query = "SELECT User_ID, Password FROM Users WHERE Name = @name;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string retrievedPassword = reader["Password"].ToString();
                            int userID = Convert.ToInt32(reader["User_ID"]);

                            if (retrievedPassword == password)
                            {
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
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
                return false;
            }
        }

        static void HallofFame()
        {
            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string password = "";

            string connectionString = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={password};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @" SELECT u.Name, h.TotalTimeSpent, h.EnemiesDefeated, h.GamesPlayed, h.BestTime FROM HallOfFame h JOIN Users u ON h.User_ID = u.User_ID ORDER BY h.GamesPlayed DESC, h.EnemiesDefeated DESC, h.TotalTimeSpent ASC;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.Clear();
                        Console.WriteLine("Sala Chwały:");
                        Console.WriteLine("Nazwa | Łączny czas w grze | Pokonani przeciwnicy | Zagrane gry | Najlepszy czas");
                        Console.WriteLine(new string('-', 80));

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
                        Main_Menu();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
            }
        }

        static void Exit()
        {

            Console.WriteLine("Dzięki za wsparcie!");
            Environment.Exit(0);
        }

        static void EndGame()
        {
            TimeSpan sessionDuration = endTime - startTime;
            string sessionDurationSTR = sessionDuration.ToString(@"hh\:mm\:ss");

            Console.Clear();

            string server = "localhost";
            string database = "fighting_chess_v3";
            string username = "root";
            string passwd = "";

            string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";
            string query = @"UPDATE halloffame SET GamesPlayed = GamesPlayed + 1, EnemiesDefeated = EnemiesDefeated + @enemiesDefeated, BestTime = CASE WHEN @sessionDuration < BestTime OR BestTime = '00:00:00' THEN @sessionDuration ELSE BestTime END, TotalTimeSpent = ADDTIME(TotalTimeSpent, @sessionDuration) WHERE User_ID = @userId;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@userId", Session.UserID);
                        cmd.Parameters.AddWithValue("@sessionDuration", sessionDurationSTR);
                        cmd.Parameters.AddWithValue("@enemiesDefeated", enemiesdefeated);


                        int resaults = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Zaktualizowano {resaults} rekordów.");
                        Console.WriteLine("Przeszedłeś mapę !!!");
                        Console.WriteLine($"Czas trwania sesji: {sessionDuration}");
                        Console.WriteLine($"Czas trwania sesji STR: {sessionDurationSTR}");
                        Console.WriteLine($"Zabito : {enemiesdefeated} wrogów");
                        Console.WriteLine($@"Wciśnij dowolny przycisk, aby kontynuować");
                        Console.ReadKey();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Błąd bazy danych: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
            Console.Clear();

        }

        class Menu
        {
            private int SelectedIndex;
            private string[] Options;
            private string Prompt;

            public Menu(string prompt, string[] options)
            {
                Prompt = prompt;
                Options = options;
                SelectedIndex = 0;
            }
            private void DisplayOptions()
            {
                WriteLine(Prompt);
                for (int i = 0; i < Options.Length; i++)
                {
                    string currentOption = Options[i];

                    if (i == SelectedIndex)
                    {
                        ForegroundColor = ConsoleColor.Blue;
                        BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.White;
                        BackgroundColor = ConsoleColor.Black;
                    }
                    WriteLine($"<< {currentOption} >>");
                }
                ResetColor();
            }
            public int Run()            //możliwość poruszania się strzałkami i potwierdzenie enterem
            {
                ConsoleKey keyPressed;
                do
                {
                    Clear();
                    DisplayOptions();
                    ConsoleKeyInfo keyInfo = ReadKey(true);
                    keyPressed = keyInfo.Key;
                    if (keyPressed == ConsoleKey.UpArrow)
                    {
                        SelectedIndex--;
                        if (SelectedIndex == -1)
                        {
                            SelectedIndex = Options.Length - 1;
                        }
                    }
                    else if (keyPressed == ConsoleKey.DownArrow)
                    {
                        SelectedIndex++;
                        if (SelectedIndex == Options.Length)
                        {
                            SelectedIndex = 0;
                        }
                    }
                }
                while (keyPressed != ConsoleKey.Enter);

                return SelectedIndex;
            }
        }

        static void Game_Start()
        {
            string mapSize = "Wybierz rozmiar planszy";
            string[] choice = { "10x10", "15x12", "20x20", "30x30" };
            Menu sizeOfMap = new Menu(mapSize, choice);
            int maps = sizeOfMap.Run();
            int mapY = 10;
            int mapX = 10;

            switch (maps)
            {
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

            string charPrompt = "Wybierz ilość postaci: ";
            string[] quantity = { "Jedna", "Dwie", "Trzy", "Cztery" };
            Menu charMenu = new Menu(charPrompt, quantity);
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

            aliveComputerCharactersCount = character_quantity;
            alivePlayerCharactersCount = character_quantity;
            enemiesdefeated = character_quantity;

            for (int i = 0; i < character_quantity; i++)
            {
                Console.WriteLine($"Wybierz klasę dla postaci nr {i + 1}:");
                Console.WriteLine("1. Medyk\n2. Zwiadowca\n3. Obrońca\n4. Psionik\n5. Strzelec");
                string charClass = $"Wybierz klasę dla postaci nr {i + 1}: ";
                string[] classes = { "Medyk", "Zwiadowca", "Obrońca", "Psionik", "Strzelec" };
                Menu charClassMenu = new Menu(charClass, classes);
                int characterType = charClassMenu.Run();

                string server = "localhost";
                string database = "fighting_chess_v3";
                string username = "root";
                string passwd = "";

                string constring = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={passwd};";

                try
                {
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

            startTime = DateTime.Now;

            for (int j = 0; j < character_quantity; j++)
            {
                int randomX = new Random().Next(0, mapX - 1);
                int randomY = new Random().Next((mapY - 1) / 2, mapY - 1);
                characters.Add(new ComputerCharacters(randomX, randomY, (char)('A' + j), 6, 7, 3, 3, 2));
            }

            List<Tuple<int, int>> obstacles = GenerateRandomObstacles(mapX - 4, mapY - 4, (mapY * mapX) / 20, characters);
            Movement movement = new Movement(mapY, mapX, characters, obstacles);
        }



        private static List<Tuple<int, int>> GenerateRandomObstacles(int height, int width, int obstacleCount, List<Characters> characters)
        {
            List<Tuple<int, int>> obstacles = new List<Tuple<int, int>>();
            Random rnd = new Random();
            for (int i = 0; i < obstacleCount; i++)
            {
                int obstacleX = rnd.Next(width);
                int obstacleY = rnd.Next(height);
                while (characters.Any(c => c.PositionX == obstacleX && c.PositionY == obstacleY))
                {
                    obstacleX = rnd.Next(width);
                    obstacleY = rnd.Next(height);
                }
                obstacles.Add(new Tuple<int, int>(obstacleX, obstacleY));
            }
            return obstacles;
        }

        public class Characters
        {

            public int PositionX { get; set; }
            public int PositionY { get; set; }
            public char Symbol { get; set; }
            public int Moves { get; set; }
            public int Hp { get; set; }
            public int Damage { get; set; }
            public int Ammo { get; set; }
            public int Shield { get; set; }

            public Characters(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield)
            {
                PositionX = positionX;
                PositionY = positionY;
                Symbol = symbol;
                Moves = moves;
                Hp = hp;
                Damage = damage;
                Ammo = ammo;
                Shield = shield;
            }
        }

        public class ComputerCharacters : Characters
        {
            public ComputerCharacters(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
            {
            }
        }

        public class Apothecary : Characters
        {
            public Apothecary(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
            {
            }
        }

        public class Scout : Characters
        {
            public Scout(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
            {
            }
        }

        public class Guard : Characters
        {
            public Guard(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
            {
            }
        }

        public class Psionic : Characters
        {
            public Psionic(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
            {
            }
        }

        public class Gunner : Characters
        {
            public Gunner(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
            {
            }
        }

        class Board
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public List<Characters> Characters { get; set; }
            public List<Tuple<int, int>> Obstacles { get; set; }
            public int HighlightedCharacterId { get; set; }

            public Board(int height, int width, List<Characters> characters, List<Tuple<int, int>> obstacles, int highlightedCharacterId)
            {
                Height = height;
                Width = width;
                Characters = characters;
                Obstacles = obstacles;
                HighlightedCharacterId = highlightedCharacterId;

                for (int i = 0; i < height; i++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bool playerPresent = false;
                        bool obstaclePresent = false;
                        foreach (var character in Characters)
                        {
                            if (i == character.PositionY && x == character.PositionX)
                            {
                                Console.ForegroundColor = character is ComputerCharacters ? ConsoleColor.Red : ConsoleColor.Green;

                                if (highlightedCharacterId != -1 && highlightedCharacterId == Array.IndexOf(Characters.ToArray(), character))
                                {
                                    Console.BackgroundColor = ConsoleColor.White;
                                }

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

        public class Movement
        {
            private int enemiesRemaining;
            public int Height { get; set; }
            public int Width { get; set; }
            public List<Characters> Character { get; set; }
            public List<Tuple<int, int>> Obstacles { get; set; }
            public Movement(int height, int width, List<Characters> characters, List<Tuple<int, int>> obstacles)
            {
                Height = height;
                Width = width;
                Character = characters;
                Obstacles = obstacles;

                ConsoleKeyInfo keyInfo;
                int chosenCharacterId = 0;
                int highlightedCharacterId = -1;

                do
                {
                    do
                    {
                        regeneration();
                        game_loop();
                    } while (true);
                } while (aliveComputerCharactersCount > 0 && alivePlayerCharactersCount > 0);






                void regeneration()
                {
                    for (int i = 0; i < characters.Count; i++)
                    {
                        if (characters[i] is Apothecary)
                        {
                            Character[i].Moves = 6;
                            Character[i].Shield = 2;

                        }
                        else if (characters[i] is Gunner)
                        {
                            Character[i].Moves = 6;
                            Character[i].Shield = 1;

                        }
                        else if (characters[i] is Psionic)
                        {
                            Character[i].Moves = 6;
                            Character[i].Shield = 1;

                        }
                        else if (characters[i] is Guard)
                        {
                            Character[i].Moves = 4;
                            Character[i].Shield = 5;

                        }
                        else if (characters[i] is Scout)
                        {
                            Character[i].Moves = 9;
                            Character[i].Shield = 0;

                        }
                        else if (characters[i] is ComputerCharacters)
                        {
                            Character[i].Moves = 6;
                            Character[i].Shield = 2;
                            Character[i].Ammo = 3;
                        }
                    }
                }
                void game_loop()
                {
                    do
                    {
                        if (alivePlayerCharactersCount == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Przegrana");
                            Console.ReadLine();
                            Main_Menu();
                        }
                        else if (aliveComputerCharactersCount == 0)
                        {
                            endTime = DateTime.Now;
                            EndGame();
                            Main_Menu();
                        }

                        Console.Clear();
                        new Board(height, width, Character, Obstacles, highlightedCharacterId);
                        Console.WriteLine($"Postać {Character[chosenCharacterId].Symbol} wykonuje ruch {Character[chosenCharacterId].Moves}");

                        for (int i = 0; i < Character.Count; i++)
                        {
                            Characters currentCharacter = Character[i];
                            Console.ForegroundColor = currentCharacter is ComputerCharacters ? ConsoleColor.Red : ConsoleColor.Green;
                            Console.WriteLine($"{currentCharacter.Symbol} - HP: {currentCharacter.Hp} Shield: {currentCharacter.Shield} Ammo: {currentCharacter.Ammo}");
                            Console.ResetColor();
                        }

                        if (Character[chosenCharacterId] is ComputerCharacters)
                        {
                            chosenCharacterId = 1;
                        }
                        keyInfo = Console.ReadKey();

                        if (char.IsDigit(keyInfo.KeyChar))
                        {
                            int selectedCharacter = keyInfo.KeyChar - '1';

                            if (selectedCharacter >= 0 && selectedCharacter < Character.Count && Character[selectedCharacter] is not ComputerCharacters)
                            {
                                chosenCharacterId = selectedCharacter;
                                highlightedCharacterId = -1;
                            }
                        }

                        if (keyInfo.Key == ConsoleKey.L)
                        {
                            HighlightEnemy();
                        }
                        if (keyInfo.Key == ConsoleKey.R)
                        {
                            Reload();
                        }

                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            if (Character[chosenCharacterId].Moves >= 2)
                            {
                                Shoot();

                            }
                        }



                        if (keyInfo.Key == ConsoleKey.Spacebar)
                        {
                            if (Character[chosenCharacterId] is Apothecary && Character[chosenCharacterId].Moves >= 3)
                            {
                                apothecary_heal();
                            }

                            else if (Character[chosenCharacterId] is Psionic && Character[chosenCharacterId].Moves > 0)
                            {
                                PsionicSpecialAbility();
                            }
                            else if (Character[chosenCharacterId] is Guard && Character[chosenCharacterId].Moves >= 2 && Character[chosenCharacterId].Shield > 0)
                            {
                                GuardianSpecialAbility();
                            }
                        }

                        MoveCharacter(Character[chosenCharacterId], keyInfo);

                    } while (keyInfo.Key != ConsoleKey.Escape);

                    MoveComputer();

                }


                void GuardianSpecialAbility()
                {
                    Console.WriteLine($"Guardian {Character[chosenCharacterId].Symbol} uses special ability!");

                    int closestPlayerId = FindClosestPlayerOtherThanGuardianAndComputer(Character[chosenCharacterId]);

                    if (closestPlayerId != -1)
                    {
                        Characters closestPlayer = Character[closestPlayerId];
                        closestPlayer.Shield += 3;

                        Character[chosenCharacterId].Shield = 0;
                        Character[chosenCharacterId].Moves -= 2;

                        Console.WriteLine($"Guardian {Character[chosenCharacterId].Symbol} gives 3 armor to Player {closestPlayer.Symbol}!");
                    }
                    else
                    {
                        Console.WriteLine("Nie ma gracza, któremu można przekazać shielda.");
                    }

                    Thread.Sleep(1000);
                }

                int FindClosestPlayerOtherThanGuardianAndComputer(Characters guardian)
                {
                    int closestPlayerId = -1;
                    double minDistance = double.MaxValue;

                    for (int i = 0; i < Character.Count; i++)
                    {
                        if (i != chosenCharacterId && !(Character[i] is Guard) && !(Character[i] is ComputerCharacters))
                        {
                            double distance = CalculateDistance(guardian, Character[i]);

                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestPlayerId = i;
                            }
                        }
                    }

                    return closestPlayerId;
                }


                void PsionicSpecialAbility()
                {
                    Console.WriteLine($"Psionic {Character[chosenCharacterId].Symbol} uses special ability!");

                    foreach (var character in Character)
                    {
                        if (character is ComputerCharacters)
                        {
                            int shieldDmg = Math.Min(character.Shield, 1);
                            character.Shield -= shieldDmg;
                            Console.WriteLine($"Computer {character.Symbol} loses {shieldDmg} shield!");

                            if (shieldDmg < 1)
                            {
                                character.Hp -= 1;
                                Console.WriteLine($"Computer {character.Symbol} receives -1 HP!");
                            }
                        }
                    }

                    Character[chosenCharacterId].Moves = 0;
                    Thread.Sleep(1000);
                }

                void apothecary_heal()
                {
                    if (Character[chosenCharacterId] is Apothecary apothecary)
                    {
                        int healRange = 3;

                        foreach (var character in Character)
                        {
                            if (character != apothecary && !(character is ComputerCharacters) && CalculateDistance(apothecary, character) <= healRange)
                            {
                                character.Hp += 1;
                                Console.WriteLine($"Gracz {apothecary.Symbol} uleczył sojusznika {character.Symbol} za 1 hp!");
                            }
                        }

                        Character[chosenCharacterId].Moves = Character[chosenCharacterId].Moves - 3;
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Console.WriteLine("Leczenie dozwolone tylko dla klasy medyka!");
                        Thread.Sleep(1000);
                    }
                }


                void Reload()
                {
                    if (Character[chosenCharacterId].Moves >= 2)
                    {
                        int[] saved_ammo = new int[characters.Count];
                        for (int j = 0; j < characters.Count; j++)
                        {
                            saved_ammo[j] = characters[j].Ammo;
                        }
                        if (Character[chosenCharacterId] is Apothecary)
                        {
                            Character[chosenCharacterId].Ammo = 3;
                        }
                        else if (Character[chosenCharacterId] is Gunner)
                        {
                            Character[chosenCharacterId].Ammo = 6;
                        }
                        else if (Character[chosenCharacterId] is Psionic)
                        {
                            Character[chosenCharacterId].Ammo = 2;
                        }
                        else if (Character[chosenCharacterId] is Guard)
                        {
                            Character[chosenCharacterId].Ammo = 1;
                        }
                        else if (Character[chosenCharacterId] is Scout)
                        {
                            Character[chosenCharacterId].Ammo = 5;
                        }

                        Character[chosenCharacterId].Moves -= 2;
                        Console.WriteLine($"Gracz {Character[chosenCharacterId].Symbol} przeładował!");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Console.WriteLine($"Gracz {Character[chosenCharacterId].Symbol} posiada niewystarczającą ilość ruchów!");
                        Thread.Sleep(1000);
                    }
                }

                void Shoot()
                {
                    if (Character[chosenCharacterId].Ammo > 0)
                    {


                        if (highlightedCharacterId != -1 && Character[highlightedCharacterId] is ComputerCharacters)
                        {
                            Console.WriteLine($"Player {Character[chosenCharacterId].Symbol} shoots at Computer {Character[highlightedCharacterId].Symbol}!");

                            double distance = CalculateDistance(Character[chosenCharacterId], Character[highlightedCharacterId]);
                            int adjustedDamage = CalculateAdjustedDamage(Character[chosenCharacterId].Damage, distance);
                            Character[chosenCharacterId].Moves = Character[chosenCharacterId].Moves - 2;


                            if (distance > 10)
                            {
                                adjustedDamage = 0;
                            }
                            else if (distance < 10 && distance > 6)
                            {
                                adjustedDamage = Math.Max(1, adjustedDamage - 1);
                            }
                            else
                            {
                                adjustedDamage = Math.Max(1, adjustedDamage);
                            }
                            if (Character[highlightedCharacterId].Shield > 0)
                            {
                                int rest_of_dmg = adjustedDamage - Character[highlightedCharacterId].Shield;
                                Character[highlightedCharacterId].Shield -= adjustedDamage;

                                if (rest_of_dmg <= 0)
                                {
                                    adjustedDamage = 0;
                                }
                                else
                                {
                                    adjustedDamage = rest_of_dmg;
                                }
                                Thread.Sleep(2000);
                            }

                            Character[highlightedCharacterId].Hp -= adjustedDamage;
                            Character[chosenCharacterId].Ammo -= 1;
                            Console.WriteLine($"Distance to Computer {Character[highlightedCharacterId].Symbol}: {distance:F2}");
                            Console.WriteLine($"Adjusted Damage: {adjustedDamage}");


                            if (Character[highlightedCharacterId].Hp <= 0)
                            {
                                Console.WriteLine($"Computer {Character[highlightedCharacterId].Symbol} is defeated!");


                                Character.RemoveAt(highlightedCharacterId);
                                highlightedCharacterId = -1;
                                aliveComputerCharactersCount--;

                            }

                        }
                        else
                        {
                            Console.WriteLine("No highlighted computer character to shoot at.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Za mało ammo");
                        Thread.Sleep(2000);
                    }

                }

                double CalculateDistance(Characters character1, Characters character2)
                {
                    int deltaX = character1.PositionX - character2.PositionX;
                    int deltaY = character1.PositionY - character2.PositionY;
                    return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                }

                int CalculateAdjustedDamage(int baseDamage, double distance)
                {

                    int maxDistance = Math.Max(Height, Width);
                    double damageFactor = 1.0 - (distance / maxDistance);
                    int adjustedDamage = (int)(baseDamage * damageFactor);


                    adjustedDamage = Math.Max(1, adjustedDamage);

                    return adjustedDamage;
                }

                void HighlightEnemy()
                {

                    int nextHighlightedId = highlightedCharacterId + 1;

                    if (nextHighlightedId >= Character.Count)
                    {
                        nextHighlightedId = -1;
                    }

                    while (nextHighlightedId != highlightedCharacterId)
                    {
                        if (nextHighlightedId >= 0 && Character[nextHighlightedId] is ComputerCharacters)
                        {
                            double distance = CalculateDistance(Character[chosenCharacterId], Character[nextHighlightedId]);
                            Console.WriteLine($"Distance to highlighted character: {distance:F2}");

                            highlightedCharacterId = nextHighlightedId;
                            break;
                        }

                        nextHighlightedId++;

                        if (nextHighlightedId >= Character.Count)
                        {
                            nextHighlightedId = -1;
                        }
                    }
                }


                void MoveCharacter(Characters character, ConsoleKeyInfo keyInfo)
                {
                    if (character.Moves > 0)
                    {
                        int newPositionX = character.PositionX;
                        int newPositionY = character.PositionY;

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.UpArrow:
                                newPositionY = Math.Max(0, character.PositionY - (character is Scout ? 2 : 1));
                                CollisionDetector();
                                break;
                            case ConsoleKey.DownArrow:
                                newPositionY = Math.Min(Height - 1, character.PositionY + (character is Scout ? 2 : 1));
                                CollisionDetector();
                                break;
                            case ConsoleKey.LeftArrow:
                                newPositionX = Math.Max(0, character.PositionX - (character is Scout ? 2 : 1));
                                CollisionDetector();
                                break;
                            case ConsoleKey.RightArrow:
                                newPositionX = Math.Min(Width - 1, character.PositionX + (character is Scout ? 2 : 1));
                                CollisionDetector();
                                break;
                        }

                        void CollisionDetector()
                        {
                            if (!IsCollision(newPositionX, newPositionY))
                            {
                                character.PositionX = newPositionX;
                                character.PositionY = newPositionY;
                                character.Moves--;


                            }

                        }
                    }
                }


                bool IsCollision(int x, int y)
                {
                    foreach (var otherCharacters in Character)
                    {
                        if (otherCharacters.PositionX == x && otherCharacters.PositionY == y)
                        {
                            return true;
                        }
                    }
                    foreach (var obstacle in Obstacles)
                    {
                        if (obstacle.Item1 == x && obstacle.Item2 == y)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                void MoveComputer()
                {
                    for (int i = 0; i < Character.Count; i++)
                    {
                        if (Character[i] is ComputerCharacters computerCharacter)
                        {
                            int closestPlayerId = FindClosestPlayer(computerCharacter);

                            for (int moveCount = 0; moveCount < computerCharacter.Moves; moveCount++)
                            {
                                if (closestPlayerId != -1)
                                {
                                    Characters closestPlayer = Character[closestPlayerId];
                                    double distanceToPlayer = CalculateDistance(computerCharacter, closestPlayer);

                                    if (distanceToPlayer < 5)
                                        ShootAtPlayer(computerCharacter, closestPlayer);
                                    else
                                        MoveTowardsPlayer(computerCharacter, closestPlayer);

                                    ComputerReload(computerCharacter);

                                }
                            }
                        }
                    }
                }


                void MoveTowardsPlayer(Characters computerCharacter, Characters targetPlayer)
                {
                    int deltaX = targetPlayer.PositionX - computerCharacter.PositionX;
                    int deltaY = targetPlayer.PositionY - computerCharacter.PositionY;

                    int newPositionX = computerCharacter.PositionX;
                    int newPositionY = computerCharacter.PositionY;

                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
                    {
                        newPositionX += Math.Sign(deltaX);
                    }
                    else
                    {
                        newPositionY += Math.Sign(deltaY);
                    }

                    if (!IsCollision(newPositionX, newPositionY))
                    {
                        computerCharacter.PositionX = newPositionX;
                        computerCharacter.PositionY = newPositionY;

                        Obstacles.RemoveAll(obstacle => obstacle.Item1 == newPositionX && obstacle.Item2 == newPositionY);

                        Console.Clear();
                        new Board(Height, Width, Character, Obstacles, -1);
                        Thread.Sleep(500);
                    }
                }

                void ComputerReload(Characters character)
                {
                    if (character is ComputerCharacters computerCharacter && computerCharacter.Ammo == 0)
                    {
                        Console.WriteLine($"Komputer {computerCharacter.Symbol} przeładowuje!");
                        Thread.Sleep(2000);
                        computerCharacter.Ammo = 3;
                    }
                }

                void ShootAtPlayer(Characters computerCharacter, Characters targetPlayer)
                {
                    if (computerCharacter.Ammo > 0 && computerCharacter.Moves >= 2)
                    {
                        Console.WriteLine($"Komputer {computerCharacter.Symbol} trafia postać {targetPlayer.Symbol}!");
                        Thread.Sleep(2000);

                        double distance = CalculateDistance(computerCharacter, targetPlayer);
                        int adjustedDamage = CalculateAdjustedDamage(computerCharacter.Damage, distance);
                        computerCharacter.Moves = computerCharacter.Moves - 2;


                        if (distance > 10)
                        {
                            adjustedDamage = 0;
                        }
                        else if (distance < 10 && distance > 6)
                        {
                            adjustedDamage = Math.Max(1, adjustedDamage - 1);
                        }
                        else
                        {
                            adjustedDamage = Math.Max(1, adjustedDamage);
                        }
                        if (targetPlayer.Shield > 0)
                        {
                            int rest_of_dmg = adjustedDamage - targetPlayer.Shield;
                            targetPlayer.Shield -= adjustedDamage;
                            if (rest_of_dmg <= 0)
                            {
                                adjustedDamage = 0;
                            }
                            else if (rest_of_dmg > 0)
                            {
                                adjustedDamage = rest_of_dmg;
                            }
                            Thread.Sleep(2000);
                        }

                        targetPlayer.Hp -= adjustedDamage;
                        computerCharacter.Ammo = computerCharacter.Ammo - 1;


                        if (targetPlayer.Hp <= 0)
                        {
                            Console.WriteLine($"Postać {targetPlayer.Symbol} została pokonana!");
                            alivePlayerCharactersCount--;
                            Thread.Sleep(2000);

                            Character.RemoveAt(Array.IndexOf(Character.ToArray(), targetPlayer));
                            chosenCharacterId = 0;

                            Console.Clear();
                            new Board(Height, Width, Character, Obstacles, -1);
                            Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Komputerowi {computerCharacter.Symbol} skończyła się amunicja!");
                        Thread.Sleep(2000);
                    }
                }

                int FindClosestPlayer(Characters computerCharacter)
                {
                    int closestPlayerId = -1;
                    double minDistance = double.MaxValue;

                    foreach (var playerCharacter in Character.Where(c => c is not ComputerCharacters))
                    {
                        double distance = CalculateDistance(computerCharacter, playerCharacter);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestPlayerId = Array.IndexOf(Character.ToArray(), playerCharacter);
                        }
                    }
                    return closestPlayerId;
                }
            }
        }
    }
}