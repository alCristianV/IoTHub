using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IoTHubAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), 500)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(/*[FromBody]*/ UserForRegisterDto userForRegisterDto)
        {

            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Email))
            {
                return BadRequest("Email already exists");
            }

            var userToCreate = new User()
            {
                Email = userForRegisterDto.Email
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Email.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized("Account does not exist");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Email)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config
                .GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });

        }

        [HttpPost("device")]
        public async Task<IActionResult> DeviceAuth(string connectionString)
        {
            var device = await this._repo.GetDeviceAuthorize(connectionString);

            if (device == null)
            {
                return Unauthorized("Device does not exist");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,device.Id.ToString()),
                new Claim(ClaimTypes.Name, device.Name)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config
                .GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });

        }


    }
}