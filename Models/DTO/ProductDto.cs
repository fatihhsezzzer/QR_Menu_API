namespace Mazina_Backend.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public string Description_TR { get; set; }
        public string Description_EN { get; set; }
        public float Price { get; set; }
        public bool Is_Active { get; set; }
        public string ImagePath { get; set; }
        public int CategoryID { get; set; }
        public int? HalfPortionOf { get; set; }
        public int? WhatOf { get; set; }
        public string? WhatIs_TR { get; set; }
        public string? WhatIs_EN { get; set; }

        public float SortOrder { get; set; }
        public List<AllergenDto> ProductAllergens { get; set; }
    }

    public class AllergenDto
    {
        public int AllergenId { get; set; }
        public string Name_TR { get; set; }
        public string ImagePath { get; set; }
    }
}
