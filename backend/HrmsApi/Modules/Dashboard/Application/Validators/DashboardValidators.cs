using FluentValidation;
using HrmsApi.Modules.Dashboard.Application.DTOs;
using HrmsApi.Modules.Dashboard.Domain;
using System;
using System.Linq;

namespace HrmsApi.Modules.Dashboard.Application.Validators
{
    public class ChatbotQueryDTOValidator : AbstractValidator<ChatbotQueryDTO>
    {
        public ChatbotQueryDTOValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message cannot be empty")
                .MaximumLength(500).WithMessage("Message cannot exceed 500 characters");
                
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");
                
            RuleFor(x => x.Language)
                .Must(x => string.IsNullOrEmpty(x) || x == "en" || x == "fr" || x == "es")
                .WithMessage("Supported languages are: en, fr, es");
        }
    }
    
    public class CreateMoodEntryDTOValidator : AbstractValidator<CreateMoodEntryDTO>
    {
        public CreateMoodEntryDTOValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");
                
            RuleFor(x => x.Mood)
                .IsInEnum().WithMessage("Invalid mood value");
                
            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Comment));
        }
    }
    
    public class CreateIntentDTOValidator : AbstractValidator<CreateIntentDTO>
    {
        public CreateIntentDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Intent name is required")
                .MaximumLength(100).WithMessage("Intent name cannot exceed 100 characters")
                .Matches("^[a-z0-9_]+$").WithMessage("Intent name can only contain lowercase letters, numbers, and underscores");
                
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(255).WithMessage("Description cannot exceed 255 characters");
                
            RuleFor(x => x.TrainingPhrases)
                .NotEmpty().WithMessage("At least one training phrase is required");
                
            RuleForEach(x => x.TrainingPhrases)
                .NotEmpty().WithMessage("Training phrase cannot be empty")
                .MaximumLength(200).WithMessage("Training phrase cannot exceed 200 characters");
                
            RuleFor(x => x.ResponseTemplate)
                .NotEmpty().WithMessage("Response template is required");
                
            RuleForEach(x => x.Entities)
                .SetValidator(new EntityDTOValidator());
        }
    }
    
    public class EntityDTOValidator : AbstractValidator<EntityDTO>
    {
        private readonly string[] validTypes = new[] { "text", "number", "date", "email", "phone", "url", "boolean" };
        
        public EntityDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Entity name is required")
                .MaximumLength(100).WithMessage("Entity name cannot exceed 100 characters")
                .Matches("^[a-z0-9_]+$").WithMessage("Entity name can only contain lowercase letters, numbers, and underscores");
                
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Entity type is required")
                .Must(type => validTypes.Contains(type.ToLower()))
                .WithMessage($"Entity type must be one of: {string.Join(", ", validTypes)}");
        }
    }
    
    public class CreateQuoteDTOValidator : AbstractValidator<CreateQuoteDTO>
    {
        public CreateQuoteDTOValidator()
        {
            RuleFor(x => x.QuoteText)
                .NotEmpty().WithMessage("Quote text is required")
                .MaximumLength(500).WithMessage("Quote text cannot exceed 500 characters");
                
            RuleFor(x => x.Author)
                .MaximumLength(255).WithMessage("Author name cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Author));
                
            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Category));
        }
    }
}
