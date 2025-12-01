// Assets/FirebaseToolkit/Editor/CreateCompleteSaveSystemScene.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace FirebaseToolkit.Editor
{
#if UNITY_EDITOR
    public class CreateCompleteSaveSystemScene
    {
        [MenuItem("FirebaseToolkit/Create Complete Save System Scene")]
        public static void CreateScene()
        {
            // Create new scene with default objects (Main Camera, Directional Light)
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Create EventSystem
            if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // Create Firebase Manager
            GameObject firebaseManager = new GameObject("FirebaseManager");
            // Add FirebaseManager component (user needs to have the script)
            var fbManager = firebaseManager.AddComponent(typeof(MonoBehaviour));
            
            // Create Auth Canvas with all panels
            GameObject authCanvas = CreateAuthCanvas();
            
            // Create Game Canvas
            GameObject gameCanvas = CreateGameCanvas();
            gameCanvas.SetActive(false); // Disabled by default

            // Create Game Manager
            GameObject gameManager = new GameObject("GameManager");
            // Add SaveSystemGameManager component (user needs to have the script)
            var gmManager = gameManager.AddComponent(typeof(MonoBehaviour));

            // Save scene
            string scenePath = "Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scenes/SaveSystemDemo.unity";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(scenePath));
            EditorSceneManager.SaveScene(scene, scenePath);
            
            Debug.Log("✅ Complete Save System Demo scene created successfully!");
            Debug.Log($"📍 Scene saved to: {scenePath}");
            Debug.Log("⚠️ IMPORTANT: Manually add FirebaseManager and SaveSystemGameManager scripts and wire references in Inspector!");
        }

        static GameObject CreateAuthCanvas()
        {
            GameObject authCanvas = new GameObject("AuthCanvas");
            var canvas = authCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            
            var scaler = authCanvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            authCanvas.AddComponent<GraphicRaycaster>();

            // Create Login Panel
            CreateLoginPanel(authCanvas.transform);
            
            // Create Register Panel
            CreateRegisterPanel(authCanvas.transform);
            
            // Create Forgot Password Panel
            CreateForgotPasswordPanel(authCanvas.transform);

            // Add AuthUI component
            // Note: Script must exist in your project for this to work
            var authUIType = System.Type.GetType("FirebaseToolkit.Auth.AuthUI,Assembly-CSharp");
            if (authUIType != null)
            {
                authCanvas.AddComponent(authUIType);
                Debug.Log("AuthUI component added to AuthCanvas.");
            }
            else
            {
                Debug.LogWarning("AuthUI script not found. Please add it manually to the AuthCanvas GameObject.");
            }

            return authCanvas;
        }

        static void CreateLoginPanel(Transform parent)
        {
            GameObject loginPanel = new GameObject("LoginPanel");
            loginPanel.transform.SetParent(parent, false);
            
            var image = loginPanel.AddComponent<Image>();
            image.color = HexToColor("#00000088");
            
            var rect = loginPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600, 500);
            rect.anchoredPosition = Vector2.zero;

            // Title
            CreateFixedText(loginPanel.transform, "Title", "LOGIN", 48, FontStyles.Bold, Color.white, 
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0, -50), new Vector2(560, 60));
            
            // Email Input
            CreatePositionedInputField(loginPanel.transform, "EmailInput", "Email", 
                TMP_InputField.ContentType.EmailAddress, new Vector2(0, -140), new Vector2(500, 50));
            
            // Password Input
            CreatePositionedInputField(loginPanel.transform, "PasswordInput", "Password", 
                TMP_InputField.ContentType.Password, new Vector2(0, -210), new Vector2(500, 50));
            
            // Login Button
            CreatePositionedButton(loginPanel.transform, "LoginButton", "Sign In", 
                HexToColor("#27AE60"), new Vector2(0, -280), new Vector2(250, 50));
            
            // Show Register Button
            CreatePositionedButton(loginPanel.transform, "ShowRegisterButton", "Create Account", 
                HexToColor("#3498DB"), new Vector2(0, -350), new Vector2(250, 50));
            
            // Status Text
            CreateFixedText(loginPanel.transform, "StatusText", "", 20, FontStyles.Normal, HexToColor("#F1C40F"), 
                TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0, 30), new Vector2(560, 40));
        }

        static void CreateRegisterPanel(Transform parent)
        {
            GameObject registerPanel = new GameObject("RegisterPanel");
            registerPanel.transform.SetParent(parent, false);
            registerPanel.SetActive(false); // Hidden by default
            
            var image = registerPanel.AddComponent<Image>();
            image.color = HexToColor("#00000088");
            
            var rect = registerPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600, 600);
            rect.anchoredPosition = Vector2.zero;

            // Title
            CreateFixedText(registerPanel.transform, "Title", "REGISTER", 48, FontStyles.Bold, Color.white, 
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0, -50), new Vector2(560, 60));
            
            // Email Input
            CreatePositionedInputField(registerPanel.transform, "EmailInput", "Email", 
                TMP_InputField.ContentType.EmailAddress, new Vector2(0, -140), new Vector2(500, 50));
            
            // Password Input
            CreatePositionedInputField(registerPanel.transform, "PasswordInput", "Password", 
                TMP_InputField.ContentType.Password, new Vector2(0, -210), new Vector2(500, 50));
            
            // Password Confirm Input
            CreatePositionedInputField(registerPanel.transform, "PasswordConfirmInput", "Confirm Password", 
                TMP_InputField.ContentType.Password, new Vector2(0, -280), new Vector2(500, 50));
            
            // Register Button
            CreatePositionedButton(registerPanel.transform, "RegisterButton", "Create Account", 
                HexToColor("#27AE60"), new Vector2(0, -350), new Vector2(250, 50));
            
            // Show Login Button
            CreatePositionedButton(registerPanel.transform, "ShowLoginButton", "Back to Login", 
                HexToColor("#95A5A6"), new Vector2(0, -420), new Vector2(250, 50));
            
            // Status Text
            CreateFixedText(registerPanel.transform, "StatusText", "", 20, FontStyles.Normal, HexToColor("#F1C40F"), 
                TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0, 30), new Vector2(560, 40));
        }

        static void CreateForgotPasswordPanel(Transform parent)
        {
            GameObject forgotPanel = new GameObject("ForgotPasswordPanel");
            forgotPanel.transform.SetParent(parent, false);
            forgotPanel.SetActive(false); // Hidden by default
            
            var image = forgotPanel.AddComponent<Image>();
            image.color = HexToColor("#00000088");
            
            var rect = forgotPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600, 400);
            rect.anchoredPosition = Vector2.zero;

            // Title
            CreateFixedText(forgotPanel.transform, "Title", "RESET PASSWORD", 48, FontStyles.Bold, Color.white, 
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0, -50), new Vector2(560, 60));
            
            // Reset Email Input
            CreatePositionedInputField(forgotPanel.transform, "ResetEmailInput", "Email", 
                TMP_InputField.ContentType.EmailAddress, new Vector2(0, -140), new Vector2(500, 50));
            
            // Send Reset Button
            CreatePositionedButton(forgotPanel.transform, "SendResetButton", "Send Reset Email", 
                HexToColor("#E67E22"), new Vector2(0, -210), new Vector2(250, 50));
            
            // Back To Login Button
            CreatePositionedButton(forgotPanel.transform, "BackToLoginButton", "Back to Login", 
                HexToColor("#95A5A6"), new Vector2(0, -280), new Vector2(250, 50));
            
            // Status Text
            CreateFixedText(forgotPanel.transform, "StatusText", "", 20, FontStyles.Normal, HexToColor("#F1C40F"), 
                TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0, 30), new Vector2(560, 40));
        }

        static GameObject CreateGameCanvas()
        {
            GameObject gameCanvas = new GameObject("GameCanvas");
            var canvas = gameCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            
            var scaler = gameCanvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            gameCanvas.AddComponent<GraphicRaycaster>();

            // Game Panel (background)
            GameObject gamePanel = new GameObject("GamePanel");
            gamePanel.transform.SetParent(gameCanvas.transform, false);
            var panelImage = gamePanel.AddComponent<Image>();
            panelImage.color = HexToColor("#ECF0F1");
            var panelRect = gamePanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Create all panels
            CreateStatsPanel(gameCanvas.transform);
            CreateActionsPanel(gameCanvas.transform);
            CreateSavePanel(gameCanvas.transform);
            CreateLoadPanel(gameCanvas.transform);
            CreateAutoSavePanel(gameCanvas.transform);
            CreateStatusText(gameCanvas.transform);

            // Add SaveSystemGameManager component
            // Note: Script must exist in your project for this to work
            var gameManagerType = System.Type.GetType("FirebaseToolkit.SaveSystem.SaveSystemGameManager,Assembly-CSharp");
            if (gameManagerType != null)
            {
                gameCanvas.AddComponent(gameManagerType);
                Debug.Log("SaveSystemGameManager component added to GameCanvas.");
            }
            else
            {
                Debug.LogWarning("SaveSystemGameManager script not found. Please add it manually to the GameCanvas GameObject.");
            }

            return gameCanvas;
        }

        static void CreateStatsPanel(Transform parent)
        {
            GameObject panel = new GameObject("StatsPanel");
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = HexToColor("#1E3A5F");
            
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(800, 200);
            rect.anchoredPosition = new Vector2(0, -100);

            // Title - anchored at top center of panel
            CreateFixedText(panel.transform, "TitleText", "SAVE SYSTEM DEMO", 48, FontStyles.Bold, Color.white,
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0, -30), new Vector2(760, 60));
            
            // Level Text - anchored at top left
            CreateFixedText(panel.transform, "LevelText", "Level: 1", 24, FontStyles.Normal, Color.white,
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(30, -100), new Vector2(350, 30));
            
            // Coins Text - anchored at top right
            CreateFixedText(panel.transform, "CoinsText", "Coins: 0", 24, FontStyles.Normal, Color.white,
                TextAlignmentOptions.Right, new Vector2(1f, 1f), new Vector2(-30, -100), new Vector2(350, 30));
            
            // Experience Text - anchored at top left, below level
            CreateFixedText(panel.transform, "ExperienceText", "XP: 0/100", 24, FontStyles.Normal, Color.white,
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(30, -140), new Vector2(350, 30));
            
            // Playtime Text - anchored at top right, below coins
            CreateFixedText(panel.transform, "PlaytimeText", "Playtime: 0:00", 24, FontStyles.Normal, Color.white,
                TextAlignmentOptions.Right, new Vector2(1f, 1f), new Vector2(-30, -140), new Vector2(350, 30));
        }

        static void CreateActionsPanel(Transform parent)
        {
            GameObject panel = new GameObject("ActionsPanel");
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = HexToColor("#2C3E50");
            
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(800, 150);
            rect.anchoredPosition = new Vector2(0, 50);

            // Earn Coins Button - left position
            CreatePositionedButton(panel.transform, "EarnCoinsButton", "💰 Earn Coins", 
                HexToColor("#27AE60"), new Vector2(-280, 0), new Vector2(250, 50));
            
            // Gain XP Button - center position
            CreatePositionedButton(panel.transform, "GainXPButton", "⭐ Gain XP", 
                HexToColor("#1ABC9C"), new Vector2(0, 0), new Vector2(250, 50));
            
            // Level Up Button - right position
            CreatePositionedButton(panel.transform, "LevelUpButton", "🎯 Level Up", 
                HexToColor("#F39C12"), new Vector2(280, 0), new Vector2(250, 50));
        }

        static void CreateSavePanel(Transform parent)
        {
            GameObject panel = new GameObject("SavePanel");
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = HexToColor("#D35400");
            
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(350, 250);
            rect.anchoredPosition = new Vector2(300, 200);

            // Title - anchored at top center of panel
            CreateFixedText(panel.transform, "Title", "💾 SAVE GAME", 32, FontStyles.Bold, Color.white,
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(300, 40));
            
            // Save Slot 1 Button
            CreatePositionedButton(panel.transform, "SaveSlot1Button", "Save Slot 1", 
                HexToColor("#E67E22"), new Vector2(0, -85), new Vector2(300, 50));
            
            // Save Slot 2 Button
            CreatePositionedButton(panel.transform, "SaveSlot2Button", "Save Slot 2", 
                HexToColor("#E67E22"), new Vector2(0, -145), new Vector2(300, 50));
            
            // Save Slot 3 Button
            CreatePositionedButton(panel.transform, "SaveSlot3Button", "Save Slot 3", 
                HexToColor("#E67E22"), new Vector2(0, -205), new Vector2(300, 50));
        }

        static void CreateLoadPanel(Transform parent)
        {
            GameObject panel = new GameObject("LoadPanel");
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = HexToColor("#2980B9");
            
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(350, 250);
            rect.anchoredPosition = new Vector2(-300, 200);

            // Title - anchored at top center of panel
            CreateFixedText(panel.transform, "Title", "📂 LOAD GAME", 32, FontStyles.Bold, Color.white,
                TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(300, 40));
            
            // Load Slot 1 Button
            CreatePositionedButton(panel.transform, "LoadSlot1Button", "Load Slot 1", 
                HexToColor("#3498DB"), new Vector2(0, -85), new Vector2(300, 50));
            
            // Load Slot 2 Button
            CreatePositionedButton(panel.transform, "LoadSlot2Button", "Load Slot 2", 
                HexToColor("#3498DB"), new Vector2(0, -145), new Vector2(300, 50));
            
            // Load Slot 3 Button
            CreatePositionedButton(panel.transform, "LoadSlot3Button", "Load Slot 3", 
                HexToColor("#3498DB"), new Vector2(0, -205), new Vector2(300, 50));
        }

        static void CreateAutoSavePanel(Transform parent)
        {
            GameObject panel = new GameObject("AutoSavePanel");
            panel.transform.SetParent(parent, false);
            
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(400, 60);
            rect.anchoredPosition = new Vector2(-250, -50);

            // Create Toggle
            GameObject toggleObj = new GameObject("AutoSaveToggle");
            toggleObj.transform.SetParent(panel.transform, false);
            
            var toggle = toggleObj.AddComponent<Toggle>();
            toggle.isOn = true; // Default ON
            
            var toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.anchorMin = Vector2.zero;
            toggleRect.anchorMax = Vector2.one;
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;

            // Background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(toggleObj.transform, false);
            var bgImage = background.AddComponent<Image>();
            bgImage.color = HexToColor("#34495E");
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.5f);
            bgRect.anchorMax = new Vector2(0f, 0.5f);
            bgRect.pivot = new Vector2(0f, 0.5f);
            bgRect.sizeDelta = new Vector2(50, 30);
            bgRect.anchoredPosition = new Vector2(10, 0);

            // Checkmark
            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(background.transform, false);
            var checkImage = checkmark.AddComponent<Image>();
            checkImage.color = HexToColor("#27AE60");
            var checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = new Vector2(5, 5);
            checkRect.offsetMax = new Vector2(-5, -5);

            toggle.graphic = checkImage;
            toggle.targetGraphic = bgImage;

            // Label
            GameObject label = new GameObject("Label");
            label.transform.SetParent(toggleObj.transform, false);
            var labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "🔄 Enable Auto-Save (Every 30s)";
            labelText.fontSize = 20;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(70, 0);
            labelRect.offsetMax = new Vector2(0, 0);
        }

        static void CreateStatusText(Transform parent)
        {
            GameObject statusObj = new GameObject("StatusText");
            statusObj.transform.SetParent(parent, false);
            
            var tmp = statusObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 28;
            tmp.color = Color.yellow;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            
            var rect = statusObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(800, 50);
            rect.anchoredPosition = new Vector2(0, 50);
        }

        static void CreateText(Transform parent, string name, string text, float fontSize, FontStyles style, float height, Color? color = null)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            
            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = color ?? Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            
            var layoutElement = obj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
        }

        static void CreateInputField(Transform parent, string name, string placeholder, TMP_InputField.ContentType contentType)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            var bgImage = inputObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var inputField = inputObj.AddComponent<TMP_InputField>();
            
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(inputObj.transform, false);
            var textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);
            textArea.AddComponent<RectMask2D>();
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(textArea.transform, false);
            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = 24;
            textComponent.color = Color.white;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textArea.transform, false);
            var placeholderComponent = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderComponent.text = placeholder;
            placeholderComponent.fontSize = 24;
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
            
            var colors = button.colors;
            colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
            button.colors = colors;
            
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
            
            var layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 50;
            layoutElement.flexibleWidth = 1;
        }

        static void CreateFixedButton(Transform parent, string name, string text, Color color, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            var button = buttonObj.AddComponent<Button>();
            var image = buttonObj.AddComponent<Image>();
            image.color = color;
            button.targetGraphic = image;
            
            var colors = button.colors;
            colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
            button.colors = colors;
            
            var rect = buttonObj.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = size.x;
            layoutElement.preferredHeight = size.y;
        }

        static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            if (hex.Length == 8) // RGBA
            {
                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color32(r, g, b, a);
            }
            else // RGB
            {
                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color32(r, g, b, 255);
            }
        }

        static void CreateFixedText(Transform parent, string name, string text, float fontSize, FontStyles style, Color color, 
            TextAlignmentOptions alignment, Vector2 anchor, Vector2 position, Vector2 size)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = alignment;
            
            var rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        static void CreatePositionedInputField(Transform parent, string name, string placeholder, 
            TMP_InputField.ContentType contentType, Vector2 position, Vector2 size)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            var bgImage = inputObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var inputField = inputObj.AddComponent<TMP_InputField>();
            
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(inputObj.transform, false);
            var textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);
            textArea.AddComponent<RectMask2D>();
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(textArea.transform, false);
            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = 24;
            textComponent.color = Color.white;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textArea.transform, false);
            var placeholderComponent = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderComponent.text = placeholder;
            placeholderComponent.fontSize = 24;
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
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        static void CreatePositionedButton(Transform parent, string name, string text, Color color, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            var button = buttonObj.AddComponent<Button>();
            var image = buttonObj.AddComponent<Image>();
            image.color = color;
            button.targetGraphic = image;
            
            var colors = button.colors;
            colors.highlightedColor = new Color(color.r * 1.2f, color.g * 1.2f, color.b * 1.2f);
            colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
            button.colors = colors;
            
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
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }
    }
#endif
}
