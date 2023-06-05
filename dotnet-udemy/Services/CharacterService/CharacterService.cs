using System.Security.Claims;

namespace dotnet_udemy.Services.CharacterService;

public class CharacterService: ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
        .FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacter()
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
        var dbCharacters = await _context.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .Where(c => c.User!.Id == GetUserId())
            .ToListAsync();
        serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterResponseDto>(c)).ToList();
        
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

        try
        {
            var dbCharacter = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
            if (dbCharacter is null)
            {
                throw new Exception("Character not found");
            }

            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(dbCharacter);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
        
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();
        
        serviceResponse.Data = await _context.Characters
            .Where(c => c.User!.Id == GetUserId())
            .Select(c => _mapper.Map<GetCharacterResponseDto>(c))
            .ToListAsync();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
    {
        var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();
        
        try
        {
            var character = 
                await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

            if (character is null || character.User!.Id != GetUserId())
            {
                throw new Exception($"Character with id '{updatedCharacter.Id}' not exist");
            }

            character = _mapper.Map<Character>(updatedCharacter);
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
        
        try
        {
            var character = await _context.Characters.FirstAsync(c => c.Id == id && c.User!.Id == GetUserId());
            if (character is null)
            {
                throw new Exception($"Character with id '{id}' not exist");
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            serviceResponse.Data = 
                await _context.Characters
                    .Where(c => c.User!.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterResponseDto>(c)).ToListAsync();
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterResponseDto>> AddCharacterSkill(AddSkillCharacterDto newCharacterSkill)
    {
        var response = new ServiceResponse<GetCharacterResponseDto>();
        try
        {
            var character = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                                          c.User!.Id == GetUserId());
            if (character is null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }

            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
            
            if (skill is null)
            {
                response.Success = false;
                response.Message = "Skill not found.";
                return response;
            }
            
            character.Skills!.Add(skill);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}