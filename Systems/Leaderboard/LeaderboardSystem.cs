using UnityEngine;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirebaseToolkit.Leaderboard
{
    public class LeaderboardSystem
    {
        private DatabaseReference leaderboardRef;
        private string leaderboardId;
        private FirebaseManager firebaseManager;

        public LeaderboardSystem(string leaderboardId = "global")
        {
            this.leaderboardId = leaderboardId;
            firebaseManager = FirebaseManager.Instance;

            if (!firebaseManager.IsReady)
            {
                Debug.LogError("[LeaderboardSystem] Firebase not initialized!");
                return;
            }

            leaderboardRef = firebaseManager.GetDatabaseReference($"leaderboards/{leaderboardId}");
        }

        /// <summary>
        /// Submit score for currently signed-in user
        /// </summary>
        public void SubmitScore(long score, Action<bool, string> onComplete = null)
        {
            if (!firebaseManager.IsSignedIn)
            {
                Debug.LogError("[LeaderboardSystem] User must be signed in to submit scores");
                onComplete?.Invoke(false, "Please sign in first");
                return;
            }

            string userId = firebaseManager.GetUserId();
            string displayName = firebaseManager.GetUserDisplayName();

            var scoreData = new Dictionary<string, object>
            {
                ["displayName"] = displayName,
                ["score"] = score,
                ["timestamp"] = ServerValue.Timestamp,
                ["userId"] = userId,
                ["email"] = firebaseManager.GetUserEmail()
            };

            leaderboardRef.Child(userId).SetValueAsync(scoreData).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[LeaderboardSystem] Failed to submit: {task.Exception}");
                    onComplete?.Invoke(false, "Failed to submit score");
                }
                else
                {
                    Debug.Log($"[LeaderboardSystem] Score submitted: {displayName} - {score}");
                    
                    // Update user stats
                    UpdateUserStats(userId, score);
                    
                    onComplete?.Invoke(true, "Score submitted successfully!");
                }
            });
        }

        public void GetTopScores(int count, Action<List<LeaderboardEntry>> onComplete)
        {
            leaderboardRef
                .OrderByChild("score")
                .LimitToLast(count)
                .GetValueAsync()
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("[LeaderboardSystem] Failed to get scores");
                        onComplete?.Invoke(new List<LeaderboardEntry>());
                        return;
                    }

                    DataSnapshot snapshot = task.Result;
                    List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

                    if (!snapshot.Exists)
                    {
                        onComplete?.Invoke(entries);
                        return;
                    }

                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        try
                        {
                            var entry = new LeaderboardEntry
                            {
                                userId = child.Key,
                                displayName = child.Child("displayName").Value?.ToString() ?? "Unknown",
                                score = long.Parse(child.Child("score").Value?.ToString() ?? "0"),
                                timestamp = child.Child("timestamp").Exists
                                    ? long.Parse(child.Child("timestamp").Value.ToString())
                                    : 0
                            };
                            entries.Add(entry);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to parse entry: {e.Message}");
                        }
                    }

                    // Sort descending
                    entries = entries.OrderByDescending(e => e.score).ToList();
                    
                    // Assign ranks
                    for (int i = 0; i < entries.Count; i++)
                    {
                        entries[i].rank = i + 1;
                    }

                    onComplete?.Invoke(entries);
                });
        }

        /// <summary>
        /// Get current user's rank
        /// </summary>
        public void GetMyRank(Action<int, LeaderboardEntry> onComplete)
        {
            if (!firebaseManager.IsSignedIn)
            {
                onComplete?.Invoke(-1, null);
                return;
            }

            string userId = firebaseManager.GetUserId();
            
            leaderboardRef.Child(userId).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || !task.Result.Exists)
                {
                    onComplete?.Invoke(-1, null);
                    return;
                }

                DataSnapshot playerSnapshot = task.Result;
                long playerScore = long.Parse(playerSnapshot.Child("score").Value.ToString());

                var playerEntry = new LeaderboardEntry
                {
                    userId = userId,
                    displayName = playerSnapshot.Child("displayName").Value?.ToString() ?? "You",
                    score = playerScore
                };

                // Count higher scores
                leaderboardRef
                    .OrderByChild("score")
                    .StartAt(playerScore + 1)
                    .GetValueAsync()
                    .ContinueWith(rankTask =>
                    {
                        int betterScores = (int)rankTask.Result.ChildrenCount;
                        int rank = betterScores + 1;
                        
                        playerEntry.rank = rank;
                        onComplete?.Invoke(rank, playerEntry);
                    });
            });
        }

        /// <summary>
        /// Update user statistics
        /// </summary>
        private void UpdateUserStats(string userId, long score)
        {
            var userRef = firebaseManager.GetDatabaseReference($"users/{userId}");
            
            // Increment games played
            userRef.Child("gamesPlayed").RunTransaction(mutableData =>
            {
                int current = mutableData.Value != null ? int.Parse(mutableData.Value.ToString()) : 0;
                mutableData.Value = current + 1;
                return TransactionResult.Success(mutableData);
            });

            // Update total score
            userRef.Child("totalScore").RunTransaction(mutableData =>
            {
                long current = mutableData.Value != null ? long.Parse(mutableData.Value.ToString()) : 0;
                mutableData.Value = current + score;
                return TransactionResult.Success(mutableData);
            });
        }
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public string userId;
        public string displayName;
        public long score;
        public int rank;
        public long timestamp;

        public override string ToString()
        {
            return $"#{rank} {displayName}: {score:N0}";
        }
    }
}
