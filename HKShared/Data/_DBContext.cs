using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Emit;

namespace HKShared.Data
{
    public class AppDBContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public virtual DbSet<AccountLog> AccountLogs { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }

        public virtual DbSet<MediaAlbum> MediaAlbums { get; set; }
        public virtual DbSet<MediaFile> MediaFiles { get; set; }

        public virtual DbSet<Taxonomy> Taxonomies { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductTaxo> ProductTaxos { get; set; }
        public virtual DbSet<ProductAttr> ProductAttribs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }


        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MediaAlbum>(entity => {
                entity.HasIndex(e => e.ShortName).IsUnique();

                entity.HasMany<MediaFile>(g => g.MediaFiles)
                .WithOne(s => s.MediaAlbum)
                .HasForeignKey(s => s.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            });
        }

        public virtual IQueryable<Taxonomy> ItemCats
        {
            get { return Taxonomies.Where(x => x.Type == TaxoType.Category); }
        }

        public virtual IQueryable<Taxonomy> ItemCollecs
        {
            get { return Taxonomies.Where(x => x.Type == TaxoType.Collection); }
        }

        public virtual IQueryable<Taxonomy> ItemVens
        {
            get { return Taxonomies.Where(x => x.Type == TaxoType.Vendor); }
        }
        public virtual IQueryable<Taxonomy> ItemVariants
        {
            get { return Taxonomies.Where(x => x.Type == TaxoType.Variants); }
        }
        public virtual IQueryable<Taxonomy> ItemVariantsParent
        {
            get { return Taxonomies.Where(x => x.Type == TaxoType.Variants && x.ParentId == null); }
        }
        public virtual IQueryable<Taxonomy> ItemVariantsChild
        {
            get { return Taxonomies.Where(x => x.Type == TaxoType.Variants && x.ParentId != null); }
        }
    }
}

