using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class HistoryViewModel
    {
        public List<History> Histories { get; set; }
        public decimal Balance { get; set; }
        public string ContributorName { get; set; }
    }
}
