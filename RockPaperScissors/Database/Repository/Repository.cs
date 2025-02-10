using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Xml.Schema;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;


namespace Database.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly RpsGameContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<Repository<T>> _logger;

        public Repository(RpsGameContext context, ILogger<Repository<T>> logger)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _logger = logger;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                if (typeof(T) == typeof(User))
                {
                    return await _dbSet
                        .Include("MatchesPlayer1")
                        .Include("MatchesPlayer2")
                        .Include("SentTransactions")
                        .Include("ReceivedTransactions").ToListAsync();
                }
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при получении всех записей: {ex.Message}");
                return Enumerable.Empty<T>();
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                if (typeof(T) == typeof(User))
                {
                    return await _dbSet
                        .Include(u => ((User)(object)u).MatchesPlayer1)
                        .Include(u => ((User)(object)u).MatchesPlayer2)
                        .Include(u => ((User)(object)u).SentTransactions)
                        .Include(u => ((User)(object)u).ReceivedTransactions)
                        .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id) as T;
                }
                if (typeof(T) == typeof(MatchHistory))
                {
                    return await _dbSet
                        .Include(m => ((MatchHistory)(object)m).Player1)
                        .Include(m => ((MatchHistory)(object)m).Player2)
                        .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id) as T;
                }
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при получении записи по ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при добавлении записи: {ex.Message}");
            }

            return entity;
        }

        public async Task<T> UpdateAsync(int id, T entity)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обновлении записи с ID {id}: {ex.Message}");
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при удалении записи с ID {id}: {ex.Message}");
                return false;
            }

        }
    }
}