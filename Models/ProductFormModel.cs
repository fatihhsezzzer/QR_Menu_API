using Microsoft.AspNetCore.Mvc;

namespace Mazina_Backend.Models
{
    public class ProductFormModel
    {
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public string Description_TR { get; set; }
        public string Description_EN { get; set; }
        public decimal Price { get; set; }
        public bool Is_Active { get; set; }
        public int? HalfPortionOf { get; set; }
        public int CategoryID { get; set; }
        public float SortOrder { get; set; }
        public List<ProductAllergenFormModel>? ProductAllergens { get; set; }
    }

    public class ProductAllergenFormModel
    {
        public int AllergenId { get; set; }
    }

}
