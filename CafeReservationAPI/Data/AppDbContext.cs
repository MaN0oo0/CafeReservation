using CafeReservationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeReservationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Table> Tables => Set<Table>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
    }
}
