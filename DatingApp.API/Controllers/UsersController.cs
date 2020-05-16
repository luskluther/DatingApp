using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user); // converting /mapping user to userfordetaileddto
            return Ok(userToReturn);
        }

    }
}