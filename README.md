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
   1. Create a new project in [Firebase Console](https://console.firebase.google.com/):
      - Click "Create a project"
      - Enter a project name
      - Accept the terms and continue
      - Disable Google Analytics (optional)
      - Click "Create project"

   2. Enable Authentication:
      - In Firebase Console, go to Authentication > Get Started
      - Click "Email/Password" under Sign-in providers
      - Enable Email/Password authentication
      - Click Save

   3. Create Firestore Database:
      - Go to Firestore Database > Create Database
      - Choose "Start in test mode" for development
      - Select a location closest to your users
      - Click "Enable"

   4. Get Service Account Credentials:
      - Go to Project Settings (gear icon) > Service accounts
      - Click "Generate New Private Key"
      - Save the downloaded JSON file as `firebase-service-account.json.json` in your project root
      - Keep this file secure and never commit it to version control

   5. Get Web API Key:
      - In Project Settings > General
      - Copy the "Web API Key" from the project settings

3. **Application Configuration**
   1. Create `appsettings.Development.json` in the root directory:
   ```json
   {
     "Firebase": {
       "ProjectId": "your-project-id",      // Found in Project Settings
       "WebApiKey": "your-web-api-key",     // Found in Project Settings > General
       "ServiceAccountPath": "firebase-service-account.json.json"  // Path to the JSON file you downloaded
     }
   }
   ```
   
   2. Important Security Notes:
      - Never commit `firebase-service-account.json.json` to version control
      - Never commit `appsettings.Development.json` to version control
      - Both files are already in .gitignore for security
      - Keep your API keys and service account credentials secure

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
