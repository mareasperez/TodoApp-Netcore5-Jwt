using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Configuration;
using TodoApp.Configuration.DTOs.Responses;
using TodoApp.Models.DTOs.Requests;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagerController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagerController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if (ModelState.IsValid)
            {
                var existUser = await _userManager.FindByEmailAsync(user.Email);
                if (existUser != null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>(){
                    "Email is already registered"
                },
                        Success = false
                    });
                }
                var newUser = new IdentityUser() { Email = user.Email, UserName = user.Username };
                var is_create = await _userManager.CreateAsync(newUser, user.Password);
                if (is_create.Succeeded)
                {
                   var jwtToken =GenerateJwtToken(newUser);
                   return Ok( new RegistrationResponse(){
                       Success = true,
                       Token = jwtToken
                   });
                }
                else
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = is_create.Errors.Select(x=> x.Description).ToList(),
                        Success = false
                    });
                }
            }
            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>(){
                    "Invalid payload"
                },
                Success = false
            });
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> login ([FromBody] UserRegistrationDto user)
        {
            if (ModelState.IsValid){
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null){
                    return BadRequest(new RegistrationResponse(){
                        Errors = new List<string>(){
                            "Invalid login request"
                        },
                        Success = false
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);
                if (!isCorrect){
                    return BadRequest(new RegistrationResponse(){
                        Errors = new List<string>(){
                            "Invalid login request"
                        },
                        Success = false
                    });
                }
                var jwtToken = GenerateJwtToken(existingUser);
                return Ok(new RegistrationResponse(){
                    Success = true,
                    Token = jwtToken
                });
            }// aqui quede :V en el login
            else {
                return BadRequest(new RegistrationResponse(){
                Errors = new List<string>(){
                    "Invalid payload"
                },
                Success = false
            });
            }
        }
        private string GenerateJwtToken(IdentityUser user)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject  = new ClaimsIdentity(new []{
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = JwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}