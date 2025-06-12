namespace Mazina_Backend.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public bool Is_New { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Drink { get; set; }

        public float SortOrder { get; set; }
        public string ImagePath { get; set; }
        public int Size { get; set; }

    }
}
