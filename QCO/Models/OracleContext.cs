using Microsoft.EntityFrameworkCore;

namespace QCO.Models
{
    public class OracleContext : DbContext
    {
        public OracleContext(DbContextOptions<OracleContext> options) : base(options) { }

        public DbSet<NewView3> NewView3 { get; set; }
        public DbSet<ViewCAD> VW_CAD { get; set; }
        public DbSet<VW_BOOKING_DETAILS> VW_BOOKING_DETAILS { get; set; }
              protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure NewView3 as a keyless entity
            modelBuilder.Entity<NewView3>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("NEWVIEW_3"); // Optional: Specify the view name if applicable
            });

            // Configure VW_CAD as a keyless entity
            modelBuilder.Entity<ViewCAD>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_CAD"); // Optional: Specify the view name if applicable
            });
            modelBuilder.Entity<VW_BOOKING_DETAILS>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_BOOKING_DETAILS"); // Optional: Specify the view name if applicable
            });
        }
    }

    public class NewView3
    {
        public string BOOKING_NO { get; set; }
        public string BUYER_NAME { get; set; }
        public int ROW_SEQUENCE_NO {  get; set; }
        public string OPERATION_NAME { get; set; }
        public string RESOURCE_NAME { get; set; }
        public decimal? OPERATOR_SMV { get; set; }
        public decimal? HELPER_SMV { get; set; }
        public decimal? TOTAL_SMV { get; set; }
    }

    public class ViewCAD
    {
        public string IR_IB { get; set; }
        public string JOB_NO_MST { get; set; }
        public string STYLE_REF_NO { get; set; }
        public string STYLE_DESCRIPTION { get; set; }
        public string SEASON_BUYER_WISE { get; set; }
        public string SEASON_NAME { get; set; }
        public string SEASON_YEAR { get; set; }
        public string BUYER_NAME { get; set; }
        public string BRAND_NAME { get; set; }
    }

    public class VW_BOOKING_DETAILS
    {
        public string JOB_NO { get; set; }
        public string STYLE_REF_NO { get; set; }

        public string? GARMENTS_ITEM { get; set; }

        public string? COLOR_NAME { get; set; }

        public string? FABRIC_DESCRIPTION { get; set; }
        public string? BODY_PARTS { get; set; }

        public decimal? GSM_WEIGHT { get; set; }
    }
}
