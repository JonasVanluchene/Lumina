using Lumina.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Lumina.Repository
{
    public class LuminaDbContext : IdentityDbContext<User>
    {
        public LuminaDbContext(DbContextOptions<LuminaDbContext> options) : base(options)
        {
        }

        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryEmotion> JournalEntryEmotions { get; set; }
        public DbSet<JournalEntryActivity> JournalEntryActivities { get; set; }
        public DbSet<JournalEntryTag> JournalEntryTags { get; set; }
        public DbSet<JournalEntryAttachment> JournalEntryAttachments { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserJourney> UserJourneys { get; set; }
        public DbSet<ReflectionJourney> ReflectionJourneys { get; set; }
        public DbSet<JourneyPrompt> JourneyPrompts { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalProgress> GoalProgresses { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<Emotion> Emotions { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasOne(u => u.Preferences)
                .WithOne(p => p.User)
                .HasForeignKey<UserPreferences>(p => p.UserId);


            // Add further configuration as needed
        }
    }
}
