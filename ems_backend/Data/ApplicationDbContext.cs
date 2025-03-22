using Microsoft.EntityFrameworkCore;
using ems_backend.Models;

namespace ems_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ticket - User (Disable Cascade Delete)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.TicketsOwned)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ticket - Event (Disable Cascade Delete)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            // Payment - User (Disable Cascade Delete)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Payment - Ticket (Disable Cascade Delete)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Ticket)
                .WithMany()
                .HasForeignKey(p => p.TicketId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
