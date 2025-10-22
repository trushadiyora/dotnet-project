using ContactManagerApp.Models;
using System.Text.Json;
using System.Collections.Concurrent;

namespace ContactManagerApp.Services
{
    public class LocalStorageService
    {
        private readonly string _dataPath;
        private readonly ConcurrentDictionary<string, Contact> _contacts;
        private readonly object _lock = new object();

        public LocalStorageService()
        {
            _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "contacts.json");
            _contacts = new ConcurrentDictionary<string, Contact>();
            LoadContacts();
        }

        private void LoadContacts()
        {
            try
            {
                if (File.Exists(_dataPath))
                {
                    var json = File.ReadAllText(_dataPath);
                    var contacts = JsonSerializer.Deserialize<List<Contact>>(json);
                    if (contacts != null)
                    {
                        foreach (var contact in contacts)
                        {
                            _contacts[contact.Id] = contact;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading contacts: {ex.Message}");
            }
        }

        private void SaveContacts()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
                var json = JsonSerializer.Serialize(_contacts.Values.ToList(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contacts: {ex.Message}");
            }
        }

        public async Task<bool> AddContactAsync(Contact contact)
        {
            try
            {
                Console.WriteLine($"LocalStorage: Adding contact: {contact.Name} with ID: {contact.Id}");
                Console.WriteLine($"Contact UserId: {contact.UserId}");
                Console.WriteLine($"Contact Email: {contact.Email}");
                Console.WriteLine($"Contact Phone: {contact.PhoneNumber}");
                
                _contacts[contact.Id] = contact;
                SaveContacts();
                
                Console.WriteLine($"Contact saved successfully to local storage. Total contacts now: {_contacts.Count}");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding contact: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<List<Contact>> GetAllContactsAsync(string userId)
        {
            try
            {
                Console.WriteLine($"LocalStorage: Getting contacts for user: {userId}");
                Console.WriteLine($"Total contacts in storage: {_contacts.Count}");
                
                var userContacts = _contacts.Values
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
                
                Console.WriteLine($"Retrieved {userContacts.Count} contacts for user {userId}");
                
                // Debug: Print all contacts
                foreach (var contact in userContacts)
                {
                    Console.WriteLine($"Contact: {contact.Name} ({contact.Email}) - UserId: {contact.UserId}");
                }
                
                return await Task.FromResult(userContacts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contacts: {ex.Message}");
                return new List<Contact>();
            }
        }

        public async Task<Contact?> GetContactByIdAsync(string contactId)
        {
            try
            {
                _contacts.TryGetValue(contactId, out var contact);
                return await Task.FromResult(contact);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contact: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            try
            {
                contact.UpdatedAt = DateTime.UtcNow;
                _contacts[contact.Id] = contact;
                SaveContacts();
                Console.WriteLine($"Contact updated: {contact.Name}");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating contact: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteContactAsync(string contactId)
        {
            try
            {
                var removed = _contacts.TryRemove(contactId, out _);
                if (removed)
                {
                    SaveContacts();
                    Console.WriteLine($"Contact deleted: {contactId}");
                }
                return await Task.FromResult(removed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting contact: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Contact>> SearchContactsAsync(string userId, string searchTerm)
        {
            try
            {
                var allContacts = await GetAllContactsAsync(userId);
                
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return allContacts;

                searchTerm = searchTerm.ToLower();
                
                var results = allContacts.Where(c => 
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm) ||
                    c.PhoneNumber.Contains(searchTerm)
                ).ToList();

                Console.WriteLine($"Search for '{searchTerm}' returned {results.Count} results");
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching contacts: {ex.Message}");
                return new List<Contact>();
            }
        }
    }
}
