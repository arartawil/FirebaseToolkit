// Assets/FirebaseToolkit/Editor/CreateLoginUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FirebaseToolkit.Editor
{
#if UNITY_EDITOR
    public class CreateLoginUI
    {
        [MenuItem("FirebaseToolkit/Create Login UI")]
        public static void CreateLoginScreen()
        {
            // Create Canvas
            GameObject canvas = new GameObject("LoginCanvas");
            var canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            var canvasScaler = canvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvas.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem if not exists
            if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // Create LoginPanel
            GameObject loginPanel = new GameObject("LoginPanel");
            loginPanel.transform.SetParent(canvas.transform, false);
            var panelImage = loginPanel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            var panelRect = loginPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(600, 500);
            panelRect.anchoredPosition = Vector2.zero;

            // Add VerticalLayoutGroup
            var layoutGroup = loginPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 20;
            layoutGroup.padding = new RectOffset(40, 40, 40, 40);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            // Title
            CreateTitle(loginPanel.transform);

            // Email Input
            CreateInputField(loginPanel.transform, "EmailInput", "Email", TMP_InputField.ContentType.EmailAddress);

            // Password Input
            CreateInputField(loginPanel.transform, "PasswordInput", "Password", TMP_InputField.ContentType.Password);

            // Login Button
            CreateButton(loginPanel.transform, "LoginButton", "Sign In", new Color(0.15f, 0.68f, 0.38f));

            // Register Button
            CreateButton(loginPanel.transform, "ShowRegisterButton", "Create Account", new Color(0.16f, 0.5f, 0.73f));

            // Status Text
            CreateStatusText(loginPanel.transform);

            Selection.activeGameObject = canvas;
            Debug.Log("✅ Login UI created successfully! Assign it to your AuthUI script.");
        }

        static void CreateTitle(Transform parent)
        {
            GameObject title = new GameObject("TitleText");
            title.transform.SetParent(parent, false);
            var tmp = title.AddComponent<TextMeshProUGUI>();
            tmp.text = "LOGIN";
            tmp.fontSize = 48;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            
            var rect = title.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 60);
            
            var layoutElement = title.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 60;
        }

        static void CreateInputField(Transform parent, string name, string placeholder, TMP_InputField.ContentType contentType)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            // Background
            var bgImage = inputObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var inputField = inputObj.AddComponent<TMP_InputField>();
            
            // Text Area
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(inputObj.transform, false);
            var textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);
            textArea.AddComponent<RectMask2D>();
            
            // Text Component
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(textArea.transform, false);
            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = 20;
            textComponent.color = Color.white;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // Placeholder
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textArea.transform, false);
            var placeholderComponent = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderComponent.text = placeholder;
            placeholderComponent.fontSize = 20;
            placeholderComponent.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            placeholderComponent.fontStyle = FontStyles.Italic;
            var placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;
            
            inputField.textViewport = textAreaRect;
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderComponent;
            inputField.contentType = contentType;
            
            var rect = inputObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 50);
            
            var layoutElement = inputObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 50;
            layoutElement.flexibleWidth = 1;
        }

        static void CreateButton(Transform parent, string name, string text, Color color)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            var button = buttonObj.AddComponent<Button>();
            var image = buttonObj.AddComponent<Image>();
            image.color = color;
            button.targetGraphic = image;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var rect = buttonObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 50);
            
            var layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 50;
            layoutElement.flexibleWidth = 1;
        }

        static void CreateStatusText(Transform parent)
        {
            GameObject statusObj = new GameObject("StatusText");
            statusObj.transform.SetParent(parent, false);
            var tmp = statusObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 18;
            tmp.color = Color.yellow;
            tmp.alignment = TextAlignmentOptions.Center;
            
            var rect = statusObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 40);
            
            var layoutElement = statusObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 40;
        }
    }
#endif
}
