using System.Diagnostics;

namespace Yahtzee;

public class YahtzeeScorer
{
    private readonly Random _random = new();

    private int CalculateMaxScore(int[] diceRoll)
    {
        if (diceRoll == null || diceRoll.Length == 0)
        {
            throw new ArgumentException("dice roll null or empty exception", nameof(diceRoll));
        }

        var valueCounts = new Dictionary<int, int>();

        foreach (int value in diceRoll)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"invalid dice value: {value}. dice value must be positive",
                    nameof(diceRoll));
            }

            valueCounts[value] = valueCounts.GetValueOrDefault(value, 0) + 1;
        }

        int maxScore = 0;
        foreach (var kvp in valueCounts)
        {
            int score = kvp.Key * kvp.Value;
            if (score > maxScore)
            {
                maxScore = score;
            }
        }

        return maxScore;
    }

    private int[] GenerateRoll(int dice, int sides)
    {
        if (dice <= 0 || sides <= 0)
        {
            throw new ArgumentException("dice or sides must be positive", nameof(dice));
        }

        var roll = new int[dice];

        for (int i = 0; i < dice; i++)
        {
            roll[i] = _random.Next(1, sides + 1);
        }

        return roll;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rounds"></param>
    /// <param name="i">number of dice</param>
    /// <param name="j">number of sides</param>
    /// <returns></returns>
    public YahtzeeResult RollTheDice(int rounds, int i, int j)
    {
        if (rounds <= 0) throw new ArgumentException("round must be positive", nameof(rounds));

        var stopwatch = new Stopwatch();
        var results = new List<RollResult>(rounds);

        try
        {
            for (int round = 0; round < rounds; round++)
            {
                var diceRoll = GenerateRoll(i, j);
                int score = CalculateMaxScore(diceRoll);
                var roundResult = new RollResult
                {
                    RoundNumber = round,
                    DiceRoll = (int[])diceRoll.Clone(),
                    Score = score
                };
                results.Add(roundResult);

                Console.WriteLine($"Round {round:N0}: [{string.Join(", ", diceRoll)}] => Score: {score:N0}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error during roll at round {results.Count + 1}", ex);
        }
        finally
        {
            stopwatch.Stop();
        }
        
        return new YahtzeeResult
        {
            Rounds = results,
            ElapsedTime = stopwatch.Elapsed,
            NumDice = i,
            NumSides = j
        };
    }
}