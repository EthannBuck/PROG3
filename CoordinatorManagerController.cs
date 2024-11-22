using Microsoft.AspNetCore.Mvc;
using PROG3.Helpers;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using PROG3.Models;

namespace PROG3.Controllers
{
    public class CoordinatorManagerController : Controller
    {
        private readonly TableStorageHelper _tableStorageHelper;

        public CoordinatorManagerController(TableStorageHelper tableStorageHelper)
        {
            _tableStorageHelper = tableStorageHelper;
        }

        // Display pending claims for review
        public async Task<IActionResult> ReviewClaims()
        {
            var pendingClaims = await _tableStorageHelper.GetClaimsByStatusAsync("Pending");
            return View(pendingClaims);
        }

        public async Task<IActionResult> ReviewClaim(string claimId, string lecturerId)
        {
            var claimEntity = await _tableStorageHelper.RetrieveEntityAsync<ClaimEntity>(lecturerId, claimId);

            if (claimEntity == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("ReviewClaims");
            }

            return View("~/Views/Shared/ReviewClaim.cshtml", claimEntity);
        }

        // Approve a claim
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(string lecturerId, string claimId)
        {
            var claimEntity = await _tableStorageHelper.RetrieveEntityAsync<ClaimEntity>(lecturerId, claimId);  

            if (claimEntity != null)
            {
                claimEntity.Status = "Approved";
                await _tableStorageHelper.InsertOrMergeEntityAsync(claimEntity);
                TempData["SuccessMessage"] = "Claim approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction("ReviewClaim");
        }

        // Reject a claim
        [HttpPost]
        public async Task<IActionResult> RejectClaim(string lecturerId, string claimId)
        {
            var claimEntity = await _tableStorageHelper.RetrieveEntityAsync<ClaimEntity>(lecturerId, claimId); 

            if (claimEntity != null)
            {
                claimEntity.Status = "Rejected";
                await _tableStorageHelper.InsertOrMergeEntityAsync(claimEntity);
                TempData["SuccessMessage"] = "Claim rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Claim not found.";
            }

            return RedirectToAction("ReviewClaim");
        }

        [HttpPost]
        public async Task<IActionResult> ReviewClaim(string claimId, string lecturerId, string action)
        {
            if (string.IsNullOrEmpty(lecturerId) || string.IsNullOrEmpty(claimId))
            {
                TempData["ErrorMessage"] = "Invalid lecturerId or claimId.";
                return RedirectToAction("ReviewClaims");
            }

            var claimEntity = await _tableStorageHelper.RetrieveEntityAsync<ClaimEntity>(lecturerId, claimId);

            if (claimEntity == null)
            {
                TempData["ErrorMessage"] = "Claim not found.";
                return RedirectToAction("ReviewClaims");
            }

            // Update the status of the claim based on action
            if (action == "approve")
            {
                claimEntity.Status = "Approved";
            }
            else if (action == "reject")
            {
                claimEntity.Status = "Rejected";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid action.";
                return RedirectToAction("ReviewClaims");
            }

            await _tableStorageHelper.InsertOrMergeEntityAsync(claimEntity);

            TempData["SuccessMessage"] = $"Claim {action}d successfully.";
            return RedirectToAction("ReviewClaims");
        }






    }
}
