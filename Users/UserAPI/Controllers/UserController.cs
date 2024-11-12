using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.DTO;
using UserAPI.Constants;
using UserAPI.Exceptions;
using AutoMapper;

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

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserGetDTO>> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var existingUser = await _userRepository.GetById(id);
            if (existingUser == null)
            {
                return NotFound(ErrorMessages.UserNotFoundExceptionDetails);
            }

            bool isUpdated = false ;
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
