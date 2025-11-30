// Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scripts/SaveDemoManager.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FirebaseToolkit.Examples
{
    /// <summary>
    /// Demo manager for SaveSystem - shows how to save/load game state
    /// </summary>
    public class SaveDemoManager : MonoBehaviour
    {
        [Header("Game State")]
        [SerializeField] private int playerLevel = 1;
        [SerializeField] private int coins = 0;
        [SerializeField] private int health = 100;
        [SerializeField] private int experience = 0;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TMP_InputField slotNameInput;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Buttons")]
        [SerializeField] private Button addCoinsButton;
        [SerializeField] private Button addExpButton;
        [SerializeField] private Button takeDamageButton;
        [SerializeField] private Button healButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

        private SaveSystem.SaveSystem saveSystem;
        private float playtimeStart;

        void Start()
        {
            // Wait for Firebase to be ready
            FirebaseManager.Instance.OnFirebaseReady += OnFirebaseReady;

            if (FirebaseManager.Instance.IsReady)
            {
                OnFirebaseReady();
            }

            playtimeStart = Time.time;
            UpdateUI();
            SetupButtons();
        }

        void OnFirebaseReady()
        {
            Debug.Log("[SaveDemo] Firebase ready, initializing SaveSystem");
            
            if (FirebaseManager.Instance.IsSignedIn)
            {
                saveSystem = new SaveSystem.SaveSystem();
                ShowStatus("SaveSystem ready!", Color.green);
            }
            else
            {
                ShowStatus("Please sign in to use SaveSystem", Color.yellow);
            }
        }

        void SetupButtons()
        {
            if (addCoinsButton != null)
                addCoinsButton.onClick.AddListener(() => AddCoins(50));

            if (addExpButton != null)
                addExpButton.onClick.AddListener(() => AddExperience(100));

            if (takeDamageButton != null)
                takeDamageButton.onClick.AddListener(() => TakeDamage(20));

            if (healButton != null)
                healButton.onClick.AddListener(() => Heal(30));

            if (saveButton != null)
                saveButton.onClick.AddListener(SaveGame);

            if (loadButton != null)
                loadButton.onClick.AddListener(LoadGame);
        }

        void AddCoins(int amount)
        {
            coins += amount;
            UpdateUI();
            ShowStatus($"Added {amount} coins!", Color.green);
        }

        void AddExperience(int amount)
        {
            experience += amount;
            
            // Level up every 500 exp
            while (experience >= 500)
            {
                experience -= 500;
                playerLevel++;
                ShowStatus($"Level Up! Now level {playerLevel}", Color.yellow);
            }

            UpdateUI();
        }

        void TakeDamage(int amount)
        {
            health = Mathf.Max(0, health - amount);
            UpdateUI();
            ShowStatus($"Took {amount} damage!", Color.red);

            if (health == 0)
            {
                ShowStatus("You died! Game Over", Color.red);
            }
        }

        void Heal(int amount)
        {
            health = Mathf.Min(100, health + amount);
            UpdateUI();
            ShowStatus($"Healed {amount} HP!", Color.green);
        }

        void SaveGame()
        {
            if (saveSystem == null)
            {
                ShowStatus("SaveSystem not initialized!", Color.red);
                return;
            }

            string slotName = slotNameInput != null && !string.IsNullOrEmpty(slotNameInput.text) 
                ? slotNameInput.text 
                : "slot1";

            ShowStatus("Saving...", Color.yellow);

            // Create save data
            SaveSystem.SaveData data = new SaveSystem.SaveData
            {
                playerLevel = playerLevel,
                coins = coins,
                health = health,
                experience = experience,
                maxHealth = 100,
                playtime = Time.time - playtimeStart,
                currentScene = "SaveSystemDemo",
                playerPosition = Vector3.zero
            };

            // Save to cloud
            saveSystem.SaveGame(data, slotName, (success, message) =>
            {
                if (success)
                {
                    ShowStatus($"Game saved to '{slotName}'!", Color.green);
                    Debug.Log($"[SaveDemo] Game saved successfully to {slotName}");
                }
                else
                {
                    ShowStatus($"Save failed: {message}", Color.red);
                    Debug.LogError($"[SaveDemo] Save failed: {message}");
                }
            });
        }

        void LoadGame()
        {
            if (saveSystem == null)
            {
                ShowStatus("SaveSystem not initialized!", Color.red);
                return;
            }

            string slotName = slotNameInput != null && !string.IsNullOrEmpty(slotNameInput.text) 
                ? slotNameInput.text 
                : "slot1";

            ShowStatus("Loading...", Color.yellow);

            saveSystem.LoadGame(slotName, (data, success) =>
            {
                if (success && data != null)
                {
                    // Restore game state
                    playerLevel = data.playerLevel;
                    coins = data.coins;
                    health = data.health;
                    experience = data.experience;

                    UpdateUI();
                    ShowStatus($"Game loaded from '{slotName}'!", Color.green);
                    Debug.Log($"[SaveDemo] Game loaded successfully from {slotName}");
                }
                else
                {
                    ShowStatus($"Load failed: No save found in '{slotName}'", Color.red);
                    Debug.LogError($"[SaveDemo] Load failed from {slotName}");
                }
            });
        }

        void UpdateUI()
        {
            if (levelText != null)
                levelText.text = $"Level: {playerLevel}";

            if (coinsText != null)
                coinsText.text = $"Coins: {coins}";

            if (healthText != null)
                healthText.text = $"Health: {health}/100";

            if (experienceText != null)
                experienceText.text = $"EXP: {experience}/500";
        }

        void ShowStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }

            Debug.Log($"[SaveDemo] {message}");
        }

        void OnDestroy()
        {
            FirebaseManager.Instance.OnFirebaseReady -= OnFirebaseReady;

            if (addCoinsButton != null) addCoinsButton.onClick.RemoveAllListeners();
            if (addExpButton != null) addExpButton.onClick.RemoveAllListeners();
            if (takeDamageButton != null) takeDamageButton.onClick.RemoveAllListeners();
            if (healButton != null) healButton.onClick.RemoveAllListeners();
            if (saveButton != null) saveButton.onClick.RemoveAllListeners();
            if (loadButton != null) loadButton.onClick.RemoveAllListeners();
        }
    }
}
