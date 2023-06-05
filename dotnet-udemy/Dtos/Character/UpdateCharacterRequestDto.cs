namespace dotnet_udemy.Dtos.Character;

public class UpdateCharacterRequestDto
{
    public int Id { get; set; }
    public int HitPoints { get; set; }
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int Intelligence { get; set; }
}