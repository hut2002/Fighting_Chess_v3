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
{   // Klasa bazowa reprezentująca postacie w grze
    public class Characters
    {
        public int PositionX { get; set; }          // Współrzędna X pozycji postaci na planszy
        public int PositionY { get; set; }        // Współrzędna Y pozycji postaci na planszy
        public char Symbol { get; set; }        // Symbol reprezentujący postać na planszy
        public int Moves { get; set; }        // Liczba dostępnych ruchów postaci
        public int Hp { get; set; }        // Punkty zdrowia postaci
        public int Damage { get; set; }        // Siła ataku postaci
        public int Ammo { get; set; }        // Ilość amunicji postaci (dotyczy np. strzelca)
        public int Shield { get; set; }        // Wartość tarczy ochronnej postaci
        // Konstruktor inicjujący właściwości postaci
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
    // Klasa reprezentująca postacie kontrolowane przez komputer
    public class ComputerCharacters : Characters
    {
        // Konstruktor inicjujący właściwości postaci kontrolowanych przez komputer
        public ComputerCharacters(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
        {
        }
    }
    // Klasa reprezentująca postać medyka
    public class Apothecary : Characters
    {
        // Konstruktor inicjujący właściwości postaci medyka
        public Apothecary(int positionX, int positionY, char symbol, int moves, int hp, int damage, int ammo, int shield) : base(positionX, positionY, symbol, moves, hp, damage, ammo, shield)
        {
        }
    }
    // Klasa reprezentująca postać zwiadowcy
    public class Scout : Characters
    {
        // Konstruktor inicjujący właściwości postaci zwiadowcy
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
}
