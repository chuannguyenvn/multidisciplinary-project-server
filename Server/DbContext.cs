using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PlantInformation> PlantInformations { get; set; }
    public DbSet<PlantLog> PlantLogs { get; set; }
}