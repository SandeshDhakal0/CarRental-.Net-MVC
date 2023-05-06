using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Models;
using HamroCarRental.Models.Identity;

namespace HamroCarRental.Data;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

   // public DbSet<Actor> Actors { get; set; }
    //public DbSet<CastMember> CastMembers { get; set; }
    public DbSet<CarCategory> CarCategories { get; set; }
    public DbSet<CarCopy> carCopies { get; set; }
    public DbSet<CarDetail> CarDetails { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanType> LoanTypes { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<MembershipCategory> MembershipCategories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public object CarDetail { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<CastMember>().HasKey(cm => new
        //{
        //    cm.ActorNumber,
        //    cm.CarNumber
        //});

        //modelBuilder.Entity<CastMember>().HasOne(a => a.Actor).WithMany(cm => cm.CastMembers)
        //    .HasForeignKey(a => a.ActorNumber);
        //modelBuilder.Entity<CastMember>().HasOne(a => a.CarDetail).WithMany(cm => cm.CastMembers)
        //    .HasForeignKey(a => a.CarNumber);


        modelBuilder.Entity<CarCopy>().HasOne(dt => dt.CarDetail).WithMany(dt => dt.carCopies)
            .HasForeignKey(dt => dt.CarNumber);


        modelBuilder.Entity<CarDetail>().HasOne(dt => dt.CarCategory).WithMany(dt => dt.CarDetails)
            .HasForeignKey(dt => dt.CategoryNumber);
        modelBuilder.Entity<CarDetail>().HasOne(dt => dt.Brand).WithMany(dt => dt.CarDetails)
            .HasForeignKey(dt => dt.BrandNumber);
        modelBuilder.Entity<CarDetail>().HasOne(dt => dt.Studio).WithMany(dt => dt.CarDetails)
            .HasForeignKey(dt => dt.StudioNumber);

        modelBuilder.Entity<CarDetail>().Property(dt => dt.PenaltyCharge).HasPrecision(10, 3);
        modelBuilder.Entity<CarDetail>().Property(dt => dt.StandardCharge).HasPrecision(10, 3);


        modelBuilder.Entity<Loan>().HasOne(dt => dt.CarCopy).WithMany(dt => dt.Loans)
            .HasForeignKey(dt => dt.CopyNumber);
        //modelBuilder.Entity<Loan>().HasOne(dt => dt.LoanType).WithMany(dt => dt.Loans)
        //    .HasForeignKey(dt => dt.LoanTypeNumber);
        modelBuilder.Entity<Loan>().HasOne(dt => dt.Member).WithMany(dt => dt.Loans)
            .HasForeignKey(dt => dt.MemberNumber);
        modelBuilder.Entity<Loan>().Property(dt => dt.ReturnAmount).HasPrecision(10, 3);

        modelBuilder.Entity<Member>().HasOne(dt => dt.MembershipCategory).WithMany(dt => dt.Members)
            .HasForeignKey(dt => dt.MemberCategoryNumber);

        base.OnModelCreating(modelBuilder);
    }

    internal Task GetCarCategoryAsync(int id)
    {
        throw new NotImplementedException();
    }
}