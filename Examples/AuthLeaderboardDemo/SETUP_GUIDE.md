# Firebase Toolkit v0.1.0 - Auth + Leaderboard Demo Setup Guide

## 🎯 What's Included in v0.1.0

✅ **Email/Password Authentication**
- User registration with validation
- Sign in with email/password
- Sign out functionality
- Password reset (future)

✅ **Authenticated Leaderboards**
- Only authenticated users can submit scores
- User profiles integrated with leaderboard
- Top 10 scores display
- Real-time updates

✅ **Complete UI System**
- Login panel
- Registration panel
- Profile panel
- Game/Leaderboard panel

## 📁 Project Structure

```
Assets/FirebaseToolkit/
├── Core/
│   └── FirebaseManager.cs (UPDATED with auth events)
├── Systems/
│   ├── Auth/
│   │   ├── AuthSystem.cs (NEW)
│   │   └── AuthUI.cs (NEW)
│   └── Leaderboard/
│       └── LeaderboardSystem.cs (UPDATED with auth)
└── Examples/
    └── AuthLeaderboardDemo/
        ├── Scenes/
        │   └── AuthLeaderboardDemo.unity
        ├── Scripts/
        │   └── GameController.cs (NEW)
        └── Prefabs/
```

## 🚀 Quick Setup Guide

### Step 1: Create New Scene

1. In Unity, go to `Assets/FirebaseToolkit/Examples/AuthLeaderboardDemo/Scenes/`
2. Create a new scene: **Right-click → Create → Scene**
3. Name it: `AuthLeaderboardDemo`
4. Double-click to open it

### Step 2: Add FirebaseManager

1. In **Hierarchy**: Right-click → **Create Empty**
2. Name it: `FirebaseManager`
3. **Add Component** → Search `FirebaseManager`
4. Add the script

### Step 3: Create Auth UI

#### A. Create Auth Panel
1. **Hierarchy** → Right-click → **UI → Canvas**
2. Right-click **Canvas** → **UI → Panel**
3. Rename to: `AuthPanel`

#### B. Login Panel (Child of AuthPanel)
1. Right-click **AuthPanel** → **UI → Panel**
2. Name: `LoginPanel`
3. Add children:
   - **UI → Text (TextMeshPro)** → `Title` → Text: "Login"
   - **UI → Input Field (TextMeshPro)** → `EmailInput` → Placeholder: "Email"
   - **UI → Input Field (TextMeshPro)** → `PasswordInput` → Placeholder: "Password"
     - Set Content Type: **Password**
   - **UI → Button** → `LoginButton` → Text: "Login"
   - **UI → Button** → `ShowRegisterButton` → Text: "Create Account"
   - **UI → Text (TextMeshPro)** → `StatusText` → Text: ""

#### C. Register Panel (Child of AuthPanel)
1. Right-click **AuthPanel** → **UI → Panel**
2. Name: `RegisterPanel`
3. Add children:
   - **UI → Text (TextMeshPro)** → `Title` → Text: "Register"
   - **UI → Input Field (TextMeshPro)** → `EmailInput` → Placeholder: "Email"
   - **UI → Input Field (TextMeshPro)** → `PasswordInput` → Placeholder: "Password"
     - Set Content Type: **Password**
   - **UI → Input Field (TextMeshPro)** → `PasswordConfirmInput` → Placeholder: "Confirm Password"
     - Set Content Type: **Password**
   - **UI → Button** → `RegisterButton` → Text: "Register"
   - **UI → Button** → `ShowLoginButton` → Text: "Back to Login"
   - **UI → Text (TextMeshPro)** → `StatusText` → Text: ""

#### D. Profile Panel (Child of AuthPanel)
1. Right-click **AuthPanel** → **UI → Panel**
2. Name: `ProfilePanel`
3. Add children:
   - **UI → Text (TextMeshPro)** → `DisplayNameText` → Text: "Welcome!"
   - **UI → Text (TextMeshPro)** → `EmailText` → Text: "Email: "
   - **UI → Button** → `SignOutButton` → Text: "Sign Out"

### Step 4: Add AuthUI Script

1. Select **AuthPanel** in Hierarchy
2. **Add Component** → Search `AuthUI`
3. Assign all references:
   - **Panels:**
     - Login Panel → Drag `LoginPanel`
     - Register Panel → Drag `RegisterPanel`
     - Profile Panel → Drag `ProfilePanel`
   - **Login UI:**
     - Login Email → Drag `LoginPanel/EmailInput`
     - Login Password → Drag `LoginPanel/PasswordInput`
     - Login Button → Drag `LoginPanel/LoginButton`
     - Show Register Button → Drag `LoginPanel/ShowRegisterButton`
     - Login Status Text → Drag `LoginPanel/StatusText`
   - **Register UI:**
     - Register Email → Drag `RegisterPanel/EmailInput`
     - Register Password → Drag `RegisterPanel/PasswordInput`
     - Register Password Confirm → Drag `RegisterPanel/PasswordConfirmInput`
     - Register Button → Drag `RegisterPanel/RegisterButton`
     - Show Login Button → Drag `RegisterPanel/ShowLoginButton`
     - Register Status Text → Drag `RegisterPanel/StatusText`
   - **Profile UI:**
     - Profile Email Text → Drag `ProfilePanel/EmailText`
     - Profile Display Name Text → Drag `ProfilePanel/DisplayNameText`
     - Sign Out Button → Drag `ProfilePanel/SignOutButton`

### Step 5: Create Game Panel

1. Right-click **Canvas** → **UI → Panel**
2. Name: `GamePanel`
3. Add children:
   - **UI → Text (TextMeshPro)** → `WelcomeText` → Text: "Welcome!"
   - **UI → Text (TextMeshPro)** → `Title` → Text: "Submit Your Score"
   - **UI → Input Field (TextMeshPro)** → `ScoreInput` → Placeholder: "Enter Score"
     - Content Type: **Integer Number**
   - **UI → Button** → `SubmitScoreButton` → Text: "Submit Score"
   - **UI → Text (TextMeshPro)** → `StatusText` → Text: ""
   
4. Add Leaderboard section:
   - **UI → Text (TextMeshPro)** → `LeaderboardTitle` → Text: "Top Scores"
   - **UI → Scroll View** → `LeaderboardScrollView`
     - This creates Viewport and Content automatically
   - **UI → Button** → `RefreshButton` → Text: "Refresh Leaderboard"

### Step 6: Add GameController Script

1. Create empty GameObject in Hierarchy
2. Name: `GameController`
3. **Add Component** → Search `GameController`
4. Assign references:
   - **UI Panels:**
     - Auth Panel → Drag `AuthPanel`
     - Game Panel → Drag `GamePanel`
   - **Game UI:**
     - Welcome Text → Drag `GamePanel/WelcomeText`
     - Score Input → Drag `GamePanel/ScoreInput`
     - Submit Score Button → Drag `GamePanel/SubmitScoreButton`
     - Refresh Leaderboard Button → Drag `GamePanel/RefreshButton`
     - Status Text → Drag `GamePanel/StatusText`
     - Leaderboard Content → Drag `GamePanel/LeaderboardScrollView/Viewport/Content`
   - **Settings:**
     - Leaderboard Id: `global` (or any name you want)

### Step 7: Configure Panels Layout

#### Auth Panel Layout (Left or Center)
- Position: Center or left side of screen
- Size: 400x600
- Initially Visible: **YES**
- LoginPanel: Active by default
- RegisterPanel: Inactive
- ProfilePanel: Inactive

#### Game Panel Layout (Full Screen or Right Side)
- Position: Full screen or right side
- Size: Fill remaining space
- Initially Visible: **NO** (will show after login)

## 🎮 How to Use

### 1. Register New Account
1. Run the scene
2. Click "Create Account"
3. Enter email and password (min 6 characters)
4. Confirm password
5. Click "Register"
6. Account is created and you're automatically signed in

### 2. Sign In
1. Enter your email and password
2. Click "Login"
3. Game panel appears after successful login

### 3. Submit Score
1. Enter a score number
2. Click "Submit Score"
3. Your score is saved with your user profile

### 4. View Leaderboard
1. Leaderboard loads automatically after login
2. Click "Refresh Leaderboard" to update
3. Shows top 10 scores with player names and emails

### 5. Sign Out
1. Click "Sign Out" button
2. Returns to login screen

## 🔒 Firebase Security Rules

**IMPORTANT:** Update your Firebase Realtime Database rules:

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

This ensures:
- Only authenticated users can read leaderboards
- Users can only write their own scores
- Users can only modify their own profile

## 📊 Database Structure

### Leaderboards
```
leaderboards/
  global/
    userId1/
      playerName: "Player1"
      email: "player1@example.com"
      score: 1000
      timestamp: 1234567890
      userId: "userId1"
    userId2/
      ...
```

### User Profiles
```
users/
  userId1/
    email: "player1@example.com"
    displayName: "Player1"
    createdAt: 1234567890
    totalScore: 1000
    gamesPlayed: 5
```

## 🐛 Troubleshooting

### "Firebase not ready"
- Make sure FirebaseManager GameObject exists in scene
- Check Console for Firebase initialization errors
- Verify google-services.json is in Assets/ folder

### "User must be signed in"
- Only authenticated users can submit scores
- Make sure you're logged in before submitting

### UI References Missing
- Double-check all UI assignments in Inspector
- Make sure TextMeshPro is imported (Package Manager)

### Authentication Errors
- "Email already in use" → Try logging in instead
- "Invalid email" → Check email format
- "Password too weak" → Min 6 characters required
- "Wrong password" → Check your password
- "User not found" → Register first

## 🎨 Customization

### Change Leaderboard ID
In GameController:
- Change `leaderboardId` to create separate leaderboards
- Examples: "daily", "weekly", "level1", etc.

### Modify Top Scores Count
In GameController `LoadLeaderboard()`:
```csharp
leaderboardSystem.GetTopScores(20, entries => // Show top 20
```

### Style the UI
- Modify colors, fonts, sizes in Unity Inspector
- Create custom prefabs for leaderboard entries
- Add icons, backgrounds, animations

## 🚀 Next Steps

Ready to build? Check out:
- Save System (coming soon)
- Profile System with avatars
- Multiple leaderboards
- Real-time updates
- Social features

## 📝 Version History

### v0.1.0 (Current)
- ✅ Email/Password Authentication
- ✅ User Registration & Sign In
- ✅ Authenticated Leaderboards
- ✅ User Profile Integration
- ✅ Complete UI System
- ✅ Example Scene

---

**Happy Coding! 🔥**
