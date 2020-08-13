using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Genealogy.Service.Astract;
using System.Linq;
using Genealogy.Models;
using Genealogy.Service.Helpers;

namespace Sirius.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/user")]

    public class UserController : Controller
    {
        private IGenealogyService _genealogyService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserController(IMapper mapper, IGenealogyService genealogyService, IConfiguration configuration)
        {
            _genealogyService = genealogyService;
            _mapper = mapper;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] UserDto userDto)
        {
            var user = _genealogyService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                //return Unauthorized();
                return StatusCode(401, "Неверный логин или пароль.");

            var role = _genealogyService.GetRoleById(user.RoleId.Value);

            if (user.IsConfirmed == false)
                return StatusCode(401, "Пользователь не подтверждён.");

            var secretKey = _configuration.GetSection("AppSettings").GetChildren().AsEnumerable().Where(i => i.Key == "Secret").FirstOrDefault().Value;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString() as string),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role?.Name as string)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                user.Id,
                user.Username,
                user.FirstName,
                user.LastName,
                user.RoleId,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
                _genealogyService.CreateUser(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("amount")]
        public IActionResult GetUserAmount()
        {
            return Ok(_genealogyService.GetUserAmount());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _genealogyService.GetAllUsers();
            var userDtos = _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = _genealogyService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            try
            {
                // save 
                _genealogyService.Update(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(Guid id)
        {
            _genealogyService.Delete(id);
            return Ok();
        }

        [HttpPut("status/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult ChangeStatus(Guid id, [FromQuery] bool isConfirmed)
        {
            if (_genealogyService.ChangeUserStatus(id, isConfirmed))
            {
                return StatusCode(200, "Статус пользователя изменён.");
            }
            return StatusCode(400, "Пользователь не найден.");
        }

        [HttpGet("checkadmin/{id}")]
        public IActionResult CheckAdmin(Guid id)
        {
            try
            {
                bool isAdmin = _genealogyService.CheckAdminByUserId(id);
                return StatusCode(200, isAdmin);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}