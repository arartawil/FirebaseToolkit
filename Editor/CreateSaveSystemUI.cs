// Assets/FirebaseToolkit/Editor/CreateSaveSystemUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FirebaseToolkit.Editor
{
#if UNITY_EDITOR
    public class CreateSaveSystemUI
    {
        [MenuItem("FirebaseToolkit/Create Save System UI")]
        public static void CreateSaveSystemPanel()
        {
            // Find or create Canvas
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create EventSystem if not exists
            if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // Create StatsPanel at top
            GameObject statsPanel = new GameObject("StatsPanel");
            statsPanel.transform.SetParent(canvas.transform, false);
            
            var panelImage = statsPanel.AddComponent<Image>();
            panelImage.color = ColorFromHex("#1E3A5F"); // Dark blue
            
            var panelRect = statsPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 1f); // Top center
            panelRect.anchorMax = new Vector2(0.5f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.sizeDelta = new Vector2(800, 200);
            panelRect.anchoredPosition = new Vector2(0, 0);

            // Add VerticalLayoutGroup
            var layoutGroup = statsPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10;
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            // Create Title
            CreateStatText(statsPanel.transform, "TitleText", "SAVE SYSTEM DEMO", 48, FontStyles.Bold, 60);

            // Create Stats Labels
            CreateStatText(statsPanel.transform, "LevelText", "Level: 1", 24, FontStyles.Normal, 30);
            CreateStatText(statsPanel.transform, "CoinsText", "Coins: 0", 24, FontStyles.Normal, 30);
            CreateStatText(statsPanel.transform, "ExperienceText", "XP: 0/100", 24, FontStyles.Normal, 30);
            CreateStatText(statsPanel.transform, "PlaytimeText", "Playtime: 0:00", 24, FontStyles.Normal, 30);

            Selection.activeGameObject = statsPanel;
            Debug.Log("✅ Save System UI Panel created at top of screen!");
        }

        static void CreateStatText(Transform parent, string name, string text, float fontSize, FontStyles style, float height)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            
            var rect = textObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(760, height);
            
            var layoutElement = textObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
            layoutElement.preferredWidth = 760;
        }

        static Color ColorFromHex(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
#endif
}
