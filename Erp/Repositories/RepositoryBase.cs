using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.DataBase;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Erp.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString = "Server=localhost\\SQLEXPRESS; Database =ERPDatabase;Encrypt=false; Integrated Security=true";
        }

        public DbContextOptions<ErpDbContext> options = new DbContextOptionsBuilder<ErpDbContext>()
    .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ERPDatabase;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=false;")
    .Options;

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
