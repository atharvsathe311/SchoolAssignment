using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.DTO;
using AutoMapper;
using CommonLibrary.Constants;
using CommonLibrary.Exceptions;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a list of users.
        /// </summary>
        /// <remarks>
        /// Returns a list of all users.
        /// Accessible by Admin and Teacher roles.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(IEnumerable<UserGetDTO>), 200)] 
        [ProducesResponseType(401)]  
        [ProducesResponseType(403)]  
        [ProducesResponseType(500)] 
        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUsers()
        {
            var users = await _userRepository.GetAll();

            if (users == null)
            {
                return NotFound(ErrorMessages.UserNotFoundExceptionDetails);
            }

            var userDto = _mapper.Map<IEnumerable<UserGetDTO>>(users);
            return Ok(userDto);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <remarks>
        /// Returns a single user based on the provided user ID.
        /// Accessible by Admin and Teacher roles.
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(UserGetDTO), 200)]  
        [ProducesResponseType(401)]  
        [ProducesResponseType(403)]  
        [ProducesResponseType(404)]  
        [ProducesResponseType(500)]  
        public async Task<ActionResult<UserGetDTO>> GetUser(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound(ErrorMessages.UserNotFoundExceptionDetails);
            }

            var userDto = _mapper.Map<UserGetDTO>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <remarks>
        /// Adds a new user to the system.
        /// Accessible only by Admin role.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserGetDTO), 200)]  
        [ProducesResponseType(400)]  
        [ProducesResponseType(401)]  
        [ProducesResponseType(403)]  
        [ProducesResponseType(500)]  
        public async Task<ActionResult<UserGetDTO>> AddUser(UserPostDTO userPostDTO)
        {
            var user = _mapper.Map<User>(userPostDTO);

            var alreadyRegisteredUser = await _userRepository.GetByEmail(user.Email);

            if (alreadyRegisteredUser != null)
            {
                throw new CustomException(ErrorMessages.UserExistsExceptionDetails);
            }

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            var result = await _userRepository.Add(user);
            var userDto = _mapper.Map<UserGetDTO>(result);
            return Ok(userDto);
        }

        /// <summary>
        /// Updates an existing user by ID.
        /// </summary>
        /// <remarks>
        /// Updates a user's information such as username, email, and password.
        /// Accessible only by Admin role.
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserGetDTO), 200)]  
        [ProducesResponseType(400)]  
        [ProducesResponseType(401)]  
        [ProducesResponseType(403)]  
        [ProducesResponseType(404)]  
        [ProducesResponseType(500)]  
        public async Task<ActionResult<UserGetDTO>> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var existingUser = await _userRepository.GetById(id);
            if (existingUser == null)
            {
                return NotFound(ErrorMessages.UserNotFoundExceptionDetails);
            }

            bool isUpdated = false;
            if (!string.IsNullOrEmpty(userUpdateDTO.Username))
            {
                existingUser.Username = userUpdateDTO.Username;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Email))
            {
                existingUser.Email = userUpdateDTO.Email;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userUpdateDTO.Password);
                isUpdated = true;
            }

            if (isUpdated)
            {
                existingUser.UpdatedAt = DateTime.Now;
                var updatedUser = await _userRepository.Update(existingUser);
                var updatedUserDTO = _mapper.Map<UserGetDTO>(updatedUser);
                return Ok(updatedUserDTO);
            }
            return BadRequest(ErrorMessages.NothingToUpdate);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <remarks>
        /// Deletes the user based on the provided user ID.
        /// Accessible only by Admin role.
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]  
        [ProducesResponseType(401)]  
        [ProducesResponseType(403)]  
        [ProducesResponseType(404)]  
        [ProducesResponseType(500)]  
        public async Task<ActionResult<UserGetDTO>> DeleteUser(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound(ErrorMessages.UserNotFoundExceptionDetails);
            }
            var result = await _userRepository.Delete(id);
            return Ok(result);
        }
    }
}
