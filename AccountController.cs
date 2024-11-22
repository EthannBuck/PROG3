using Microsoft.AspNetCore.Mvc;
using PROG3.Helpers;
using PROG3.Models;
using BCrypt.Net;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace PROG3.Controllers
{
    public class AccountController : Controller
    {
        private readonly TableStorageHelper _tableStorageHelper;

        public AccountController(TableStorageHelper tableStorageHelper)
        {
            _tableStorageHelper = tableStorageHelper;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and Password cannot be empty.");
                return View();
            }

            // Create new UserEntity to be saved
            var userEntity = new UserEntity
            {
                PartitionKey = "Users", 
                RowKey = username,     
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _tableStorageHelper.InsertOrMergeEntityAsync(userEntity);

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var userEntity = await _tableStorageHelper.RetrieveEntityAsync<UserEntity>("Users", username);

            if (userEntity == null || !BCrypt.Net.BCrypt.Verify(password, userEntity.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            // Store the username in session or cookies to track login
            HttpContext.Session.SetString("LecturerUsername", username);
            return RedirectToAction("SubmitClaim", "Lecturer");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LecturerUsername");
            return RedirectToAction("Login");
        }
    }
}
