using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Portals_of_Madness
{
    public partial class GDBBackupModel : DbContext
    {
        public GDBBackupModel()
            : base("name=GDBBackupModel")
        {
        }

        public virtual DbSet<AbilityDatabase> AbilityDatabase { get; set; }
        public virtual DbSet<AICharactersDatabase> AICharactersDatabase { get; set; }
        public virtual DbSet<PlayerCharactersDatabase> PlayerCharactersDatabase { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AICharactersDatabase>()
                .Property(e => e.resourceName)
                .IsFixedLength();

            modelBuilder.Entity<PlayerCharactersDatabase>()
                .Property(e => e.resourceName)
                .IsFixedLength();

            modelBuilder.Entity<PlayerCharactersDatabase>()
                .Property(e => e.rarity)
                .IsFixedLength();
        }
    }
}
