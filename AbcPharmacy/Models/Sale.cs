namespace AbcPharmacy.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int QtySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public DateTime SoldOn { get; set; }
    }
}
