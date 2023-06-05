namespace dotnet_udemy.Services.CharacterService;

public interface ICharacterService
{
    Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacter();
    Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id);
    Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter);
    Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter);
    Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id);
    Task<ServiceResponse<GetCharacterResponseDto>> AddCharacterSkill(AddSkillCharacterDto newCharacterSkill);
}