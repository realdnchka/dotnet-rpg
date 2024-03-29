using System.Security.Claims;
using dotnet_udemy.Dtos.Weapon;

namespace dotnet_udemy.Services.WeaponService;

public class WeaponService: IWeaponService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    public async Task<ServiceResponse<GetCharacterResponseDto>> AddWeapon(AddWeaponDto newWeapon)
    {
        var response = new ServiceResponse<GetCharacterResponseDto>();
        try
        {
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                                          c.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User
                                              .FindFirstValue(ClaimTypes.NameIdentifier)!));
            if (character is null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }

            // var weapon = _mapper.Map<Weapon>(newWeapon);

            var weapon = new Weapon
            {
                Name = newWeapon.Name,
                Damage = newWeapon.Damage,
                CharacterId = newWeapon.CharacterId
            };
            
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();

            response.Data = _mapper.Map<GetCharacterResponseDto>(character);
        }
        catch(Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}