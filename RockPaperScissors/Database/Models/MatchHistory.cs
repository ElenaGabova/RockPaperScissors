using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
    /// <summary>
    /// История матча между двумя пользователями.
    /// </summary>
    public class MatchHistory
    {
        /// <summary>
        /// Уникальный идентификатор матча.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор первого игрока.
        /// </summary>
        public int Player1Id { get; set; }

        /// <summary>
        /// Идентификатор второго игрока .
        /// </summary>
        public int Player2Id { get; set; }

        /// <summary>
        /// Сумма ставки, сделанной в матче.
        /// </summary>
        public decimal BetAmount { get; set; }

        /// <summary>
        /// Идентификатор победителя матча.
        /// </summary>
        public int? WinnerId { get; set; }

        /// <summary>
        /// Идентификатор проигравшего матча.
        /// </summary>
        public int? LoserId { get; set; }
        /// <summary>
        /// Ход первого игрока.
        /// </summary>
        public char? MovePlayer1 { get; set; }

        /// <summary>
        /// Ход второго игрока.
        /// </summary>
        public char? MovePlayer2 { get; set; }

        /// <summary>
        /// Дата и время создания матча.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Навигационное свойство для первого игрока.
        /// </summary>
        public User Player1 { get; set; }

        /// <summary>
        /// Навигационное свойство для второго игрока.
        /// </summary>
        public User Player2 { get; set; }

        /// <summary>
        /// Навигационное свойство для победителя матча.
        /// </summary>
        public User Winner { get; set; }

        /// <summary>
        /// Навигационное свойство для проигравшего матча.
        /// </summary>
        public User Loser { get; set; }

        /// <summary>
        /// Признак ничьи.
        /// </summary>
        public bool isDraw { get; set; }
    }
}