namespace SimchaFund.Data
{
    public class Contributor
    {
        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public DateTime Date { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Amount { get; set; }
    }
}
