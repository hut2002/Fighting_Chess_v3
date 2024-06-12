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
    // Klasa Movement odpowiada za zarządzanie ruchami postaci
    public class Movement
    {
        private int enemiesRemaining;
        public int Height { get; set; }
        public int Width { get; set; }
        public List<Characters> Character { get; set; }
        public List<Tuple<int, int>> Obstacles { get; set; }
        // Konstruktor klasy Movement
        public Movement(int height, int width, List<Characters> characters, List<Tuple<int, int>> obstacles)
        {
            Height = height;
            Width = width;
            Character = characters;
            Obstacles = obstacles;

            ConsoleKeyInfo keyInfo;
            int chosenCharacterId = 0;
            int highlightedCharacterId = -1;
            // Pętla główna gry
            do
            {
                do
                {
                    // Regeneracja postaci
                    regeneration();
                    // Rozpoczęcie głównej pętli gry
                    game_loop();
                } while (true);
            } while (Database.aliveComputerCharactersCount > 0 && Database.alivePlayerCharactersCount > 0);





            // Metoda do regeneracji postaci
            void regeneration()
            {
                // Regeneracja każdej postaci
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
            // Główna pętla gry
            void game_loop()
            {
                do
                {
                    // Sprawdzenie warunków końca gry
                    if (Database.alivePlayerCharactersCount == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Przegrana");
                        Console.ReadLine();
                        MenuOn.Main_Menu();
                    }
                    else if (Database.aliveComputerCharactersCount == 0)
                    {
                        Database.endTime = DateTime.Now;
                        GameLogic.EndGame();
                        MenuOn.Main_Menu();
                    }
                    // Wyświetlenie planszy i informacji o postaciach
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
                    // Obsługa ruchu gracza
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
                    // Obsługa akcji gracza
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

            // Metoda obsługująca specjalną zdolność Guardian
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

            // Metoda znajdująca najbliższego gracza innego niż Guardian i Computer
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

            // Metoda do wykorzystania specjalnej zdolności przez postać Psionic
            void PsionicSpecialAbility()
            {
                Console.WriteLine($"Psionic {Character[chosenCharacterId].Symbol} uses special ability!");

                foreach (var character in Character)
                {
                    // Zmniejszenie tarczy postaci komputerowej
                    if (character is ComputerCharacters)
                    {
                        int shieldDmg = Math.Min(character.Shield, 1);
                        character.Shield -= shieldDmg;
                        Console.WriteLine($"Computer {character.Symbol} loses {shieldDmg} shield!");

                        if (shieldDmg < 1)
                        {
                            // Zmniejszenie punktów życia postaci komputerowej
                            character.Hp -= 1;
                            Console.WriteLine($"Computer {character.Symbol} receives -1 HP!");
                        }
                    }
                }
                // Ustawienie ruchów aktualnej postaci na 0 po użyciu zdolności
                Character[chosenCharacterId].Moves = 0;
                Thread.Sleep(1000);
            }
            // Metoda do uzdrowienia innych postaci przez postać Apothecary
            void apothecary_heal()
            {
                if (Character[chosenCharacterId] is Apothecary apothecary)
                {
                    int healRange = 3;

                    foreach (var character in Character)
                    {
                        if (character != apothecary && !(character is ComputerCharacters) && CalculateDistance(apothecary, character) <= healRange)
                        {
                            // Zwiększenie punktów życia wybranych postaci
                            character.Hp += 1;
                            Console.WriteLine($"Gracz {apothecary.Symbol} uleczył sojusznika {character.Symbol} za 1 hp!");
                        }
                    }
                    // Zmniejszenie ruchów postaci po użyciu zdolności leczenia
                    Character[chosenCharacterId].Moves = Character[chosenCharacterId].Moves - 3;
                    Thread.Sleep(1000);
                }
                else
                {
                    // Komunikat o błędzie - leczenie dostępne tylko dla postaci Apothecary
                    Console.WriteLine("Leczenie dozwolone tylko dla klasy medyka!");
                    Thread.Sleep(1000);
                }
            }

            // Metoda do przeładowania broni aktualnej postaci
            void Reload()
            {
                if (Character[chosenCharacterId].Moves >= 2)
                {
                    // Zapisanie wcześniejszej liczby nabojów
                    int[] saved_ammo = new int[characters.Count];
                    for (int j = 0; j < characters.Count; j++)
                    {
                        saved_ammo[j] = characters[j].Ammo;
                    }
                    // Przeładowanie broni w zależności od typu postaci
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

                    // Zmniejszenie liczby ruchów po przeładowaniu
                    Character[chosenCharacterId].Moves -= 2;
                    Console.WriteLine($"Gracz {Character[chosenCharacterId].Symbol} przeładował!");
                    Thread.Sleep(1000);
                }
                else
                {
                    // Komunikat o braku wystarczającej liczby ruchów do przeładowania broni
                    Console.WriteLine($"Gracz {Character[chosenCharacterId].Symbol} posiada niewystarczającą ilość ruchów!");
                    Thread.Sleep(1000);
                }
            }
            // Metoda do wykonania strzału przez aktualną postać
            void Shoot()
            {
                if (Character[chosenCharacterId].Ammo > 0)
                {


                    if (highlightedCharacterId != -1 && Character[highlightedCharacterId] is ComputerCharacters)
                    {
                        // Wykonanie strzału w kierunku wskazanej postaci komputerowej
                        Console.WriteLine($"Player {Character[chosenCharacterId].Symbol} shoots at Computer {Character[highlightedCharacterId].Symbol}!");

                        double distance = CalculateDistance(Character[chosenCharacterId], Character[highlightedCharacterId]);
                        int adjustedDamage = CalculateAdjustedDamage(Character[chosenCharacterId].Damage, distance);
                        Character[chosenCharacterId].Moves = Character[chosenCharacterId].Moves - 2;

                        // Obliczenie dostosowanego obrażenia w zależności od odległości
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
                        // Obsługa tarczy ochronnej postaci komputerowej
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
                        // Zmniejszenie ilości punktów życia postaci komputerowej o dostosowane obrażenia
                        Character[highlightedCharacterId].Hp -= adjustedDamage;
                        // Zmniejszenie ilości amunicji gracza o 1
                        Character[chosenCharacterId].Ammo -= 1;
                        Console.WriteLine($"Distance to Computer {Character[highlightedCharacterId].Symbol}: {distance:F2}");
                        Console.WriteLine($"Adjusted Damage: {adjustedDamage}");

                        // Sprawdzenie, czy postać komputerowa została pokonana
                        if (Character[highlightedCharacterId].Hp <= 0)
                        {
                            Console.WriteLine($"Computer {Character[highlightedCharacterId].Symbol} is defeated!");

                            // Usunięcie pokonanej postaci komputerowej z listy postaci
                            Character.RemoveAt(highlightedCharacterId);
                            highlightedCharacterId = -1;
                            Database.aliveComputerCharactersCount--;

                        }

                    }
                    else
                    {
                        Console.WriteLine("No highlighted computer character to shoot at.");
                    }
                }
                else
                {
                    // Informacja o braku amunicji
                    Console.WriteLine("Za mało ammo");
                    Thread.Sleep(2000);
                }

            }

            double CalculateDistance(Characters character1, Characters character2)
            {
                // Obliczenie różnicy współrzędnych X i Y między dwiema postaciami

                int deltaX = character1.PositionX - character2.PositionX;
                int deltaY = character1.PositionY - character2.PositionY;
                // Obliczenie odległości między dwiema postaciami przy użyciu twierdzenia Pitagorasa
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }

            int CalculateAdjustedDamage(int baseDamage, double distance)
            {
                // Obliczenie maksymalnej odległości na mapie
                int maxDistance = Math.Max(Height, Width);
                // Obliczenie współczynnika obrażeń w zależności od odległości
                double damageFactor = 1.0 - (distance / maxDistance);
                // Obliczenie dostosowanego obrażenia na podstawie podstawowego obrażenia i współczynnika obrażeń
                int adjustedDamage = (int)(baseDamage * damageFactor);

                // Sprawdzenie, czy dostosowane obrażenie nie jest mniejsze niż 1
                adjustedDamage = Math.Max(1, adjustedDamage);

                return adjustedDamage;
            }

            void HighlightEnemy()
            {
                // Znalezienie następnej wrogiej postaci, która ma zostać wyróżniona
                int nextHighlightedId = highlightedCharacterId + 1;

                if (nextHighlightedId >= Character.Count)
                {
                    nextHighlightedId = -1;
                }
                // Pętla szukająca następnej wrogiej postaci do wyróżnienia
                while (nextHighlightedId != highlightedCharacterId)
                {
                    if (nextHighlightedId >= 0 && Character[nextHighlightedId] is ComputerCharacters)
                    {
                        // Obliczenie odległości do wyróżnionej postaci
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
                // Sprawdza, czy postać ma wystarczającą liczbę ruchów
                if (character.Moves > 0)
                {
                    int newPositionX = character.PositionX;
                    int newPositionY = character.PositionY;
                    // Obsługuje ruch postaci w zależności od naciśniętego klawisza
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
                    // Funkcja sprawdzająca kolizje z przeszkodami i innymi postaciami
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
                // Sprawdza, czy istnieje kolizja na podanych współrzędnych
                foreach (var otherCharacters in Character)
                {
                    // Sprawdza kolizję z innymi postaciami
                    if (otherCharacters.PositionX == x && otherCharacters.PositionY == y)
                    {
                        return true;
                    }
                }
                foreach (var obstacle in Obstacles)
                {
                    // Sprawdza kolizję z przeszkodami
                    if (obstacle.Item1 == x && obstacle.Item2 == y)
                    {
                        return true;
                    }
                }
                return false;
            }

            void MoveComputer()
            {
                // Funkcja odpowiedzialna za ruch komputerowych postaci
                for (int i = 0; i < Character.Count; i++)
                {
                    if (Character[i] is ComputerCharacters computerCharacter)
                    {
                        // Znajduje najbliższego gracza dla danej komputerowej postaci
                        int closestPlayerId = FindClosestPlayer(computerCharacter);

                        for (int moveCount = 0; moveCount < computerCharacter.Moves; moveCount++)
                        {
                            if (closestPlayerId != -1)
                            {
                                Characters closestPlayer = Character[closestPlayerId];
                                double distanceToPlayer = CalculateDistance(computerCharacter, closestPlayer);
                                // Jeśli gracz jest w zasięgu strzału, komputer strzela
                                if (distanceToPlayer < 5)
                                    ShootAtPlayer(computerCharacter, closestPlayer);
                                // W przeciwnym razie komputer porusza się w kierunku gracza
                                else
                                    MoveTowardsPlayer(computerCharacter, closestPlayer);
                                // Sprawdza, czy komputer musi przeładować broń
                                ComputerReload(computerCharacter);

                            }
                        }
                    }
                }
            }


            void MoveTowardsPlayer(Characters computerCharacter, Characters targetPlayer)
            {
                // Funkcja poruszająca komputerową postać w kierunku gracza
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
                // Sprawdza, czy na nowych współrzędnych nie ma kolizji
                if (!IsCollision(newPositionX, newPositionY))
                {
                    computerCharacter.PositionX = newPositionX;
                    computerCharacter.PositionY = newPositionY;
                    // Usuwa przeszkody, jeśli komputer je omija
                    Obstacles.RemoveAll(obstacle => obstacle.Item1 == newPositionX && obstacle.Item2 == newPositionY);
                    // Odświeża widok planszy
                    Console.Clear();
                    new Board(Height, Width, Character, Obstacles, -1);
                    Thread.Sleep(500);
                }
            }

            void ComputerReload(Characters character)
            {
                // Funkcja sprawdza, czy komputerowa postać musi przeładować broń
                if (character is ComputerCharacters computerCharacter && computerCharacter.Ammo == 0)
                {
                    Console.WriteLine($"Komputer {computerCharacter.Symbol} przeładowuje!");
                    Thread.Sleep(2000);
                    computerCharacter.Ammo = 3;
                }
            }

            void ShootAtPlayer(Characters computerCharacter, Characters targetPlayer)
            {
                // Funkcja obsługująca strzał komputerowej postaci w gracza
                if (computerCharacter.Ammo > 0 && computerCharacter.Moves >= 2)
                {
                    Console.WriteLine($"Komputer {computerCharacter.Symbol} trafia postać {targetPlayer.Symbol}!");
                    Thread.Sleep(2000);
                    // Obliczenie odległości między komputerową postacią a graczem
                    double distance = CalculateDistance(computerCharacter, targetPlayer);
                    // Obliczenie dostosowanego obrażenia na podstawie odległości
                    int adjustedDamage = CalculateAdjustedDamage(computerCharacter.Damage, distance);
                    computerCharacter.Moves = computerCharacter.Moves - 2;

                    // Modyfikacja obrażeń w zależności od odległości
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
                    // Zmniejszenie obrażeń przez tarczę
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
                    // Zmniejszenie punktów życia gracza i amunicji komputerowej postaci
                    targetPlayer.Hp -= adjustedDamage;
                    computerCharacter.Ammo = computerCharacter.Ammo - 1;

                    // Obsługa pokonania gracza
                    if (targetPlayer.Hp <= 0)
                    {
                        Console.WriteLine($"Postać {targetPlayer.Symbol} została pokonana!");
                        Database.alivePlayerCharactersCount--;
                        Thread.Sleep(2000);
                        // Usunięcie pokonanej postaci z listy
                        Character.RemoveAt(Array.IndexOf(Character.ToArray(), targetPlayer));
                        chosenCharacterId = 0;
                        // Odświeżenie widoku planszy po usunięciu postaci
                        Console.Clear();
                        new Board(Height, Width, Character, Obstacles, -1);
                        Thread.Sleep(2000);
                    }
                }
                else
                {
                    // Komunikat o braku amunicji
                    Console.WriteLine($"Komputerowi {computerCharacter.Symbol} skończyła się amunicja!");
                    Thread.Sleep(2000);
                }
            }

            int FindClosestPlayer(Characters computerCharacter)
            {
                // Funkcja znajduje najbliższego gracza dla danej komputerowej postaci
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
