using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productBD = _context.Products.FirstOrDefault(s => s.Id == product.Id);

            if (productBD != null)
            {
                if (productBD.ImageUrl != null)
                {
                    productBD.ImageUrl = product.ImageUrl;
                }

                productBD.SerialNumber = product.SerialNumber;
                productBD.Description = product.Description;
                productBD.Status = product.Status;
                productBD.Price = product.Price;
                productBD.Cost = product.Cost;
                productBD.CategoryId = product.CategoryId;
                productBD.BrandId = product.BrandId;
                productBD.FatherId = product.FatherId;

                _context.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == "Category")
            {
                return _context.Categories.Where(c => c.Status == true).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            if (obj == "Brand")
            {
                return _context.Brands.Where(c => c.Status == true).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            if (obj == "Product")
            {
                return _context.Products.Where(c => c.Status == true).Select(c => new SelectListItem
                {
                    Text = c.Description,
                    Value = c.Id.ToString()
                });
            }

            return null;
        }
    }
}
