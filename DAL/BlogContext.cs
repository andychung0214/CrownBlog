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

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Member> Members { get; set; }

        public virtual DbSet<MultiLanguage> MultiLanguages { get; set; }

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

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Email)
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.AuthCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(64);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<MultiLanguage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MultiLanguage");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateUser).HasMaxLength(64);

                entity.Property(e => e.Keyword)
                    .IsRequired()
                    .HasMaxLength(24);

                entity.Property(e => e.Lang)
                    .IsRequired()
                    .HasMaxLength(24)
                    .IsUnicode(false);

                entity.Property(e => e.Project)
                    .IsRequired()
                    .HasMaxLength(24);

                entity.Property(e => e.Value).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
