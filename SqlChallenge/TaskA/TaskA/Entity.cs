namespace TaskA;

public class Entity
{
    public int EntityId { get; set; }
    public string EntityFirstName { get; set; } = string.Empty;
    public string? EntityMiddleName { get; set; }
    public string? EntityLastName { get; set; }
    public DateTime? EntityDob { get; set; }
    public bool IsMaster { get; set; }
    public string? Address { get; set; }
    public string? EntityGender { get; set; }

    public List<string> ValidationErrors { get; set; } = new List<string>();
    public bool IsValid => !ValidationErrors.Any();
}