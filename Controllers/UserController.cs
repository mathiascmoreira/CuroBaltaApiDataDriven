using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController: Controller
    {
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        [AllowAnonymous]
         public async Task<ActionResult<User>> Post(
             [FromServices] DataContext context,
             [FromBody]User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";
                return Ok(model);
            }
            catch(Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            var user = await context.Users
                 .AsNoTracking()
                 .Where(x => x.Username == model.Username && x.Password == model.Password)
                 .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);

            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }


        [HttpPost]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<dynamic>> Put(
           [FromServices] DataContext context,
           [FromRoute] int id,
           [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário" });
            }
        }

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("anonimo")]
        //public string Anonimo() => "Anonimo";
        //[HttpGet]
        //[Authorize]
        //[Route("autenticado")]
        //public string Autenticado() => "Autenticado";
        //[HttpGet]
        //[Route("funcionario")]
        //[Authorize(Roles = "employee")]
        //public string Funcionario() => "Funcionario";
        //[HttpGet]
        //[Route("gerente")]
        //[Authorize(Roles = "manager")]
        //public string Gerente() => "Gerente";
    }
}
