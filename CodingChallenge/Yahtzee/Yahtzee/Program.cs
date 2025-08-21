using Yahtzee;

Console.WriteLine("Yahtzee High Score Calculator");
Console.WriteLine("============================\n");

try
{
    var scorer = new YahtzeeScorer();

    Console.WriteLine("Starting main simulation...\n");

    const int rounds = 1000;
    
    int i = 10; // number of dice
    int j = 24; // number of sides

    var result = scorer.RollTheDice(rounds, i, j);
    result.DisplaySummary();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Environment.ExitCode = 1;
}

Console.WriteLine("\nPress any key to close...");
Console.ReadKey();