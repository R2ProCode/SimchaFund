using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;

namespace SimchaFund.Web.Controllers
{
    public class SimchasController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True;TrustServerCertificate=True";
        public IActionResult Index()
        {
            SimchaFundRepository repository = new SimchaFundRepository(_connectionString);

            if (TempData["message"] != null)
            {
                ViewBag.Message = (string)TempData["message"];
            }

            return View(new SimchasViewModel
            {
                Simchas = repository.GetSimchas(),
                ContributorsAmount = repository.GetContributorCount()
            });
        }


        public IActionResult Contributions(int simchaId)
        {
            SimchaFundRepository repository = new SimchaFundRepository(_connectionString);
            var contributors = repository.GetContributors();

            contributors.ForEach(c => c.Balance = repository.GetContributorBalance(c.ContributorId));
            contributors.ForEach(c => c.Amount = repository.GetContributionForSimcha(simchaId, c.ContributorId));

            return View(new SimchaContributionsViewModel
            {   
                Contributors = contributors,
                SimchaName = repository.GetSimchaNameById(simchaId),
                SimchaId = simchaId,
            });
        }

        [HttpPost]
        public IActionResult UpdateContributions(int simchaId, List<Contributor> contributors)
        {
            SimchaFundRepository repository = new SimchaFundRepository(_connectionString);
            repository.UpdateContributionsForSimcha(simchaId, contributors);
            TempData["message"] = "Contributions updated successfully!";
            return Redirect("/simchas/index");
        }

        [HttpPost]
        public IActionResult New(Simcha simcha)
        {
            SimchaFundRepository repository = new SimchaFundRepository(_connectionString);
            repository.AddSimcha(simcha);
            TempData["message"] = "Simcha added successfully!";
            return Redirect("/simchas/index");
        }
    }
}
