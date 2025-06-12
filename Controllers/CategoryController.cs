using Mazina_Backend.Data;
using Mazina_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mazina_Backend.Models.DTO;

namespace Mazina_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CategoryController(MyDbContext context)
        {
            _context = context;
        }

        // Kategori ekleme (resim yükleme destekli)
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] CategoryFormModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "Kategori bilgisi eksik." });
            }

            string imagePath = null;

            // Resim dosyasını kaydetme
            if (model.File != null && model.File.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var uniqueFileName = $"{model.File.FileName}";
                var filePath = Path.Combine(uploads, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
                imagePath = $"/uploads/{uniqueFileName}";
            }

            // Yeni kategori oluşturma
            var category = new Category
            {
                Name_TR = model.Name_TR,
                Name_EN = model.Name_EN,
                SortOrder=model.SortOrder,
                Is_New = model.Is_New,
                Is_Active = model.Is_Active,
                ImagePath = imagePath
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryID }, category);
        }

        // Belirli bir kategoriyi getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = "Kategori bulunamadı." });
            }

            return Ok(category);
        }

        // Tüm kategorileri listeleme
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c=>c.SortOrder)
                .ToListAsync();
            return Ok(categories);
        }

        // Kategori güncelleme (resim yükleme destekli)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryFormModel model)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = "Kategori bulunamadı." });
            }

            // Resim dosyasını güncelleme
            if (model.File != null && model.File.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var uniqueFileName = $"{model.File.FileName}";
                var filePath = Path.Combine(uploads, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
                category.ImagePath = $"/uploads/{uniqueFileName}";
            }

            // Kategori bilgilerini güncelleme
            category.Name_TR = model.Name_TR;
            category.Name_EN = model.Name_EN;
            category.SortOrder = model.SortOrder;
            category.Is_Active= model.Is_Active;
            category.Is_New = model.Is_New;
            category.Is_Drink = model.Is_Drink;


            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("UpdateCategoryOrder")]
        public async Task<IActionResult> UpdateCategoryOrder([FromBody] List<Category> updatedCategories)
        {
            if (updatedCategories == null || updatedCategories.Count == 0)
            {
                return BadRequest(new { Message = "Güncellenecek kategoriler eksik." });
            }

            foreach (var updatedCategory in updatedCategories)
            {
                var category = await _context.Categories.FindAsync(updatedCategory.CategoryID);
                if (category != null)
                {
                    category.SortOrder = updatedCategory.SortOrder;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Kategori sıralamaları güncellendi." });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Kategoriyi bul
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = "Kategori bulunamadı." });
            }

            // Bu kategoriye bağlı ürünlerin varlığını kontrol et
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryID == id);
            if (hasProducts)
            {
                return BadRequest(new { Message = "Bu kategoriye ait ürünler var, silinemez." });
            }

            // Kategoriye bağlı ürün yoksa sil
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }



        public class CategoryFormModel
        {
            public string Name_TR { get; set; }
            public string Name_EN { get; set; }
            public float SortOrder { get; set; }
            public bool Is_New { get; set; }
            public bool Is_Active { get; set; }
            public bool Is_Drink { get; set; }


            public IFormFile? File { get; set; } // Resim dosyası
        }
    }

}

