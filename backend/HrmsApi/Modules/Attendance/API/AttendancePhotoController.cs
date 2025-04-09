using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Attendance.Domain;
using HrmsApi.Modules.Attendance.Application.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace HrmsApi.Modules.Attendance.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancePhotoController : ControllerBase
    {
        private readonly HrmsDbContext _context;
        private readonly string _uploadDirectory;

        public AttendancePhotoController(HrmsDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _uploadDirectory = Path.Combine(environment.ContentRootPath, "Uploads", "AttendancePhotos");

            // Ensure the upload directory exists
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        // POST: api/AttendancePhoto/Upload
        [HttpPost("Upload")]
        public async Task<ActionResult<AttendancePhotoDTO>> UploadPhoto([FromForm] UploadAttendancePhotoDTO uploadDto)
        {
            try
            {
                if (uploadDto.Photo == null || uploadDto.Photo.Length == 0)
                {
                    return BadRequest("No photo uploaded");
                }

                var attendance = await _context.Attendances.FindAsync(uploadDto.AttendanceId);
                if (attendance == null)
                {
                    return NotFound("Attendance record not found");
                }

                // Generate a unique filename
                string fileName = $"{Guid.NewGuid()}_{uploadDto.AttendanceId}_{(uploadDto.IsClockIn ? "in" : "out")}.jpg";
                string filePath = Path.Combine(_uploadDirectory, fileName);

                // Save the photo to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadDto.Photo.CopyToAsync(stream);
                }

                // Create a new attendance photo record
                var attendancePhoto = new AttendancePhoto
                {
                    Id = Guid.NewGuid(),
                    AttendanceId = uploadDto.AttendanceId,
                    IsClockIn = uploadDto.IsClockIn,
                    PhotoUrl = $"/api/AttendancePhoto/View/{fileName}",
                    StoragePath = filePath,
                    CaptureTime = DateTime.UtcNow,
                    DeviceInfo = uploadDto.DeviceInfo,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                _context.AttendancePhotos.Add(attendancePhoto);
                await _context.SaveChangesAsync();

                return Ok(new AttendancePhotoDTO
                {
                    Id = attendancePhoto.Id,
                    AttendanceId = attendancePhoto.AttendanceId,
                    IsClockIn = attendancePhoto.IsClockIn,
                    PhotoUrl = attendancePhoto.PhotoUrl,
                    CaptureTime = attendancePhoto.CaptureTime,
                    DeviceInfo = attendancePhoto.DeviceInfo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/AttendancePhoto/View/{fileName}
        [HttpGet("View/{fileName}")]
        public IActionResult ViewPhoto(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_uploadDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Photo not found");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/AttendancePhoto/Attendance/{attendanceId}
        [HttpGet("Attendance/{attendanceId}")]
        public async Task<ActionResult<AttendancePhotoDTO>> GetAttendancePhotos(Guid attendanceId)
        {
            try
            {
                var photos = await _context.AttendancePhotos
                    .Where(p => p.AttendanceId == attendanceId)
                    .ToListAsync();

                if (!photos.Any())
                {
                    return NotFound("No photos found for this attendance record");
                }

                var result = photos.Select(p => new AttendancePhotoDTO
                {
                    Id = p.Id,
                    AttendanceId = p.AttendanceId,
                    IsClockIn = p.IsClockIn,
                    PhotoUrl = p.PhotoUrl,
                    CaptureTime = p.CaptureTime,
                    DeviceInfo = p.DeviceInfo
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
