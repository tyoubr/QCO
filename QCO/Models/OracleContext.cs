using Microsoft.EntityFrameworkCore;

namespace QCO.Models
{
    public class OracleContext : DbContext
    {
        public OracleContext(DbContextOptions<OracleContext> options) : base(options) { }

        public DbSet<NewView3> NewView3 { get; set; }
              protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure NewView3 as a keyless entity
            modelBuilder.Entity<NewView3>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("NEWVIEW_3"); // Optional: Specify the view name if applicable
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
}
