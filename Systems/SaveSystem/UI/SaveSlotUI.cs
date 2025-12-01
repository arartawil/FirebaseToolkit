// Assets/FirebaseToolkit/Systems/SaveSystem/UI/SaveSlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FirebaseToolkit.SaveSystem
{
    /// <summary>
    /// UI for individual save slot button
    /// </summary>
    public class SaveSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI slotNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI playtimeText;
        [SerializeField] private TextMeshProUGUI timestampText;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button deleteButton;

        public event Action OnLoadClicked;
        public event Action OnDeleteClicked;

        private SaveMetadata metadata;

        void Start()
        {
            if (loadButton != null)
                loadButton.onClick.AddListener(() => OnLoadClicked?.Invoke());

            if (deleteButton != null)
                deleteButton.onClick.AddListener(() => OnDeleteClicked?.Invoke());
        }

        /// <summary>
        /// Set slot data from metadata
        /// </summary>
        public void SetSlotData(SaveMetadata metadata)
        {
            this.metadata = metadata;

            if (slotNameText != null)
                slotNameText.text = FormatSlotName(metadata.slotName);

            if (levelText != null)
                levelText.text = $"Level {metadata.playerLevel}";

            if (playtimeText != null)
                playtimeText.text = $"Playtime: {metadata.GetFormattedPlaytime()}";

            if (timestampText != null)
                timestampText.text = FormatTimestamp(metadata.GetDateTime());
        }

        string FormatSlotName(string slotName)
        {
            // Make slot names more readable
            switch (slotName)
            {
                case "autosave": return "Auto Save";
                case "slot1": return "Save Slot 1";
                case "slot2": return "Save Slot 2";
                case "slot3": return "Save Slot 3";
                default: return slotName;
            }
        }

        string FormatTimestamp(DateTime dateTime)
        {
            TimeSpan ago = DateTime.Now - dateTime;

            if (ago.TotalMinutes < 1)
                return "Just now";
            else if (ago.TotalHours < 1)
                return $"{(int)ago.TotalMinutes}m ago";
            else if (ago.TotalDays < 1)
                return $"{(int)ago.TotalHours}h ago";
            else if (ago.TotalDays < 7)
                return $"{(int)ago.TotalDays}d ago";
            else
                return dateTime.ToString("MMM dd, yyyy");
        }

        void OnDestroy()
        {
            if (loadButton != null)
                loadButton.onClick.RemoveAllListeners();

            if (deleteButton != null)
                deleteButton.onClick.RemoveAllListeners();
        }
    }
}
