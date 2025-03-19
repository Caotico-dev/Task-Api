using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Mi_Task_Api.Model
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public DbSet<MiTasks> Tasks { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<ScoredTasks> ScoredTasks { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MiTasks>(builder =>
            {
                builder.HasOne<User>(t => t.User)
                       .WithMany(u => u.MiTasks)
                       .HasForeignKey(t => t.IdUser)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.Property(t => t.IdUser)
                       .IsRequired(required: true);

                builder.HasKey(t => t.TaskId);

                builder.Property(t => t.TaskId)
                       .ValueGeneratedOnAdd();

                builder.Property(t => t.Description)
                       .IsRequired(required: true)
                       .HasMaxLength(400);

                builder.Property(t => t.Prioritis)
                       .IsRequired(required: true)
                       .HasMaxLength(100);

                builder.Property(t => t.Term)
                       .IsRequired(required: true);

                builder.Property(t => t.Resource)
                       .IsRequired(required: true)
                       .HasMaxLength(100);

                builder.Property(t => t.Status)
                       .IsRequired(required: true)
                       .HasMaxLength(20);

                builder.Property(t => t.Dependecy)
                       .IsRequired(required: true)
                       .HasMaxLength(100);

                builder.Property(t => t.SubTasks)
                       .IsRequired(required: true)
                       .HasMaxLength(100);

                builder.Property(t => t.Comments)
                       .IsRequired(required: true)
                       .HasMaxLength(100);

                builder.Property(t => t.ExpectedResults)
                       .IsRequired(required: true)
                       .HasMaxLength(100);


            });

            builder.Entity<Friends>(builder =>
            {
                builder.HasKey(f => f.Id);

                builder.HasOne<User>(f => f.User)
                       .WithMany(m => m.Friends)
                       .HasForeignKey(f => f.IdUser).OnDelete(DeleteBehavior.Cascade);

                builder.Property(f => f.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(f => f.IdUser)
                       .IsRequired(required: true);

                builder.Property(f => f.IdFriendShip)
                       .IsRequired(required: true);

                builder.Property(f => f.Status)
                       .IsRequired(required: true)
                       .HasMaxLength(20);

                builder.Property(f => f.Date)
                       .IsRequired(required: true);

            });

            builder.Entity<ScoredTasks>(builder =>
            {
                builder.HasKey(s => s.Id);

                builder.HasOne<User>(s => s.User)
                       .WithMany(u => u.ScoredTasks)
                       .HasForeignKey(s => s.IdUser).OnDelete(DeleteBehavior.Cascade);

                builder.HasOne<MiTasks>(t => t.MiTasks)
                       .WithMany(m => m.ScoredTasks)
                       .HasForeignKey(t => t.IdTask).OnDelete(DeleteBehavior.NoAction);

                builder.Property(s => s.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(s => s.IdUser)
                       .IsRequired(required: true);

                builder.Property(s => s.IdTask)
                       .IsRequired(required: true);

                builder.Property(s => s.Status)
                       .IsRequired(required: true)
                       .HasMaxLength(20);


            });
            base.OnModelCreating(builder);

        }

    }
}
