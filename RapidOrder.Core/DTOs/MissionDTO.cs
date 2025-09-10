public class MissionDto
{
    public long Id { get; set; }
    public DateTime StartedAt { get; set; }
    public string Status { get; set; } = "";
    public PlaceDto Place { get; set; } = default!;
}

public class PlaceDto
{
    public int Id { get; set; }
    public int Number { get; set; }
}