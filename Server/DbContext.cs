using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}