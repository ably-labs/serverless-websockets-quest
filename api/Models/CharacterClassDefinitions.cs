using System;
using System.Collections.Generic;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public static class CharacterClassDefinitions
    {

        public static class Fighter
        {
            public static string CharacterClass => "fighter";
            public static int InitialHealth => 60;
            public static int Damage => Randomizer.Next(1, 21);
        }

        public static class Ranger
        {
            public static string CharacterClass => "ranger";
            public static int InitialHealth => 50;
            public static int Damage => Randomizer.Next(1, 21);
        }

        public static class Mage
        {
            public static string CharacterClass => "mage";
            public static int InitialHealth => 40;
            public static int Damage => Randomizer.Next(1, 21);
        }

        public static class Monster
        {
            public static string Name => "Monstarrr";
            public static string CharacterClass => "monster";
            public static int InitialHealth => 100;
            public static int Damage => Randomizer.Next(5, 21);
        }

        public static int GetDamageFor(string className)
        {
            return characterDamage[className].Invoke();
        }

        public static int GetInitialHealthFor(string className)
        {
            return characterHealth[className].Invoke();
        }

        private static Random Randomizer = new Random();

        private static Dictionary<string, Func<int>> characterDamage = new Dictionary<string, Func<int>> {
            { Fighter.CharacterClass, () => Fighter.Damage },
            { Ranger.CharacterClass, () => Ranger.Damage },
            { Mage.CharacterClass, () => Mage.Damage },
            { Monster.CharacterClass, () => Monster.Damage }
        };

        private static Dictionary<string, Func<int>> characterHealth = new Dictionary<string, Func<int>> {
            { Fighter.CharacterClass, () => Fighter.InitialHealth },
            { Ranger.CharacterClass, () => Ranger.InitialHealth },
            { Mage.CharacterClass, () => Mage.InitialHealth },
            { Monster.CharacterClass, () => Monster.InitialHealth }
        };
    }
}