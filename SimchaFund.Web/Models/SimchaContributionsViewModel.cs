using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class SimchaContributionsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }
}
