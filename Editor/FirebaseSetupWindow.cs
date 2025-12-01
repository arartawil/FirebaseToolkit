using UnityEngine;
using UnityEditor;
using System.IO;

namespace FirebaseToolkit.Editor
{
    public class FirebaseSetupWindow : EditorWindow
    {
        private Object googleServicesJson;
        private bool firebaseManagerExists = false;
        private string statusMessage = "";
        private MessageType statusType = MessageType.Info;

        [MenuItem("FirebaseToolkit/Firebase Setup Wizard", priority = 0)]
        public static void ShowWindow()
        {
            FirebaseSetupWindow window = GetWindow<FirebaseSetupWindow>("Firebase Setup");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 400);
            window.Show();
        }

        private void OnEnable()
        {
            CheckFirebaseManagerExists();
            CheckGoogleServicesJson();
        }

        private void OnGUI()
        {
            // Header
            GUILayout.Space(10);
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("🔥 Firebase Toolkit Setup", headerStyle);
            
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Configure Firebase for your Unity project", EditorStyles.centeredGreyMiniLabel);
            
            GUILayout.Space(20);
            
            // Step 1: Google Services JSON
            DrawSectionHeader("Step 1: Upload google-services.json");
            EditorGUILayout.HelpBox(
                "Download your google-services.json file from Firebase Console:\n" +
                "1. Go to Project Settings\n" +
                "2. Select your app\n" +
                "3. Download google-services.json",
                MessageType.Info
            );
            
            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();
            googleServicesJson = EditorGUILayout.ObjectField(
                "google-services.json:",
                googleServicesJson,
                typeof(TextAsset),
                false
            );
            
            if (EditorGUI.EndChangeCheck())
            {
                statusMessage = "";
            }
            
            GUILayout.Space(5);
            
            EditorGUI.BeginDisabledGroup(googleServicesJson == null);
            if (GUILayout.Button("📤 Upload JSON to Assets Folder", GUILayout.Height(30)))
            {
                UploadGoogleServicesJson();
            }
            EditorGUI.EndDisabledGroup();
            
            // Check if JSON exists
            if (File.Exists(Application.dataPath + "/google-services.json"))
            {
                EditorGUILayout.HelpBox("✓ google-services.json found in Assets folder", MessageType.None);
            }
            
            GUILayout.Space(20);
            
            // Step 2: Firebase Manager
            DrawSectionHeader("Step 2: Create FirebaseManager");
            EditorGUILayout.HelpBox(
                "FirebaseManager is a singleton that initializes and manages Firebase services.",
                MessageType.Info
            );
            
            GUILayout.Space(5);
            
            if (firebaseManagerExists)
            {
                EditorGUILayout.HelpBox("✓ FirebaseManager already exists in scene", MessageType.None);
                
                if (GUILayout.Button("🔄 Recreate FirebaseManager", GUILayout.Height(30)))
                {
                    CreateFirebaseManager(true);
                }
            }
            else
            {
                if (GUILayout.Button("➕ Create FirebaseManager GameObject", GUILayout.Height(30)))
                {
                    CreateFirebaseManager(false);
                }
            }
            
            GUILayout.Space(20);
            
            // Status Message
            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.HelpBox(statusMessage, statusType);
            }
            
            GUILayout.Space(10);
            
            // Footer buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("📚 Open Documentation", GUILayout.Height(25)))
            {
                string readmePath = Application.dataPath + "/FirebaseToolkit/README.md";
                if (File.Exists(readmePath))
                {
                    Application.OpenURL("file:///" + readmePath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Documentation", 
                        "README.md not found at: " + readmePath, "OK");
                }
            }
            
            if (GUILayout.Button("🔗 Firebase Console", GUILayout.Height(25)))
            {
                Application.OpenURL("https://console.firebase.google.com/");
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
        }

        private void DrawSectionHeader(string title)
        {
            GUIStyle sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            EditorGUILayout.LabelField(title, sectionStyle);
            
            // Draw line
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y += rect.height;
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            
            GUILayout.Space(10);
        }

        private void UploadGoogleServicesJson()
        {
            if (googleServicesJson == null)
            {
                statusMessage = "Please select a google-services.json file first.";
                statusType = MessageType.Warning;
                return;
            }

            string sourcePath = AssetDatabase.GetAssetPath(googleServicesJson);
            string targetPath = "Assets/google-services.json";

            try
            {
                // Check if it's a valid JSON file
                TextAsset textAsset = googleServicesJson as TextAsset;
                if (textAsset == null)
                {
                    statusMessage = "Selected file is not a valid text asset.";
                    statusType = MessageType.Error;
                    return;
                }

                // Verify it contains Firebase configuration
                string jsonContent = textAsset.text;
                if (!jsonContent.Contains("project_info") || !jsonContent.Contains("client"))
                {
                    statusMessage = "This doesn't appear to be a valid google-services.json file.";
                    statusType = MessageType.Error;
                    return;
                }

                // Copy to Assets root
                if (sourcePath != targetPath)
                {
                    AssetDatabase.CopyAsset(sourcePath, targetPath);
                    AssetDatabase.Refresh();
                    
                    statusMessage = "✓ google-services.json successfully uploaded to Assets folder!";
                    statusType = MessageType.Info;
                    
                    Debug.Log("Firebase Setup: google-services.json uploaded to " + targetPath);
                }
                else
                {
                    statusMessage = "✓ google-services.json is already in the correct location.";
                    statusType = MessageType.Info;
                }

                CheckGoogleServicesJson();
            }
            catch (System.Exception e)
            {
                statusMessage = "Error uploading file: " + e.Message;
                statusType = MessageType.Error;
                Debug.LogError("Firebase Setup Error: " + e.Message);
            }
        }

        private void CreateFirebaseManager(bool recreate)
        {
            // Check if FirebaseManager already exists
            GameObject existingManager = GameObject.Find("FirebaseManager");
            
            if (existingManager != null)
            {
                if (recreate)
                {
                    if (EditorUtility.DisplayDialog(
                        "Recreate FirebaseManager?",
                        "This will delete the existing FirebaseManager and create a new one. Continue?",
                        "Yes", "Cancel"))
                    {
                        DestroyImmediate(existingManager);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    statusMessage = "FirebaseManager already exists in the scene.";
                    statusType = MessageType.Warning;
                    return;
                }
            }

            // Create new FirebaseManager GameObject
            GameObject firebaseManager = new GameObject("FirebaseManager");
            
            // Try to add the FirebaseManager component
            var firebaseManagerType = System.Type.GetType("FirebaseToolkit.Core.FirebaseManager,Assembly-CSharp");
            if (firebaseManagerType != null)
            {
                firebaseManager.AddComponent(firebaseManagerType);
                statusMessage = "✓ FirebaseManager created successfully with component attached!";
                statusType = MessageType.Info;
                Debug.Log("Firebase Setup: FirebaseManager GameObject created with FirebaseManager component.");
            }
            else
            {
                statusMessage = "✓ FirebaseManager GameObject created. Please add the FirebaseManager script manually.";
                statusType = MessageType.Warning;
                Debug.LogWarning("Firebase Setup: FirebaseManager script not found. Please add it manually.");
            }

            // Mark as DontDestroyOnLoad (if possible)
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(firebaseManager);
            }

            // Select the new GameObject
            Selection.activeGameObject = firebaseManager;
            
            // Mark scene as dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            CheckFirebaseManagerExists();
        }

        private void CheckFirebaseManagerExists()
        {
            firebaseManagerExists = GameObject.Find("FirebaseManager") != null;
        }

        private void CheckGoogleServicesJson()
        {
            string jsonPath = Application.dataPath + "/google-services.json";
            if (File.Exists(jsonPath) && googleServicesJson == null)
            {
                // Try to load the existing JSON file
                string assetPath = "Assets/google-services.json";
                googleServicesJson = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            }
        }

        private void OnFocus()
        {
            CheckFirebaseManagerExists();
        }
    }
}
