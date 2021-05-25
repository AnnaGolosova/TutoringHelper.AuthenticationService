using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using TH.AuthService.Models;
using TH.AuthService.Models.RequestModels;

namespace TH.AuthService.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private List<Student> students = new List<Student>
        {
            new Student { Login="bob@gmail.com", Password="12345", Name = "Bob" },
            new Student { Login="alice@gmail.com", Password="55555", Name = "Alice" }
        };

        [Authorize]
        [HttpGet("/login")]
        public IActionResult GetLogin()
        {
            return Ok($"Ваш логин: {User.Identity.Name}");
        }

        [HttpPost("/token")]
        public IActionResult Token([FromBody]LoginRequestModel requestModel)
        {
            if (requestModel == null)
            { 
                throw new ArgumentNullException(nameof(requestModel));
            }

            var identity = GetIdentity(requestModel.Username, requestModel.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Created(string.Empty, response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            Student person = students.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim("Name", person.Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
