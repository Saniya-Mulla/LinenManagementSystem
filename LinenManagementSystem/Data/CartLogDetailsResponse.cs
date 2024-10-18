namespace LinenManagementSystem.Data
{
    public class CartLogDetailsResponse
    {
        public int CartLogId { get; set; }
        public string ReceiptNumber { get; set; }
        public int? ReportedWeight { get; set; }
        public decimal ActualWeight { get; set; }
        public string Comments { get; set; }
        public DateTime DateWeighed { get; set; }

        // Related entities
        public CartDto Cart { get; set; }
        public LocationDto Location { get; set; }
        public EmployeeDto Employee { get; set; }

        // Linen details
        public List<LinenDetailDto> Linen { get; set; }

    }
}
