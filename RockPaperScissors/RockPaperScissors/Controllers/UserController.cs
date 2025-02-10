using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Mvc;

namespace RockPaperScissors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IRepository<User> _userService;
        public UserController(IRepository<User> userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Получение всех пользователей.
        /// </summary>
        /// <returns>Список пользователей</returns>
        /// 
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> Index()
        {
            var items = await _userService.GetAllAsync();
            return items;
        }

        
        /// <summary>
        /// Получение пользователя по ИД.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Пользователь.</returns>
        /// 
        [HttpGet("{userId}", Name = nameof(GetById))]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int userId)
        {
            var item = await _userService.GetByIdAsync(userId);
            if (item == null)
                return NotFound();
            return Ok(item);
        }
        

        /// <summary>
        /// Создание нового пользователя.
        /// </summary>
        /// <param name="Name">Имя пользователя.</param>
        /// <param name="Description">Баланс пользователя.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(string userName, decimal balance)
        {
            var user = new User() { Username = userName, Balance = balance };

            if (!ModelState.IsValid)
                return BadRequest(user);

            await _userService.CreateAsync(user);
            return Ok(new { Message = "Added" });
        }

        /// <summary>
        /// Обновление пользователя.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int userId, string userName, decimal balance)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return BadRequest();

            user.Username = userName;
            user.Balance = balance;

            if (!ModelState.IsValid)
                return BadRequest(user);

            await _userService.UpdateAsync(userId, user);
            return Ok(new { Message = "Changed" });
        }

        /// <summary>
        /// Удаление пользователя по ИД.
        /// </summary>
        /// <param name="itemId">Ид пользователя.</param>
        /// <returns>Признак, удален ли пользователь.</returns>
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int userId)
        {
            var existingItem = await _userService.GetByIdAsync(userId);
            if (existingItem == null)
                return NotFound();

            bool? deleted = await _userService.DeleteAsync(userId);
            return Ok(new { Message = "Deleted" });
        }
    }
}
