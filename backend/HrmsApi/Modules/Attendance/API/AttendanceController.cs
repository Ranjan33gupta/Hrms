using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Attendance.Domain;
using HrmsApi.Modules.Attendance.Application.DTOs;
using HrmsApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrmsApi.Modules.Attendance.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public AttendanceController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/Attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetAttendance()
        {
            try
            {
                // Check if Shifts table exists
                bool shiftsExist = true;
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SELECT 1 FROM \"Shifts\" LIMIT 1");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Shifts table may not exist: {ex.Message}");
                    shiftsExist = false;
                }

                List<Domain.Attendance> attendances;
                if (shiftsExist)
                {
                    attendances = await _context.Attendances
                        .Include(a => a.Shift)
                        .ToListAsync();
                }
                else
                {
                    attendances = await _context.Attendances
                        .ToListAsync();
                }

                var employees = await _context.Employees.ToListAsync();
                var result = new List<AttendanceDTO>();

                foreach (var attendance in attendances)
                {
                    var dto = MapToDto(attendance, employees.FirstOrDefault(e => e.Id == attendance.EmployeeId)?.FullName ?? "Unknown");
                    result.Add(dto);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetAttendance: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Attendance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceDTO>> GetAttendance(Guid id)
        {
            try
            {
                // Check if Shifts table exists
                bool shiftsExist = true;
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SELECT 1 FROM \"Shifts\" LIMIT 1");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Shifts table may not exist: {ex.Message}");
                    shiftsExist = false;
                }

                Domain.Attendance? attendance;
                if (shiftsExist)
                {
                    attendance = await _context.Attendances
                        .Include(a => a.Shift)
                        .FirstOrDefaultAsync(a => a.Id == id);
                }
                else
                {
                    attendance = await _context.Attendances
                        .FirstOrDefaultAsync(a => a.Id == id);
                }

                if (attendance == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employees.FindAsync(attendance.EmployeeId);

                var dto = MapToDto(attendance, employee?.FullName ?? "Unknown");

                return dto;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetAttendance: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Attendance/Employee/5
        [HttpGet("Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetAttendanceByEmployee(Guid employeeId)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Use raw SQL to bypass the Status column issue
                var sql = $@"
                    SELECT a.""Id"", a.""EmployeeId"", a.""Date"", a.""ClockIn"", a.""ClockOut"",
                           a.""Notes"", a.""CreatedAt"", a.""UpdatedAt"", a.""CreatedBy"", a.""UpdatedBy""
                    FROM ""Attendances"" a
                    WHERE a.""EmployeeId"" = '{employeeId}'";

                var attendances = new List<Domain.Attendance>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;

                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var attendance = new Domain.Attendance
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                EmployeeId = reader.GetGuid(reader.GetOrdinal("EmployeeId")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ? reader.GetString(reader.GetOrdinal("Notes")) : null,
                                CreatedAt = !reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? reader.GetDateTime(reader.GetOrdinal("CreatedAt")) : DateTime.MinValue,
                                UpdatedAt = !reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? reader.GetDateTime(reader.GetOrdinal("UpdatedAt")) : (DateTime?)null,
                                CreatedBy = !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetString(reader.GetOrdinal("CreatedBy")) : null,
                                UpdatedBy = !reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? reader.GetString(reader.GetOrdinal("UpdatedBy")) : null
                            };

                            // Handle TimeSpan specifically for ClockIn and ClockOut
                            if (!reader.IsDBNull(reader.GetOrdinal("ClockIn")))
                            {
                                var clockIn = reader.GetValue(reader.GetOrdinal("ClockIn"));
                                if (clockIn is TimeSpan ts)
                                    attendance.ClockIn = ts;
                                else if (clockIn is DateTime dt)
                                    attendance.ClockIn = dt.TimeOfDay;
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("ClockOut")))
                            {
                                var clockOut = reader.GetValue(reader.GetOrdinal("ClockOut"));
                                if (clockOut is TimeSpan ts)
                                    attendance.ClockOut = ts;
                                else if (clockOut is DateTime dt)
                                    attendance.ClockOut = dt.TimeOfDay;
                            }

                            attendances.Add(attendance);
                        }
                    }
                }

                var result = new List<AttendanceDTO>();
                foreach (var attendance in attendances)
                {
                    var dto = MapToDto(attendance, employee.FullName);
                    result.Add(dto);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetAttendanceByEmployee: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Attendance/Employee/5/Date/2023-01-01
        [HttpGet("Employee/{employeeId}/Date/{date}")]
        public async Task<ActionResult<AttendanceDTO>> GetAttendanceByEmployeeAndDate(Guid employeeId, DateTime date)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Convert to UTC date for PostgreSQL compatibility
                var utcDate = date.ToUniversalTime().Date;

                var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == utcDate);

                if (attendance == null)
                {
                    return NotFound("No attendance record found for this date");
                }

                var dto = MapToDto(attendance, employee.FullName);

                return dto;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting attendance by date: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Attendance/Employee/5/Today
        [HttpGet("Employee/{employeeId}/Today")]
        public async Task<ActionResult<AttendanceDTO>> GetTodayAttendance(Guid employeeId)
        {
            try
            {
                // Use UTC date for PostgreSQL compatibility
                var today = DateTime.UtcNow.Date;
                return await GetAttendanceByEmployeeAndDate(employeeId, today);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting today's attendance: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Attendance/ClockIn
        [HttpPost("ClockIn")]
        public async Task<ActionResult<AttendanceDTO>> ClockIn(ClockInDTO clockInDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var employee = await _context.Employees.FindAsync(clockInDto.EmployeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                var now = DateTime.UtcNow;
                var today = now.Date;

                // Check if employee has already clocked in today
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == clockInDto.EmployeeId && a.Date.Date == today);

                if (existingAttendance != null)
                {
                    if (existingAttendance.ClockIn != null)
                    {
                        return BadRequest("Employee has already clocked in today");
                    }

                    // Update existing attendance record
                    existingAttendance.ClockIn = now.TimeOfDay;
                    existingAttendance.Status = AttendanceStatus.Present;

                    // Location information
                    string locationNotes = string.Empty;
                    if (!string.IsNullOrEmpty(clockInDto.CheckInLocation))
                    {
                        locationNotes = $"Location: {clockInDto.CheckInLocation}";
                        existingAttendance.CheckInLocation = clockInDto.CheckInLocation;
                    }

                    if (clockInDto.CheckInLatitude.HasValue && clockInDto.CheckInLongitude.HasValue)
                    {
                        existingAttendance.CheckInLatitude = clockInDto.CheckInLatitude;
                        existingAttendance.CheckInLongitude = clockInDto.CheckInLongitude;
                    }

                    if (!string.IsNullOrEmpty(clockInDto.CheckInDevice))
                    {
                        existingAttendance.CheckInDevice = clockInDto.CheckInDevice;
                    }

                    if (!string.IsNullOrEmpty(clockInDto.CheckInIpAddress))
                    {
                        existingAttendance.CheckInIpAddress = clockInDto.CheckInIpAddress;
                    }

                    string originalNotes = string.IsNullOrEmpty(existingAttendance.Notes) ? string.Empty : existingAttendance.Notes;
                    string combinedNotes = string.IsNullOrEmpty(locationNotes)
                        ? originalNotes
                        : (string.IsNullOrEmpty(originalNotes) ? locationNotes : $"{originalNotes} | {locationNotes}");

                    existingAttendance.Notes = combinedNotes;
                    existingAttendance.UpdatedAt = DateTime.UtcNow;
                    existingAttendance.UpdatedBy = "System";

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AttendanceExists(existingAttendance.Id))
                        {
                            return NotFound();
                        }
                        throw;
                    }

                    var dto = MapToDto(existingAttendance, employee.FullName);

                    return dto;
                }

                // Create new attendance record
                string newLocationNotes = !string.IsNullOrEmpty(clockInDto.CheckInLocation)
                    ? $"Location: {clockInDto.CheckInLocation}"
                    : string.Empty;

                var attendance = new Domain.Attendance
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = clockInDto.EmployeeId,
                    Date = today,
                    ClockIn = now.TimeOfDay,
                    Notes = newLocationNotes,
                    Status = AttendanceStatus.Present,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                // Set location data if available
                if (!string.IsNullOrEmpty(clockInDto.CheckInLocation))
                {
                    attendance.CheckInLocation = clockInDto.CheckInLocation;
                }

                if (clockInDto.CheckInLatitude.HasValue && clockInDto.CheckInLongitude.HasValue)
                {
                    attendance.CheckInLatitude = clockInDto.CheckInLatitude;
                    attendance.CheckInLongitude = clockInDto.CheckInLongitude;
                }

                if (!string.IsNullOrEmpty(clockInDto.CheckInDevice))
                {
                    attendance.CheckInDevice = clockInDto.CheckInDevice;
                }

                if (!string.IsNullOrEmpty(clockInDto.CheckInIpAddress))
                {
                    attendance.CheckInIpAddress = clockInDto.CheckInIpAddress;
                }

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                var attendanceDto = MapToDto(attendance, employee.FullName);

                return CreatedAtAction("GetAttendance", new { id = attendance.Id }, attendanceDto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during clock-in: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error during clock-in: {ex.Message}");
            }
        }

        // PUT: api/Attendance/ClockOut
        [HttpPut("ClockOut")]
        public async Task<IActionResult> ClockOut(ClockOutDTO clockOutDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var employee = await _context.Employees.FindAsync(clockOutDto.EmployeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                var today = DateTime.UtcNow.Date;
                var now = DateTime.UtcNow;

                // Ensure we're using UTC for the query
                var attendance = await _context.Attendances
                    .AsNoTracking()  // Use AsNoTracking to avoid EF Core tracking issues with DateTime kinds
                    .FirstOrDefaultAsync(a => a.EmployeeId == clockOutDto.EmployeeId && a.Date.Date == today);

                if (attendance == null)
                {
                    return NotFound("No clock-in record found for today. Please clock in first.");
                }

                if (attendance.ClockOut != null)
                {
                    return BadRequest("Employee has already clocked out today");
                }

                // Create a new instance to avoid tracking issues
                var updatedAttendance = new Domain.Attendance
                {
                    Id = attendance.Id,
                    EmployeeId = attendance.EmployeeId,
                    Date = attendance.Date,
                    ClockIn = attendance.ClockIn,
                    ClockOut = now.TimeOfDay,
                    CheckInLocation = attendance.CheckInLocation,
                    CheckInDevice = attendance.CheckInDevice,
                    CheckInIpAddress = attendance.CheckInIpAddress,
                    CheckInLatitude = attendance.CheckInLatitude,
                    CheckInLongitude = attendance.CheckInLongitude,
                    ShiftId = attendance.ShiftId,
                    Status = attendance.Status,
                    Notes = attendance.Notes,
                    CreatedAt = attendance.CreatedAt,
                    CreatedBy = attendance.CreatedBy
                };
                
                // Handle location data
                string locationNotes = string.Empty;
                if (!string.IsNullOrEmpty(clockOutDto.CheckOutLocation))
                {
                    locationNotes = $"Clock-out Location: {clockOutDto.CheckOutLocation}";
                    updatedAttendance.CheckOutLocation = clockOutDto.CheckOutLocation;
                }
                
                if (clockOutDto.CheckOutLatitude.HasValue && clockOutDto.CheckOutLongitude.HasValue)
                {
                    updatedAttendance.CheckOutLatitude = clockOutDto.CheckOutLatitude;
                    updatedAttendance.CheckOutLongitude = clockOutDto.CheckOutLongitude;
                }
                
                if (!string.IsNullOrEmpty(clockOutDto.CheckOutDevice))
                {
                    updatedAttendance.CheckOutDevice = clockOutDto.CheckOutDevice;
                }
                
                if (!string.IsNullOrEmpty(clockOutDto.CheckOutIpAddress))
                {
                    updatedAttendance.CheckOutIpAddress = clockOutDto.CheckOutIpAddress;
                }
                
                // Update notes
                string existingNotes = string.IsNullOrEmpty(updatedAttendance.Notes) ? string.Empty : updatedAttendance.Notes;
                string combinedNotes = string.IsNullOrEmpty(locationNotes) 
                    ? existingNotes 
                    : (string.IsNullOrEmpty(existingNotes) ? locationNotes : $"{existingNotes} | {locationNotes}");
                
                updatedAttendance.Notes = combinedNotes;
                updatedAttendance.UpdatedAt = DateTime.UtcNow;
                updatedAttendance.UpdatedBy = "System";

                // Use Update method instead of tracking changes
                _context.Attendances.Update(updatedAttendance);
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(updatedAttendance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var dto = MapToDto(updatedAttendance, employee.FullName);
                
                return Ok(dto);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.Error.WriteLine($"Clock-out error: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error during clock-out: {ex.Message}");
            }
        }

        // DELETE: api/Attendance/5
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(Guid id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }

        private AttendanceDTO MapToDto(Domain.Attendance attendance, string employeeName)
        {
            // Calculate hours worked if clock out is available
            double hoursWorked = 0;
            if (attendance.ClockOut.HasValue)
            {
                // Calculate the time difference in hours
                TimeSpan duration = attendance.ClockOut.Value - attendance.ClockIn;
                hoursWorked = duration.TotalHours;
            }

            // Parse location data from notes if available
            string? checkInLocation = attendance.CheckInLocation;
            string? checkOutLocation = attendance.CheckOutLocation;

            if (string.IsNullOrEmpty(checkInLocation) && !string.IsNullOrEmpty(attendance.Notes))
            {
                // Try to extract location information from notes
                var notes = attendance.Notes;

                if (notes.Contains("Location:"))
                {
                    var locationStart = notes.IndexOf("Location:") + "Location:".Length;
                    var locationEnd = notes.IndexOf("|", locationStart);
                    if (locationEnd == -1) locationEnd = notes.Length;
                    checkInLocation = notes.Substring(locationStart, locationEnd - locationStart).Trim();
                }
            }

            if (string.IsNullOrEmpty(checkOutLocation) && !string.IsNullOrEmpty(attendance.Notes))
            {
                var notes = attendance.Notes;

                if (notes.Contains("Clock-out Location:"))
                {
                    var locationStart = notes.IndexOf("Clock-out Location:") + "Clock-out Location:".Length;
                    var locationEnd = notes.IndexOf("|", locationStart);
                    if (locationEnd == -1) locationEnd = notes.Length;
                    checkOutLocation = notes.Substring(locationStart, locationEnd - locationStart).Trim();
                }
            }

            return new AttendanceDTO
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                EmployeeName = employeeName,
                Date = attendance.Date,
                ClockIn = attendance.ClockIn,
                ClockOut = attendance.ClockOut,
                Status = attendance.Status,
                Notes = attendance.Notes,
                HoursWorked = hoursWorked,

                // Extracted location data
                CheckInLocation = checkInLocation ?? string.Empty,
                CheckOutLocation = checkOutLocation ?? string.Empty,

                CreatedAt = attendance.CreatedAt,
                UpdatedAt = attendance.UpdatedAt
            };
        }
    }
}
