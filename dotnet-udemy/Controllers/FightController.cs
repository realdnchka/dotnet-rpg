using dotnet_udemy.Dtos.Fight;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_udemy.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FightController: ControllerBase
{
    private readonly IFightService _fightService;

    public FightController(IFightService fightService)
    {
        _fightService = fightService;
    }
    
    //TODO Implementing Strategy Pattern
    [HttpPost("Weapon")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto request)
    {
        return Ok(await _fightService.WeaponAttack(request));
    }
    
    [HttpPost("Skill")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(SkillAttackDto request)
    {
        return Ok(await _fightService.SkillAttack(request));
    }
    
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<FightResultDto>>> Fight(FightRequestDto request)
    {
        return Ok(await _fightService.Fight(request));
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore()
    {
        return Ok(await _fightService.GetHighScore());
    }
}