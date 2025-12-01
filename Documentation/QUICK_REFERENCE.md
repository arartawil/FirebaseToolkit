# Firebase Toolkit v0.2.0 - Quick Reference

## 🚀 Installation Checklist

- [ ] Import Firebase SDK (Auth + Database)
- [ ] Add `google-services.json` to `Assets/`
- [ ] Copy FirebaseToolkit to project
- [ ] Enable Email/Password auth in Firebase Console
- [ ] Set up database security rules
- [ ] Add FirebaseManager to scene
- [ ] Configure save system paths (optional)

## 📝 Code Snippets

### Initialize Firebase
```csharp
// Automatic initialization (default)
// FirebaseManager initializes on Awake

// Manual initialization
FirebaseManager.Instance.InitializeFirebase();

// Check if ready
if (FirebaseManager.Instance.IsReady) {
    // Firebase is ready
}
```

### Authentication

#### Register User
```csharp
using FirebaseToolkit.Auth;

AuthSystem auth = new AuthSystem();
auth.RegisterUser("user@email.com", "password", (success, message) => {
    Debug.Log(message);
});
```

#### Sign In
```csharp
auth.SignInUser("user@email.com", "password", (success, message) => {
    if (success) {
        // User signed in
    }
});
```

#### Sign Out
```csharp
auth.SignOut();
```

#### Check Auth Status
```csharp
if (FirebaseManager.Instance.IsSignedIn) {
    string email = FirebaseManager.Instance.GetUserEmail();
    string name = FirebaseManager.Instance.GetUserDisplayName();
}
```

### Leaderboard

#### Create Leaderboard
```csharp
using FirebaseToolkit.Leaderboard;

LeaderboardSystem leaderboard = new LeaderboardSystem("global");
```

#### Submit Score
```csharp
string name = FirebaseManager.Instance.GetUserDisplayName();
leaderboard.SubmitScore(name, 1000, success => {
    if (success) {
        Debug.Log("Score submitted!");
    }
});
```

#### Get Top Scores
```csharp
leaderboard.GetTopScores(10, entries => {
    foreach (var entry in entries) {
        Debug.Log($"#{entry.rank} {entry.playerName}: {entry.score}");
    }
});
```

### Events

#### Subscribe to Events
```csharp
void Start() {
    FirebaseManager.Instance.OnFirebaseReady += OnFirebaseReady;
    FirebaseManager.Instance.OnUserSignedIn += OnUserSignedIn;
    FirebaseManager.Instance.OnUserSignedOut += OnUserSignedOut;
}

void OnFirebaseReady() {
    Debug.Log("Firebase ready!");
}

void OnUserSignedIn(Firebase.Auth.FirebaseUser user) {
    Debug.Log($"Signed in: {user.Email}");
}

void OnUserSignedOut() {
    Debug.Log("Signed out");
}

void OnDestroy() {
    // Unsubscribe
    FirebaseManager.Instance.OnFirebaseReady -= OnFirebaseReady;
    FirebaseManager.Instance.OnUserSignedIn -= OnUserSignedIn;
    FirebaseManager.Instance.OnUserSignedOut -= OnUserSignedOut;
}
```

## 💾 Save System

### Save Game
```csharp
using FirebaseToolkit.SaveSystem;

SaveSystem.SaveSystem saveSystem = new SaveSystem.SaveSystem();

SaveSystem.SaveData data = new SaveSystem.SaveData
{
    playerLevel = 5,
    coins = 1000,
    health = 80,
    experience = 250
};

saveSystem.SaveGame(data, "slot1", (success, message) => {
    Debug.Log(message);
});
```

### Load Game
```csharp
saveSystem.LoadGame("slot1", (data, success) => {
    if (success) {
        playerLevel = data.playerLevel;
        coins = data.coins;
    }
});
```

### Get All Saves
```csharp
saveSystem.GetAllSaves((saves) => {
    foreach (var save in saves) {
        Debug.Log($"{save.slotName}: Level {save.playerLevel}, {save.GetFormattedPlaytime()}");
    }
});
```

### Delete Save
```csharp
saveSystem.DeleteSave("slot2", (success) => {
    if (success) Debug.Log("Deleted");
});
```

### Auto-Save
```csharp
AutoSave autoSave = gameObject.AddComponent<AutoSave>();
autoSave.SetAutoSaveEnabled(true);
autoSave.SetSaveInterval(30f); // Every 30 seconds

autoSave.OnAutoSaveComplete += (success) => {
    if (success) ShowNotification("Auto-saved");
};
```

## 🔒 Firebase Security Rules

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
    },
    "saves": {
      "$userId": {
        ".read": "auth != null && auth.uid == $userId",
        ".write": "auth != null && auth.uid == $userId"
      }
    }
  }
}
```

## 📊 Database Paths

### Leaderboards
```
leaderboards/{leaderboardId}/{userId}
```

### Saves
```
saves/{userId}/{slotName}
saves/{userId}/{slotName}_metadata
```

### User Profiles
```
users/{userId}
```

## 🎯 Common Tasks

### Get User Info
```csharp
string userId = FirebaseManager.Instance.GetUserId();
string email = FirebaseManager.Instance.GetUserEmail();
string name = FirebaseManager.Instance.GetUserDisplayName();
FirebaseUser user = FirebaseManager.Instance.CurrentUser;
```

### Update Display Name
```csharp
AuthSystem auth = new AuthSystem();
auth.UpdateDisplayName("NewName", (success, message) => {
    Debug.Log(message);
});
```

### Create Multiple Leaderboards
```csharp
LeaderboardSystem global = new LeaderboardSystem("global");
LeaderboardSystem daily = new LeaderboardSystem("daily");
LeaderboardSystem level1 = new LeaderboardSystem("level1");
```

### Submit Score for Current User
```csharp
void SubmitScore(long score) {
    if (!FirebaseManager.Instance.IsSignedIn) {
        Debug.Log("Must be signed in");
        return;
    }
    
    string name = FirebaseManager.Instance.GetUserDisplayName();
    leaderboard.SubmitScore(name, score, success => {
        if (success) {
            // Refresh leaderboard
            LoadLeaderboard();
        }
    });
}
```

## 🐛 Common Errors & Solutions

### "Firebase not initialized"
**Solution:** Wait for `OnFirebaseReady` event
```csharp
FirebaseManager.Instance.OnFirebaseReady += Initialize;
```

### "User must be signed in"
**Solution:** Check authentication before submitting
```csharp
if (FirebaseManager.Instance.IsSignedIn) {
    // Submit score
}
```

### "Email already in use"
**Solution:** User should sign in instead of registering

### "Invalid email"
**Solution:** Validate email format

### "Password too weak"
**Solution:** Password must be at least 6 characters

## 📱 Scene Setup Quick Steps

1. Create Canvas
2. Add FirebaseManager GameObject
3. Add AuthUI panels (Login, Register, Profile)
4. Add GameController
5. Assign all UI references
6. Test in Play mode

## 🎮 Testing Workflow

1. **Start scene** → Shows login panel
2. **Register** → Create test account
3. **Login** → Sign in with account
4. **Submit score** → Enter and submit
5. **View leaderboard** → See your score
6. **Sign out** → Return to login
7. **Login again** → See persistent data

## 💡 Best Practices

### Always Check Ready State
```csharp
if (FirebaseManager.Instance.IsReady) {
    // Safe to use Firebase
}
```

### Always Check Auth State
```csharp
if (FirebaseManager.Instance.IsSignedIn) {
    // Safe to submit scores
}
```

### Unsubscribe from Events
```csharp
void OnDestroy() {
    // Clean up event subscriptions
}
```

### Handle Callbacks
```csharp
auth.SignInUser(email, password, (success, message) => {
    if (success) {
        // Success path
    } else {
        // Error path - show message to user
    }
});
```

## 📦 Required Packages

- **Firebase Auth** (FirebaseAuth.unitypackage)
- **Firebase Database** (FirebaseDatabase.unitypackage)
- **TextMeshPro** (Unity Package Manager)

## 🔗 Useful Links

- Firebase Console: https://console.firebase.google.com
- Firebase Unity SDK: https://firebase.google.com/download/unity
- Documentation: See README.md
- Setup Guide: See Examples/AuthLeaderboardDemo/SETUP_GUIDE.md

## 📞 Need Help?

1. Check console for error messages
2. Review setup guide
3. Verify all references are assigned
4. Check Firebase Console settings
5. Review security rules

---

**Firebase Toolkit v0.1.0** | Built for Unity 2020.3+
