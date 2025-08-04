using InnoStay_DAL;
using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace InnoStay_WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IInnoStayRepository repo { get; set; }
        private readonly IConfiguration config;
        
        public UserController(IInnoStayRepository repository, IConfiguration configuration)
        {
            repo = repository;
            config = configuration;
        }

        [AllowAnonymous]
        [HttpPost("ValidateUserCredentials")]
        public IActionResult ValidateUserCredentials([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var role = repo.ValidateCredentials(loginRequest.Email, loginRequest.Password);
                if (string.IsNullOrEmpty(role))
                    return Unauthorized(new { error = "Invalid credentials" });

                var user = repo.GetUserByEmail(loginRequest.Email);
                if (user == null)
                    return Unauthorized(new { error = "Invalid credentials" });

                var jwtCfg = config.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name,             user.FirstName),
                        new Claim(ClaimTypes.Role,             role)
                    };

                
                var tokenObj = new JwtSecurityToken(
                    issuer: jwtCfg["Issuer"],
                    audience: jwtCfg["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtCfg["ExpiresInMinutes"])),
                    signingCredentials: creds
                );
                var jwt = new JwtSecurityTokenHandler().WriteToken(tokenObj);

                return Ok(new
                {
                    token = jwt,
                    role,
                    name = user.FirstName,
                    userID = user.UserId
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Server error" });
            }
        }

        [Authorize(Roles ="User,Admin")]
        [HttpGet]
        [Route("GetUserDetailsById")]
        public IActionResult GetUserDetailsById(int userId)
        {
            InnoStay_DAL.Models.User result = new InnoStay_DAL.Models.User();
            try
            {
                if (userId >= 0)
                {
                    result = repo.GetUserById(userId);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest("User Not Found");
                    }
                }
                else
                {
                    return NotFound("User Id Should be greater than 0");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("AddCustomer")]
        public IActionResult AddCustomer([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.Role))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                bool ok = repo.AddUser(user);
                if (ok) return Ok("Customer added");
                return StatusCode(500, "Could not add customer");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles ="User,Admin")]
        [HttpPut]
        public IActionResult UpdateUserDetails(InnoStay_DAL.Models.User user)
        {
            bool result = false;
            try
            {
                result = repo.UpdateUser(user);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] SignUpDTO signup)
        {

            if (string.IsNullOrWhiteSpace(signup.FirstName) ||
                string.IsNullOrWhiteSpace(signup.LastName) ||
                string.IsNullOrWhiteSpace(signup.Email) ||
                string.IsNullOrWhiteSpace(signup.Password))
            {
                return BadRequest("All fields are required.");
            }

            try
            {

                var user = new User
                {
                    FirstName = signup.FirstName,
                    LastName = signup.LastName,
                    Email = signup.Email,
                    Password = signup.Password,  
                    Role = "User"
                };

                bool created = repo.AddUser(user);
                if (!created)
                    return StatusCode(500, "Registration failed. Please try again.");

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete]
        public IActionResult DeleteUser(int userId)
        {
            bool result = false;
            try
            {
                result = repo.DeleteUser(userId);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Ok(result);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            List<InnoStay_DAL.Models.User> result = new List<InnoStay_DAL.Models.User>();
            try
            {
                result = repo.GetUsers();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return Ok(result);
        }

    }
}
