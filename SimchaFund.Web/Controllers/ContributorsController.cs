using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;

namespace SimchaFund.Web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True;TrustServerCertificate=True";
        public IActionResult Index()
        {
            var repository = new SimchaFundRepository(_connectionString);

            if (TempData["message"] != null)
            {
                ViewBag.Message = (string)TempData["message"];
            }

            var contributors = repository.GetContributors();
            contributors.ForEach(c => c.Balance = repository.GetContributorBalance(c.Id));
            return View(new ContributorsViewModel
            {
                Contributors = contributors,
                TotalContributions = repository.GetTotalContributions()
            });

        }

        [HttpPost]
        public IActionResult New(Contributor contributor, decimal initialDeposit)
        {
            var repository = new SimchaFundRepository(_connectionString);
            repository.AddContributor(contributor, initialDeposit);
            TempData["message"] = $"{contributor.FirstName} {contributor.LastName} added successfully!";
            return Redirect("/contributors/index");
        }

        [HttpPost]
        public IActionResult Edit(Contributor contributor)
        {
            var repository = new SimchaFundRepository(_connectionString);
            repository.UpdateContributor(contributor);
            TempData["message"] = $"{contributor.FirstName} {contributor.LastName} updated successfully!";
            return Redirect("/contributors/index");
        }

        public IActionResult History(int contributorId)
        {
            var repository = new SimchaFundRepository(_connectionString);
            var viewModel = new HistoryViewModel();
            viewModel.Histories = repository.GetHistoryForContributor(contributorId);
            if (viewModel.Histories == null)
            {
                return Redirect("/contributors/index");
            }
            viewModel.Balance = repository.GetContributorBalance(contributorId);
            viewModel.ContributorName = repository.GetContributorNameById(contributorId);
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var repository = new SimchaFundRepository(_connectionString);
            repository.AddDepositForContributor(deposit);
            TempData["message"] = $"Deposit added successfully!";
            return Redirect("/contributors/index");
        }
    }
}
