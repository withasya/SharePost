using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharePost.Data;
using SharePost.Dtos;
using SharePost.Models;

namespace SharePost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(ApplicationDbContext context, IWebHostEnvironment environment) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PostCreateDto dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)    //görselin varlığının kontrolü için
                return BadRequest("Görsel yüklenmedi.");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}"; //her görselin benzersin adı olması için
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");        //görseli oluşturduğumuz klasöre kaydettik, görselin yolu

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);      //eğer oluşturmamıssak kendi oluşturuyor uploads klasörünü


            var filePath = Path.Combine(uploadPath, fileName);  //üstteki verileri birleştirip tam yol oluşturdu

            using (var stream = new FileStream(filePath, FileMode.Create)) //dosya işlemleri okuma yazma
            {
                await dto.Image.CopyToAsync(stream);  // görseli belirledğimiz yola kaydeder
            }


            //görsel tamam postu oluşturacağız 

            var post = new Post
            {
                Description = dto.Description,
                ImagePath = $"/uploads/{fileName}"
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();  // Asenkron kaydetme

            return Ok(new { post.Id, post.Description, post.ImagePath });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound(new { message = "Post bulunamadı." });

            // Görselin tam URL'sini oluşturuyoruz
            var imageUrl = $"{Request.Scheme}://{Request.Host}{post.ImagePath}";

            // Geri döneceğimiz veri
            var response = new
            {
                post.Id,
                post.Description,
                ImageUrl = imageUrl
            };

            return Ok(response);
        }


    }
}
