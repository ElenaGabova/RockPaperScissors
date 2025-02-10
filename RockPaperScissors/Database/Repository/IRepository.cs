namespace Database.Repository
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Получение всех элементов таблицы.
        /// </summary>
        /// <returns>Элементы таблицы.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получение элемента таблицы по ИД элемента.
        /// </summary>
        /// <returns>Элемент таблицы.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Создание нового элемента в таблице.
        /// </summary>
        /// <param name="item">Элемент для вставки.</param>
        /// <returns>Вставленный элемент.</returns>
        Task<T> CreateAsync(T item);

        /// <summary>
        /// Обновление элемента в таблице.
        /// </summary>
        /// <param name="id">Ид элемента для обновления</param>
        /// <param name="item">Элемент для обновления.</param>
        /// <returns>Обновленный элемент.</returns>
        Task<T> UpdateAsync(int id, T item);

        /// <summary>
        /// Удаление элемента из таблицы.
        /// </summary>
        /// <param name="id">Ид элемента для удаления.</param>
        /// <returns>Признак, удален ли элемент.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
