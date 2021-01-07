using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shop.Models;
using Shop.Data;

namespace Shop.Controllers
{ 
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader ="User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = 0)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context)
        {
            var categories = await context.Categories
            .AsNoTracking()
            .ToListAsync();

            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context)
        {
            var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            int id,
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            if (model.Id != id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Categories.Remove(category);

                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}


// dotnet add package Microsoft.EntityFrameworkCore.SqlServer
// dotnet add package Microsoft.EntityFrameworkCore.InMemory
// dotnet add package Microsoft.EntityFrameworkCore.Design
// dotnet add package Microsoft.AspNetCore.Authentication
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
// dotnet add package Swachbuckle.AspNetCore -v 5.0.0-rc4
// dotnet tool install --global dotnet-ef 
// dotnet watch run
// dotnet ef migrations add InitialCreate