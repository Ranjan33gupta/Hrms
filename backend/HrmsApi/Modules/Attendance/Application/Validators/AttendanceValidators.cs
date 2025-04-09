using FluentValidation;
using HrmsApi.Modules.Attendance.Application.DTOs;
using System;

namespace HrmsApi.Modules.Attendance.Application.Validators
{
    public class ClockInDTOValidator : AbstractValidator<ClockInDTO>
    {
        public ClockInDTOValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");
                
            RuleFor(x => x.CheckInLatitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.CheckInLatitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90 degrees");
                
            RuleFor(x => x.CheckInLongitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.CheckInLongitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180 degrees");
        }
    }
    
    public class ClockOutDTOValidator : AbstractValidator<ClockOutDTO>
    {
        public ClockOutDTOValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");
                
            RuleFor(x => x.CheckOutLatitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.CheckOutLatitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90 degrees");
                
            RuleFor(x => x.CheckOutLongitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.CheckOutLongitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180 degrees");
        }
    }
    
    public class UploadAttendancePhotoDTOValidator : AbstractValidator<UploadAttendancePhotoDTO>
    {
        public UploadAttendancePhotoDTOValidator()
        {
            RuleFor(x => x.AttendanceId)
                .NotEmpty().WithMessage("Attendance ID is required");
                
            RuleFor(x => x.Photo)
                .NotNull().WithMessage("Photo is required");
                
            RuleFor(x => x.Photo.Length)
                .LessThanOrEqualTo(10 * 1024 * 1024) // 10MB max
                .When(x => x.Photo != null)
                .WithMessage("Photo size must be less than 10MB");
                
            RuleFor(x => x.Photo.ContentType)
                .Must(x => x == "image/jpeg" || x == "image/png" || x == "image/jpg")
                .When(x => x.Photo != null)
                .WithMessage("Only JPEG and PNG images are allowed");
        }
    }
}
