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
    // Klasa PlayMusic zawiera metodę do odtwarzania muzyki i wywoływania metody MenuOn.Main_Menu()
    public static class PlayMusic
    {
        // Metoda Music odpowiada za odtwarzanie pliku muzycznego i wywołanie metody MenuOn.Main_Menu()
        public static void Music()
        {
            {
                try
                {
                    // Ścieżka do pliku muzycznego
                    string audioFilePath = @"C:\Users\hutek\source\repos\Fightinhwaijdwpaj\Fighting_Chess_v3\Fighting_Chess_v3\2020-03-22_-_A_Bit_Of_Hope_-_David_Fesliyan.mp3";
                    // Inicjalizacja odtwarzacza audio i odtworzenie pliku muzycznego
                    using (var audioFile = new AudioFileReader(audioFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        // Po rozpoczęciu odtwarzania muzyki wywołujemy menu główne
                        MenuOn.Main_Menu();
                    }
                }
                catch (Exception ex)
                {
                    // Obsługa wyjątków
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                }
            }
        }
        // Klasa Config zawiera właściwość AudioFilePath, która przechowuje ścieżkę do pliku audio
        class Config
        {
            public string AudioFilePath { get; set; }
        }
    }
}
