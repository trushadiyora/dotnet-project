# Contact Manager App

A modern ASP.NET Core MVC application for managing contacts with Firebase Authentication and Firestore database integration.

## Features

- User Authentication with Firebase
- Contact Management (CRUD operations)
- Cloud Storage with Firestore
- Responsive Design
- Search Functionality
- Modern UI with Bootstrap and Font Awesome

## Prerequisites

- .NET 9.0 SDK
- Firebase Account and Project
- Visual Studio 2022 or VS Code
- Git

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone https://github.com/trushadiyora/dotnet-project.git
   cd dotnet-project
   ```

2. **Firebase Setup**
   - Create a new project in [Firebase Console](https://console.firebase.google.com/)
   - Enable Email/Password Authentication
   - Create a Firestore Database
   - Download the service account key JSON file from Project Settings > Service Accounts

3. **Application Configuration**
   - Create `appsettings.Development.json` in the root directory
   - Add the following configuration (replace with your Firebase details):
   ```json
   {
     "Firebase": {
       "ProjectId": "your-project-id",
       "WebApiKey": "your-web-api-key",
       "ServiceAccountPath": "path-to-your-service-account-json"
     }
   }
   ```

4. **Install Dependencies**
   ```bash
   dotnet restore
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

6. **Access the Application**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## Project Structure

- `Controllers/` - MVC Controllers
  - `AccountController.cs` - Handles user authentication
  - `ContactsController.cs` - Manages contact operations
  - `HomeController.cs` - Basic navigation
- `Models/` - Data models
  - `Contact.cs` - Contact entity
  - `User.cs` - User entity
- `Services/` - Business logic
  - `AuthService.cs` - Firebase authentication
  - `FirebaseService.cs` - Firestore operations
- `Views/` - MVC views
  - `Contacts/` - Contact management views
  - `Home/` - Home and navigation views
  - `Shared/` - Layout and shared components

## Key Features Implementation

### Authentication
- Firebase Authentication using REST API
- Session-based auth state management
- Registration and Login functionality

### Contact Management
- Create, Read, Update, Delete (CRUD) operations
- Search functionality
- Firestore integration for data persistence

### User Interface
- Responsive design using Bootstrap
- Modern UI with gradients and cards
- Font Awesome icons
- Client-side validation

## Environment Variables

The following environment variables need to be set:
- `GOOGLE_APPLICATION_CREDENTIALS` - Path to Firebase service account JSON file
  - Can be set in appsettings.json or as an environment variable

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Development Notes

- The application uses .NET 9.0 features
- Firebase Admin SDK for server-side operations
- FirebaseAuth REST API for client-side auth
- Firestore for data storage
- Bootstrap 5 for styling

## Common Issues and Solutions

1. **Firebase Configuration**
   - Ensure service account JSON path is correct
   - Verify Firebase project settings

2. **Authentication Issues**
   - Check Firebase Auth is enabled
   - Verify API key and project ID

3. **Database Access**
   - Confirm Firestore rules are properly set
   - Verify service account permissions

## License

This project is licensed under the MIT License - see the LICENSE file for details