namespace MediaModelClasses
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Media : DbContext
    {
        public Media()
            : base("name=Media")
        {
        }

        public virtual DbSet<album> album { get; set; }
        public virtual DbSet<artist> artist { get; set; }
        public virtual DbSet<comic_series> comic_series { get; set; }
        public virtual DbSet<movies> movies { get; set; }
        public virtual DbSet<porn> porn { get; set; }
        public virtual DbSet<tv_episodes> tv_episodes { get; set; }
        public virtual DbSet<tv_shows> tv_shows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<artist>()
                .HasMany(e => e.album)
                .WithRequired(e => e.artist)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tv_shows>()
                .HasMany(e => e.tv_episodes)
                .WithRequired(e => e.tv_shows)
                .WillCascadeOnDelete(false);
        }
    }
}
