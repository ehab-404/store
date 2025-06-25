using Microsoft.AspNetCore.Mvc;
using testRestApi.data;
using Microsoft.EntityFrameworkCore;
using testRestApi.data.models;
using testRestApi.dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace testRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoriesController : ControllerBase
    {
        private readonly appdbcontext dbi;
        public CategoriesController(appdbcontext db)
        {
            dbi = db;

        }
        // GET: api/<CategoriesController>
        [HttpGet]

        public async Task<IActionResult> GetCategories()
        {
            var result = await dbi.categories.Where(x=>x.IsDeleted==false).ToListAsync();
            return Ok(result);
        }
        // GET: api/<CategoriesController/GetCategoriesWithDeleted>


        [HttpGet("/GetCategoriesWithDeleted")]

        public async Task<IActionResult> GetCategoriesWithDeleted()
        {
            var result = await dbi.categories.ToListAsync();
            
            return Ok(result);
        }

        // GET api/<CategoriesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await dbi.categories.FirstOrDefaultAsync(x => x.Id == id&& x.IsDeleted == false);
            return Ok(result);
        }

        // POST api/<CategoriesController>
        [HttpPost]
        public async Task<IActionResult> AddCategory(categorymdl obj)
        {
            Category x = new Category();

            x.name=obj.name;
            x.notes=obj.notes;
            x.CreatedAt = DateTime.UtcNow;
            dbi.categories.Add(x);
            await dbi.SaveChangesAsync();
            return Ok(x);

        }
        // PUT api/<CategoriesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(categorymdl obj,int id)
        {
            var c = await dbi.categories.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null){ return NotFound($"category of id :{id} not exist"); }
            

            else
            {
                c.name = obj.name;
                c.notes = obj.notes;
                c.UpdatedAt = DateTime.UtcNow;
                await dbi.SaveChangesAsync();
                return Ok(c);
            }
        }

        [HttpPut("/restore{id}")]
        public async Task<IActionResult> RestoreCategory( int id)
        {
            var c = await dbi.categories.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) { return NotFound($"category of id :{id} not exist"); }


            else
            {
                c.IsDeleted = false;
                await dbi.SaveChangesAsync();
                return Ok(c);
            }
        }



        // DELETE api/<CategoriesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var c = await dbi.categories.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) { return NotFound($"category of id :{id} not exist"); }
            else
            {
                dbi.categories.Remove(c);
              await  dbi.SaveChangesAsync();
                return Ok(c);
            }
        }




        // DELETE bulk
        [HttpDelete("/bulk delete")]
        public async Task<IActionResult> BulkDeleteCategory(List<int> ids)
        {

            foreach (var id in ids)
            {
                var c = await dbi.categories.FirstOrDefaultAsync(x => x.Id == id);
                if (c == null) { return NotFound($"category of id :{id} not exist"); }
                else
                {
                    dbi.categories.Remove(c);

                }

            }
            await dbi.SaveChangesAsync();
            return Ok();


        }


    } }
