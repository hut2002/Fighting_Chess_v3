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
using Fighting_Chess_v3;


namespace Fighting_Chess_v3
{
    public static class MenuOn
    {
        // Klasa reprezentująca interfejs menu
        public class Menu
        {
            private int SelectedIndex; // Indeks wybranej opcji
            private string[] Options;  // Lista opcji menu
            private string Prompt;     // Tekst zachęty do wyboru opcji

            // Konstruktor klasy Menu
            public Menu(string prompt, string[] options)
            {
                // Inicjalizacja menu
                Prompt = prompt;
                Options = options;
                SelectedIndex = 0;
            }
            // Metoda wyświetlająca opcje menu w konsoli
            private void DisplayOptions()
            {
                // Wyświetlanie opcji menu
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
            // Metoda obsługująca interakcję użytkownika z menu
            public int Run()      
            {
                // Obsługa menu
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
        // Metoda reprezentująca główne menu gry
        public static void Main_Menu()
        {
            string gamePrompt = "Wybierz opcję: ";
            string[] options = { "Logowanie", "Zacznij grę bez logowania", "Tablica Chwały", "Zarejestruj się", "Panel Administracyjny", "Instrukcja Gry", "Wyjdź" };
            Menu charMenu = new Menu(gamePrompt, options);
            int selectedIndex = charMenu.Run();

            Console.CursorVisible = false;

            switch (selectedIndex)
            {
                case 0:
                    Database.Login(); // Logowanie użytkownika
                    break;
                case 1:
                    GameLogic.Game_Start(); // Rozpoczęcie gry bez logowania
                    break;
                case 2:
                    Database.HallofFame(); // Wyświetlenie tablicy chwały
                    break;
                case 3:
                    Database.Register(); // Rejestracja nowego użytkownika
                    break;
                case 4:
                    Database.AdminPanel();  // Panel administracyjny
                    break;
                case 5:
                    Instructions.ShowInstructions(); // Wyświetlenie instrukcji gry
                    break;
                case 6:
                    GameLogic.Exit(); // Wyjście z gry
                    break;
            }
        }
        // Metoda reprezentująca menu panelu administracyjnego
        public static void AdminPanelMenu()
        {
            string prompt = "Wybierz opcję:";
            string[] options = { "Aktualizuj statystyki postaci", "Aktualizuj dane o użytkownikach", "Powrót do menu głównego" };
            Menu updateMenu = new Menu(prompt, options);
            int selectedIndex = updateMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    Database.UpdateCharacterStatsMenu();  // Aktualizacja statystyk postaci
                    break;
                case 1:
                    Database.UpdateUserData(); // Aktualizacja danych użytkowników
                    break;
                case 2:
                    Main_Menu();   // Powrót do menu głównego
                    break; 
            }
        }
    }
}
