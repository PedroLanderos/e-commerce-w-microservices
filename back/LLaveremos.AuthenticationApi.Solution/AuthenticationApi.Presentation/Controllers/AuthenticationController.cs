using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using Llaveremos.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<Response>> Register(AppUserDTO appUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userInterface.Register(appUserDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userInterface.Login(loginDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            if (id<=0)
                return BadRequest("invalid id");
            var user = await userInterface.GetUser(id);

            return user.Id > 0 ? Ok(user) : NotFound();
        }
    }
}
