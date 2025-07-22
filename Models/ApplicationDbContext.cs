using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace TicketManagementAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) // Call the base constructor of DbContext
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
    }
}
