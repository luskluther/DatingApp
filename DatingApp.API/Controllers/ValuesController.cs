using DatingApp.API.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.API.Controllers
{
    // Routes for this controller , [controller] is just the placeholder for the controller name
    [Route("api/[controller]")]
    [ApiController] // This attribute is very useful this sends good respones and can be used for validations as well
    [Authorize] // This is telling that all methods inside the values controller to be authorized
    public class ValuesController : ControllerBase // the difference between this asd :Controller is the one we are using does not provide view related help as we are using angular for that
    {
        private readonly DataContext _dc;
        public ValuesController(DataContext dc)
        {
            this._dc = dc;
        }

        // GET api/values
        // IEnumerable is generally a colleciton of things. If you give like this its just collection of strings IEnumerable<string> 
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await _dc.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _dc.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
