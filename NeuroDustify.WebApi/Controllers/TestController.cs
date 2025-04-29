using Microsoft.AspNetCore.Mvc;

namespace NeuroDustify.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // GET: api/test
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test endpoint is working!");
        }

        // POST: api/test
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest("Value cannot be empty.");
            }

            return Ok($"Received: {value}");
        }

        // PUT: api/test/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest("Value cannot be empty.");
            }

            return Ok($"Updated item {id} with value: {value}");
        }

        // DELETE: api/test/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Deleted item {id}");
        }
    }
}

