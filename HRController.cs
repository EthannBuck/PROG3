using Microsoft.AspNetCore.Mvc;
using PROG3.Helpers;
using PROG3.Models;
using System.Threading.Tasks;

namespace PROG3.Controllers
{
    public class HRController : Controller
    {
        private readonly TableStorageHelper _tableStorageHelper;

        public HRController(TableStorageHelper tableStorageHelper)
        {
            _tableStorageHelper = tableStorageHelper;
        }

        // Display claims for invoice creation
        public async Task<IActionResult> ReviewClaims()
        {
            var approvedClaims = await _tableStorageHelper.GetClaimsByStatusAsync("Approved");
            return View(approvedClaims); 
        }

        // View details of a specific claim
        public async Task<IActionResult> ViewClaimDetails(string claimId, string lecturerId)
        {
            var claimEntity = await _tableStorageHelper.RetrieveEntityAsync<ClaimEntity>(lecturerId, claimId);

            if (claimEntity == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("GenerateReports");
            }

            return View("ClaimInvoiceDetails", claimEntity);
        }

        public async Task<IActionResult> GenerateReports()
        {
            var approvedClaims = await _tableStorageHelper.GetClaimsByStatusAsync("Approved");

            if (approvedClaims == null || !approvedClaims.Any())
            {
                TempData["ErrorMessage"] = "No approved claims found.";
                return RedirectToAction("GenerateReports");
            }

            var reportData = approvedClaims.Select(c => new
            {
                c.ClaimId,
                c.LecturerId,
                TotalAmount = c.TotalAmount.ToString("C"), 
                c.Status,
                c.SubmissionDate
            }).ToList();

            return View("~/Views/Shared/GenerateReports.cshtml", reportData);
        }
    }
}
