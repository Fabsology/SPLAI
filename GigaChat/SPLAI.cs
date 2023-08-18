using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class SimpleProbabilityLanguageAI
{
    private Dictionary<string, List<string>> responseDatabase;
    private Dictionary<string, Dictionary<string, int>> responseStatistics;
    public const bool debug = false;

    public SimpleProbabilityLanguageAI()
    {
        InitializeResponseDatabase();
        InitializeResponseStatistics();
    }

    public string GenerateResponse(string input)
    {
        Dictionary<string, double> responseProbabilities = new Dictionary<string, double>();

        foreach (var pattern in responseDatabase.Keys)
        {
            if (CheckPatternInInput(pattern, input))
            {
                foreach (var response in responseDatabase[pattern])
                {
                    double probability = CalculateResponseProbability(pattern, response);

                    if (debug) { 
                        Console.ForegroundColor = ConsoleColor.Blue;
                        System.Console.WriteLine("\t\t{0}", response);
                        Console.ForegroundColor = ConsoleColor.Magenta;                
                        System.Console.WriteLine("\t\t\t" + probability);
                    }

                    responseProbabilities[response] = probability;
                }
            }
        }

        if (responseProbabilities.Count == 0)
        {
            return "Keine Ahnung was du meinst. Kannst du die Anfrage bitte anders stellen?";
        }

        var bestResponse = responseProbabilities.OrderByDescending(kv => kv.Value).First();
        return bestResponse.Key;
    }

    public void Learn(string pattern, string response)
    {
        pattern = pattern.ToLower();
        if (responseDatabase.ContainsKey(pattern))
        {
            responseDatabase[pattern].Add(response);
        }
        else
        {
            responseDatabase[pattern] = new List<string> { response };
        }
        UpdateResponseStatistics(pattern, response);
    }

    public void SaveResponsesToFile(string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (var pattern in responseDatabase.Keys)
            {
                string responses = string.Join("|", responseDatabase[pattern]);
                writer.WriteLine($"{pattern}:{responses}");
            }
        }
    }

    public void LoadResponsesFromFile(string fileName)
    {
        responseDatabase.Clear();

        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string pattern = parts[0];
                    string[] responses = parts[1].Split('|');
                    responseDatabase[pattern] = responses.ToList();
                }
            }
        }
        InitializeResponseStatistics();
    }

    private void InitializeResponseDatabase()
    {
        responseDatabase = new Dictionary<string, List<string>>
        {
        };
    }

    private void InitializeResponseStatistics()
    {
        responseStatistics = new Dictionary<string, Dictionary<string, int>>();
        foreach (var pattern in responseDatabase.Keys)
        {
            responseStatistics[pattern] = new Dictionary<string, int>();
            foreach (var response in responseDatabase[pattern])
            {
                responseStatistics[pattern][response] = 1;
            }
        }
    }

    private void UpdateResponseStatistics(string pattern, string response)
    {
        if (!responseStatistics.ContainsKey(pattern))
        {
            responseStatistics[pattern] = new Dictionary<string, int>();
        }

        if (responseStatistics[pattern].ContainsKey(response))
        {
            responseStatistics[pattern][response]++;
        }
        else
        {
            responseStatistics[pattern][response] = 1;
        }
    }

    private double CalculateResponseProbability(string pattern, string response)
    {
        int totalResponses = responseStatistics[pattern].Values.Sum();
        int responseCount = responseStatistics[pattern][response];

        // Verbesserte Gewichtung basierend auf der Anzahl der Vorkommen
        double weight = 1.0 + (responseCount / (double)totalResponses);

        return weight;
    }

    private bool CheckPatternInInput(string pattern, string input)
    {
        double requiredMatchPercentage = 0.5; // Mindestens 55% Übereinstimmung erforderlich

        int patternLength = pattern.Length;
        int requiredMatches = (int)Math.Ceiling(patternLength * requiredMatchPercentage);

        for (int startIndex = 0; startIndex <= input.Length - patternLength; startIndex++)
        {
            int matchingCharacters = 0;

            for (int i = 0; i < patternLength; i++)
            {
                char inputCharacter = char.ToLower(input[startIndex + i]);
                char patternCharacter = pattern[i];

                if (inputCharacter == patternCharacter)
                {
                    matchingCharacters++;
                }
            }

            if (matchingCharacters >= requiredMatches)
            {
                return true; // Mindestens 55% des Musters gefunden
            }
        }

        return false;
    }



}