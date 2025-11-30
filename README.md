# Firebase Toolkit for Unity

High-level Firebase systems for Unity - Authentication, Leaderboards, Save System, Profiles, and more.

## 🎯 Current Version: v0.1.0

### ✅ What's Included

- **Email/Password Authentication**
  - User registration with validation
  - Sign in/Sign out functionality
  - Password reset capability
  - User profile management

- **Authenticated Leaderboards**
  - Submit scores (authenticated users only)
  - Get top scores with rankings
  - User profile integration
  - Real-time database sync

- **Complete UI System**
  - Login/Register panels
  - Profile management
  - Leaderboard display
  - Game integration example

## 📦 Installation

### Prerequisites
1. **Unity 2020.3 or higher**
2. **Firebase SDK for Unity**
   - Download from: https://firebase.google.com/download/unity
   - Import these packages:
     - FirebaseAuth.unitypackage
     - FirebaseDatabase.unitypackage

### Setup Steps
1. Import Firebase SDK packages
2. Add `google-services.json` to `Assets/` folder
3. Copy Firebase Toolkit to your project
4. Follow the setup guide in `Examples/AuthLeaderboardDemo/SETUP_GUIDE.md`

## 🚀 Quick Start

### 1. Add FirebaseManager to Scene
```csharp
// FirebaseManager initializes automatically
// Access anywhere via singleton:
FirebaseManager.Instance
```

### 2. Implement Authentication
```csharp
using FirebaseToolkit.Auth;

AuthSystem auth = new AuthSystem();

// Register new user
auth.RegisterUser(email, password, (success, message) => {
    if (success) {
        Debug.Log("Registration successful!");
    }
});

// Sign in
auth.SignInUser(email, password, (success, message) => {
    if (success) {
        Debug.Log("Signed in!");
    }
});

// Sign out
auth.SignOut();
```

### 3. Use Leaderboard System
```csharp
using FirebaseToolkit.Leaderboard;

LeaderboardSystem leaderboard = new LeaderboardSystem("global");

// Submit score (requires authentication)
leaderboard.SubmitScore(playerName, score, success => {
    if (success) {
        Debug.Log("Score submitted!");
    }
});

// Get top scores
leaderboard.GetTopScores(10, entries => {
    foreach (var entry in entries) {
        Debug.Log($"#{entry.rank} {entry.playerName}: {entry.score}");
    }
});
```

## 📖 Documentation

### Core Components

#### FirebaseManager
Central singleton for Firebase initialization and configuration.

**Properties:**
- `IsReady` - Firebase initialization status
- `IsSignedIn` - User authentication status
- `CurrentUser` - Current Firebase user
- `Database` - Firebase Realtime Database reference
- `Auth` - Firebase Authentication reference

**Events:**
- `OnFirebaseReady` - Fired when Firebase is initialized
- `OnUserSignedIn` - Fired when user signs in
- `OnUserSignedOut` - Fired when user signs out

#### AuthSystem
Handles all authentication operations.

**Methods:**
- `RegisterUser(email, password, callback)`
- `SignInUser(email, password, callback)`
- `SignOut()`
- `SendPasswordResetEmail(email, callback)`
- `UpdateDisplayName(name, callback)`

#### LeaderboardSystem
Manages leaderboard data and rankings.

**Methods:**
- `SubmitScore(playerName, score, callback)`
- `GetTopScores(count, callback)`

## 🎮 Examples

### Complete Auth + Leaderboard Demo
Located in: `Examples/AuthLeaderboardDemo/`

Features:
- Full authentication flow
- Score submission
- Leaderboard display
- User profiles

**Setup Guide:** See `Examples/AuthLeaderboardDemo/SETUP_GUIDE.md`

### Simple Leaderboard Demo
Located in: `Examples/LeaderboardDemo/`

Basic leaderboard implementation without authentication.

## 🔒 Firebase Security Rules

**IMPORTANT:** Configure your Firebase Realtime Database rules:

```json
{
  "rules": {
    "leaderboards": {
      "$leaderboardId": {
        ".read": "auth != null",
        "$userId": {
          ".write": "auth != null && auth.uid == $userId"
        }
      }
    },
    "users": {
      "$userId": {
        ".read": "auth != null",
        ".write": "auth != null && auth.uid == $userId"
      }
    }
  }
}
```

## 📊 Database Structure

### Leaderboards
```
leaderboards/
  {leaderboardId}/
    {userId}/
      playerName: string
      email: string
      score: number
      timestamp: timestamp
```

### User Profiles
```
users/
  {userId}/
    email: string
    displayName: string
    createdAt: timestamp
    totalScore: number
    gamesPlayed: number
```

## 🛠️ API Reference

### FirebaseManager Events

```csharp
// Subscribe to Firebase ready event
FirebaseManager.Instance.OnFirebaseReady += () => {
    Debug.Log("Firebase is ready!");
};

// Subscribe to sign in event
FirebaseManager.Instance.OnUserSignedIn += (user) => {
    Debug.Log($"User signed in: {user.Email}");
};

// Subscribe to sign out event
FirebaseManager.Instance.OnUserSignedOut += () => {
    Debug.Log("User signed out");
};
```

### Authentication Flow

```csharp
// Complete authentication example
AuthSystem auth = new AuthSystem();

// Register
auth.RegisterUser("user@example.com", "password123", (success, message) => {
    Debug.Log(message);
});

// Sign in
auth.SignInUser("user@example.com", "password123", (success, message) => {
    if (success) {
        // User is now signed in
        string userId = FirebaseManager.Instance.GetUserId();
        string email = FirebaseManager.Instance.GetUserEmail();
    }
});

// Update profile
auth.UpdateDisplayName("PlayerName", (success, message) => {
    Debug.Log(message);
});

// Sign out
auth.SignOut();
```

### Leaderboard Operations

```csharp
// Create leaderboard
LeaderboardSystem leaderboard = new LeaderboardSystem("global");

// Submit score
string playerName = FirebaseManager.Instance.GetUserDisplayName();
leaderboard.SubmitScore(playerName, 1000, success => {
    if (success) {
        // Score submitted
    }
});

// Get top 10 scores
leaderboard.GetTopScores(10, entries => {
    foreach (var entry in entries) {
        Debug.Log($"#{entry.rank} - {entry.playerName}: {entry.score}");
    }
});
```

## 🐛 Troubleshooting

### Common Issues

**Firebase not initializing:**
- Verify `google-services.json` is in `Assets/` folder
- Check Package Name matches Firebase console
- Ensure Firebase SDK is properly imported

**Authentication errors:**
- Enable Email/Password auth in Firebase Console
- Check network connectivity
- Verify Firebase API key is valid

**Leaderboard not working:**
- User must be signed in to submit scores
- Check Firebase security rules
- Verify database URL is correct

### Debug Logging

Enable debug logs in FirebaseManager Inspector:
- Check "Enable Debug Logs"

## 🗺️ Roadmap

### v0.2.0 (Planned)
- [ ] Cloud Save System
- [ ] Advanced Profile System
- [ ] Social Authentication (Google, Facebook)
- [ ] Multiple leaderboards management
- [ ] Real-time leaderboard updates

### v0.3.0 (Planned)
- [ ] Offline support
- [ ] Cloud Functions integration
- [ ] Analytics integration
- [ ] Push notifications

## 📝 Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history.

## 📄 License

[Your License Here]

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## 📧 Support

For issues and questions:
- Check the documentation
- Review example scenes
- Submit an issue on GitHub

---

**Built with ❤️ for Unity developers**
