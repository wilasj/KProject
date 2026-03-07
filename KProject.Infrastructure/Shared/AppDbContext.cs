using System.Reflection;
using KProject.Domain.Clientes;
using KProject.Domain.Convites;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Domain.Vendas;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KProject.Infrastructure.Shared;

public sealed class AppDbContext(DbContextOptions options) : IdentityUserContext<IdentityUser<int>, int>(options), IDataProtectionKeyContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SetAspNetCoreIdentityDatabaseNamesInSnakeCase(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void SetAspNetCoreIdentityDatabaseNamesInSnakeCase(ModelBuilder modelBuilder)
    {
        // Adapted from: https://github.com/efcore/EFCore.NamingConventions/issues/2#issuecomment-612651161
        // For identity model v2 as defined in IdentityDbContext.OnModelCreatingVersion2.
        modelBuilder.Entity<IdentityUser<int>>().ToTable("asp_net_users");
        modelBuilder.Entity<IdentityUser<int>>().HasIndex(u => u.NormalizedUserName).HasDatabaseName("user_name_index").IsUnique();
        modelBuilder.Entity<IdentityUser<int>>().HasIndex(u => u.NormalizedEmail).HasDatabaseName("email_index");
        
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("asp_net_user_tokens");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("asp_net_user_logins");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("asp_net_user_claims");

    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Lote> Lotes { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Convite> Convites { get; set; }
}