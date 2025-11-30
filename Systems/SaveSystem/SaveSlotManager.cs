// Assets/FirebaseToolkit/Systems/SaveSystem/SaveSlotManager.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace FirebaseToolkit.SaveSystem
{
    /// <summary>
    /// Manages save slot UI - displays available saves
    /// </summary>
    public class SaveSlotManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private GameObject slotButtonPrefab;
        [SerializeField] private Button refreshButton;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Settings")]
        [SerializeField] private int maxSlots = 5;

        public event System.Action<string> OnSlotSelected;
        public event System.Action<string> OnSlotDeleted;

        private SaveSystem saveSystem;
        private List<GameObject> slotButtons = new List<GameObject>();

        void Start()
        {
            saveSystem = new SaveSystem();

            if (refreshButton != null)
                refreshButton.onClick.AddListener(RefreshSlots);

            RefreshSlots();
        }

        /// <summary>
        /// Refresh save slot display
        /// </summary>
        public void RefreshSlots()
        {
            ShowLoading(true);
            ClearSlots();

            saveSystem.GetAllSaves((saves) =>
            {
                ShowLoading(false);

                if (saves.Count == 0)
                {
                    ShowStatus("No saves found", Color.yellow);
                    return;
                }

                ShowStatus($"Found {saves.Count} save(s)", Color.green);

                foreach (var save in saves)
                {
                    CreateSlotButton(save);
                }
            });
        }

        /// <summary>
        /// Create UI button for a save slot
        /// </summary>
        void CreateSlotButton(SaveMetadata metadata)
        {
            GameObject slotObj = Instantiate(slotButtonPrefab, slotContainer);

            // Set slot info
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();
            if (slotUI != null)
            {
                slotUI.SetSlotData(metadata);
                slotUI.OnLoadClicked += () => OnSlotSelected?.Invoke(metadata.slotName);
                slotUI.OnDeleteClicked += () => DeleteSlot(metadata.slotName);
            }

            slotButtons.Add(slotObj);
        }

        /// <summary>
        /// Delete a save slot
        /// </summary>
        void DeleteSlot(string slotName)
        {
            // Confirm deletion
            if (!ConfirmDeletion(slotName))
                return;

            ShowStatus("Deleting...", Color.yellow);

            saveSystem.DeleteSave(slotName, (success) =>
            {
                if (success)
                {
                    ShowStatus("Deleted successfully", Color.green);
                    OnSlotDeleted?.Invoke(slotName);
                    RefreshSlots();
                }
                else
                {
                    ShowStatus("Failed to delete", Color.red);
                }
            });
        }

        /// <summary>
        /// Simple confirmation (you might want a proper dialog)
        /// </summary>
        bool ConfirmDeletion(string slotName)
        {
            // In real game, use proper UI dialog
            return true; // For now, always confirm
        }

        void ClearSlots()
        {
            foreach (var slot in slotButtons)
            {
                Destroy(slot);
            }
            slotButtons.Clear();
        }

        void ShowLoading(bool show)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(show);
        }

        void ShowStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }
        }

        void OnDestroy()
        {
            if (refreshButton != null)
                refreshButton.onClick.RemoveListener(RefreshSlots);
        }
    }
}
