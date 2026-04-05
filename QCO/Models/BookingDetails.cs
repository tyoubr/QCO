public class BookingDetails
{
    public string Id { get; set; } // Booking No
    public string Text { get; set; } // Display Text
    public string BuyerName { get; set; }
    public decimal? TotalSmv { get; set; }
    public List<string> Processes { get; set; }
}