using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    public class RpsGameContext : DbContext
    {
        public RpsGameContext(DbContextOptions<RpsGameContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Пользователи в системе.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// История матчей.
        /// </summary>
        public DbSet<MatchHistory> MatchHistories { get; set; }

        /// <summary>
        /// Денежные транзакции.
        /// </summary>
        public DbSet<GameTransaction> GameTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                // Имя обязательное и имеет максимум 20 символов.
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(20);

                // Имя пользователя уникальное, индекс по имени пользователя.
                entity.HasIndex(u => u.Username)
                      .IsUnique();

                // Значение баланса по умолчанию 100.00.
                entity.Property(u => u.Balance)
                      .HasDefaultValue(100.00m);
            });

            modelBuilder.Entity<MatchHistory>(entity =>
            {
                entity.HasKey(m => m.Id);

                // Ставка обязательна.
                entity.Property(m => m.BetAmount)
                      .IsRequired()
                      .HasColumnType("decimal(10,2)");

                // Связь с первым игроком
                entity.HasOne(m => m.Player1)
                      .WithMany(u => u.MatchesPlayer1)
                      .HasForeignKey(m => m.Player1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь со вторым игроком
                entity.HasOne(m => m.Player2)
                    .WithMany(u => u.MatchesPlayer2)
                    .HasForeignKey(m => m.Player2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с победителем матча (может быть null)
                entity.HasOne(m => m.Winner)
                      .WithMany()
                      .HasForeignKey(m => m.WinnerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Индекс  по победителю.
                entity.HasIndex(m => m.WinnerId);
                
                // Связь с проигравшим матча (может быть null)
                entity.HasOne(m => m.Loser)
                      .WithMany()
                      .HasForeignKey(m => m.LoserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Индекс  по проигравшему.
                entity.HasIndex(m => m.LoserId);

            });

            modelBuilder.Entity<GameTransaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                // Сумма транзакции обязательна.
                entity.Property(t => t.Amount)
                      .IsRequired();

                // Тип транзакции обязательный.
                entity.Property(t => t.TransactionType)
                      .IsRequired();

                // Связь с отправителем транзакции
                entity.HasOne(t => t.FromUser)
                      .WithMany(u => u.SentTransactions)
                      .HasForeignKey(t => t.FromUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Связь с получателем транзакции
                entity.HasOne(t => t.ToUser)
                      .WithMany(u => u.ReceivedTransactions)
                      .HasForeignKey(t => t.ToUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Составной индекс для ускорения поиска транзакций между двумя пользователями
                entity.HasIndex(t => new { t.FromUserId, t.ToUserId });
            });
        }
    }
}
