using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.Controllers.Categories;
using TimeHacker.Domain.Contracts.IServices.Users;

namespace TimeHacker.Application.Controllers.Users
{
    [Authorize]
    [ApiController]
    [Route("/api/Users")]
    public class UsersController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


    }
}
