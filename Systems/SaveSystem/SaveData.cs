// Assets/FirebaseToolkit/Systems/SaveSystem/SaveData.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseToolkit.SaveSystem
{
    /// <summary>
    /// Base class for game save data
    /// Extend this with your game-specific data
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // Basic info
        public string saveVersion = "1.0";
        public long timestamp;
        public string slotName;
        
        // Game progress
        public int playerLevel;
        public float playtime; // Total playtime in seconds
        public Vector3 playerPosition;
        public string currentScene;
        
        // Player stats
        public int health;
        public int maxHealth;
        public int coins;
        public int experience;
        
        // Inventory
        public List<string> inventory = new List<string>();
        public string equippedWeapon;
        public string equippedArmor;
        
        // Quest progress
        public List<string> completedQuests = new List<string>();
        public Dictionary<string, int> questProgress = new Dictionary<string, int>();
        
        // Settings
        public float masterVolume = 1.0f;
        public float musicVolume = 1.0f;
        public float sfxVolume = 1.0f;
        public int graphicsQuality = 2;
        
        // Custom data (use this for game-specific data)
        public Dictionary<string, object> customData = new Dictionary<string, object>();

        public SaveData()
        {
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Convert to JSON string
        /// </summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Create from JSON string
        /// </summary>
        public static SaveData FromJson(string json)
        {
            return JsonUtility.FromJson<SaveData>(json);
        }

        /// <summary>
        /// Get readable timestamp
        /// </summary>
        public DateTime GetDateTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        /// <summary>
        /// Get formatted playtime (e.g., "2h 15m")
        /// </summary>
        public string GetFormattedPlaytime()
        {
            TimeSpan time = TimeSpan.FromSeconds(playtime);
            
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}h {time.Minutes}m";
            else if (time.TotalMinutes >= 1)
                return $"{(int)time.TotalMinutes}m {time.Seconds}s";
            else
                return $"{time.Seconds}s";
        }
    }

    /// <summary>
    /// Metadata about a save (without full data)
    /// Used for displaying save slots
    /// </summary>
    [Serializable]
    public class SaveMetadata
    {
        public string slotName;
        public long timestamp;
        public int playerLevel;
        public float playtime;
        public string currentScene;
        public string saveVersion;

        public SaveMetadata(SaveData data)
        {
            slotName = data.slotName;
            timestamp = data.timestamp;
            playerLevel = data.playerLevel;
            playtime = data.playtime;
            currentScene = data.currentScene;
            saveVersion = data.saveVersion;
        }

        public DateTime GetDateTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        public string GetFormattedPlaytime()
        {
            TimeSpan time = TimeSpan.FromSeconds(playtime);
            
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}h {time.Minutes}m";
            else if (time.TotalMinutes >= 1)
                return $"{(int)time.TotalMinutes}m";
            else
                return $"{time.Seconds}s";
        }
    }
}
