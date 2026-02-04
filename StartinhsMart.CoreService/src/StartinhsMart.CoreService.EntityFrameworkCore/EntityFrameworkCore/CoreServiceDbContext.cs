using StartinhsMart.CoreService.Pets;
using StartinhsMart.CoreService.Categories;
using StartinhsMart.CoreService.Orders;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace StartinhsMart.CoreService.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class CoreServiceDbContext :
    AbpDbContext<CoreServiceDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    public DbSet<AppFileDescriptors.AppFileDescriptor> AppFileDescriptors { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public CoreServiceDbContext(DbContextOptions<CoreServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(CoreServiceConsts.DbTablePrefix + "YourEntities", CoreServiceConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Pet>(b =>
                {
                    b.ToTable(CoreServiceConsts.DbTablePrefix + "Pets", CoreServiceConsts.DbSchema);
                    b.ConfigureByConvention();
                    b.Property(x => x.TenantId).HasColumnName(nameof(Pet.TenantId));
                    b.Property(x => x.ImageId).HasColumnName(nameof(Pet.ImageId));
                    b.Property(x => x.Category).HasColumnName(nameof(Pet.Category));
                    b.Property(x => x.Name).HasColumnName(nameof(Pet.Name));
                    b.Property(x => x.Breed).HasColumnName(nameof(Pet.Breed));
                    b.Property(x => x.Age).HasColumnName(nameof(Pet.Age));
                    b.Property(x => x.Gender).HasColumnName(nameof(Pet.Gender));
                    b.Property(x => x.Color).HasColumnName(nameof(Pet.Color));
                    b.Property(x => x.Weight).HasColumnName(nameof(Pet.Weight));
                    b.Property(x => x.HealthStatus).HasColumnName(nameof(Pet.HealthStatus));
                    b.Property(x => x.Vaccinations).HasColumnName(nameof(Pet.Vaccinations));
                    b.Property(x => x.Description).HasColumnName(nameof(Pet.Description));
                    b.Property(x => x.Price).HasColumnName(nameof(Pet.Price));
                    b.Property(x => x.Quantity).HasColumnName(nameof(Pet.Quantity));
                    b.Property(x => x.IsBooth).HasColumnName(nameof(Pet.IsBooth));
                    b.Property(x => x.IsStock).HasColumnName(nameof(Pet.IsStock));
                });

        builder.Entity<AppFileDescriptors.AppFileDescriptor>(b =>
                    {
                        b.ToTable(CoreServiceConsts.DbTablePrefix + "FileDescriptors", CoreServiceConsts.DbSchema);
                        b.ConfigureByConvention();
                        b.Property(x => x.Name);
                        b.Property(x => x.MimeType);
                    });

        builder.Entity<Category>(b =>
                    {
                        b.ToTable(CoreServiceConsts.DbTablePrefix + "Categories", CoreServiceConsts.DbSchema);
                        b.ConfigureByConvention();
                        b.Property(x => x.Name).IsRequired().HasMaxLength(256);
                        b.Property(x => x.Slug).HasMaxLength(256);
                        b.Property(x => x.Description).HasMaxLength(2000);
                        b.HasIndex(x => x.Name);
                        b.HasIndex(x => x.Slug);
                        b.HasIndex(x => x.ParentCategoryId);
                    });

        builder.Entity<Order>(b =>
                    {
                        b.ToTable(CoreServiceConsts.DbTablePrefix + "Orders", CoreServiceConsts.DbSchema);
                        b.ConfigureByConvention();
                        b.Property(x => x.OrderNumber).IsRequired().HasMaxLength(50);
                        b.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
                        b.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
                        b.Property(x => x.ShippingPhone).IsRequired().HasMaxLength(20);
                        b.Property(x => x.CustomerName).HasMaxLength(256);
                        b.Property(x => x.CustomerEmail).HasMaxLength(256);
                        b.Property(x => x.Note).HasMaxLength(1000);
                        b.Property(x => x.CancellationReason).HasMaxLength(500);
                        b.HasIndex(x => x.OrderNumber);
                        b.HasIndex(x => x.UserId);
                        b.HasIndex(x => x.Status);
                        b.HasIndex(x => x.PaymentStatus);
                        b.HasIndex(x => x.CreationTime);

                    });

        builder.Entity<OrderItem>(b =>
                    {
                        b.ToTable(CoreServiceConsts.DbTablePrefix + "OrderItems", CoreServiceConsts.DbSchema);
                        b.ConfigureByConvention();
                        b.Property(x => x.PetName).IsRequired().HasMaxLength(256);
                        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                        b.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
                        b.HasIndex(x => x.OrderId);
                        b.HasIndex(x => x.PetId);
                        b.HasOne(x => x.Order).WithMany(o => o.Items).HasForeignKey(x => x.OrderId).IsRequired();
                    });
    }
}