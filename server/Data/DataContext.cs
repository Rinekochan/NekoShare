using Microsoft.EntityFrameworkCore;
using server.Entities;

namespace server.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}