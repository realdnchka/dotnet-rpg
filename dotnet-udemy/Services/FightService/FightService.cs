using dotnet_udemy.Dtos.Fight;

namespace dotnet_udemy.Services.FightService;

public class FightService: IFightService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public FightService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _context.Characters
                .Include(c => c.Weapon)
                .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
            
            var opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

            if (attacker is null || opponent is null)
            {
                throw new Exception("Something fishy is going on...");
            }

            var damage = DoWeaponAttack(attacker, opponent);
            if (opponent.HitPoints <= 0)
                response.Message = $"Opponent {opponent.Name} has been defeated";

            await _context.SaveChangesAsync();

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                Opponent = opponent.Name,
                AttackerHp = attacker.HitPoints,
                OpponentHp = opponent.HitPoints,
                Damage = damage
            };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();
        try
        {
            var attacker = await _context.Characters
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
            
            var opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
            
            if (attacker is null || opponent is null || attacker.Skills is null)
            {
                throw new Exception("Something fishy is going on...");
            }
            
            var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

            if (skill is null)
            {
                throw new Exception($"{attacker.Name} doesn't know that skill.");
            }
            
            var damage = DoSkillAttack(skill, attacker, opponent);
            if (opponent.HitPoints <= 0)
                response.Message = $"Opponent {opponent.Name} has been defeated";

            await _context.SaveChangesAsync();

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                Opponent = opponent.Name,
                AttackerHp = attacker.HitPoints,
                OpponentHp = opponent.HitPoints,
                Damage = damage
            };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
    {
        var response = new ServiceResponse<FightResultDto>
        {
            Data = new FightResultDto()
        };

        try
        {
            var characters = await _context.Characters
                .Include(c => c.Skills)
                .Include(c => c.Weapon)
                .Where(c => request.CharactersId.Contains(c.Id))
                .ToListAsync();

            bool defeated = false;
            while (!defeated)
            {
                foreach (var attacker in characters)
                {
                    var opponents = characters
                        .Where(c => c.Id != attacker.Id)
                        .ToList();
                    var opponent = opponents[new Random().Next(opponents.Count)];

                    int damage = 0;
                    string attackUsed = "";

                    bool useWeapon = new Random().Next(2) == 0;
                    if (useWeapon && attacker.Weapon is not null)
                    {
                        attackUsed = attacker.Weapon.Name;
                        damage = DoWeaponAttack(attacker, opponent);
                    }
                    else if(!useWeapon && attacker.Skills is not null)
                    {
                        var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                        attackUsed = skill.Name;
                        damage = DoSkillAttack(skill, attacker, opponent);
                    }
                    else
                    {
                        response.Data.Log.Add($"{attacker.Name} has no to attack with");
                        continue;
                    }
                    
                    response.Data.Log
                        .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage > 0 ? damage : 0)} damage");

                    if (opponent.HitPoints <= 0 )
                    {
                        defeated = true;
                        attacker.Wins++;
                        opponent.Defeats++;
                        response.Data.Log.Add($"{opponent.Name} has been defeated by {attacker.Name}");
                        break;
                    }
                }
            }
            
            characters.ForEach(c =>
            {
                c.Fights++;
                c.HitPoints = 100;
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
    {
        var characters = await _context.Characters
            .Where(c => c.Fights > 0)
            .OrderByDescending(c => c.Wins)
            .ThenBy(c => c.Defeats)
            .ToListAsync();

        var response = new ServiceResponse<List<HighScoreDto>>
        {
            Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList(),
        };

        return response;
    }

    //TODO Implement Strategy Pattern
    private static int DoSkillAttack(Skill skill, Character attacker, Character opponent)
    {
        int damage = skill.Damage + new Random().Next(attacker.Intelligence);
        damage -= new Random().Next(opponent.Defense);

        if (damage > 0)
            opponent.HitPoints -= damage;
        return damage;
    }
    
    private static int DoWeaponAttack(Character attacker, Character opponent)
    {
        if (attacker.Weapon is null)
            throw new Exception("Attacker weapon not found!");
        
        int damage = attacker.Weapon!.Damage + new Random().Next(attacker.Strength);
        damage -= new Random().Next(opponent.Defense);

        if (damage > 0)
            opponent.HitPoints -= damage;
        return damage;
    }
}