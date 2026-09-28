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
    public virtual DbSet<TblSewingEfficiency> TblSewingEfficiency { get; set; }
    public DbSet<FloorDropdown> FloorDropdown { get; set; }
    public DbSet<SewingEfficiencyDetailModel> SewingEfficiencyDetail { get; set; }
    public virtual DbSet<TblInderectManpower> TblInderectManpower { get; set; }
    public virtual DbSet<TblRecapeInfo> TblRecapeInfo { get; set; }
    public virtual DbSet<TblRecapDetails> TblRecapDetails { get; set; }
    public virtual DbSet<TblRecapMaster> TblRecapMasters { get; set; }
    public virtual DbSet<TblRecapItemDetails> TblRecapItemDetails { get; set; }
    public DbSet<TblCadConsFiles> TblCadConsFiles { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect sensitive information, use a configuration file for your connection string.
        => optionsBuilder.UseSqlServer("server=103.9.134.216;Initial Catalog=QCO;User ID=sa;Password=TKL@007#;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Floor DropDown
        modelBuilder.Entity<FloorDropdown>().HasNoKey();
        //SewingEfficiencyDetailModel
        modelBuilder.Entity<SewingEfficiencyDetailModel>().HasNoKey();

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
            entity.Property(e => e.Cutwidth).HasColumnName("CUTWIDTH");
            entity.Property(e => e.Efficiency).HasColumnName("EFFICIENCY");
            entity.Property(e => e.Fabricdes)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("FABRICDES");
            entity.Property(e => e.Fabricusage)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("FABRICUSAGE");
            entity.Property(e => e.Fullwidth).HasColumnName("FULLWIDTH");
            entity.Property(e => e.Gmntcolor)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("GMNTCOLOR");
            entity.Property(e => e.Gmntitem)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("GMNTITEM");
            entity.Property(e => e.Gsm).HasColumnName("GSM");
            entity.Property(e => e.Markerqty).HasColumnName("MARKERQTY");
            entity.Property(e => e.Opt01).HasColumnName("Opt01");//for marker length
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
            entity.HasMany(d => d.TblCadConsFiles)
                .WithOne(f => f.CadDetail)
                .HasForeignKey(f => f.Caddid)
                .HasConstraintName("FK_TBL_CAD_CONS_FILES_TBL_CAD_CONS_D");
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
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("UPDATEDAT");
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
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UPDATEDBY");
        });

        //TblSewingEfficiency
        modelBuilder.Entity<TblSewingEfficiency>(entity =>
        {
            entity.HasKey(e => e.Trid);

            entity.ToTable("TBL_SEWING_EFFICIENCY");

            entity.Property(e => e.Trid).HasColumnName("TRID");
            entity.Property(e => e.AfterWashProduction).HasColumnName("AFTER_WASH_PRODUCTION");
            entity.Property(e => e.BookingNo)
                .HasMaxLength(50)
                .HasColumnName("BOOKING_NO");
            entity.Property(e => e.Company)
                .HasMaxLength(50)
                .HasColumnName("COMPANY");
            entity.Property(e => e.EfficiencyPercent).HasColumnName("EFFICIENCY_PERCENT");
            entity.Property(e => e.EightPm).HasColumnName("EIGHT_PM");
            entity.Property(e => e.ElevenPm).HasColumnName("ELEVEN_PM");
            entity.Property(e => e.Floor).HasColumnName("FLOOR");
            entity.Property(e => e.GenWorkingHr).HasColumnName("GEN_WORKING_HR");
            entity.Property(e => e.Helper).HasColumnName("HELPER");
            entity.Property(e => e.InderectMan).HasColumnName("INDERECT_MAN");
            entity.Property(e => e.Item)
                .HasMaxLength(50)
                .HasColumnName("ITEM");
            entity.Property(e => e.LineNo)
                .HasMaxLength(50)
                .HasColumnName("LINE_NO");
            entity.Property(e => e.LineTarget).HasColumnName("LINE_TARGET");
            entity.Property(e => e.ManPower).HasColumnName("MAN_POWER");
            entity.Property(e => e.NinePm).HasColumnName("NINE_PM");
            entity.Property(e => e.Operator).HasColumnName("OPERATOR");
            entity.Property(e => e.SevenPm).HasColumnName("SEVEN_PM");
            entity.Property(e => e.SixPm).HasColumnName("SIX_PM");
            entity.Property(e => e.Smv).HasColumnName("SMV");
            entity.Property(e => e.Style)
                .HasMaxLength(50)
                .HasColumnName("STYLE");
            entity.Property(e => e.StyleTarget).HasColumnName("STYLE_TARGET");
            entity.Property(e => e.SwDate)
                .HasColumnType("datetime")
                .HasColumnName("SW_DATE");
            entity.Property(e => e.TenPm).HasColumnName("TEN_PM");
            entity.Property(e => e.Trdate)
                .HasColumnType("datetime")
                .HasColumnName("TRDATE");
            entity.Property(e => e.TwelveAm).HasColumnName("TWELVE_AM");
        });
        //TblInderectManpower
        modelBuilder.Entity<TblInderectManpower>(entity =>
        {
            entity.HasKey(e => e.Impid);

            entity.ToTable("TBL_INDERECT_MANPOWER");

            entity.Property(e => e.Impid).HasColumnName("IMPID");
            entity.Property(e => e.AutoElasticMake).HasColumnName("AUTO_ELASTIC_MAKE");
            entity.Property(e => e.MedicalLeave).HasColumnName("MEDICAL_LEAVE");
            entity.Property(e => e.Others).HasColumnName("OTHERS");
            entity.Property(e => e.Pregnent).HasColumnName("PREGNENT");
            entity.Property(e => e.Remarks)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("REMARKS");
            entity.Property(e => e.Repoter).HasColumnName("REPOTER");
            entity.Property(e => e.SizeSetSample).HasColumnName("SIZE_SET_SAMPLE");
            entity.Property(e => e.Total).HasColumnName("TOTAL");
            entity.Property(e => e.TraineeSupervisor).HasColumnName("TRAINEE_SUPERVISOR");
            entity.Property(e => e.Trid).HasColumnName("TRID");

            entity.HasOne(d => d.Tr).WithMany(p => p.TblInderectManpowers)
                .HasForeignKey(d => d.Trid)
                .HasConstraintName("FK_TBL_INDERECT_MANPOWER_TBL_SEWING_EFFICIENCY");
        });

        modelBuilder.Entity<TblRecapeInfo>(entity =>
        {
            entity.HasKey(e => e.Trid);
            entity.ToTable("TBL_RECAPE_INFO");
            entity.Property(e => e.Trid).ValueGeneratedOnAdd().HasColumnName("TRID");
            entity.Property(e => e.BuyerName).HasMaxLength(50).HasColumnName("BUYER_NAME");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("CREATED_AT");
            entity.Property(e => e.CreatedBy).HasMaxLength(50).HasColumnName("CREATED_BY");
            entity.Property(e => e.E1).HasMaxLength(50);
            entity.Property(e => e.E2).HasMaxLength(50);
            entity.Property(e => e.E3).HasMaxLength(50);
            entity.Property(e => e.E4).HasMaxLength(50);
            entity.Property(e => e.E5).HasMaxLength(50);
            entity.Property(e => e.OfferdQty).HasColumnName("OFFERD_QTY");
            entity.Property(e => e.QuotedPrice).HasColumnName("QUOTED_PRICE");
            entity.Property(e => e.SeasonName).HasMaxLength(50).HasColumnName("SEASON_NAME");
            entity.Property(e => e.SeasonYear).HasColumnName("SEASON_YEAR");
            entity.Property(e => e.StyleRef).HasMaxLength(50).HasColumnName("STYLE_REF");
            entity.Property(e => e.TeamLeaderName).HasMaxLength(50).HasColumnName("TEAM_LEADER_NAME");
            entity.Property(e => e.TgtPrice).HasColumnName("TGT_PRICE");
            entity.Property(e => e.Trdate).HasColumnType("datetime").HasColumnName("TRDATE");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("UPDATED_AT");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50).HasColumnName("UPDATED_BY");
        });
        modelBuilder.Entity<TblRecapDetails>(entity =>
        {
            entity.HasKey(e => e.Rcdid);

            entity.ToTable("TBL_RECAP_DETAILS");

            entity.Property(e => e.Rcdid).HasColumnName("RCDID");
            entity.Property(e => e.ConsPerUnit).HasColumnName("CONS_PER_UNIT");
            entity.Property(e => e.Fabrication).HasMaxLength(250).HasColumnName("FABRICATION");
            entity.Property(e => e.Gsm).HasColumnName("GSM");
            entity.Property(e => e.ItemName).HasMaxLength(50).HasColumnName("ITEM_NAME");
            entity.Property(e => e.BodyPart).HasMaxLength(50).HasColumnName("BODY_PART");
            entity.Property(e => e.ColorName).HasMaxLength(50).HasColumnName("COLOR_NAME");
            entity.Property(e => e.Rcmid).HasColumnName("RCMID");
            entity.Property(e => e.TotalQty).HasColumnName("TOTAL_QTY");
            entity.HasOne(d => d.Rcm).WithMany(p => p.TblRecapDetails).HasForeignKey(d => d.Rcmid)
                .HasConstraintName("FK_TBL_RECAP_DETAILS_TBL_RECAP_MASTER");
        });

        modelBuilder.Entity<TblRecapMaster>(entity =>
        {
            entity.HasKey(e => e.Rcmid);
            entity.ToTable("TBL_RECAP_MASTER");
            entity.Property(e => e.Rcmid).HasColumnName("RCMID");
            entity.Property(e => e.ActShipmentDate).HasColumnName("ACT_SHIPMENT_DATE");
            entity.Property(e => e.BookingNo).HasMaxLength(50).HasColumnName("BOOKING_NO");
            entity.Property(e => e.BuyerName).HasMaxLength(50).HasColumnName("BUYER_NAME");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("CREATED_AT");
            entity.Property(e => e.CreatedBy).HasMaxLength(50).HasColumnName("CREATED_BY");
            entity.Property(e => e.FacShipmentDate).HasColumnName("FAC_SHIPMENT_DATE");
            entity.Property(e => e.Photo).HasColumnName("PHOTO");
            entity.Property(e => e.PhotoContentType).HasMaxLength(50).HasColumnName("PHOTO_CONTENT_TYPE");
            entity.Property(e => e.PoNo).HasMaxLength(50).HasColumnName("PO_NO");
            entity.Property(e => e.TeamLeaderName).HasMaxLength(50).HasColumnName("TEAM_LEADER_NAME");
            entity.Property(e => e.RecapMonth).HasMaxLength(50).HasColumnName("RECAP_MONTH");
            entity.Property(e => e.RecapeYear).HasColumnName("RECAPE_YEAR");
            entity.Property(e => e.SewingFactory).HasMaxLength(50).HasColumnName("SEWING_FACTORY");
            entity.Property(e => e.DyeingFactory).HasMaxLength(50).HasColumnName("DYEING_FACTORY");
            entity.Property(e => e.StyleName).HasMaxLength(50).HasColumnName("STYLE_NAME");
            entity.Property(e => e.Remarks).HasMaxLength(50).HasColumnName("REMARKS");
            entity.Property(e => e.SubmissionDate).HasColumnName("SUBMISSION_DATE");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("UPDATED_AT");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50).HasColumnName("UPDATED_BY");
        });

        modelBuilder.Entity<TblRecapItemDetails>(entity =>
        {
            entity.HasKey(e => e.Itemid);
            entity.ToTable("TBL_RECAP_ITEM_DETAILS");
            entity.Property(e => e.Itemid).HasColumnName("ITEMID");
            entity.Property(e => e.IsEmb).HasColumnName("IS_EMB");
            entity.Property(e => e.IsPrint).HasColumnName("IS_PRINT");
            entity.Property(e => e.IsWash).HasColumnName("IS_WASH");
            entity.Property(e => e.ItemName).HasMaxLength(50).HasColumnName("ITEM_NAME");
            entity.Property(e => e.Remarks).HasMaxLength(100).HasColumnName("REMARKS");
            entity.Property(e => e.OfferedQty).HasColumnName("OFFERED_QTY");
            entity.Property(e => e.QuoatedPrice).HasColumnName("QUOATED_PRICE");
            entity.Property(e => e.Rcmid).HasColumnName("RCMID");
            entity.HasOne(d => d.Rcm).WithMany(p => p.TblRecapItemDetails)
                .HasForeignKey(d => d.Rcmid)
                .HasConstraintName("FK_TBL_RECAP_ITEM_DETAILS_TBL_RECAP_ITEM_DETAILS");
        });

        modelBuilder.Entity<TblCadConsFiles>(entity =>
        {
            entity.HasKey(e => e.Fileid);

            entity.ToTable("TBL_CAD_CONS_FILES");

            entity.Property(e => e.Fileid)
                .HasColumnName("FILEID");

            entity.Property(e => e.Caddid)
                .HasColumnName("CADDID");

            entity.Property(e => e.Filename)
                .HasMaxLength(250)
                .HasColumnName("FILENAME");

            entity.Property(e => e.Filepath)
                .HasMaxLength(250)
                .HasColumnName("FILEPATH");

            entity.Property(e => e.Filesize)
                .HasColumnName("FILESIZE");

            entity.Property(e => e.Contenttype)
                .HasMaxLength(250)
                .HasColumnName("CONTENTTYPE");

            entity.Property(e => e.Createddate)
                .HasColumnName("CREATEDDATE");

            entity.Property(e => e.Createdby)
                .HasMaxLength(100)
                .HasColumnName("CREATEDBY");

            entity.HasOne(d => d.CadDetail)
                .WithMany(p => p.TblCadConsFiles)
                .HasForeignKey(d => d.Caddid)
                .HasConstraintName("FK_TBL_CAD_CONS_FILES_TBL_CAD_CONS_D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
