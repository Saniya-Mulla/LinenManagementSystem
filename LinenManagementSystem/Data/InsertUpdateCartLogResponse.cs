namespace LinenManagementSystem.Data
{
    public class InsertUpdateCartLogResponse
    {
        public int CartLogId { get; set; }
        public string ReceiptNumber { get; set; }
        public int? ReportedWeight { get; set; }
        public decimal ActualWeight { get; set; }
        public string Comments { get; set; }
        public DateTime DateWeighed { get; set; }

        // Related entities
        public int CartId { get; set; }
        public int LocationId { get; set; }
        public int EmployeeId { get; set; }

        // Linen details
        public List<LinenDetailDto> Linen { get; set; }
    }
}
