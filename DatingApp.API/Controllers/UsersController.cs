using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))] // so with this filter , every call to any method here , will update its last active
    public class UsersController : ControllerBase
    {
        private readonly DataContext _dc;
        private readonly IDatingRepo _datingRepo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepo datingRepo, IMapper mapper)
        {
            this._mapper = mapper;
            this._datingRepo = datingRepo;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepo.GetUsers();
            var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users); // mapper
            return Ok(userToReturn);
        }


        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user); // converting /mapping user to userfordetaileddto
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            // this is how we are checking if the user is the current user that has the token to the service
                // that is attampting to access this controller route
                // if token doesn match the user is not authorized.
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }
            // getting the user that we want to update
            var userFromRepo = await _datingRepo.GetUser(id);
            // mapping userforupdate to userfrom repo
            _mapper.Map(userForUpdateDto,userFromRepo); // this will check the profile and map userdto that we are getting and user from repo

            if (await _datingRepo.SaveAll()) { 
                return NoContent(); // save successful so return nothing thats the appropriate return
            }

            throw new Exception($"Updating user {id} failed on save");
        }
    }
}