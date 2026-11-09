using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProyectoGalactico.Models;

namespace ProyectoGalactico.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CardCharacter> CardsCharacter => Set<CardCharacter>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<BattleRecord> BattleRecords => Set<BattleRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Species).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Faction).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.CharacterState).HasConversion<string>().HasMaxLength(20);
            entity.HasMany(c => c.Cards)
                .WithOne(card => card.Character)
                .HasForeignKey(card => card.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CardCharacter>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Power).HasMaxLength(150).IsRequired();
            entity.Property(c => c.SpecialHability).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Weapon).HasMaxLength(150).IsRequired();
            entity.Property(c => c.PictureUrl).HasMaxLength(500).IsRequired();
            entity.HasIndex(c => c.CharacterId);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HashAutorizationCode).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Winner).HasMaxLength(500);

            var listComparer = new ValueComparer<List<int>>(
                (left, right) => left != null && right != null && left.SequenceEqual(right),
                value => value == null ? 0 : value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
                value => value == null ? new List<int>() : value.ToList());

            entity.Property(e => e.ParticipantsIds)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                    value => JsonSerializer.Deserialize<List<int>>(value, (JsonSerializerOptions?)null) ?? new List<int>())
                .Metadata.SetValueComparer(listComparer);

            entity.Property(e => e.DeadIds)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                    value => JsonSerializer.Deserialize<List<int>>(value, (JsonSerializerOptions?)null) ?? new List<int>())
                .Metadata.SetValueComparer(listComparer);
        });

        modelBuilder.Entity<BattleRecord>(entity =>
        {
            entity.HasKey(b => b.Id);

            // Guardar el resultado como texto
            entity.Property(b => b.Outcome)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Relación con Event
            entity.HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Character
            entity.HasOne(b => b.Character)
                .WithMany()
                .HasForeignKey(b => b.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con CardCharacter
            entity.HasOne(b => b.Card)
                .WithMany()
                .HasForeignKey(b => b.CardId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(b => b.EventId);
            entity.HasIndex(b => b.CharacterId);
        });
    }
}
