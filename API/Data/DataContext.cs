using System;
using API.Entities;
using API.Entities.MojElektro;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
    IdentityUserToken<int>>(options)
{
    public DbSet<Stavba> Stavbe { get; set; }
    public DbSet<MerilnoMesto> MerilnaMesta { get; set; }
    public DbSet<Odcitek> Odcitki { get; set; }
    public DbSet<GeoTocka> GeoTocke { get; set; } = null!;
    public DbSet<MojElektroMerilnoMesto> MojElektroMerilnaMesta { get; set; }
    public DbSet<MojElektro15MinMeritev> MojElektro15MinMeritve { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();


        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
    }
}
