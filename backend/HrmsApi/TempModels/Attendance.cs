using System;
using System.Collections.Generic;

namespace HrmsApi.TempModels;

public partial class Attendance
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateTime Date { get; set; }

    public DateTime ClockIn { get; set; }

    public DateTime? ClockOut { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
