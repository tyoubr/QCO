using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class QCOContext : DbContext
{
    public QCOContext()
    {
    }

    public QCOContext(DbContextOptions<QCOContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
    public virtual DbSet<TblLayoutMonitoringSheet> TblLayoutMonitoringSheets { get; set; }
    public virtual DbSet<TblLayoutMonitoringSheetD> TblLayoutMonitoringSheetDs { get; set; }
    public virtual DbSet<tbl_Cause_Of_Delay> tbl_Cause_Of_Delays { get; set; }
    public virtual DbSet<tbl_Cause_Detail> tbl_Cause_Details { get; set; }
    public DbSet<TblCauseLookup> tbl_Cause_Lookups { get; set; }

    //RANA
    public virtual DbSet<TblCadConsD> TblCadConsDs { get; set; }
    public virtual DbSet<TblCadConsM> TblCadConsMs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect sensitive information, use a configuration file for your connection string.
        => optionsBuilder.UseSqlServer("server=103.9.134.216;Initial Catalog=QCO;User ID=sa;Password=TKL@007#;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AspNetRole entity configuration
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        // AspNetRoleClaim entity configuration
        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");
            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        // AspNetUser entity configuration
        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        // AspNetUserClaim entity configuration
        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        // AspNetUserLogin entity configuration
        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        // AspNetUserToken entity configuration
        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        // TblLayoutMonitoringSheet entity configuration
        // TblLayoutMonitoringSheet entity configuration
        modelBuilder.Entity<TblLayoutMonitoringSheet>(entity =>
        {
            entity.HasKey(e => e.Slno);
            entity.ToTable("tbl_LayoutMonitoringSheet");
            entity.Property(e => e.Slno).HasColumnName("SLNO");
            entity.Property(e => e.Company)
           .HasMaxLength(50)
           .IsUnicode(false)
           .HasColumnName("COMPANY");
            entity.Property(e => e.BookingNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BOOKING_NO");
            entity.Property(e => e.BuyerName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BUYER_NAME");
            entity.Property(e => e.FeedFinish).HasColumnType("datetime").HasColumnName("FEED_FINISH");
            entity.Property(e => e.FeedStart).HasColumnType("datetime").HasColumnName("FEED_START");
            entity.Property(e => e.LineNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LINE_NO");
            entity.Property(e => e.MonitoringDate).HasColumnType("datetime").HasColumnName("MONITORING_DATE");
            entity.Property(e => e.PreStyleFinish).HasColumnType("datetime").HasColumnName("PRE_STYLE_FINISH");
            entity.Property(e => e.NewStyleFinish).HasColumnType("datetime").HasColumnName("NEW_STYLE_FINISH");
            entity.Property(e => e.Total_SMV).HasColumnType("float").HasColumnName("TOTAL_SMV");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATUS");
        });

        // TblLayoutMonitoringSheetD entity configuration
        modelBuilder.Entity<TblLayoutMonitoringSheetD>(entity =>
        {
            // Primary Key
            entity.HasKey(e => e.Trnsid);

            // Table Name Mapping
            entity.ToTable("tbl_LayoutMonitoringSheet_D");

            // Column Mappings
            entity.Property(e => e.Trnsid).HasColumnName("TRNSID");
            entity.Property(e => e.Slno).HasColumnName("SLNO");
            entity.Property(e => e.SequenceNo).HasColumnName("SEQUENCE_NO");  // Correct mapping for SequenceNo
            entity.Property(e => e.ProcessName).HasColumnName("PROCESS_NAME");
            entity.Property(e => e.ResourceName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RESOURCE_NAME");
            entity.Property(e => e.McSetupStart).HasColumnName("MC_SETUP_START");
            entity.Property(e => e.McSetupFinish).HasColumnName("MC_SETUP_FINISH");
            entity.Property(e => e.McSetupDuration)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("MC_SETUP_DURATION");
            entity.Property(e => e.Remarks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REMARKS");

            // Relationship Configuration with TblLayoutMonitoringSheet (assuming a foreign key relation exists)
            entity.HasOne(d => d.LayoutMonitoringSheet)
                .WithMany(p => p.DetailRecords)
                .HasForeignKey(d => d.Slno)
                .HasConstraintName("FK_TblLayoutMonitoringSheet_Slno");
        });

        modelBuilder.Entity<tbl_Cause_Of_Delay>(entity =>
        {
            entity.HasKey(e => e.CAUSEID);

            entity.ToTable("tbl_Cause_Of_Delay");

            entity.Property(e => e.CAUSE_OF_DELAY)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.REMARKS)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<tbl_Cause_Detail>(entity =>
        {
            entity.HasKey(e => e.DID);
            entity.ToTable("tbl_Cause_Details");

            entity.Property(e => e.REMARKS).IsUnicode(false);

            // Explicitly map TRNSID as a foreign key to avoid incorrect naming
            entity.Property(e => e.TRNSID).HasColumnName("TRNSID");

            // Define the foreign key relationship
            entity.HasOne<TblLayoutMonitoringSheetD>()
                  .WithMany(d => d.SavedCauses)  // Ensure this is the right navigation property
                  .HasForeignKey(e => e.TRNSID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        //RANA
        modelBuilder.Entity<TblCadConsD>(entity =>
        {
            entity.HasKey(e => e.Caddid);

            entity.ToTable("TBL_CAD_CONS_D");

            entity.Property(e => e.Caddid).HasColumnName("CADDID");
            entity.Property(e => e.Cadmid).HasColumnName("CADMID");
            entity.Property(e => e.Comments)
                .IsUnicode(false)
                .HasColumnName("COMMENTS");
            entity.Property(e => e.Consdzn).HasColumnName("CONSDZN");
            entity.Property(e => e.Conspcs).HasColumnName("CONSPCS");
            entity.Property(e => e.Contenttype)
                .HasMaxLength(50)
                .HasColumnName("CONTENTTYPE");
            entity.Property(e => e.Cutwidth).HasColumnName("CUTWIDTH");
            entity.Property(e => e.Efficiency).HasColumnName("EFFICIENCY");
            entity.Property(e => e.Fabricdes)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FABRICDES");
            entity.Property(e => e.Fabricusage)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FABRICUSAGE");
            entity.Property(e => e.Filename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FILENAME");
            entity.Property(e => e.Filepath)
                .HasMaxLength(50)
                .HasColumnName("FILEPATH");
            entity.Property(e => e.Filesize).HasColumnName("FILESIZE");
            entity.Property(e => e.Fullwidth).HasColumnName("FULLWIDTH");
            entity.Property(e => e.Gmntcolor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GMNTCOLOR");
            entity.Property(e => e.Gmntitem)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GMNTITEM");
            entity.Property(e => e.Gsm).HasColumnName("GSM");
            entity.Property(e => e.Markerqty).HasColumnName("MARKERQTY");
            entity.Property(e => e.Opt01)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OPT01");
            entity.Property(e => e.Opt02)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OPT02");
            entity.Property(e => e.Opt03)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OPT03");
            entity.Property(e => e.Ptnnmbr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PTNNMBR");
            entity.Property(e => e.Shrinkagel).HasColumnName("SHRINKAGEL");
            entity.Property(e => e.Shrinkagew).HasColumnName("SHRINKAGEW");
            entity.Property(e => e.Sizeratio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SIZERATIO");
            entity.Property(e => e.Transdate)
                .HasColumnType("datetime")
                .HasColumnName("TRANSDATE");
            entity.Property(e => e.Wastage).HasColumnName("WASTAGE");

            entity.HasOne(d => d.Cadm).WithMany(p => p.TblCadConsDs)
                .HasForeignKey(d => d.Cadmid)
                .HasConstraintName("FK_TBL_CAD_CONS_D_TBL_CAD_CONS_D");
        });

        modelBuilder.Entity<TblCadConsM>(entity =>
        {
            entity.HasKey(e => e.Cadmid);

            entity.ToTable("TBL_CAD_CONS_M");

            entity.Property(e => e.Cadmid).HasColumnName("CADMID");
            entity.Property(e => e.Brand)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BRAND");
            entity.Property(e => e.Buyer)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BUYER");
            entity.Property(e => e.Caddate)
                .HasColumnType("datetime")
                .HasColumnName("CADDATE");
            entity.Property(e => e.Comments)
                .IsUnicode(false)
                .HasColumnName("COMMENTS");
            entity.Property(e => e.Consfor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONSFOR");
            entity.Property(e => e.Ir)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IR");
            entity.Property(e => e.Isapproved).HasColumnName("ISAPPROVED");
            entity.Property(e => e.Job)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("JOB");
            entity.Property(e => e.Opt01)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OPT01");
            entity.Property(e => e.Opt02)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OPT02");
            entity.Property(e => e.Opt03)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OPT03");
            entity.Property(e => e.Patternmaster)
                .HasMaxLength(50)
                .HasColumnName("PATTERNMASTER");
            entity.Property(e => e.Season)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SEASON");
            entity.Property(e => e.Seasonyear)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SEASONYEAR");
            entity.Property(e => e.Style)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STYLE");
            entity.Property(e => e.Styledes)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STYLEDES");
            entity.Property(e => e.Styleref)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STYLEREF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
