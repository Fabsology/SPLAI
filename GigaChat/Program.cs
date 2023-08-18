using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigaChat
{
    internal class Program
    {
        public const string FILENAME = "speech.ai";
        static void Main(string[] args)
        {
            SimpleProbabilityLanguageAI languageAI = new SimpleProbabilityLanguageAI();
            Console.WriteLine("Willkommen! Ich bin deine probabilistische Sprach-KI. Frage mich etwas:");
            
            if (System.IO.File.Exists(FILENAME))
            {
                languageAI.LoadResponsesFromFile(FILENAME);
            }

            while (true)
            {
                Console.Write("»");
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                    break;

                if (input == "hilfe") {
                    Console.WriteLine("lernen \t\t Lernen eines neuen Musters");
                    Console.WriteLine("speichern \t Speichern der aktuellen Sprachbibliothek");
                    Console.WriteLine("laden \t\t Laden einer Sprachbibliothek");

                }
                else if (input.ToLower().StartsWith("lernen"))
                {
                    Console.WriteLine("Muster eingeben:");
                    string pattern = Console.ReadLine();
                    Console.WriteLine("Antwort eingeben:");
                    string response = Console.ReadLine();
                    languageAI.Learn(pattern, response);
                    Console.WriteLine("Gelernt!");
                }
                else if (input.ToLower().StartsWith("speichern"))
                {
                    Console.WriteLine("Dateiname zum Speichern eingeben:");
                    string fileName = Console.ReadLine();
                    languageAI.SaveResponsesToFile(fileName);
                    Console.WriteLine("Daten gespeichert!");
                }
                else if (input.ToLower().StartsWith("laden"))
                {
                    Console.WriteLine("Dateiname zum Laden eingeben:");
                    string fileName = Console.ReadLine();
                    languageAI.LoadResponsesFromFile(fileName);
                    Console.WriteLine("Daten geladen!");
                }
                else
                {
                    string response = languageAI.GenerateResponse(input);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("AI: " + response);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            languageAI.SaveResponsesToFile(FILENAME);
        }
    }
}
