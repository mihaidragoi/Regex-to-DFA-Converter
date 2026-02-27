using System;
using System.IO;

namespace Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string regex = "";

            try
            {
                if (File.Exists("input.txt"))
                    regex = File.ReadAllText("input.txt").Trim();
                else
                    regex = "a(b|c)*";
            }
            catch { regex = "a(b|c)*"; }

            Console.WriteLine($"Expresia regulata initiala: {regex}\n");

            var converter = new RegexToDFA();
            DeterministicFiniteAutomaton M = null;

            bool running = true;
            while (running)
            {
                running = AfisareMeniu(converter, ref M, regex);
            }
        }

        static bool AfisareMeniu(RegexToDFA converter, ref DeterministicFiniteAutomaton M, string regex)
        {
            Console.WriteLine("\n--- MENIU ---");
            Console.WriteLine("1. Afiseaza forma poloneza postfixata");
            Console.WriteLine("2. Construieste si afiseaza Automatul Finit Determinist (M)");
            Console.WriteLine("3. Afiseaza arborele sintactic");
            Console.WriteLine("4. Verifica un cuvant in automat");
            Console.WriteLine("5. Iesire");
            Console.Write("Alege o optiune: ");

            string optiune = Console.ReadLine();
            Console.WriteLine();

            switch (optiune)
            {
                case "1":
                    string polishForm = converter.PolishForm(regex);
                    Console.WriteLine("Forma poloneza: " + polishForm);
                    break;

                case "2":
                    M = converter.BuildDFAFromRegex(regex);
                    Console.WriteLine("Automatul a fost construit cu succes!");
                    M.PrintAutomaton();
                    var originalOut = Console.Out;
                    using (var writer = new StringWriter())
                    {
                        Console.SetOut(writer);
                        M.PrintAutomaton();
                        Console.SetOut(originalOut);
                        File.WriteAllText("DFA_Output.txt", writer.ToString());
                    }
                    Console.WriteLine("Automatul a fost salvat in DFA_Output.txt");
                    break;

                case "3":
                    converter.SyntaxTreeDisplay(regex);
                    Console.WriteLine("Arborele sintactic a fost afisat.");
                    break;

                case "4":
                    if (M == null)
                    {
                        Console.WriteLine("Eroare: Automatul nu a fost construit inca. Alege optiunea 2 mai intai.");
                    }
                    else
                    {
                        Console.Write("Introdu cuvantul de verificat: ");
                        string word = Console.ReadLine();
                        M.CheckWord(word);
                    }
                    break;

                case "5":
                     Console.WriteLine("Iesire din program. La revedere!");
                     return false;

                default:
                    Console.WriteLine("Optiune invalida.");
                    break;
            }
            return true;
        }
    }
}