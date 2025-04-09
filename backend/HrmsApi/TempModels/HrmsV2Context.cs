using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.TempModels;

public partial class HrmsV2Context : DbContext
{
    public HrmsV2Context()
    {
    }

    public HrmsV2Context(DbContextOptions<HrmsV2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
