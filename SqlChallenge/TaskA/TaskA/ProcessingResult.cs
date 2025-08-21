namespace TaskA;

public class ProcessingResult
{
    public List<Entity> ValidEntities { get; set; } = new List<Entity>();
    public List<Entity> InvalidEntities { get; set; } = new List<Entity>();
    public int TotalProcessed { get; set; }
    public List<string> ProcessingErrors { get; set; } = new List<string>();
}