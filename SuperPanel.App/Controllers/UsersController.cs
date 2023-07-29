using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperPanel.App.Data;
using X.PagedList;

namespace SuperPanel.App.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var users = _userRepository.QueryAll(pageNumber, pageSize);
            return View(users);
        }

        public IActionResult Delete(long id)
        {
            return NoContent();
        }
    }
}
