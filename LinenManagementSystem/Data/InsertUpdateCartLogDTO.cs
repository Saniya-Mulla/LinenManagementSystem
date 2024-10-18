namespace LinenManagementSystem.Data
{
    public class InsertUpdateCartLogDTO
    {
        public int? CartLogId { get; set; }
        public string ReceiptNumber { get; set; }
        public int? ReportedWeight { get; set; }
        public int ActualWeight { get; set; }
        public string Comments { get; set; }
        public DateTime DateWeighed { get; set; }
        public int CartId { get; set; }
        public int LocationId { get; set; }
        public int EmployeeId { get; set; }

        public List<LinenUpdInsertDTO> Linen { get; set; }
    }

    public class LinenUpdInsertDTO {
        public int LinenId { get; set; }
        public int Count { get; set; }

    }

   
}
