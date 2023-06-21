using Memochka.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Memochka.Models.MemochkaDbContext
{
    public class MemochkaContext : DbContext
    {
        public MemochkaContext()
        {
        }

        public MemochkaContext(DbContextOptions<MemochkaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleParagraph> ArticleParagraphs { get; set; }
        public virtual DbSet<Meme> Memes { get; set; }
        public virtual DbSet<MemeCategory> MemeCategories { get; set; }
        public virtual DbSet<MemePicture> MemePictures { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Relations Configuring
            //Users-Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
            //Memes-User
            modelBuilder.Entity<Meme>()
                .HasOne(m=>m.User)
                .WithMany(u=>u.Memes)
                .HasForeignKey(u => u.UserId);
            //Memes-MemeCategory
            modelBuilder.Entity<Meme>()
                .HasOne(m=>m.MemeCategory)
                .WithMany(c=>c.Memes)
                .HasForeignKey(u => u.CategoryId);
            //MemePictures-Meme
            modelBuilder.Entity<MemePicture>()
                .HasOne(p => p.Meme)
                .WithMany(m => m.MemePictures)
                .HasForeignKey(p => p.MemeId);
            //Articles-User
            modelBuilder.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId);
            //ArticleParagraphs-Article
            modelBuilder.Entity<ArticleParagraph>()
                .HasOne(ap => ap.Article)
                .WithMany(a => a.ArticleParagraphs)
                .HasForeignKey(ap => ap.ArticleId);
        }
        //Properties Configuring
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer("name=ConnectionStrings:BaseConnection");
    }
}
