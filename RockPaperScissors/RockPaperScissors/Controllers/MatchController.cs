using System.Reflection;
using Database;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RockPaperScissors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MatchHistory> _matchRepository;
        private readonly IRepository<GameTransaction> _transactionRepository;

        public MatchController(IRepository<User> userRepository, IRepository<MatchHistory> matchRepository, IRepository<GameTransaction> transactionRepository)
        {
            _userRepository = userRepository;
            _matchRepository = matchRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMatch([FromBody] CreateMatchRequest request)
        {
            if (request.BetAmount <= 0)
            {
                return BadRequest("Ставка должна быть положительной");
            }

            // Получение игроков
            var player1 = await _userRepository.GetByIdAsync(request.Player1Id);
            var player2 = await _userRepository.GetByIdAsync(request.Player2Id);

            if (player1 == null || player2 == null)
            {
                return NotFound("Один или оба игрока не найдены");
            }
            // Проверка достаточного баланса для обоих игроков
            if (player1.Balance < request.BetAmount || player2.Balance < request.BetAmount)
            {
                return BadRequest("Один из игроков не имеет достаточного баланса для ставки");
            }

            // Запись истории матча
            var match = new MatchHistory
            {
                Player1Id = player1.Id,
                Player2Id = player2.Id,
                BetAmount = request.BetAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _matchRepository.CreateAsync(match);
            return Ok(new { message = "Матч успешно создан." });
        }

        public class CreateMatchRequest
        {
            public int Player1Id { get; set; }
            public int Player2Id { get; set; }
            public decimal BetAmount { get; set; }
        }
    }
}
