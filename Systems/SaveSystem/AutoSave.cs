// Assets/FirebaseToolkit/Systems/SaveSystem/AutoSave.cs

using UnityEngine;
using System;

namespace FirebaseToolkit.SaveSystem
{
    /// <summary>
    /// Auto-save component - attach to GameObject for automatic saving
    /// </summary>
    public class AutoSave : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool enableAutoSave = true;
        [SerializeField] private float saveInterval = 30f; // Save every 30 seconds
        [SerializeField] private string autoSaveSlot = "autosave";

        [Header("Status")]
        [SerializeField] private float timeSinceLastSave = 0f;
        [SerializeField] private bool isSaving = false;

        public event Action<bool> OnAutoSaveComplete;

        private SaveSystem saveSystem;
        private float playtimeStart;

        void Start()
        {
            saveSystem = new SaveSystem();
            playtimeStart = Time.time;

            if (enableAutoSave)
            {
                Debug.Log($"[AutoSave] Enabled - will save every {saveInterval} seconds");
            }
        }

        void Update()
        {
            if (!enableAutoSave || isSaving)
                return;

            timeSinceLastSave += Time.deltaTime;

            if (timeSinceLastSave >= saveInterval)
            {
                PerformAutoSave();
            }
        }

        /// <summary>
        /// Manually trigger auto-save
        /// </summary>
        public void PerformAutoSave()
        {
            if (isSaving)
            {
                Debug.LogWarning("[AutoSave] Already saving, skipping...");
                return;
            }

            isSaving = true;
            timeSinceLastSave = 0f;

            Debug.Log("[AutoSave] Performing auto-save...");

            // Get current game state
            SaveData data = CollectGameData();

            // Save to cloud
            saveSystem.SaveGame(data, autoSaveSlot, (success, message) =>
            {
                isSaving = false;

                if (success)
                {
                    Debug.Log("[AutoSave] Auto-save successful");
                }
                else
                {
                    Debug.LogError($"[AutoSave] Failed: {message}");
                }

                OnAutoSaveComplete?.Invoke(success);
            });
        }

        /// <summary>
        /// Collect current game data
        /// Override this in your game-specific class
        /// </summary>
        protected virtual SaveData CollectGameData()
        {
            SaveData data = new SaveData();

            // Update playtime
            data.playtime = Time.time - playtimeStart;

            // Try to get data from GameManager (if exists)
            GameObject gameManagerObj = GameObject.Find("GameManager");
            if (gameManagerObj != null)
            {
                // You can get data from your GameManager here
                // Example: GameManager gm = gameManagerObj.GetComponent<GameManager>();
                // data.playerLevel = gm.playerLevel;
            }

            return data;
        }

        /// <summary>
        /// Enable/disable auto-save at runtime
        /// </summary>
        public void SetAutoSaveEnabled(bool enabled)
        {
            enableAutoSave = enabled;
            Debug.Log($"[AutoSave] {(enabled ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// Change save interval
        /// </summary>
        public void SetSaveInterval(float interval)
        {
            saveInterval = interval;
            Debug.Log($"[AutoSave] Interval changed to {interval} seconds");
        }

        void OnApplicationQuit()
        {
            // Save on quit
            if (enableAutoSave && !isSaving)
            {
                Debug.Log("[AutoSave] Saving on quit...");
                PerformAutoSave();
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Save on pause (mobile)
            if (pauseStatus && enableAutoSave && !isSaving)
            {
                Debug.Log("[AutoSave] Saving on pause...");
                PerformAutoSave();
            }
        }
    }
}
