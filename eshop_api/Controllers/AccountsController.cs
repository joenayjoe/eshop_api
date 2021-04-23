using AutoMapper;
using eshop_api.Auth;
using eshop_api.DTO;
using eshop_api.Exceptions;
using eshop_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eshop_api.Configurations
{
    [Route("api/account")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<AccountsController> logger;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public AccountsController(
            UserManager<AppUser> userManager,
            ILogger<AccountsController> logger,
            IMapper mapper,
            IAuthManager authManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDTO)
        {
            if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(userRegisterDTO.ConfirmPassword), "Password Confirmation does not match");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }


            var userExist = await userManager.FindByEmailAsync(userRegisterDTO.Email);
            if (userExist != null)
            {
                throw new EntityAlreadyExistExcption($"Email address is already taken.");
            }

            var user = mapper.Map<AppUser>(userRegisterDTO);
            user.UserName = userRegisterDTO.Email;

            var result = await userManager.CreateAsync(user, userRegisterDTO.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(ModelState);
            }

            await userManager.AddToRoleAsync(user, UserRoles.Customer);

            return Ok();

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await authManager.ValidateUser(userLoginDTO);

            if (user == null)
            {
                return Unauthorized(new { Staus = 401, Message = "Invalid user credentials" });
            }

            var token = await authManager.CreateToken();

            var authResponse = new UserLoginResponseDTO
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccessToken = token
            };

            return Ok(authResponse);

        }
    }
}
