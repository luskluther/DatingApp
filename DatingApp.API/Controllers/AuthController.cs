using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            this._mapper = mapper;
            this._config = config;
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) // doesnt need to include [frombody] as apicontroller attribute in the beginning takes care
        {
            // DTO Data transer objects are used to map domain classes to a simpler classes which doesnt need all attribues of the original domain class instead
            // has only attributes that are enough to transfer over the wire and not all other attributes
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _repo.UserExists(userForRegisterDto.UserName))
            {
                // This badrequest is provided by ControllerBase that we are inheriting
                return BadRequest("Username already exists.");
            }

            var userToCreate = new User
            {
                UserName = userForRegisterDto.UserName
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            //This is the same status code as createdatroute , not sure what that is though
            return StatusCode(201);
        }

        // After user registers , we will send him back a JWT token
        // For all subsequent requests , the client will send us this token which can be used to authenticate the user while making other requests.
        // What exactly is happening here ? for the loging process ? read below : 
        // 1. First we are checking to see if we have  a user and their id pass words match in the db
        // 2. Always using lower care login
        // 3. Building up token after this which contains users id and username
        // 4. In order to make sure the token that the server sends is valid it needs to be signed by the server. Creating a security key and using this 
        // as part of the signing credentials and encrypting this key with a hash algo
        // we actually then careate the token with descipriton by passing the claim as subjects and expiry date of one day
        // 5. Then we pass the signing credentials following which we created and then create a jwt handler which allows to create a token based on passed
        // token desciption
        // 6. We will then write this to the repsonse that we send to the user/client.
        // 7. This token can be used by the users as authorization
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) // doesnt need to include [frombody] as apicontroller attribute in the beginning takes care
        {
            var userFromRepo = await _repo.Login(userForLoginDto.UserName, userForLoginDto.Password);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }
            // Here we are building the token to the user to return to the user with the users information
            // contain two bits of information about the user, the users id and users username
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()), // creating the users id
                new Claim(ClaimTypes.Name, userFromRepo.UserName) // creating users username
            };

            // This is the key we are using to sign and send to the user
            var key = new SymmetricSecurityKey(Encoding.UTF8.
                        GetBytes(_config.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // we are signing here 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);
            return Ok(
            new
            {
                token = tokenHandler.WriteToken(token),
                user // sending user as well to client when logged in
            });
        }
    }
}