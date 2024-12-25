using Microsoft.EntityFrameworkCore;
using NekoShare.Entities;

namespace NekoShare.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}