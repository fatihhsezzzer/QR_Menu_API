using Mazina_Backend.Data;
using Mazina_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazina_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergenController : ControllerBase
    {
        private readonly MyDbContext _context;

        public AllergenController(MyDbContext context)
        {
            _context = context;
        }

        // CREATE: Yeni alerjen ekle
        [HttpPost]
        public async Task<IActionResult> CreateAllergen([FromForm] AllergenFormModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name_TR)|| string.IsNullOrEmpty(model.Name_EN))
            {
                return BadRequest(new { Message = "Geçersiz alerjen bilgisi." });
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

            var allergen = new Allergen
            {
                Name_TR = model.Name_TR,
                Name_EN = model.Name_EN,
                ImagePath = imagePath
            };

            _context.Allergens.Add(allergen);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllergenById), new { id = allergen.AllergenID }, allergen);
        }


        // READ: Belirli bir alerjeni getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllergenById(int id)
        {
            var allergen = await _context.Allergens.FindAsync(id);

            if (allergen == null)
            {
                return NotFound(new { Message = "Alerjen bulunamadı." });
            }

            return Ok(allergen);
        }

        // READ: Tüm alerjenleri getir
        [HttpGet]
        public async Task<IActionResult> GetAllAllergens()
        {
            var allergens = await _context.Allergens.ToListAsync();
            return Ok(allergens);
        }

        // UPDATE: Bir alerjeni güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAllergen(int id, [FromForm] AllergenFormModel updatedAllergen, IFormFile? file)
        {
          

            var existingAllergen = await _context.Allergens.FindAsync(id);
            if (existingAllergen == null)
            {
                return NotFound(new { Message = "Güncellenecek alerjen bulunamadı." });
            }

            // Yeni resim yüklendiyse güncelle
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploads, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Yeni resim yolunu ayarla
                existingAllergen.ImagePath = $"/uploads/{uniqueFileName}";
            }

            // Diğer alanları güncelle
            existingAllergen.Name_TR = updatedAllergen.Name_TR;
            existingAllergen.Name_EN = updatedAllergen.Name_EN;

            // Değişiklikleri kaydet
            _context.Entry(existingAllergen).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: Bir alerjeni sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllergen(int id)
        {
            var allergen = await _context.Allergens.FindAsync(id);
            if (allergen == null)
            {
                return NotFound(new { Message = "Silinecek alerjen bulunamadı." });
            }

            _context.Allergens.Remove(allergen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AllergenFormModel
        {
            public string Name_TR { get; set; }
            public string Name_EN { get; set; }

            public IFormFile? File { get; set; }
        }

    }
}
