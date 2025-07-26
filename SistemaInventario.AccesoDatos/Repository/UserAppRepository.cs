using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;

namespace SistemaInventario.DataAccess.Repository
{
    public class UserAppRepository : Repository<UserApp>, IUserAppRepository
    {
        private readonly ApplicationDbContext _context;

        public UserAppRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

    }
}
