using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;

namespace FirebaseToolkit.Auth
{
    /// <summary>
    /// Handles Firebase Authentication operations
    /// </summary>
    public class AuthSystem
    {
        private FirebaseAuth auth;
        private FirebaseManager firebaseManager;

        public FirebaseUser CurrentUser => auth?.CurrentUser;
        public bool IsSignedIn => CurrentUser != null;

        public AuthSystem()
        {
            firebaseManager = FirebaseManager.Instance;
            
            if (!firebaseManager.IsReady)
            {
                Debug.LogError("[AuthSystem] Firebase not initialized!");
                return;
            }

            auth = firebaseManager.Auth;
        }

        /// <summary>
        /// Register new user with email and password
        /// </summary>
        public void RegisterUser(string email, string password, Action<bool, string> onComplete)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                onComplete?.Invoke(false, "Email and password cannot be empty");
                return;
            }

            if (password.Length < 6)
            {
                onComplete?.Invoke(false, "Password must be at least 6 characters");
                return;
            }

            Debug.Log($"[AuthSystem] Registering user: {email}");

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("[AuthSystem] Registration was cancelled");
                    onComplete?.Invoke(false, "Registration cancelled");
                    return;
                }

                if (task.IsFaulted)
                {
                    string errorMessage = ParseAuthError(task.Exception);
                    Debug.LogError($"[AuthSystem] Registration failed: {errorMessage}");
                    onComplete?.Invoke(false, errorMessage);
                    return;
                }

                // Registration successful
                AuthResult authResult = task.Result;
                FirebaseUser newUser = authResult.User;
                Debug.Log($"[AuthSystem] User registered successfully: {newUser.Email}");
                
                // Create user profile in database
                CreateUserProfile(newUser.UserId, email);
                
                onComplete?.Invoke(true, "Registration successful!");
            });
        }

        /// <summary>
        /// Sign in existing user with email and password
        /// </summary>
        public void SignInUser(string email, string password, Action<bool, string> onComplete)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                onComplete?.Invoke(false, "Email and password cannot be empty");
                return;
            }

            Debug.Log($"[AuthSystem] Signing in user: {email}");

            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("[AuthSystem] Sign in was cancelled");
                    onComplete?.Invoke(false, "Sign in cancelled");
                    return;
                }

                if (task.IsFaulted)
                {
                    string errorMessage = ParseAuthError(task.Exception);
                    Debug.LogError($"[AuthSystem] Sign in failed: {errorMessage}");
                    onComplete?.Invoke(false, errorMessage);
                    return;
                }

                // Sign in successful
                AuthResult authResult = task.Result;
                FirebaseUser user = authResult.User;
                Debug.Log($"[AuthSystem] User signed in successfully: {user.Email}");
                onComplete?.Invoke(true, "Sign in successful!");
            });
        }

        /// <summary>
        /// Sign out current user
        /// </summary>
        public void SignOut()
        {
            if (CurrentUser == null)
            {
                Debug.LogWarning("[AuthSystem] No user signed in");
                return;
            }

            string userEmail = CurrentUser.Email;
            auth.SignOut();
            Debug.Log($"[AuthSystem] User signed out: {userEmail}");
        }

        /// <summary>
        /// Send password reset email
        /// </summary>
        public void SendPasswordResetEmail(string email, Action<bool, string> onComplete)
        {
            if (string.IsNullOrEmpty(email))
            {
                onComplete?.Invoke(false, "Email cannot be empty");
                return;
            }

            auth.SendPasswordResetEmailAsync(email).ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    string errorMessage = ParseAuthError(task.Exception);
                    onComplete?.Invoke(false, errorMessage);
                    return;
                }

                Debug.Log($"[AuthSystem] Password reset email sent to: {email}");
                onComplete?.Invoke(true, "Password reset email sent!");
            });
        }

        /// <summary>
        /// Update user display name
        /// </summary>
        public void UpdateDisplayName(string displayName, Action<bool, string> onComplete)
        {
            if (CurrentUser == null)
            {
                onComplete?.Invoke(false, "No user signed in");
                return;
            }

            UserProfile profile = new UserProfile
            {
                DisplayName = displayName
            };

            CurrentUser.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    onComplete?.Invoke(false, "Failed to update profile");
                    return;
                }

                Debug.Log($"[AuthSystem] Display name updated: {displayName}");
                
                // Update in database too
                UpdateUserProfileInDatabase(CurrentUser.UserId, displayName);
                
                onComplete?.Invoke(true, "Display name updated!");
            });
        }

        /// <summary>
        /// Create user profile in database
        /// </summary>
        private void CreateUserProfile(string userId, string email)
        {
            var userData = new System.Collections.Generic.Dictionary<string, object>
            {
                ["email"] = email,
                ["displayName"] = email.Split('@')[0], // Use email prefix as default name
                ["createdAt"] = ServerValue.Timestamp,
                ["totalScore"] = 0,
                ["gamesPlayed"] = 0
            };

            firebaseManager.GetDatabaseReference($"users/{userId}")
                .SetValueAsync(userData)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        Debug.Log("[AuthSystem] User profile created in database");
                    }
                });
        }

        /// <summary>
        /// Update user profile in database
        /// </summary>
        private void UpdateUserProfileInDatabase(string userId, string displayName)
        {
            firebaseManager.GetDatabaseReference($"users/{userId}/displayName")
                .SetValueAsync(displayName);
        }

        /// <summary>
        /// Parse Firebase auth errors into user-friendly messages
        /// </summary>
        private string ParseAuthError(AggregateException exception)
        {
            if (exception == null) return "Unknown error";

            Firebase.FirebaseException firebaseEx = null;
            
            foreach (Exception e in exception.InnerExceptions)
            {
                firebaseEx = e as Firebase.FirebaseException;
                if (firebaseEx != null) break;
            }

            if (firebaseEx == null)
                return exception.InnerException?.Message ?? "Unknown error";

            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return "This email is already registered";
                
                case AuthError.InvalidEmail:
                    return "Invalid email address";
                
                case AuthError.WeakPassword:
                    return "Password is too weak (min 6 characters)";
                
                case AuthError.WrongPassword:
                    return "Incorrect password";
                
                case AuthError.UserNotFound:
                    return "No account found with this email";
                
                case AuthError.NetworkRequestFailed:
                    return "Network error. Check your connection";
                
                case AuthError.TooManyRequests:
                    return "Too many attempts. Please try again later";

                case AuthError.UserDisabled:
                    return "This account has been disabled";

                default:
                    return $"Error: {errorCode}";
            }
        }
    }
}
