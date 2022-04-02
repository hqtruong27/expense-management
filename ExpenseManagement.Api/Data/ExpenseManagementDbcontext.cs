using ExpenseManagement.Api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Api.Data
{
    public class ExpenseManagementDbcontext : IdentityDbContext<User, Role, string>
    {
        public ExpenseManagementDbcontext(DbContextOptions options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<UserExpense> UserExpenses { get; set; } = default!;
        public DbSet<DebtCharge> DebtCharges { get; set; } = default!;
        public DbSet<DebtReminder> DebtReminders { get; set; } = default!;
        public DbSet<TransactionHistory> TransactionHistories { get; set; } = default!;
        public DbSet<Log> Logs { get; set; } = default!;
        public DbSet<Chat> Chats { get; set; } = default!;
        public DbSet<UserChat> UserChats { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
        public DbSet<TotpSercurityToken> TotpSercurityTokens { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserExpense>()
                   .HasKey(p => new { p.UserId, p.ExpenseId });
            
            builder.Entity<UserExpense>().HasOne(x => x.Expense)
                   .WithOne(i => i.UserExpense)
                   .HasForeignKey<UserExpense>(x => x.ExpenseId);

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

            //builder.Entity<Chat>().HasQueryFilter(x => !x.IsDeleted);
            //builder.Entity<Expense>().HasQueryFilter(x => !x.IsDeleted);
            //builder.Entity<UserExpense>().HasQueryFilter(x => !x.IsDeleted);
            //builder.Entity<Chat>().HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<UserChat>()
                   .HasKey(x => new { x.UserId, x.ChatId });

            builder.FilterDeletedRecords();

            base.OnModelCreating(builder);
        }
    }
}