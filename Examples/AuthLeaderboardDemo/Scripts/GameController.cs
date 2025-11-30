using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FirebaseToolkit.Auth;
using FirebaseToolkit.Leaderboard;

namespace FirebaseToolkit.Examples
{
    public class GameController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject authUI;
        [SerializeField] private GameObject gameUI;
        
        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI welcomeText;
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private Button addScoreButton;
        [SerializeField] private Button submitScoreButton;
        [SerializeField] private Button signOutButton;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Leaderboard UI")]
        [SerializeField] private TextMeshProUGUI leaderboardText;
        [SerializeField] private TextMeshProUGUI myRankText;
        [SerializeField] private Button refreshLeaderboardButton;

        private AuthSystem authSystem;
        private LeaderboardSystem leaderboard;
        private AuthUI authUIComponent;
        
        private long currentScore = 0;

        void Start()
        {
            // Get AuthUI component
            authUIComponent = authUI.GetComponent<AuthUI>();
            if (authUIComponent != null)
            {
                authUIComponent.OnLoginSuccess += OnUserSignedIn;
                authUIComponent.OnRegisterSuccess += OnUserSignedIn;
            }

            // Setup buttons
            addScoreButton.onClick.AddListener(OnAddScoreClicked);
            submitScoreButton.onClick.AddListener(OnSubmitScoreClicked);
            signOutButton.onClick.AddListener(OnSignOutClicked);
            refreshLeaderboardButton.onClick.AddListener(RefreshLeaderboard);

            // Start with auth UI visible, game UI hidden
            ShowAuthUI();

            // Wait for Firebase to initialize
            FirebaseManager.Instance.OnFirebaseReady += OnFirebaseReady;
            
            if (FirebaseManager.Instance.IsReady)
            {
                OnFirebaseReady();
            }
        }

        void OnFirebaseReady()
        {
            // Initialize systems after Firebase is ready
            authSystem = new AuthSystem();
            leaderboard = new LeaderboardSystem("global");

            Debug.Log("[GameController] Firebase ready, waiting for user to login...");
        }

        void OnUserSignedIn()
        {
            ShowGameUI();
            
            string displayName = FirebaseManager.Instance.GetUserDisplayName();
            welcomeText.text = $"Welcome, {displayName}!";
            
            RefreshLeaderboard();
            UpdateMyRank();
        }

        void OnAddScoreClicked()
        {
            // Simulate earning points
            int points = Random.Range(10, 100);
            currentScore += points;
            
            currentScoreText.text = $"Current Score: {currentScore:N0}";
            ShowStatus($"+{points} points!", Color.green);
        }

        void OnSubmitScoreClicked()
        {
            if (currentScore == 0)
            {
                ShowStatus("No score to submit!", Color.red);
                return;
            }

            ShowStatus("Submitting score...", Color.yellow);
            submitScoreButton.interactable = false;

            leaderboard.SubmitScore(currentScore, (success, message) =>
            {
                submitScoreButton.interactable = true;

                if (success)
                {
                    ShowStatus($"Score submitted: {currentScore:N0}", Color.green);
                    currentScore = 0;
                    currentScoreText.text = "Current Score: 0";
                    
                    RefreshLeaderboard();
                    UpdateMyRank();
                }
                else
                {
                    ShowStatus(message, Color.red);
                }
            });
        }

        void OnSignOutClicked()
        {
            authSystem.SignOut();
            currentScore = 0;
            currentScoreText.text = "Current Score: 0";
            ShowAuthUI();
        }

        void RefreshLeaderboard()
        {
            leaderboard.GetTopScores(10, (entries) =>
            {
                if (entries.Count == 0)
                {
                    leaderboardText.text = "No scores yet!\nBe the first to submit.";
                    return;
                }

                string display = "=== TOP 10 SCORES ===\n\n";
                
                string myUserId = FirebaseManager.Instance.GetUserId();
                
                foreach (var entry in entries)
                {
                    string highlight = entry.userId == myUserId ? " ⭐" : "";
                    display += $"#{entry.rank}  {entry.displayName}  -  {entry.score:N0}{highlight}\n";
                }

                leaderboardText.text = display;
            });
        }

        void UpdateMyRank()
        {
            leaderboard.GetMyRank((rank, entry) =>
            {
                if (rank == -1)
                {
                    myRankText.text = "Your Rank: Not ranked yet";
                }
                else
                {
                    myRankText.text = $"Your Rank: #{rank} - {entry.score:N0} points";
                }
            });
        }

        void ShowAuthUI()
        {
            authUI.SetActive(true);
            gameUI.SetActive(false);
        }

        void ShowGameUI()
        {
            authUI.SetActive(false);
            gameUI.SetActive(true);
        }

        void ShowStatus(string message, Color color)
        {
            statusText.text = message;
            statusText.color = color;
            
            CancelInvoke("ClearStatus");
            Invoke("ClearStatus", 3f);
        }

        void ClearStatus()
        {
            statusText.text = "";
        }

        void OnDestroy()
        {
            if (FirebaseManager.Instance != null)
            {
                FirebaseManager.Instance.OnFirebaseReady -= OnFirebaseReady;
            }

            if (authUIComponent != null)
            {
                authUIComponent.OnLoginSuccess -= OnUserSignedIn;
                authUIComponent.OnRegisterSuccess -= OnUserSignedIn;
            }

            addScoreButton.onClick.RemoveListener(OnAddScoreClicked);
            submitScoreButton.onClick.RemoveListener(OnSubmitScoreClicked);
            signOutButton.onClick.RemoveListener(OnSignOutClicked);
            refreshLeaderboardButton.onClick.RemoveListener(RefreshLeaderboard);
        }
    }
}
