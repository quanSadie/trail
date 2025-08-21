namespace Yahtzee;

public class RollResult
{
    public int RoundNumber { get; set; }
    public int[] DiceRoll { get; set; } = Array.Empty<int>();
    public int Score { get; set; }
}