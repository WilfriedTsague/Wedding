using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wedding.Models;

namespace Wedding.ContexteDB
{
    public class WeddingContext : IdentityDbContext<IdentityUser>
    {
        public WeddingContext() { }

        public WeddingContext(DbContextOptions<WeddingContext> options) : base(options)
        {
        }

        public virtual DbSet<Invite> Invites { get; set; }
        public virtual DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration des entités spécifiques de l'application
            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Inviteur)
                .WithMany()
                .HasForeignKey(i => i.IdInviteur)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Table)
                .WithMany(t => t.Invites)
                .HasForeignKey(i => i.IdTable);

            // Optionnel : Conversion des noms des tables pour correspondre aux noms de modèles
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }

            // Appel de la méthode de configuration de base
            base.OnModelCreating(modelBuilder);
        }
    }
}
