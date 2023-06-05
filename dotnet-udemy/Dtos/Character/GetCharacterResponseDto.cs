using dotnet_udemy.Dtos.Character.Skill;
using dotnet_udemy.Dtos.Weapon;

namespace dotnet_udemy.Dtos.Character;

public class GetCharacterResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "Gargoon";
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public RpgClass Class { get; set; } = RpgClass.Knight;
    public GetWeaponDto? Weapon { get; set; }
    public List<GetSkillDto>? Skills { get; set; }
    public int Fights { get; set; }
    public int Wins { get; set; }
    public int Defeats { get; set; }
}