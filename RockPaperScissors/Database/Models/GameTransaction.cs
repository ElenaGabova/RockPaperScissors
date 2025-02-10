using System;

namespace Database.Models
{
    /// <summary>
    /// Денежная транзакция .
    /// </summary>
    public class GameTransaction
    {
        /// <summary>
        /// Уникальный идентификатор транзакции.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор отправителя.
        /// </summary>
        public int FromUserId { get; set; }

        /// <summary>
        /// Идентификатор получателя.
        /// </summary>
        public int ToUserId { get; set; }

        /// <summary>
        /// Сумма перевода.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Тип транзакции.
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Дата и время проведения транзакции.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Отправитель.
        /// </summary>
        public User FromUser { get; set; }

        /// <summary>
        /// Получатель.
        /// </summary>
        public User ToUser { get; set; }
    }
}