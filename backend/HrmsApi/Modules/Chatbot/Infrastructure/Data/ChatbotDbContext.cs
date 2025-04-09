using HrmsApi.Modules.Chatbot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.Modules.Chatbot.Infrastructure.Data
{
    public class ChatbotDbContext : DbContext
    {
        public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options) : base(options)
        {
        }

        public DbSet<ChatbotIntent> Intents { get; set; }
        public DbSet<ChatbotTrainingPhrase> TrainingPhrases { get; set; }
        public DbSet<ChatbotEntity> Entities { get; set; }
        public DbSet<ChatbotConversation> Conversations { get; set; }
        public DbSet<ChatbotMessage> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships
            modelBuilder.Entity<ChatbotIntent>(entity =>
            {
                entity.ToTable("ChatbotIntents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ResponseTemplate).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.ApiEndpoint).HasMaxLength(255);
                entity.Property(e => e.RouteDestination).HasMaxLength(255);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<ChatbotTrainingPhrase>(entity =>
            {
                entity.ToTable("ChatbotTrainingPhrases");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Phrase).IsRequired();
                
                entity.HasOne(e => e.Intent)
                    .WithMany(i => i.TrainingPhrases)
                    .HasForeignKey(e => e.IntentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatbotEntity>(entity =>
            {
                entity.ToTable("ChatbotEntities");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                
                entity.HasOne(e => e.Intent)
                    .WithMany(i => i.Entities)
                    .HasForeignKey(e => e.IntentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatbotConversation>(entity =>
            {
                entity.ToTable("ChatbotConversations");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ChatbotMessage>(entity =>
            {
                entity.ToTable("ChatbotMessages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                
                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
