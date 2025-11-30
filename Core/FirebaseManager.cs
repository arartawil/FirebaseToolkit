using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using System.Collections;

namespace FirebaseToolkit
{
    /// <summary>
    /// Central manager for Firebase initialization and configuration
    /// Singleton pattern for easy access throughout the project
    /// </summary>
    public class FirebaseManager : MonoBehaviour
    {
        private static FirebaseManager instance;
        public static FirebaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("FirebaseManager");
                    instance = go.AddComponent<FirebaseManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("Configuration")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool enableDebugLogs = true;

        [Header("Status")]
        [SerializeField] private bool isInitialized = false;
        [SerializeField] private bool isReady = false;

        private FirebaseApp firebaseApp;
        private FirebaseDatabase database;
        private FirebaseAuth auth;

        // Events
        public event Action OnFirebaseReady;
        public event Action<string> OnFirebaseError;
        public event Action<FirebaseUser> OnUserSignedIn;
        public event Action OnUserSignedOut;

        // Properties
        public bool IsInitialized => isInitialized;
        public bool IsReady => isReady;
        public FirebaseDatabase Database => database;
        public FirebaseAuth Auth => auth;
        public FirebaseUser CurrentUser => auth?.CurrentUser;
        public bool IsSignedIn => CurrentUser != null;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            if (autoInitialize)
            {
                InitializeFirebase();
            }
        }

        public void InitializeFirebase()
        {
            if (isInitialized)
            {
                Log("Firebase already initialized");
                return;
            }

            Log("Initializing Firebase...");
            StartCoroutine(InitializeFirebaseAsync());
        }

        private IEnumerator InitializeFirebaseAsync()
        {
            var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

            // Wait until the task completes
            yield return new WaitUntil(() => dependencyTask.IsCompleted);

            var dependencyStatus = dependencyTask.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebaseServices();
            }
            else
            {
                LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
                OnFirebaseError?.Invoke($"Dependency error: {dependencyStatus}");
            }
        }

        private void InitializeFirebaseServices()
        {
            try
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;
                database.SetPersistenceEnabled(true);
                auth = FirebaseAuth.DefaultInstance;

                // Subscribe to auth state changes
                auth.StateChanged += OnAuthStateChanged;

                isInitialized = true;
                isReady = true;

                Log("Firebase initialized successfully!");
                OnFirebaseReady?.Invoke();

                // Check if user is already signed in
                if (CurrentUser != null)
                {
                    Log($"User already signed in: {CurrentUser.Email}");
                    OnUserSignedIn?.Invoke(CurrentUser);
                }
            }
            catch (Exception e)
            {
                LogError($"Firebase initialization failed: {e.Message}");
                OnFirebaseError?.Invoke(e.Message);
            }
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            if (CurrentUser != null)
            {
                Log($"User signed in: {CurrentUser.Email}");
                OnUserSignedIn?.Invoke(CurrentUser);
            }
            else
            {
                Log("User signed out");
                OnUserSignedOut?.Invoke();
            }
        }

        public DatabaseReference GetDatabaseReference(string path)
        {
            if (!isReady)
            {
                LogError("Firebase not ready");
                return null;
            }
            return database.GetReference(path);
        }

        public string GetUserId()
        {
            return CurrentUser?.UserId ?? SystemInfo.deviceUniqueIdentifier;
        }

        public string GetUserEmail()
        {
            return CurrentUser?.Email ?? "Anonymous";
        }

        public string GetUserDisplayName()
        {
            return CurrentUser?.DisplayName ?? CurrentUser?.Email ?? "Player";
        }

        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[FirebaseToolkit] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[FirebaseToolkit ERROR] {message}");
        }

        void OnDestroy()
        {
            if (auth != null)
            {
                auth.StateChanged -= OnAuthStateChanged;
            }

            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
