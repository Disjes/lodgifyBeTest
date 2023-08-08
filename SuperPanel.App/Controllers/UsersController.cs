using System;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Exceptions;
using Services.Services;
using SuperPanel.App.Models;

namespace SuperPanel.App.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IContactsService _contactsService;
        private readonly Misc _miscConfig;

        public UsersController(ILogger<UsersController> logger, 
            IUserRepository userRepository, 
            IContactsService contactsService,
            IOptions<Misc> miscConfig)
        {
            _logger = logger;
            _userRepository = userRepository;
            _contactsService = contactsService;
            _miscConfig = miscConfig.Value;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = _miscConfig.PageSize;
            int pageNumber = (page ?? 1);
            var users = await _userRepository.QueryAll(pageNumber, pageSize);
            return View(users);
        }
        
        [HttpDelete("Users/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _contactsService.DeleteContact(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message};{id}");
                return Conflict("There was an error trying to delete the user");
            }
        }
    }
}
