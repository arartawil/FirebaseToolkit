using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FirebaseToolkit.Auth
{
    /// <summary>
    /// UI Controller for Authentication
    /// </summary>
    public class AuthUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject forgotPasswordPanel;

        [Header("Login UI")]
        [SerializeField] private TMP_InputField loginEmail;
        [SerializeField] private TMP_InputField loginPassword;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button showRegisterButton;
        [SerializeField] private TextMeshProUGUI loginStatusText;

        [Header("Register UI")]
        [SerializeField] private TMP_InputField registerEmail;
        [SerializeField] private TMP_InputField registerPassword;
        [SerializeField] private TMP_InputField registerPasswordConfirm;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button showLoginButton;
        [SerializeField] private TextMeshProUGUI registerStatusText;

        [Header("Forgot Password UI")]
        [SerializeField] private TMP_InputField resetEmail;
        [SerializeField] private Button sendResetButton;
        [SerializeField] private Button backToLoginButton;
        [SerializeField] private TextMeshProUGUI resetStatusText;

        // Events
        public event Action OnLoginSuccess;
        public event Action OnRegisterSuccess;

        private AuthSystem authSystem;
        private bool isInitialized = false;

        void Start()
        {
            // Setup button listeners
            loginButton.onClick.AddListener(OnLoginClicked);
            registerButton.onClick.AddListener(OnRegisterClicked);
            sendResetButton.onClick.AddListener(OnSendResetClicked);
            
            showRegisterButton.onClick.AddListener(() => ShowPanel("register"));
            showLoginButton.onClick.AddListener(() => ShowPanel("login"));
            backToLoginButton.onClick.AddListener(() => ShowPanel("login"));

            // Show login panel by default
            ShowPanel("login");

            // Wait for Firebase to initialize
            FirebaseManager.Instance.OnFirebaseReady += OnFirebaseReady;
            
            if (FirebaseManager.Instance.IsReady)
            {
                OnFirebaseReady();
            }
        }

        void OnFirebaseReady()
        {
            authSystem = new AuthSystem();
            Debug.Log("[AuthUI] Firebase ready, please register or login");
        }

        void OnLoginClicked()
        {
            string email = loginEmail.text.Trim();
            string password = loginPassword.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowLoginStatus("Please fill in all fields", Color.red);
                return;
            }

            ShowLoginStatus("Signing in...", Color.yellow);
            loginButton.interactable = false;

            authSystem.SignInUser(email, password, (success, message) =>
            {
                loginButton.interactable = true;

                if (success)
                {
                    ShowLoginStatus(message, Color.green);
                    OnLoginSuccess?.Invoke();
                    
                    // Hide auth UI after successful login
                    Invoke("HideAuthUI", 0.5f);
                }
                else
                {
                    ShowLoginStatus(message, Color.red);
                }
            });
        }

        void OnRegisterClicked()
        {
            string email = registerEmail.text.Trim();
            string password = registerPassword.text;
            string confirmPassword = registerPasswordConfirm.text;

            // Validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowRegisterStatus("Please fill in all fields", Color.red);
                return;
            }

            if (password != confirmPassword)
            {
                ShowRegisterStatus("Passwords do not match", Color.red);
                return;
            }

            if (password.Length < 6)
            {
                ShowRegisterStatus("Password must be at least 6 characters", Color.red);
                return;
            }

            ShowRegisterStatus("Creating account...", Color.yellow);
            registerButton.interactable = false;

            authSystem.RegisterUser(email, password, (success, message) =>
            {
                registerButton.interactable = true;

                if (success)
                {
                    ShowRegisterStatus(message, Color.green);
                    OnRegisterSuccess?.Invoke();
                    
                    // Auto-login after registration
                    Invoke("HideAuthUI", 0.5f);
                }
                else
                {
                    ShowRegisterStatus(message, Color.red);
                }
            });
        }

        void OnSendResetClicked()
        {
            string email = resetEmail.text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                ShowResetStatus("Please enter your email", Color.red);
                return;
            }

            ShowResetStatus("Sending reset email...", Color.yellow);
            sendResetButton.interactable = false;

            authSystem.SendPasswordResetEmail(email, (success, message) =>
            {
                sendResetButton.interactable = true;

                if (success)
                {
                    ShowResetStatus(message, Color.green);
                    resetEmail.text = "";
                    
                    // Go back to login after 2 seconds
                    Invoke("ShowLoginPanel", 2f);
                }
                else
                {
                    ShowResetStatus(message, Color.red);
                }
            });
        }

        void ShowPanel(string panelName)
        {
            loginPanel.SetActive(panelName == "login");
            registerPanel.SetActive(panelName == "register");
            forgotPasswordPanel.SetActive(panelName == "forgot");

            // Clear status texts
            ClearAllStatus();
        }

        void ShowLoginPanel()
        {
            ShowPanel("login");
        }

        void HideAuthUI()
        {
            gameObject.SetActive(false);
        }

        void ShowLoginStatus(string message, Color color)
        {
            loginStatusText.text = message;
            loginStatusText.color = color;
        }

        void ShowRegisterStatus(string message, Color color)
        {
            registerStatusText.text = message;
            registerStatusText.color = color;
        }

        void ShowResetStatus(string message, Color color)
        {
            resetStatusText.text = message;
            resetStatusText.color = color;
        }

        void ClearAllStatus()
        {
            loginStatusText.text = "";
            registerStatusText.text = "";
            resetStatusText.text = "";
        }

        void OnDestroy()
        {
            if (FirebaseManager.Instance != null)
            {
                FirebaseManager.Instance.OnFirebaseReady -= OnFirebaseReady;
            }

            loginButton.onClick.RemoveListener(OnLoginClicked);
            registerButton.onClick.RemoveListener(OnRegisterClicked);
            sendResetButton.onClick.RemoveListener(OnSendResetClicked);
        }
    }
}
