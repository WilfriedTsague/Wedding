namespace Wedding.ContexteDB;
using Microsoft.EntityFrameworkCore;
using Wedding.Models;

public class WeddingContext :DbContext
{
    public WeddingContext() { }

    public WeddingContext(DbContextOptions<WeddingContext> options):base(options)
    {

    }

    public virtual DbSet<Invite> Invites { get; set; }
    public virtual DbSet<Table> Tables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invite>()
           .HasOne(i => i.Inviteur)
           .WithMany()  // Pas de collection d'invités dans Inviteur
           .HasForeignKey(i => i.IdInviteur)
           .OnDelete(DeleteBehavior.Restrict);  // Empêche la suppression en cascade

        modelBuilder.Entity<Invite>()
            .HasOne(i => i.Table)
            .WithMany(t => t.invites)
            .HasForeignKey(i => i.IdTable);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.DisplayName());
        }

        base.OnModelCreating(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Table>().HasData(
            new Table { Id = 1, NomTable = "Table 1", NbrePlaces = 10, Statut = Statut.Vide, StatutDuJour = Statut.Vide },
            new Table { Id = 2, NomTable = "Table 2", NbrePlaces = 8, Statut = Statut.En_cours, StatutDuJour = Statut.Vide }
        );

    }
}
