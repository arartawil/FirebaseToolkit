// Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scripts/SaveSystemGameManager.cs
// Complete save/load example with UI integration

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FirebaseToolkit.Examples
{
    /// <summary>
    /// Complete example showing how to use SaveSystem with game state
    /// </summary>
    public class SaveSystemGameManager : MonoBehaviour
    {
        [Header("Game State")]
        public int playerLevel = 1;
        public int coins = 0;
        public int experience = 0;
        public float playtime = 0f;

        [Header("UI References - Stats")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI playtimeText;
        
        [Header("UI References - Action Buttons")]
        [SerializeField] private Button earnCoinsButton;
        [SerializeField] private Button gainXPButton;
        [SerializeField] private Button levelUpButton;
        
        [Header("UI References - Save Buttons")]
        [SerializeField] private Button saveSlot1Button;
        [SerializeField] private Button saveSlot2Button;
        [SerializeField] private Button saveSlot3Button;
        
        [Header("UI References - Load Buttons")]
        [SerializeField] private Button loadSlot1Button;
        [SerializeField] private Button loadSlot2Button;
        [SerializeField] private Button loadSlot3Button;
        
        [Header("UI References - Status")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Toggle autoSaveToggle;

        private SaveSystem.SaveSystem saveSystem;
        private SaveSystem.AutoSave autoSave;
        private float sessionStartTime;

        void Start()
        {
            sessionStartTime = Time.time;

            // Wait for Firebase to be ready
            FirebaseManager.Instance.OnFirebaseReady += OnFirebaseReady;

            if (FirebaseManager.Instance.IsReady)
            {
                OnFirebaseReady();
            }

            SetupButtons();
            UpdateUI();
        }

        void OnFirebaseReady()
        {
            Debug.Log("[SaveSystemGameManager] Firebase ready");

            if (!FirebaseManager.Instance.IsSignedIn)
            {
                ShowStatus("Please sign in to use Save System", Color.yellow);
                return;
            }

            InitializeSaveSystem();
        }

        void InitializeSaveSystem()
        {
            saveSystem = new SaveSystem.SaveSystem();
            
            // Setup auto-save
            autoSave = gameObject.AddComponent<SaveSystem.AutoSave>();
            autoSave.SetSaveInterval(30f); // Auto-save every 30 seconds
            
            if (autoSaveToggle != null)
            {
                autoSaveToggle.isOn = false; // Start with auto-save disabled
                autoSave.SetAutoSaveEnabled(false);
            }

            autoSave.OnAutoSaveComplete += (success) =>
            {
                if (success)
                    ShowStatus("Auto-saved!", Color.green);
            };

            ShowStatus("Save System ready!", Color.green);

            // Try to load autosave on start
            LoadFromSlot("autosave");
        }

        void SetupButtons()
        {
            if (earnCoinsButton != null)
                earnCoinsButton.onClick.AddListener(EarnCoins);

            if (gainXPButton != null)
                gainXPButton.onClick.AddListener(GainExperience);

            if (levelUpButton != null)
                levelUpButton.onClick.AddListener(LevelUp);

            if (saveSlot1Button != null)
                saveSlot1Button.onClick.AddListener(() => SaveToSlot("slot1"));

            if (saveSlot2Button != null)
                saveSlot2Button.onClick.AddListener(() => SaveToSlot("slot2"));

            if (saveSlot3Button != null)
                saveSlot3Button.onClick.AddListener(() => SaveToSlot("slot3"));

            if (loadSlot1Button != null)
                loadSlot1Button.onClick.AddListener(() => LoadFromSlot("slot1"));

            if (loadSlot2Button != null)
                loadSlot2Button.onClick.AddListener(() => LoadFromSlot("slot2"));

            if (loadSlot3Button != null)
                loadSlot3Button.onClick.AddListener(() => LoadFromSlot("slot3"));

            if (autoSaveToggle != null)
            {
                autoSaveToggle.onValueChanged.AddListener((enabled) =>
                {
                    if (autoSave != null)
                    {
                        autoSave.SetAutoSaveEnabled(enabled);
                        ShowStatus($"Auto-save {(enabled ? "enabled" : "disabled")}", Color.cyan);
                    }
                });
            }
        }

        void Update()
        {
            playtime += Time.deltaTime;
            UpdateUI();
        }

        void EarnCoins()
        {
            int amount = Random.Range(10, 100);
            coins += amount;
            ShowStatus($"+{amount} coins!", Color.yellow);
            UpdateUI();
        }

        void GainExperience()
        {
            int amount = Random.Range(5, 50);
            experience += amount;
            ShowStatus($"+{amount} XP!", Color.cyan);
            
            // Check for level up
            CheckLevelUp();
            UpdateUI();
        }

        void CheckLevelUp()
        {
            while (experience >= 100)
            {
                experience -= 100;
                playerLevel++;
                ShowStatus($"⭐ Level Up! Now level {playerLevel}", Color.green);
            }
        }

        void LevelUp()
        {
            if (experience >= 100)
            {
                playerLevel++;
                experience = 0;
                ShowStatus($"⭐ Level Up! Now level {playerLevel}", Color.green);
            }
            else
            {
                ShowStatus($"Need {100 - experience} more XP to level up", Color.red);
            }
            UpdateUI();
        }

        void SaveToSlot(string slotName)
        {
            if (saveSystem == null)
            {
                ShowStatus("Save System not initialized!", Color.red);
                return;
            }

            SaveSystem.SaveData data = new SaveSystem.SaveData
            {
                playerLevel = this.playerLevel,
                coins = this.coins,
                experience = this.experience,
                playtime = this.playtime,
                currentScene = "SaveSystemDemo",
                health = 100,
                maxHealth = 100
            };

            ShowStatus($"Saving to {slotName}...", Color.yellow);

            saveSystem.SaveGame(data, slotName, (success, message) =>
            {
                if (success)
                {
                    ShowStatus($"✓ Saved to {slotName}!", Color.green);
                    Debug.Log($"[SaveSystemGameManager] Saved to {slotName}");
                }
                else
                {
                    ShowStatus($"✗ Save failed: {message}", Color.red);
                    Debug.LogError($"[SaveSystemGameManager] Save failed: {message}");
                }
            });
        }

        void LoadFromSlot(string slotName)
        {
            if (saveSystem == null)
            {
                ShowStatus("Save System not initialized!", Color.red);
                return;
            }

            ShowStatus($"Loading from {slotName}...", Color.yellow);

            saveSystem.LoadGame(slotName, (data, success) =>
            {
                if (success && data != null)
                {
                    // Apply loaded data
                    this.playerLevel = data.playerLevel;
                    this.coins = data.coins;
                    this.experience = data.experience;
                    this.playtime = data.playtime;

                    ShowStatus($"✓ Loaded from {slotName}!", Color.green);
                    Debug.Log($"[SaveSystemGameManager] Loaded from {slotName}");
                    UpdateUI();
                }
                else
                {
                    // Only show error if not autosave (autosave might not exist on first run)
                    if (slotName != "autosave")
                    {
                        ShowStatus($"No save found in {slotName}", Color.red);
                        Debug.LogWarning($"[SaveSystemGameManager] No save in {slotName}");
                    }
                }
            });
        }

        void UpdateUI()
        {
            if (levelText != null)
                levelText.text = $"Level: {playerLevel}";

            if (coinsText != null)
                coinsText.text = $"Coins: {coins}";

            if (experienceText != null)
                experienceText.text = $"XP: {experience}/100";

            if (playtimeText != null)
                playtimeText.text = $"Playtime: {FormatTime(playtime)}";
        }

        string FormatTime(float seconds)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(seconds);
            
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}";
            else
                return $"{time.Minutes}:{time.Seconds:00}";
        }

        void ShowStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }

            Debug.Log($"[SaveSystemGameManager] {message}");
        }

        void OnDestroy()
        {
            FirebaseManager.Instance.OnFirebaseReady -= OnFirebaseReady;

            // Clean up button listeners
            if (earnCoinsButton != null) earnCoinsButton.onClick.RemoveAllListeners();
            if (gainXPButton != null) gainXPButton.onClick.RemoveAllListeners();
            if (levelUpButton != null) levelUpButton.onClick.RemoveAllListeners();
            
            if (saveSlot1Button != null) saveSlot1Button.onClick.RemoveAllListeners();
            if (saveSlot2Button != null) saveSlot2Button.onClick.RemoveAllListeners();
            if (saveSlot3Button != null) saveSlot3Button.onClick.RemoveAllListeners();
            
            if (loadSlot1Button != null) loadSlot1Button.onClick.RemoveAllListeners();
            if (loadSlot2Button != null) loadSlot2Button.onClick.RemoveAllListeners();
            if (loadSlot3Button != null) loadSlot3Button.onClick.RemoveAllListeners();

            if (autoSaveToggle != null) autoSaveToggle.onValueChanged.RemoveAllListeners();
        }
    }
}
