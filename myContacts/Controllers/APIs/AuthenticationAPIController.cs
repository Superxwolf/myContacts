using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using myContacts.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace myContacts.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationAPIController : ControllerBase
    {
        private IConfiguration _config;

        public AuthenticationAPIController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Creates a user session and returns a JWT Token
        /// </summary>
        /// <param name="userInfo">Supply the user credentials</param>
        /// <returns>JWT Token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseModel), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login(UserModel userInfo)
        {
            var user = AuthenticateUser(userInfo.username);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = GenerateJSONWebToken(user);

            return Ok(new LoginResponseModel(token));
        }

        /// <summary>
        /// End the current user session
        /// </summary>
        [HttpGet("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Logout()
        {
            return Ok("Logged out");
        }

        private UserModel AuthenticateUser(string username)
        {
            return new UserModel
            {
                username = username
            };
        }

        private string GenerateJSONWebToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.username) };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}