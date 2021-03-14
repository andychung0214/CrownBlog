using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CrownBlog.DAL
{
    public partial class BlogContext : DbContext
    {
        public BlogContext()
        {
        }

        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BlogActivity> BlogActivities { get; set; }
        public virtual DbSet<BlogArticle> BlogArticles { get; set; }
        public virtual DbSet<BlogMessage> BlogMessages { get; set; }
        public virtual DbSet<BlogTag> BlogTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=tcp:andychung0214.synology.me,1433;initial catalog=CrownBlogDB;User Id=sa;Password=Abcd1234;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BlogActivity>(entity =>
            {
                entity.ToTable("BlogActivity");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Latitude)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BlogArticle>(entity =>
            {
                entity.ToTable("BlogArticle");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Abstract).HasMaxLength(2048);

                entity.Property(e => e.BannerUrl)
                    .HasMaxLength(2048)
                    .HasColumnName("BannerURL");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.IconUrl)
                    .HasMaxLength(2048)
                    .HasColumnName("IconURL");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.Property(e => e.Url)
                    .HasMaxLength(2048)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<BlogMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.ToTable("BlogMessage");

                entity.Property(e => e.MessageId).ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.IconUrl)
                    .HasMaxLength(2048)
                    .HasColumnName("IconURL");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.Subject).HasMaxLength(256);
            });

            modelBuilder.Entity<BlogTag>(entity =>
            {
                entity.HasKey(e => e.TagId)
                    .HasName("PK_BlgTag");

                entity.ToTable("BlogTag");

                entity.Property(e => e.TagId)
                    .ValueGeneratedNever()
                    .HasColumnName("TagID");

                entity.Property(e => e.ArticleId).HasColumnName("ArticleID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.Name).HasMaxLength(2048);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
