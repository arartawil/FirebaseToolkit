# Changelog

All notable changes to Firebase Toolkit for Unity will be documented in this file.

## [0.1.0] - 2024-11-30

### 🎉 Initial Release

### Added - Core Systems
- **FirebaseManager**: Central singleton for Firebase initialization
  - Auto-initialization on scene load
  - Dependency check and error handling
  - Coroutine-based async initialization
  - Authentication state monitoring
  - User session management

- **AuthSystem**: Complete email/password authentication
  - User registration with validation
  - Email/password sign in
  - Sign out functionality
  - Password reset email
  - Display name updates
  - User-friendly error messages
  - Automatic user profile creation in database

- **LeaderboardSystem**: Authenticated leaderboard functionality
  - Submit scores (authenticated users only)
  - Get top scores with rankings
  - User profile integration
  - Email and display name tracking
  - Timestamp recording
  - Automatic ranking calculation

### Added - UI Components
- **AuthUI**: Complete authentication interface
  - Login panel with email/password
  - Registration panel with password confirmation
  - Profile panel with user info
  - Sign out button
  - Auto-switching between panels
  - Status messages with color coding
  - Field validation

- **GameController**: Demo game integration
  - Auth/Game panel management
  - Score submission interface
  - Leaderboard display with scroll view
  - Welcome message with user name
  - Auto-refresh on login
  - Status updates

### Added - Examples
- **AuthLeaderboardDemo**: Complete working example
  - Full authentication flow
  - Score submission after login
  - Top 10 leaderboard display
  - User profile integration
  - Comprehensive setup guide
  - UI layout examples

- **LeaderboardDemo**: Basic leaderboard example
  - Simple score submission
  - Leaderboard display
  - Without authentication (legacy)

### Added - Documentation
- Comprehensive README.md with:
  - Installation instructions
  - Quick start guide
  - API reference
  - Code examples
  - Troubleshooting guide
  
- SETUP_GUIDE.md for demo scene:
  - Step-by-step Unity setup
  - UI creation instructions
  - Component assignments
  - Firebase security rules
  - Database structure
  - Common issues and solutions

### Added - Events
- `OnFirebaseReady`: Firebase initialization complete
- `OnFirebaseError`: Firebase initialization error
- `OnUserSignedIn`: User successfully signed in
- `OnUserSignedOut`: User signed out
- `OnLoginSuccess`: UI login success
- `OnLogout`: UI logout

### Features
#### Authentication
- ✅ Email validation
- ✅ Password strength check (min 6 characters)
- ✅ Password confirmation matching
- ✅ User-friendly error messages
- ✅ Auto-login after registration
- ✅ Persistent sessions
- ✅ Profile creation in database

#### Leaderboard
- ✅ Authenticated score submission only
- ✅ Top N scores retrieval
- ✅ Automatic ranking
- ✅ Player name display
- ✅ Email tracking
- ✅ Timestamp recording
- ✅ Ordered by highest score
- ✅ User-specific entries

#### Database Structure
- ✅ Leaderboards organized by ID
- ✅ User profiles with metadata
- ✅ Server timestamps
- ✅ Normalized data structure

### Technical Details
- Unity version: 2020.3+
- Firebase SDK: Auth + Realtime Database
- UI Framework: TextMeshPro
- Architecture: Singleton pattern
- Async handling: Coroutines + Task callbacks

### Known Limitations
- Single leaderboard per LeaderboardSystem instance
- No offline support yet
- No real-time updates (manual refresh required)
- No social authentication providers yet

### Security
- Implemented authentication-only leaderboard access
- User-specific write permissions
- Server-side timestamp validation
- Database security rules documentation

### Breaking Changes
- N/A (Initial release)

### Migration Guide
- N/A (Initial release)

---

## [Unreleased]

### Planned for v0.2.0
- Cloud save system
- Advanced profile management
- Social authentication (Google, Facebook)
- Multiple leaderboards support
- Real-time leaderboard updates
- Offline data persistence
- Achievement system

### Planned for v0.3.0
- Cloud Functions integration
- Push notifications
- Analytics integration
- Custom authentication
- File storage support
- Team/clan system

---

## Version History

- **v0.1.0** (2024-11-30) - Initial release with Auth + Leaderboard
