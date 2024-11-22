using Microsoft.AspNetCore.Mvc;
using PROG3.Helpers;
using PROG3.Models;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace PROG3.Controllers
{
    public class LecturerController : Controller
    {
        private readonly TableStorageHelper _tableStorageHelper;
        private readonly BlobStorageHelper _blobStorageHelper;

        public LecturerController(TableStorageHelper tableStorageHelper, BlobStorageHelper blobStorageHelper)
        {
            _tableStorageHelper = tableStorageHelper;
            _blobStorageHelper = blobStorageHelper;
        }

        public IActionResult SubmitClaim()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LecturerUsername")))
            {
                return RedirectToAction("Register", "Account");  
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(double hourlyRate, double hoursWorked, IFormFile document)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("LecturerUsername")))
            {
                return RedirectToAction("Register", "Account");  
            }

            if (hourlyRate <= 0 || hoursWorked <= 0)
            {
                ModelState.AddModelError("", "Hourly Rate and Hours Worked must be greater than zero.");
                return View();
            }

            var claimId = Guid.NewGuid().ToString();
            var lecturerId = HttpContext.Session.GetString("LecturerUsername");

            string documentUrl = null;
            if (document != null)
            {
                try
                {
                    documentUrl = await _blobStorageHelper.UploadFileAsync(document);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading document: " + ex.Message);
                    return View();
                }
            }

            var totalAmount = hourlyRate * hoursWorked;

            var claimEntity = new ClaimEntity
            {
                PartitionKey = lecturerId,
                RowKey = claimId,
                HourlyRate = hourlyRate,
                HoursWorked = hoursWorked,
                TotalAmount = totalAmount,
                Status = "Pending",
                DocumentUrl = documentUrl,
                SubmissionDate = DateTime.UtcNow
            };

            try
            {
                await _tableStorageHelper.InsertOrMergeEntityAsync(claimEntity);
                TempData["SuccessMessage"] = "Claim submitted successfully.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error submitting claim: " + ex.Message);
                return View();
            }

            return RedirectToAction("SubmitClaim");
        }
    }
}
