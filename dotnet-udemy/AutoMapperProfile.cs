using dotnet_udemy.Dtos.Character.Skill;
using dotnet_udemy.Dtos.Fight;
using dotnet_udemy.Dtos.Weapon;

namespace dotnet_udemy;

public class AutoMapperProfile: Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Character, GetCharacterResponseDto>();
        CreateMap<AddCharacterRequestDto, Character>();
        CreateMap<UpdateCharacterRequestDto, Character>();
        CreateMap<AddWeaponDto, Weapon>();
        CreateMap<Weapon, GetWeaponDto>();
        CreateMap<Skill, GetSkillDto>();
        CreateMap<Character, HighScoreDto>();
    }
}