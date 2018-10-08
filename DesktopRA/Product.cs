using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopRA
{
    public class Product
    {
        public int idProduct { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string ShortDescription { get; set; }
        public int Stock { get; set; }
        public bool Featured { get; set; }
        public float Weight { get; set; }
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public int SubCategory_idSubCategory { get; set; }
        //public string SubCategory_idSubCategoryName { get; set; } //just to get the name of subcategory
        //public int idCategory { get; set; } //idCategory of subcategory of product
        public DateTime DateTimeRegister { get; set; }
        public bool AR { get; set; } //indicate if the product is used to Augmented Reality
        public string PictureMap { get; set; } //Used to save the Map Product, used to Augmented Reality

        //idProduct, Name, Description, Price,ShortDescription, Stock, Featured,Weight, Picture1, Picture2, SubCategory_idSubCategory, DateTimeRegister
    }
}
