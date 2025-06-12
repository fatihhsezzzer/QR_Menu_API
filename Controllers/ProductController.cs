using Mazina_Backend.Data;
using Mazina_Backend.Models;
using Mazina_Backend.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mazina_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductController(MyDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // ÜRÜN EKLEME
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductFormModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "Geçersiz ürün bilgisi." });
            }

            int nextProductId = await _context.Products
                .AsNoTracking()
                .Select(p => p.ProductId)
                .ToListAsync()
                .ContinueWith(task => task.Result.DefaultIfEmpty(500).Max() + 1);

            var product = new Product
            {
                ProductId = nextProductId,
                Name_TR = model.Name_TR,
                Name_EN = model.Name_EN,
                Description_TR = model.Description_TR,
                Description_EN = model.Description_EN,
                Price = (float)model.Price,
                Is_Active = model.Is_Active,
                CategoryID = model.CategoryID,
                SortOrder = model.SortOrder,
                ProductAllergens = new List<ProductAllergen>()
            };

            if (model.ProductAllergens != null && model.ProductAllergens.Any())
            {
                foreach (var allergen in model.ProductAllergens)
                {
                    var existingAllergen = await _context.Allergens.FindAsync(allergen.AllergenId);
                    if (existingAllergen != null)
                    {
                        product.ProductAllergens.Add(new ProductAllergen
                        {
                            AllergenId = allergen.AllergenId,
                            Allergen = existingAllergen
                        });
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            ClearProductCache();

            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        // TEK ÜRÜN GETİRME
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductAllergens)
                .ThenInclude(pa => pa.Allergen)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı." });
            }

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var cacheKey = "all_products";

            if (!_cache.TryGetValue(cacheKey, out List<ProductDto> products))
            {
                products = await _context.Products
                    .Include(p => p.ProductAllergens)
                    .ThenInclude(pa => pa.Allergen)
                    .OrderBy(p => p.SortOrder)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name_TR = p.Name_TR,
                        Name_EN = p.Name_EN,
                        Description_TR = p.Description_TR,
                        Description_EN = p.Description_EN,
                        Price = p.Price,
                        Is_Active = p.Is_Active,
                        ImagePath = p.ImagePath,
                        CategoryID = p.CategoryID,
                        HalfPortionOf = p.HalfPortionOf,
                        WhatOf = p.WhatOf,
                        WhatIs_TR = p.WhatIs_TR,
                        WhatIs_EN=p.WhatIs_EN,
                        SortOrder = p.SortOrder,
                        ProductAllergens = p.ProductAllergens.Select(pa => new AllergenDto
                        {
                            AllergenId = pa.AllergenId,
                            Name_TR = pa.Allergen.Name_TR,
                            ImagePath = pa.Allergen.ImagePath
                        }).ToList()
                    }).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.Normal);


                _cache.Set(cacheKey, products, cacheOptions);
            }

            return Ok(products);
        }
        // SADECE AKTİF ÜRÜNLERİ GETİR (CACHE İLE)
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts()
        {
            var cacheKey = "active_products";

            if (!_cache.TryGetValue(cacheKey, out List<ProductDto> activeProducts))
            {
                activeProducts = await _context.Products
                    .Include(p => p.ProductAllergens)
                    .ThenInclude(pa => pa.Allergen)
                    .Where(p => p.Is_Active)
                    .OrderBy(p => p.SortOrder)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name_TR = p.Name_TR,
                        Name_EN = p.Name_EN,
                        Description_TR = p.Description_TR,
                        Description_EN = p.Description_EN,
                        Price = p.Price,
                        Is_Active = p.Is_Active,
                        ImagePath = p.ImagePath,
                        CategoryID = p.CategoryID,
                        HalfPortionOf=p.HalfPortionOf,
                        WhatOf = p.WhatOf,
                        WhatIs_TR = p.WhatIs_TR,
                        WhatIs_EN = p.WhatIs_EN,
                        SortOrder = p.SortOrder,
                        ProductAllergens = p.ProductAllergens.Select(pa => new AllergenDto
                        {
                            AllergenId = pa.AllergenId,
                            Name_TR = pa.Allergen.Name_TR,
                            ImagePath = pa.Allergen.ImagePath
                        }).ToList()
                    })
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(cacheKey, activeProducts, cacheOptions);
            }

            return Ok(activeProducts);
        }


        // RESİM YÜKLEME
        [HttpPut("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Geçerli bir resim yüklenmedi." });
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı." });
            }

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var uniqueFileName = $"{file.FileName}";
            var filePath = Path.Combine(uploads, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            product.ImagePath = $"/uploads/{uniqueFileName}";
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            ClearProductCache();

            return Ok(new { Message = "Resim başarıyla yüklendi.", ImagePath = product.ImagePath });
        }

        // ÜRÜN GÜNCELLEME
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductFormModel model)
        {
            var product = await _context.Products.Include(p => p.ProductAllergens).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı." });
            }

            product.Name_TR = model.Name_TR;
            product.Name_EN = model.Name_EN;
            product.Description_TR = model.Description_TR;
            product.Description_EN = model.Description_EN;
            product.Price = (float)model.Price;
            product.Is_Active = model.Is_Active;
            product.CategoryID = model.CategoryID;
            product.SortOrder = model.SortOrder;
            product.HalfPortionOf = model.HalfPortionOf; // ✅ BURADA

            product.ProductAllergens.Clear();
            if (model.ProductAllergens != null)
            {
                foreach (var allergen in model.ProductAllergens)
                {
                    var existingAllergen = await _context.Allergens.FindAsync(allergen.AllergenId);
                    if (existingAllergen != null)
                    {
                        product.ProductAllergens.Add(new ProductAllergen
                        {
                            AllergenId = allergen.AllergenId,
                            Allergen = existingAllergen
                        });
                    }
                }
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            ClearProductCache();

            return NoContent();
        }


        // ÜRÜN SİLME
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p => p.ProductAllergens).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            ClearProductCache();

            return NoContent();
        }

        // KATEGORİYE GÖRE ÜRÜNLERİ GETİR
        [HttpGet("{categoryId}/products")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {   
            var products = await _context.Products
                .Where(p => p.CategoryID == categoryId)
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound(new { Message = "Bu kategoriye ait ürün bulunamadı." });
            }

            return Ok(products);
        }
        //Sıralama güncelleme
        [HttpPut("UpdateProductOrder")]
        public async Task<IActionResult> UpdateProductOrder([FromBody] List<Product> updatedProducts)
        {
            if (updatedProducts == null || updatedProducts.Count == 0)
            {
                return BadRequest(new { Message = "Güncellenecek ürünler eksik." });
            }

            foreach (var updatedProduct in updatedProducts)
            {
                var product = await _context.Products.FindAsync(updatedProduct.ProductId);
                if (product != null)
                {
                    product.SortOrder = updatedProduct.SortOrder;
                }
            }

            await _context.SaveChangesAsync();
            ClearProductCache(); // Cache temizleme işlemi

            return Ok(new { Message = "Ürün sıralamaları güncellendi." });
        }

        [HttpPost("clear-cache")]
        public IActionResult ClearProductCacheExternal()
        {
            ClearProductCache();
            return Ok(new { Message = "Product cache temizlendi." });
        }


        // CACHE TEMİZLEME METODU
        private void ClearProductCache()
        {
            _cache.Remove("all_products");
            _cache.Remove("active_products");
        }


    }
}
