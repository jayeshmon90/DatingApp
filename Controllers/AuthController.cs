using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController: Controller
    {
        
        public IAuthRepository _repo { get; }
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userDto)
        {

           if(await _repo.UserExists(userDto.Username))
            ModelState.AddModelError("username","username is already taken");
            // return BadRequest("username is already taken");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            userDto.Username=userDto.Username.ToLower();

            var userToCreate= new User
            {
                Username=userDto.Username
            };

            var createUser=await _repo.Register(userToCreate, userDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {
          var userFromRepo= await _repo.Login(userForLoginDto.Username.ToLower(),userForLoginDto.Password);

          if(userFromRepo==null)
          return Unauthorized();

          var tokenHandler= new JwtSecurityTokenHandler();
          var key=Encoding.ASCII.GetBytes("super secret key");
           var tokenDescripor = new SecurityTokenDescriptor()
           {
               Subject=new ClaimsIdentity(new Claim[]
               {
                   new Claim(ClaimTypes.NameIdentifier,  userFromRepo.id.ToString()),
                   new Claim(ClaimTypes.Name,userFromRepo.Username)
               }),
               Expires= DateTime.Now.AddDays(1),
               SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512Signature)
           };
           var token= tokenHandler.CreateToken(tokenDescripor);
           var tokenString=tokenHandler.WriteToken(token);
        return Ok(new {tokenString});
        }

    }
}