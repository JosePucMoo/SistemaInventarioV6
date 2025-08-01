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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Company company)
        {
            var companyBD = _context.Companies.FirstOrDefault(s => s.Id == company.Id);

            if (companyBD != null)
            {
                companyBD.Name = company.Name;
                companyBD.Description = company.Description;
                companyBD.Country = company.Country;
                companyBD.City = company.City;
                companyBD.Address = company.Address;
                companyBD.Telephone = company.Telephone;
                companyBD.StoreSaleId = company.StoreSaleId;
                companyBD.UpdatedById = company.UpdatedById;
                companyBD.UpdateDate = company.UpdateDate;
                _context.SaveChanges();
            }
        }
    }
}
