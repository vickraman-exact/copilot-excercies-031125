using HR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HR.Infrastructure.Data
{
    public class HRPayDezkDbContext : DbContext
    {
        public HRPayDezkDbContext(DbContextOptions<HRPayDezkDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<PayPeriod> PayPeriods { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<Deduction> Deductions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasOne(u => u.Employee)
                      .WithOne(e => e.User)
                      .HasForeignKey<User>(u => u.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            base.OnModelCreating(modelBuilder);

            // Configure the Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();

                // Configure relationships
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Position)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(e => e.PositionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Manager)
                      .WithMany(m => m.DirectReports)
                      .HasForeignKey(e => e.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure the Department entity
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.DepartmentId);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);

                // Configure relationships
                entity.HasOne(d => d.Manager)
                      .WithMany()
                      .HasForeignKey(d => d.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ParentDepartment)
                      .WithMany(p => p.SubDepartments)
                      .HasForeignKey(d => d.ParentDepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure the Position entity
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(100);

                // Configure relationships
                entity.HasOne(p => p.Department)
                      .WithMany(d => d.Positions)
                      .HasForeignKey(p => p.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure the Salary entity
            modelBuilder.Entity<Salary>(entity =>
            {
                entity.HasKey(s => s.SalaryId);
                entity.Property(s => s.Amount).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Currency).IsRequired().HasMaxLength(3);

                // Configure relationships
                entity.HasOne(s => s.Employee)
                      .WithMany()
                      .HasForeignKey(s => s.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure the PayPeriod entity
            modelBuilder.Entity<PayPeriod>(entity =>
            {
                entity.HasKey(p => p.PayPeriodId);
            });

            // Configure the Payslip entity
            modelBuilder.Entity<Payslip>(entity =>
            {
                entity.HasKey(p => p.PayslipId);
                entity.Property(p => p.GrossPay).HasColumnType("decimal(18,2)");
                entity.Property(p => p.TotalDeductions).HasColumnType("decimal(18,2)");
                entity.Property(p => p.NetPay).HasColumnType("decimal(18,2)");

                // Configure relationships
                entity.HasOne(p => p.Employee)
                      .WithMany()
                      .HasForeignKey(p => p.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PayPeriod)
                      .WithMany(pp => pp.Payslips)
                      .HasForeignKey(p => p.PayPeriodId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure the Deduction entity
            modelBuilder.Entity<Deduction>(entity =>
            {
                entity.HasKey(d => d.DeductionId);
                entity.Property(d => d.Description).IsRequired().HasMaxLength(255);
                entity.Property(d => d.Amount).HasColumnType("decimal(18,2)");

                // Configure relationships
                entity.HasOne(d => d.Payslip)
                      .WithMany(p => p.Deductions)
                      .HasForeignKey(d => d.PayslipId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Apply configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is not null && (
                           e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var now = DateTime.UtcNow;

                // Set Created timestamp for new entities
                if (entityEntry.State == EntityState.Added)
                {
                    if (entityEntry.Entity.GetType().GetProperty("Created") != null)
                    {
                        entityEntry.Property("Created").CurrentValue = now;
                    }
                }

                // Always update Modified timestamp
                if (entityEntry.Entity.GetType().GetProperty("Modified") != null)
                {
                    entityEntry.Property("Modified").CurrentValue = now;
                }
            }
        }
    }
}
