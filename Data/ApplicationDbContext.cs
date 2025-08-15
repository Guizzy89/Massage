using Massage.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Massage.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MassageV> Massages { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Добавляем отношения между таблицами
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.TimeSlot)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction); // Если удаляется запись, слот остаётся неизменным

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.SelectedMassage)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction); // Аналогично, если удалить запись, вид массажа остается
    }
}