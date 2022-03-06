using ExpenseManagement.Api.Common;
using ExpenseManagement.Api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Data
{
    public class ExpenseManagementDbcontext : IdentityDbContext<User, Role, string>
    {
        public ExpenseManagementDbcontext(DbContextOptions options) : base(options) { }

        public DbSet<Expense>? Expenses { get; set; }
        public DbSet<UserExpense>? UserExpenses { get; set; }
        public DbSet<DebtCharge>? DebtCharges { get; set; }
        public DbSet<DebtReminder>? DebtReminders { get; set; }
        public DbSet<TransactionHistory>? TransactionHistories { get; set; }
        public DbSet<Log>? Logs { get; set; }
        public DbSet<Chat>? Chats { get; set; }
        public DbSet<UserChat>? UserChats { get; set; }
        public DbSet<Message>? Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserExpense>()
                   .HasKey(p => new { p.UserId, p.ExpenseId });

            builder.Entity<DebtCharge>()
                   .HasOne(s => s.Creditor)
                   .WithMany(u => u.Creditors)
                   .HasForeignKey(u => u.CreditorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DebtCharge>()
                   .HasOne(s => s.Debtor)
                   .WithMany(u => u.Debtors)
                   .HasForeignKey(u => u.DebtorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserChat>()
                   .HasKey(x => new { x.UserId, x.ChatId });

            builder.FilterDeletedRecords();

            base.OnModelCreating(builder);
        }
    }
}