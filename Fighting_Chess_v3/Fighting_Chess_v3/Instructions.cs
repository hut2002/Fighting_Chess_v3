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
    public static class Instructions
    {
        public static void ShowInstructions()
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
            MenuOn.Main_Menu();
        }
    }
}
