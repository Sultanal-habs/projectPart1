using Microsoft.EntityFrameworkCore;
using projectPart1.Models;

namespace projectPart1.Data
{
    public class ArtGalleryDbContext : DbContext
    {
        public ArtGalleryDbContext(DbContextOptions<ArtGalleryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<ArtworkLike> ArtworkLikes { get; set; }
        public DbSet<ExhibitionArtwork> ExhibitionArtworks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.ToTable("Artists");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ArtistId").ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasMany(a => a.Artworks)
                      .WithOne(aw => aw.Artist)
                      .HasForeignKey(aw => aw.ArtistId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Status).HasConversion<byte>();
            });

            modelBuilder.Entity<Artwork>(entity =>
            {
                entity.ToTable("Artworks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ArtworkId").ValueGeneratedOnAdd();
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Type).HasColumnName("ArtworkType").HasConversion<byte>();
                entity.Property(e => e.Status).HasConversion<byte>();
            });

            modelBuilder.Entity<Exhibition>(entity =>
            {
                entity.ToTable("Exhibitions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ExhibitionId").ValueGeneratedOnAdd();
                entity.Property(e => e.Status).HasConversion<byte>();
            });

            modelBuilder.Entity<ArtworkLike>(entity =>
            {
                entity.ToTable("ArtworkLikes");
                entity.HasKey(e => e.LikeId);
                entity.Property(e => e.LikeId).ValueGeneratedOnAdd();
                entity.HasOne<Artwork>()
                      .WithMany()
                      .HasForeignKey(al => al.ArtworkId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(al => al.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExhibitionArtwork>(entity =>
            {
                entity.ToTable("ExhibitionArtworks");
                entity.HasKey(ea => new { ea.ExhibitionId, ea.ArtworkId });
                entity.HasOne<Exhibition>()
                      .WithMany()
                      .HasForeignKey(ea => ea.ExhibitionId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne<Artwork>()
                      .WithMany()
                      .HasForeignKey(ea => ea.ArtworkId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

    public class ArtworkLike
    {
        public int LikeId { get; set; }
        public int ArtworkId { get; set; }
        public int? UserId { get; set; }
        public DateTime LikedDate { get; set; } = DateTime.Now;
        public string? IpAddress { get; set; }
    }

    public class ExhibitionArtwork
    {
        public int ExhibitionId { get; set; }
        public int ArtworkId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public int? DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
    }
}
