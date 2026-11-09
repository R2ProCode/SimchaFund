using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Contribution
    {
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public decimal? Balance { get; set; }
        public string ContributorName { get; set; }
        public bool AlwaysInclude { get; set; }
    }
}
