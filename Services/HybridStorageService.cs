using ContactManagerApp.Models;
using System.Collections.Concurrent;

namespace ContactManagerApp.Services
{
    public class HybridStorageService
    {
        private readonly FirebaseService _firebaseService;
        private readonly LocalStorageService _localStorageService;
        private readonly ConcurrentDictionary<string, bool> _firebaseAvailable;

        public HybridStorageService(FirebaseService firebaseService, LocalStorageService localStorageService)
        {
            _firebaseService = firebaseService;
            _localStorageService = localStorageService;
            _firebaseAvailable = new ConcurrentDictionary<string, bool>();
        }

        private bool IsFirebaseAvailable()
        {
            return _firebaseAvailable.GetOrAdd("available", key =>
            {
                try
                {
                    // Check if Firebase service account path exists
                    var config = _firebaseService.GetType().GetField("_serviceAccountPath", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (config != null)
                    {
                        var serviceAccountPath = config.GetValue(_firebaseService) as string;
                        if (!string.IsNullOrEmpty(serviceAccountPath) && System.IO.File.Exists(serviceAccountPath))
                        {
                            Console.WriteLine("Firebase service account found, Firebase available");
                            return true;
                        }
                    }
                    Console.WriteLine("Firebase service account not found, using local storage");
                    return false;
                }
                catch
                {
                    Console.WriteLine("Firebase not available, using local storage");
                    return false;
                }
            });
        }

        public async Task<bool> AddContactAsync(Contact contact)
        {
            Console.WriteLine($"HybridStorage: Attempting to add contact: {contact.Name}");
            
            // Always use local storage for now to ensure it works
            Console.WriteLine("Using local storage for contact saving");
            var result = await _localStorageService.AddContactAsync(contact);
            
            if (result)
            {
                Console.WriteLine("Contact saved to local storage successfully");
            }
            else
            {
                Console.WriteLine("Failed to save contact to local storage");
            }
            
            return result;
        }

        public async Task<List<Contact>> GetAllContactsAsync(string userId)
        {
            Console.WriteLine($"HybridStorage: Getting all contacts for user: {userId}");
            
            // Always use local storage for now to ensure it works
            var contacts = await _localStorageService.GetAllContactsAsync(userId);
            Console.WriteLine($"Retrieved {contacts.Count} contacts from local storage");
            return contacts;
        }

        public async Task<Contact?> GetContactByIdAsync(string contactId)
        {
            Console.WriteLine($"HybridStorage: Getting contact by ID: {contactId}");
            return await _localStorageService.GetContactByIdAsync(contactId);
        }

        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            Console.WriteLine($"HybridStorage: Updating contact: {contact.Name}");
            return await _localStorageService.UpdateContactAsync(contact);
        }

        public async Task<bool> DeleteContactAsync(string contactId)
        {
            Console.WriteLine($"HybridStorage: Deleting contact: {contactId}");
            return await _localStorageService.DeleteContactAsync(contactId);
        }

        public async Task<List<Contact>> SearchContactsAsync(string userId, string searchTerm)
        {
            Console.WriteLine($"HybridStorage: Searching contacts for user: {userId}, term: {searchTerm}");
            var results = await _localStorageService.SearchContactsAsync(userId, searchTerm);
            Console.WriteLine($"Search returned {results.Count} results");
            return results;
        }
    }
}
