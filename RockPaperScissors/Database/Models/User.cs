using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Database.Models
{
    /// <summary>
    /// Пользователь системы.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Уникальное имя пользователя. 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Баланс пользователя.
        /// </summary>
        public decimal Balance { get; set; }


        /// <summary>
        /// Коллекция матчей, где пользователь является 1 игроком.
        /// </summary>
        public ICollection<MatchHistory> MatchesPlayer1 { get; set; }


        /// <summary>
        ///  Коллекция матчей, где пользователь является 2 игроком.
        /// </summary>
        public ICollection<MatchHistory> MatchesPlayer2 { get; set; }


        /// <summary>
        /// Отправленные пользователем транзакции.
        /// </summary>
        public ICollection<GameTransaction> SentTransactions { get; set; }

        /// <summary>
        /// Полученные пользователем транзакции.
        /// </summary>
        public ICollection<GameTransaction> ReceivedTransactions { get; set; }
    }
}
