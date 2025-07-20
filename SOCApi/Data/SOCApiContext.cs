using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SOCApi.Models;

namespace SOCApi.Data
{
    public class SOCApiContext : DbContext
    {
        public SOCApiContext (DbContextOptions<SOCApiContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
    }
}
