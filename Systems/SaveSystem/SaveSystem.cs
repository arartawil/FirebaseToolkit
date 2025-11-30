// Assets/FirebaseToolkit/Systems/SaveSystem/SaveSystem.cs

using UnityEngine;
using Firebase.Database;
using System;
using System.Collections.Generic;

namespace FirebaseToolkit.SaveSystem
{
    /// <summary>
    /// Cloud save system using Firebase Realtime Database
    /// </summary>
    public class SaveSystem
    {
        private DatabaseReference saveRef;
        private FirebaseManager firebaseManager;
        private string userId;

        public SaveSystem()
        {
            firebaseManager = FirebaseManager.Instance;

            if (!firebaseManager.IsReady)
            {
                Debug.LogError("[SaveSystem] Firebase not initialized!");
                return;
            }

            if (!firebaseManager.IsSignedIn)
            {
                Debug.LogError("[SaveSystem] User must be signed in to use save system!");
                return;
            }

            userId = firebaseManager.GetUserId();
            saveRef = firebaseManager.GetDatabaseReference($"saves/{userId}");
        }

        /// <summary>
        /// Save game data to cloud
        /// </summary>
        /// <param name="data">Game data to save</param>
        /// <param name="slotName">Save slot name (e.g., "slot1", "autosave")</param>
        /// <param name="onComplete">Callback with success status</param>
        public void SaveGame(SaveData data, string slotName, Action<bool, string> onComplete = null)
        {
            if (data == null)
            {
                Debug.LogError("[SaveSystem] Cannot save null data");
                onComplete?.Invoke(false, "Data is null");
                return;
            }

            // Update metadata
            data.slotName = slotName;
            data.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            Debug.Log($"[SaveSystem] Saving game to slot: {slotName}");

            // Convert to JSON
            string json = JsonUtility.ToJson(data, true);

            // Save to Firebase
            saveRef.Child(slotName).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[SaveSystem] Save failed: {task.Exception}");
                    onComplete?.Invoke(false, "Failed to save game");
                }
                else
                {
                    Debug.Log($"[SaveSystem] Game saved successfully to {slotName}");
                    
                    // Also save metadata separately for quick loading
                    SaveMetadata(new SaveMetadata(data), slotName);
                    
                    onComplete?.Invoke(true, "Game saved successfully");
                }
            });
        }

        /// <summary>
        /// Load game data from cloud
        /// </summary>
        /// <param name="slotName">Save slot name</param>
        /// <param name="onComplete">Callback with loaded data</param>
        public void LoadGame(string slotName, Action<SaveData, bool> onComplete)
        {
            Debug.Log($"[SaveSystem] Loading game from slot: {slotName}");

            saveRef.Child(slotName).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[SaveSystem] Load failed: {task.Exception}");
                    onComplete?.Invoke(null, false);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (!snapshot.Exists)
                {
                    Debug.LogWarning($"[SaveSystem] No save found in slot: {slotName}");
                    onComplete?.Invoke(null, false);
                    return;
                }

                try
                {
                    // Parse JSON
                    string json = snapshot.GetRawJsonValue();
                    SaveData data = JsonUtility.FromJson<SaveData>(json);

                    Debug.Log($"[SaveSystem] Game loaded successfully from {slotName}");
                    onComplete?.Invoke(data, true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveSystem] Failed to parse save data: {e.Message}");
                    onComplete?.Invoke(null, false);
                }
            });
        }

        /// <summary>
        /// Get metadata for a save slot (fast, doesn't load full data)
        /// </summary>
        public void GetSaveMetadata(string slotName, Action<SaveMetadata, bool> onComplete)
        {
            saveRef.Child($"{slotName}_metadata").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || !task.Result.Exists)
                {
                    onComplete?.Invoke(null, false);
                    return;
                }

                try
                {
                    string json = task.Result.GetRawJsonValue();
                    SaveMetadata metadata = JsonUtility.FromJson<SaveMetadata>(json);
                    onComplete?.Invoke(metadata, true);
                }
                catch
                {
                    onComplete?.Invoke(null, false);
                }
            });
        }

        /// <summary>
        /// Get all save slots for current user
        /// </summary>
        public void GetAllSaves(Action<List<SaveMetadata>> onComplete)
        {
            Debug.Log("[SaveSystem] Loading all save slots...");

            saveRef.GetValueAsync().ContinueWith(task =>
            {
                List<SaveMetadata> saves = new List<SaveMetadata>();

                if (task.IsFaulted || !task.Result.Exists)
                {
                    onComplete?.Invoke(saves);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot child in snapshot.Children)
                {
                    // Skip metadata entries
                    if (child.Key.EndsWith("_metadata"))
                        continue;

                    try
                    {
                        string json = child.GetRawJsonValue();
                        SaveData data = JsonUtility.FromJson<SaveData>(json);
                        saves.Add(new SaveMetadata(data));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to parse save slot {child.Key}: {e.Message}");
                    }
                }

                // Sort by timestamp (newest first)
                saves.Sort((a, b) => b.timestamp.CompareTo(a.timestamp));

                Debug.Log($"[SaveSystem] Found {saves.Count} save slots");
                onComplete?.Invoke(saves);
            });
        }

        /// <summary>
        /// Delete a save slot
        /// </summary>
        public void DeleteSave(string slotName, Action<bool> onComplete = null)
        {
            Debug.Log($"[SaveSystem] Deleting save slot: {slotName}");

            saveRef.Child(slotName).RemoveValueAsync().ContinueWith(task =>
            {
                bool success = !task.IsFaulted;

                if (success)
                {
                    // Also delete metadata
                    saveRef.Child($"{slotName}_metadata").RemoveValueAsync();
                    Debug.Log($"[SaveSystem] Save slot deleted: {slotName}");
                }
                else
                {
                    Debug.LogError($"[SaveSystem] Failed to delete save: {task.Exception}");
                }

                onComplete?.Invoke(success);
            });
        }

        /// <summary>
        /// Check if a save slot exists
        /// </summary>
        public void SaveExists(string slotName, Action<bool> onComplete)
        {
            saveRef.Child(slotName).GetValueAsync().ContinueWith(task =>
            {
                bool exists = !task.IsFaulted && task.Result.Exists;
                onComplete?.Invoke(exists);
            });
        }

        /// <summary>
        /// Save metadata separately for quick loading
        /// </summary>
        private void SaveMetadata(SaveMetadata metadata, string slotName)
        {
            string json = JsonUtility.ToJson(metadata);
            saveRef.Child($"{slotName}_metadata").SetRawJsonValueAsync(json);
        }
    }
}
