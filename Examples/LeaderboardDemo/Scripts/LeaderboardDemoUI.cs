using UnityEngine;
using UnityEngine.UI;
using FirebaseToolkit.Leaderboard;
using System.Collections.Generic;
using TMPro;

namespace FirebaseToolkit.Examples
{
    /// <summary>
    /// UI Controller for Leaderboard Demo Scene
    /// Handles user input and displays leaderboard data
    /// </summary>
    public class LeaderboardDemoUI : MonoBehaviour
    {
        [Header("Submit Score UI")]
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private InputField scoreInput;
        [SerializeField] private Button submitButton;
        [SerializeField] private Text statusText;

        [Header("Leaderboard Display UI")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Transform scoresListContent;
        [SerializeField] private GameObject scoreEntryPrefab;

        [Header("Settings")]
        [SerializeField] private int topScoresCount = 10;
        [SerializeField] private string leaderboardId = "global";

        private LeaderboardSystem leaderboardSystem;

        void Start()
        {
            // Wait for Firebase to be ready
            FirebaseManager.Instance.OnFirebaseReady += InitializeLeaderboard;
            
            if (FirebaseManager.Instance.IsReady)
            {
                InitializeLeaderboard();
            }

            // Setup button listeners
            if (submitButton != null)
                submitButton.onClick.AddListener(OnSubmitScore);

            if (refreshButton != null)
                refreshButton.onClick.AddListener(OnRefreshLeaderboard);

            UpdateStatus("Initializing Firebase...");
        }

        private void InitializeLeaderboard()
        {
            leaderboardSystem = new LeaderboardSystem(leaderboardId);
            UpdateStatus("Ready to submit scores!");
            OnRefreshLeaderboard();
        }

        public void OnSubmitScore()
        {
            if (leaderboardSystem == null)
            {
                UpdateStatus("Firebase not ready!");
                return;
            }

            string playerName = playerNameInput?.text ?? "Player";
            if (string.IsNullOrEmpty(playerName))
            {
                UpdateStatus("Please enter a player name!");
                return;
            }

            if (!long.TryParse(scoreInput?.text, out long score))
            {
                UpdateStatus("Please enter a valid score!");
                return;
            }

            UpdateStatus("Submitting score...");

            leaderboardSystem.SubmitScore(score, (success, message) =>
            {
                if (success)
                {
                    UpdateStatus($"Score submitted: {score}");
                    OnRefreshLeaderboard();
                }
                else
                {
                    UpdateStatus($"Failed to submit score: {message}");
                }
            });
        }

        public void OnRefreshLeaderboard()
        {
            if (leaderboardSystem == null)
            {
                UpdateStatus("Leaderboard not initialized!");
                return;
            }

            UpdateStatus("Loading leaderboard...");

            leaderboardSystem.GetTopScores(topScoresCount, entries =>
            {
                DisplayLeaderboard(entries);
                UpdateStatus($"Loaded {entries.Count} entries");
            });
        }

        private void DisplayLeaderboard(List<LeaderboardEntry> entries)
        {
            // Clear existing entries
            if (scoresListContent != null)
            {
                foreach (Transform child in scoresListContent)
                {
                    Destroy(child.gameObject);
                }

                // Create new entries
                foreach (var entry in entries)
                {
                    GameObject entryObj;
                    
                    if (scoreEntryPrefab != null)
                    {
                        entryObj = Instantiate(scoreEntryPrefab, scoresListContent);
                    }
                    else
                    {
                        // Create simple text entry if no prefab
                        entryObj = new GameObject($"Entry_{entry.rank}");
                        entryObj.transform.SetParent(scoresListContent);
                        var text = entryObj.AddComponent<Text>();
                        text.text = entry.ToString();
                        text.color = Color.white;
                        text.fontSize = 20;
                    }

                    // Try to set text if it has a Text component
                    var textComponent = entryObj.GetComponent<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = entry.ToString();
                    }
                }
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"[LeaderboardDemo] {message}");
        }

        void OnDestroy()
        {
            if (FirebaseManager.Instance != null)
            {
                FirebaseManager.Instance.OnFirebaseReady -= InitializeLeaderboard;
            }
        }
    }
}
