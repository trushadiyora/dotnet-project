using Microsoft.AspNetCore.Mvc;
using ContactManagerApp.Models;
using ContactManagerApp.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ContactManagerApp.Controllers
{
    public class ContactsController : Controller
    {
        private readonly HybridStorageService _storageService;

        public ContactsController(HybridStorageService storageService)
        {
            _storageService = storageService;
        }

        // Check if user is logged in
        private string GetUserId()
        {
            return HttpContext.Session.GetString("UserId") ?? string.Empty;
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(GetUserId());
        }

        // GET: Contacts/Index - List all contacts
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            string userId = GetUserId();
            var contacts = string.IsNullOrEmpty(searchTerm) 
                ? await _storageService.GetAllContactsAsync(userId)
                : await _storageService.SearchContactsAsync(userId, searchTerm);

            ViewBag.SearchTerm = searchTerm;
            return View(contacts);
        }

        // GET: Contacts/Create - Show create form
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Contacts/Create - Handle create form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                contact.UserId = GetUserId();
                bool success = await _storageService.AddContactAsync(contact);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Contact added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["ErrorMessage"] = "Failed to add contact. Please try again.";
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5 - Show edit form
        public async Task<IActionResult> Edit(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var contact = await _storageService.GetContactByIdAsync(id);
            
            if (contact == null || contact.UserId != GetUserId())
                return NotFound();

            return View(contact);
        }

        // POST: Contacts/Edit/5 - Handle edit form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Contact contact)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id != contact.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                contact.UserId = GetUserId();
                bool success = await _storageService.UpdateContactAsync(contact);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Contact updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["ErrorMessage"] = "Failed to update contact. Please try again.";
            }

            return View(contact);
        }

        // GET: Contacts/Details/5 - Show contact details
        public async Task<IActionResult> Details(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var contact = await _storageService.GetContactByIdAsync(id);
            
            if (contact == null || contact.UserId != GetUserId())
                return NotFound();

            return View(contact);
        }

        // GET: Contacts/Delete/5 - Show delete confirmation
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
                return NotFound();

            var contact = await _storageService.GetContactByIdAsync(id);
            
            if (contact == null || contact.UserId != GetUserId())
                return NotFound();

            return View(contact);
        }

        // POST: Contacts/DeleteConfirmed/5 - Handle delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            bool success = await _storageService.DeleteContactAsync(id);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Contact deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete contact. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}