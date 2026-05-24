using LOS.Models;
using LOS.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LOS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<KYCDocument> KYCDocuments { get; set; }
        public DbSet<CibilReport> CibilReports { get; set; }
        public DbSet<EligibilityResult> EligibilityResults { get; set; }
        public DbSet<ScoreCard> ScoreCards { get; set; }
        public DbSet<ApplyLoan> ApplyLoans { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<DealReview> DealReviews { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<SanctionLetter> SanctionLetters { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        //public DbSet<ApplicationStatusVM> TrackStatus { get; set; } 
    
 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Customer>()
                .Property(x => x.MonthlyIncome)
                .HasPrecision(18, 2);

            //  Seed Interest table (optional here)
            builder.Entity<Interest>().HasData(
                new Interest { InterestId = 1, LoanType = "Home Loan", InterestRate = 8.5M },
                new Interest { InterestId = 2, LoanType = "Car Loan", InterestRate = 10.0M },
                new Interest { InterestId = 3, LoanType = "Personal Loan", InterestRate = 12.0M }
            );

            base.OnModelCreating(builder);




            builder.Entity<KYCDocument>(k =>
            {
                k.HasOne(x => x.Customer)
                 .WithMany(x => x.KYCDocument)
                 .HasForeignKey(x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CibilReport>(c =>
            {
                 c.HasOne(x => x.Customer)
                 .WithMany(x => x.CibilReport)
                 .HasForeignKey(x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ScoreCard>(s =>
            {
                s.HasOne(x => x.Customer)
                 .WithMany(x => x.ScoreCard)
                 .HasForeignKey(x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<EligibilityResult>(e =>
            {
                e.HasOne(x => x.Customer)
                 .WithMany(x => x.EligibilityResult)
                 .HasForeignKey(x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            //builder.Entity<LoanDeals>(l =>
            //{
            //    l.HasOne(x => x.Customer)
            //     .WithMany(x => x.LoanDeals)
            //     .HasForeignKey(x => x.CustomerId)
            //     .OnDelete(DeleteBehavior.Restrict);

            //    l.HasOne(x => x.LoanType)
            //     .WithMany(x => x.LoanDeals)
            //     .HasForeignKey(x => x.LoanTypeId)
            //     .OnDelete(DeleteBehavior.Restrict);
            //});

            //builder.Entity<DealReview>(d =>
            //{
            //    d.HasOne(x => x.LoanDeals)
            //     .WithOne(x => x.DealReviews)
            //     .HasForeignKey<DealReview>(x => x.LoanDealId)
            //     .OnDelete(DeleteBehavior.Cascade);

            //    d.HasOne(x => x.Officer)
            //     .WithMany(x => x.DealReviews)
            //     .HasForeignKey(x => x.OfficerId)
            //     .OnDelete(DeleteBehavior.Restrict);
            //});
            builder.Entity<Deal>()
               .Property(x => x.RequestedAmount)
               .HasPrecision(18, 2);

            //builder.Entity<Deal>()
            //    .Property(x => x.MonthlyIncome)
            //    .HasPrecision(18, 2);

            builder.Entity<Deal>()
                .Property(x => x.InterestRate)
                .HasPrecision(5, 2);

            builder.Entity<DealReview>()
            .HasOne(dr => dr.Deal)
            .WithMany(d => d.DealReviews)
            .HasForeignKey(dr => dr.DealId)
            .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<SanctionLetter>(s =>
            {
                s.HasOne(x => x.LoanDeals)
                .WithOne(x => x.SanctionLetter)
                .HasForeignKey<SanctionLetter>(x => x.LoanDealId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // LoanDisbursement

            builder.Entity<Disbursement>(d =>
            {
                d.HasOne(x => x.LoanDeals)
                .WithOne(x => x.Disbursement)
                .HasForeignKey<Disbursement>(x => x.LoanDealId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplyLoan>()
               .HasOne(a => a.Customer)
               .WithMany(c => c.ApplyLoans)
               .HasForeignKey(a => a.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Deal>()
       .HasOne(d => d.Loan)
        .WithMany(l => l.Deals) // or .WithMany(l => l.Deals) if you add a collection in ApplyLoan
       .HasForeignKey(d => d.LoanId);
        }
    }
    }

