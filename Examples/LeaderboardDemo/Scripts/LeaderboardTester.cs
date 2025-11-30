using UnityEngine;
using UnityEngine.UI;
using FirebaseToolkit.Leaderboard;
using TMPro;

namespace FirebaseToolkit.Examples
{
    public class LeaderboardTester : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_InputField playerNameInput;
        public TMP_InputField scoreInput;
        public Button submitButton;
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI leaderboardText;
        public Button refreshButton;

        private LeaderboardSystem leaderboard;

        void Start()
        {
            // Validate UI references
            if (submitButton == null) Debug.LogError("[LeaderboardTester] Submit Button not assigned!");
            if (refreshButton == null) Debug.LogError("[LeaderboardTester] Refresh Button not assigned!");
            if (playerNameInput == null) Debug.LogError("[LeaderboardTester] Player Name Input not assigned!");
            if (scoreInput == null) Debug.LogError("[LeaderboardTester] Score Input not assigned!");
            if (statusText == null) Debug.LogError("[LeaderboardTester] Status Text not assigned!");
            if (leaderboardText == null) Debug.LogError("[LeaderboardTester] Leaderboard Text not assigned!");

            // Wait for Firebase to initialize
            FirebaseManager.Instance.OnFirebaseReady += OnFirebaseReady;
            
            if (FirebaseManager.Instance.IsReady)
            {
                OnFirebaseReady();
            }

            // Setup buttons
            if (submitButton != null)
                submitButton.onClick.AddListener(OnSubmitClicked);
            
            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(OnRefreshClicked);
                Debug.Log("[LeaderboardTester] Refresh button listener added");
            }

            // Set default name
            if (playerNameInput != null)
                playerNameInput.text = $"Player_{Random.Range(1000, 9999)}";
        }

        void OnFirebaseReady()
        {
            leaderboard = new LeaderboardSystem("demo");
            UpdateStatus("Firebase Ready!", Color.green);
            RefreshLeaderboard();
        }

        void OnSubmitClicked()
        {
            if (!long.TryParse(scoreInput.text, out long score))
            {
                UpdateStatus("Invalid score!", Color.red);
                return;
            }

            UpdateStatus("Submitting...", Color.yellow);

            leaderboard.SubmitScore(score, (success, message) =>
            {
                if (success)
                {
                    UpdateStatus("Score submitted!", Color.green);
                    RefreshLeaderboard();
                    scoreInput.text = "";
                }
                else
                {
                    UpdateStatus($"Submit failed: {message}", Color.red);
                }
            });
        }

        void OnRefreshClicked()
        {
            Debug.Log("[LeaderboardTester] Refresh button clicked!");
            UpdateStatus("Refreshing...", Color.yellow);
            RefreshLeaderboard();
        }

        void RefreshLeaderboard()
        {
            if (leaderboard == null)
            {
                Debug.LogWarning("[LeaderboardTester] Leaderboard is null, cannot refresh");
                UpdateStatus("Waiting for Firebase...", Color.yellow);
                return;
            }

            Debug.Log("[LeaderboardTester] Fetching top scores...");

            leaderboard.GetTopScores(10, (entries) =>
            {
                Debug.Log($"[LeaderboardTester] Received {entries.Count} entries");
                
                if (entries.Count == 0)
                {
                    Debug.Log("[LeaderboardTester] No scores found in leaderboard");
                    if (leaderboardText != null)
                        leaderboardText.text = "No scores yet!\nBe the first to submit.";
                    UpdateStatus("No scores found", Color.white);
                    return;
                }

                string display = "=== TOP SCORES ===\n\n";
                
                Debug.Log("========== LEADERBOARD ==========");
                foreach (var entry in entries)
                {
                    string line = $"#{entry.rank}  {entry.displayName}  -  {entry.score:N0}";
                    display += line + "\n";
                    Debug.Log(line);
                }
                Debug.Log("=================================");

                if (leaderboardText != null)
                    leaderboardText.text = display;
                
                UpdateStatus($"Loaded {entries.Count} scores", Color.green);
            });
        }

        void UpdateStatus(string message, Color color)
        {
            statusText.text = message;
            statusText.color = color;
        }

        void OnDestroy()
        {
            if (FirebaseManager.Instance != null)
            {
                FirebaseManager.Instance.OnFirebaseReady -= OnFirebaseReady;
            }

            if (submitButton != null)
                submitButton.onClick.RemoveListener(OnSubmitClicked);
            
            if (refreshButton != null)
                refreshButton.onClick.RemoveListener(OnRefreshClicked);
        }
    }
}
