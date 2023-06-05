using dotnet_udemy.Dtos.Weapon;

namespace dotnet_udemy.Services.WeaponService;

public interface IWeaponService
{
    Task<ServiceResponse<GetCharacterResponseDto>> AddWeapon(AddWeaponDto newWeapon);
}