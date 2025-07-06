using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Models;

namespace MyDotNetProject.Data
{
    public class BetBuddysDbContext : DbContext
    {
        public BetBuddysDbContext(DbContextOptions<BetBuddysDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<BetOption> BetOptions { get; set; }
        public DbSet<UserBet> UserBets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            
            // User relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Group relationships
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Members)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Events)
                .WithOne(e => e.Group)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event relationships
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Matches)
                .WithOne(m => m.Event)
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Match relationships
            modelBuilder.Entity<Match>()
                .HasMany(m => m.Predictions)
                .WithOne(p => p.Match)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Post relationships
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Reactions)
                .WithOne(r => r.Post)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bet relationships
            modelBuilder.Entity<Bet>()
                .HasMany(b => b.Options)
                .WithOne(bo => bo.Bet)
                .HasForeignKey(bo => bo.BetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BetOption>()
                .HasMany(bo => bo.UserBets)
                .WithOne(ub => ub.BetOption)
                .HasForeignKey(ub => ub.BetOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversation relationships
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Participant1)
                .WithMany()
                .HasForeignKey(c => c.Participant1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Participant2)
                .WithMany()
                .HasForeignKey(c => c.Participant2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for better performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<GroupMember>()
                .HasIndex(gm => new { gm.GroupId, gm.UserId })
                .IsUnique();

            modelBuilder.Entity<Prediction>()
                .HasIndex(p => new { p.EventId, p.MatchId, p.UserId })
                .IsUnique();

            modelBuilder.Entity<PostReaction>()
                .HasIndex(pr => new { pr.PostId, pr.UserId })
                .IsUnique();

            modelBuilder.Entity<Invitation>()
                .HasIndex(i => new { i.GroupId, i.UserEmail })
                .IsUnique();

            // Configure decimal precision for UserBet Amount
            modelBuilder.Entity<UserBet>()
                .Property(ub => ub.Amount)
                .HasPrecision(18, 2);
        }
    }
}
