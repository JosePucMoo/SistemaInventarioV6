using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;

namespace SistemaInventario.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryBD = _context.Stores.FirstOrDefault(s => s.Id == category.Id);

            if (categoryBD != null)
            {
                categoryBD.Name = category.Name;
                categoryBD.Description = category.Description;
                categoryBD.Status = category.Status;
                _context.SaveChanges();
            }
        }

    }
}
