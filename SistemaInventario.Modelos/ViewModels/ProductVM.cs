using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaInventario.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> BrandList { get; set; }
        public IEnumerable<SelectListItem> FatherList { get; set; }
    }
}
