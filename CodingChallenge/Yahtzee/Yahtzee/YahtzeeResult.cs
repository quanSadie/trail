namespace Yahtzee;

public class YahtzeeResult
{
    public List<RollResult> Rounds { get; set; } = [];
    public TimeSpan ElapsedTime { get; set; }
    public int NumDice { get; set; }
    public int NumSides { get; set; }
        
    public void DisplaySummary()
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Configuration: {NumDice} dice with {NumSides} sides each");
        Console.WriteLine($"Total Rounds: {Rounds.Count:N0}");
        Console.WriteLine($"Execution Time: {ElapsedTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Performance: {Rounds.Count / ElapsedTime.TotalSeconds:F0} rounds/second");
    }
}