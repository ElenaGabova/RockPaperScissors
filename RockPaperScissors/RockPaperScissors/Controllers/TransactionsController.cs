using System;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Mvc;

namespace RockPaperScissors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<GameTransaction> _gameRepository;

        public TransactionsController( IRepository<User> userRepository, IRepository<GameTransaction> gameRepository)
        {
            _userRepository = userRepository;
            _gameRepository = gameRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransferRequest request)
        {
            var fromUser = await _userRepository.GetByIdAsync(request.FromUserId);
            if (fromUser == null || fromUser.Balance < request.Amount)
            {
                return BadRequest("Недостаточно средств.");
            }

            var toUser = await _userRepository.GetByIdAsync(request.ToUserId);
            if (toUser == null)
            {
                return NotFound("Пользователь-получатель не найден.");
            }


            if (fromUser.Balance < request.Amount)
            {
                return BadRequest("Недостаточно средств для перевода");
            }

            fromUser.Balance -= request.Amount;
            toUser.Balance += request.Amount;

            var transaction = new GameTransaction
            {
                FromUserId = request.FromUserId,
                ToUserId = request.ToUserId,
                Amount = request.Amount,
                TransactionType = "Transfer",
                CreatedAt = DateTime.UtcNow
            };
            await _gameRepository.CreateAsync(transaction);
            return Ok(new { message = "Транзакция успешно выполнена." });
        }

        public class TransferRequest
        {
            public int FromUserId { get; set; }
            public int ToUserId { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
