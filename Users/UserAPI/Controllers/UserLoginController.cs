using Microsoft.AspNetCore.Mvc;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.Business.Services.Interfaces;
using UserAPI.Constants;
using UserAPI.Exceptions;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public UserLoginController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        /// <summary>
        /// Logs in a user and generates a JWT token
        /// </summary>
        /// <param name="loginRequest">The user's login credentials (username and password)</param>
        /// <returns>JWT token as a string</returns>
        /// <response code="200">Returns a JWT token if the login is successful</response>
        /// <response code="400">If the login credentials are invalid</response>
        /// <exception cref="ErrorMessages.InvalidCredentialsExceptionDetails">Thrown if the username or password is incorrect</exception>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /UserLogin
        ///     {
        ///         "username": "johndoe",
        ///         "password": "SecurePassword123"
        ///     }
        ///
        /// </remarks>
        [ProducesDefaultResponseType(typeof(string))]
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetByUsername(loginRequest.Username);

            if (user == null)
            {
                throw new CustomException(ErrorMessages.InvalidCredentialsExceptionDetails);
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(loginRequest.Password, user.Password))
            {
                throw new CustomException(ErrorMessages.InvalidCredentialsExceptionDetails);
            }

            var token = _authService.Login(loginRequest);
            return Ok(token);
        }
    }
}