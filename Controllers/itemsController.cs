using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testRestApi.data;
using testRestApi.data.models;
using testRestApi.dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace testRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class itemsController : ControllerBase

    {
        private readonly appdbcontext dbi;
        public itemsController(appdbcontext db)
        {
            dbi = db;
        }

        // GET: api/<itemsController>
        [HttpGet]
        public async Task<IActionResult> Getitems()
        {
            var result = await dbi.items.Where(x => x.IsDeleted == false).ToListAsync();
            return Ok(result);
        }


        // GET: api/<itemsController/GetitemsWithDeleted>
        [HttpGet("/GetitemsWithDeleted")]
        public async Task<IActionResult> GetitemsWithDeleted()
        {
            var result = await dbi.items.ToListAsync();
            return Ok(result);
        }

        // GET api/<itemsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await dbi.items.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (result == null) { return NotFound($"item of id {id} not exist"); }
            else { return Ok(result); }

        }

        // POST api/<itemsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] mdlitem model)
        {

            var item = new item();

            item.price = model.price;
            item.CreatedAt = DateTime.UtcNow;
            item.notes = model.notes;
            item.categoryId = model.categoryId;
            item.name = model.name;
            item.imagepath = "";

            if (item.imagepath != null && model.image!=null)
            {
                // create container folder 
                //string folderpath = Path.Combine(_WebHostEnvironment.WebRootPath, "items images");
                string folderpath = "D:\\photos";
                if (!Directory.Exists(folderpath)) { Directory.CreateDirectory(folderpath); }
               ;

                //create new image unique name
                string imgname = Guid.NewGuid().ToString() + "_" + model.image.FileName;
                //create image path
                string imgpath = Path.Combine(folderpath, imgname);
                //store image path
                item.imagepath = imgpath;

                //coping the img

                using (var stream = new FileStream(item.imagepath, FileMode.Create))
                {
                    await model.image.CopyToAsync(stream);
                    stream.Dispose();

                }

            }


            await dbi.items.AddAsync(item);
            await dbi.SaveChangesAsync();

            return Ok(item);


        }

        // PUT api/<itemsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] mdlitem obj)
        {

            var result = await dbi.items.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null) { return NotFound($"item of id {id} not exist"); }
            else
            {

                if (obj.image != null)
                {

                   if (result.imagepath !=""  && result.imagepath != null)
                {
                    FileInfo x = new FileInfo(result.imagepath);
                    if (x.Exists) { x.Delete(); };
                        result.imagepath = "";

                }
                ;

                    if (result.imagepath != null)
                    {

                        string folderpath = "D:\\photos";
                        if (!Directory.Exists(folderpath)) { Directory.CreateDirectory(folderpath); }
                       ;

                        //create new image unique name
                        string imgname = Guid.NewGuid().ToString() + "_" + obj.image.FileName;
                        //create image path
                        string imgpath = Path.Combine(folderpath, imgname);
                        //store image path
                        result.imagepath = imgpath;

                        //coping the img

                        using (var stream = new FileStream(result.imagepath, FileMode.Create))
                        {
                            await obj.image.CopyToAsync(stream);
                            stream.Dispose();
                        }


                    }
                }

                result.notes = obj.notes;
                result.price = obj.price;
                result.UpdatedAt = DateTime.UtcNow;
                result.name = obj.name;
                result.categoryId = obj.categoryId;

                await dbi.SaveChangesAsync();
                return Ok(result);


            }
        }
        //search by category id   , extension route


        [HttpGet("/bycategory{id}")]
        public async Task<IActionResult> Getbycategory(int id)
        {
            var result = dbi.items.Where(x => x.categoryId == id).ToList();
            if (result == null) { return NotFound($"items relate to category of id {id} not exist"); }
            else { return Ok(result); }

        }


        // DELETE api/<itemsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var result = await dbi.items.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null) { return NotFound($"item of id {id} not exist"); }
            else
            {


                if (result.imagepath !=""  && result.imagepath != null)
                {
                    FileInfo x = new FileInfo(result.imagepath);
                    if (x.Exists) { x.Delete(); }
                }
                ;
               

                dbi.items.Remove(result);
                await dbi.SaveChangesAsync();
                return Ok(result);
            }
        }



        //bulk delete


        // DELETE api/<itemsController>/5
        [HttpDelete("/BulkDelete")]
        public async Task<IActionResult> BulkDelete(List<int> ids)
        {
            foreach (var id in ids)
            {

                var result = await dbi.items.FirstOrDefaultAsync(x => x.Id == id);
                if (result == null) { return NotFound($"item of id {id} not exist"); }
                else
                {


                    if (result.imagepath != "" && result.imagepath != null)
                    {
                        FileInfo x = new FileInfo(result.imagepath);
                        if (x.Exists) { x.Delete(); }
                    }
                    ;


                    dbi.items.Remove(result);

                }

            }

            await dbi.SaveChangesAsync();
            return Ok();


            
        }

            [HttpPut("/restore/{id}")]
        public async Task<IActionResult> RestoreItem(int id)
        {
            var c = await dbi.items.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) { return NotFound($"item of id :{id} not exist"); }


            else
            {
                c.IsDeleted = false;
                await dbi.SaveChangesAsync();
                return Ok(c);
            }
        }
    }
}
