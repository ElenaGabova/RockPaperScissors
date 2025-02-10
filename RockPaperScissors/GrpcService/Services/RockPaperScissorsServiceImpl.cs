using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Database;
using Database.Models;
using Database.Repository;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Controllers;

namespace RockPaperScissors
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RockPaperScissorsServiceImpl : RockPaperScissorsService.RockPaperScissorsServiceBase
    {
        private readonly ILogger<RockPaperScissorsServiceImpl> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MatchHistory> _matchRepository;
        private readonly IRepository<GameTransaction> _transactionRepository;

        public RockPaperScissorsServiceImpl(ILogger<RockPaperScissorsServiceImpl> logger,
                                            IRepository<User> userRepository,
                                            IRepository<MatchHistory> matchRepository,
                                            IRepository<GameTransaction> transactionRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _matchRepository = matchRepository;
            _transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Получить текущий баланс.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<BalanceResponse> GetBalance(UserRequest request, ServerCallContext context)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user != null)
            {
                return new BalanceResponse { Balance = user.Balance.ToString() };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Пользователь с ID {request.UserId} не найден"));
            }
        }

        /// <summary>
        /// Посмотреть список матчей, в которых участвуешь.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<MatchListResponse> ListMatches(UserRequest request, ServerCallContext context)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Пользователь с ID {request.UserId} не найден"));
            }

            var result = user.MatchesPlayer1.Select(m => new MatchInfo
            {
                Result = GetMatchResult(m)
            }).ToList();

            result.AddRange(
                user.MatchesPlayer2.Select(m => new MatchInfo
                {
                    Result = GetMatchResult(m)
                }).ToList());

            return new MatchListResponse { Matches = { result.ToArray() } };
        }


        /// <summary>
        /// Получение результата матча.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>

        private string GetMatchResult(MatchHistory match)
        {
            if (match.Player1 == null && match.Player1Id != 0)
            {
                match.Player1 = _userRepository.GetByIdAsync(match.Player1Id).Result;
                _matchRepository.UpdateAsync(match.Id, match);
            }

            if (match.Player2 == null && match.Player2Id != 0)
            {
                match.Player2 = _userRepository.GetByIdAsync(match.Player2Id).Result;
                _matchRepository.UpdateAsync(match.Id, match);
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine($"Матч ID: {match.Id}, Ставка: {match.BetAmount}");
            if (match.Player2 == null && match.Player2Id != null)
                match.Player2 = _userRepository.GetByIdAsync(match.Player2Id).Result;

            if (match.MovePlayer1 == null && match.MovePlayer2 == null)
                result.AppendLine($"Ожидается игрок: {match.Player1.Username}, {match.Player2.Username}");
            else
            {
                if (match.MovePlayer1 == null)
                    result.AppendLine($"Ожидается игрок: {match.Player1.Username}");

                if (match.MovePlayer2 == null)
                    result.AppendLine($"Ожидается игрок: {match.Player2.Username}");
            }

            if (match.Winner != null)
                result.AppendLine($"Победитель: {match.Winner.Username}, проигравший:{match.Loser.Username} ");

            return result.ToString();
        }

        /// <summary>
        /// Присоедениться к игре.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<JoinMatchResponse> JoinMatch(JoinMatchRequest request, ServerCallContext context)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Пользователь с ID {request.UserId} не найден"));
            }

            var match = await _matchRepository.GetByIdAsync(request.MatchId);
            if (match == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Матч с ID {request.MatchId} не найден"));
            }
            if (match.Player1.Id != request.UserId && match.Player2.Id != request.UserId)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Пользователь не участвет в данном матче."));
            }

            if (match.Player1.Balance < match.BetAmount)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, $"У игрока {match.Player1.Username} не хватает средств для игры."));
            }

            if (match.Player2.Balance < match.BetAmount)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, $"У игрока {match.Player2.Username} не хватает средств для игры."));
            }


            char move = char.ToUpper(request.Move[0]);
            if (!GameActions.IsValidMove(move))
                throw new RpcException(new Status(StatusCode.Unavailable, "Недоступный ход. Возможные варинты: К, Н, Б."));

            if (match.Player1.Id == request.UserId)
            {
                if (match.MovePlayer1 == null)
                    match.MovePlayer1 = move;
                else
                    throw new RpcException(new Status(StatusCode.Unavailable, "Вы уже присоеденились к матчу."));
            }

            if (match.Player2.Id == request.UserId)
            {
                if (match.MovePlayer2 == null)
                    match.MovePlayer2 = move;
                else
                    throw new RpcException(new Status(StatusCode.Unavailable, "Вы уже присоеденились к матчу."));
            }

            if (match.MovePlayer1 != null && match.MovePlayer2 != null)
            {
                var result = await PlayGame(match);
                return new JoinMatchResponse { Message = result };
            }

            await _matchRepository.UpdateAsync(match.Id, match);
            return new JoinMatchResponse { Message = "Вы присоединились к матчу" };
        }


        /// <summary>
        /// Сыграть в игру.
        /// </summary>
        /// match
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public async Task<string> PlayGame(MatchHistory game)
        {
            // Получение игроков
            var player1 = await _userRepository.GetByIdAsync(game.Player1Id);
            var player2 = await _userRepository.GetByIdAsync(game.Player2Id);

            // Определение победителя (логика для "Камень, ножницы, бумага")
            int winnerId = GameActions.DetermineWinner(game.Player1Id, game.Player2Id, game.MovePlayer1.Value, game.MovePlayer2.Value);
            if (winnerId == 0)
            {
                // Ничья – можно вернуть результат или обозначить ничью (например, WinnerId = null)
                // В данном примере ничья не обрабатывается переводом средств

                return "Ничья. Ставка возвращена";
            }

            // Обновление баланса – проигравший переводит ставку победителю
            User winner = (winnerId == player1.Id) ? player1 : player2;
            User loser = (winnerId == player1.Id) ? player2 : player1;

            loser.Balance -= game.BetAmount;
            winner.Balance += game.BetAmount;

            game.WinnerId = winnerId;
            game.LoserId = loser.Id;
            game.CreatedAt = DateTime.UtcNow;


            await _matchRepository.UpdateAsync(game.Id, game);

            // Запись транзакции для матча
            var transaction = new GameTransaction
            {
                FromUserId = loser.Id,
                ToUserId = winner.Id,
                Amount = game.BetAmount,
                TransactionType = "match",
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(transaction);
            await _userRepository.UpdateAsync(winner.Id, winner);
            await _userRepository.UpdateAsync(loser.Id, loser);

            return $"Матч завершён. Победил игрок :{winner.Username}";
        }
    }
}
