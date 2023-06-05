namespace dotnet_udemy.Dtos.Character;

public class AddCharacterRequestDto
{
    public string Name { get; set; } = "Gargoon";
    public RpgClass Class { get; set; } = RpgClass.Knight;
}