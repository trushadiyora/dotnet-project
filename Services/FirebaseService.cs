using Google.Cloud.Firestore;
using ContactManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ContactManagerApp.Services
{
    public class FirebaseService
    {
    private FirestoreDb? _firestoreDb;
        private const string ContactsCollection = "contacts";
        private readonly string _projectId;
        private readonly string _serviceAccountPath;

        public FirebaseService(Microsoft.Extensions.Options.IOptions<ContactManagerApp.Configuration.FirebaseOptions> options)
        {
            var opts = options.Value;

            if (string.IsNullOrEmpty(opts?.ProjectId))
                throw new System.ArgumentException("Firebase ProjectId is not configured.");

            _projectId = opts.ProjectId;
            _serviceAccountPath = opts.ServiceAccountPath ?? string.Empty;

            // If service account path provided, set the env var now so Firestore can pick it up later.
            if (!string.IsNullOrEmpty(_serviceAccountPath))
            {
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _serviceAccountPath);
            }

            // Do not create FirestoreDb here to avoid throwing during service construction if ADC not configured.
            // It will be created lazily on first use.
            _firestoreDb = null;
        }

        private void EnsureInitialized()
        {
            if (_firestoreDb != null)
                return;

            try
            {
                // Try to create FirestoreDb with service account if path is provided
                if (!string.IsNullOrEmpty(_serviceAccountPath) && System.IO.File.Exists(_serviceAccountPath))
                {
                    System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _serviceAccountPath);
                }
                
                _firestoreDb = FirestoreDb.Create(_projectId);
            }
            catch (InvalidOperationException ex)
            {
                // Provide a helpful message for common ADC misconfiguration.
                throw new InvalidOperationException(
                    $"Firestore credentials were not found. Please ensure:\n" +
                    $"1. Service account JSON file exists at: {_serviceAccountPath}\n" +
                    $"2. Firestore Database is enabled in Firebase Console\n" +
                    $"3. Your Firebase project ID is correct: {_projectId}\n" +
                    $"See: https://console.firebase.google.com/project/{_projectId}/firestore", ex);
            }
        }

        // CREATE - Add new contact
        public async Task<bool> AddContactAsync(Contact contact)
        {
            try
            {
                Console.WriteLine($"Attempting to add contact: {contact.Name} with ID: {contact.Id}");
                EnsureInitialized();
                Console.WriteLine("Firestore initialized successfully");
                
                DocumentReference docRef = _firestoreDb!.Collection(ContactsCollection).Document(contact.Id);
                Console.WriteLine($"Document reference created: {docRef.Path}");
                
                await docRef.SetAsync(contact);
                Console.WriteLine("Contact saved successfully to Firestore");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding contact: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        // READ - Get all contacts for a user
        public async Task<List<Contact>> GetAllContactsAsync(string userId)
        {
            try
            {
                EnsureInitialized();
                Query query = _firestoreDb!.Collection(ContactsCollection)
                    .WhereEqualTo("UserId", userId)
                    .OrderByDescending("CreatedAt");

                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                
                List<Contact> contacts = new List<Contact>();
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Contact contact = document.ConvertTo<Contact>();
                        contacts.Add(contact);
                    }
                }
                
                return contacts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contacts: {ex.Message}");
                return new List<Contact>();
            }
        }

        // READ - Get single contact by ID
    public async Task<Contact?> GetContactByIdAsync(string contactId)
        {
            try
            {
                EnsureInitialized();
                DocumentReference docRef = _firestoreDb!.Collection(ContactsCollection).Document(contactId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<Contact>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contact: {ex.Message}");
                return null;
            }
        }

        // UPDATE - Update existing contact
        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            try
            {
                EnsureInitialized();
                contact.UpdatedAt = DateTime.UtcNow;
                DocumentReference docRef = _firestoreDb!.Collection(ContactsCollection).Document(contact.Id);
                await docRef.SetAsync(contact, SetOptions.Overwrite);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating contact: {ex.Message}");
                return false;
            }
        }

        // DELETE - Delete contact
        public async Task<bool> DeleteContactAsync(string contactId)
        {
            try
            {
                EnsureInitialized();
                DocumentReference docRef = _firestoreDb!.Collection(ContactsCollection).Document(contactId);
                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting contact: {ex.Message}");
                return false;
            }
        }

        // SEARCH - Search contacts by name or phone
        public async Task<List<Contact>> SearchContactsAsync(string userId, string searchTerm)
        {
            try
            {
                var allContacts = await GetAllContactsAsync(userId);
                
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return allContacts;

                searchTerm = searchTerm.ToLower();
                
                return allContacts.Where(c => 
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm) ||
                    c.PhoneNumber.Contains(searchTerm)
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching contacts: {ex.Message}");
                return new List<Contact>();
            }
        }
    }
}