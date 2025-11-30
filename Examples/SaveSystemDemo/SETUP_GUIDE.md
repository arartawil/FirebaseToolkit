# Save System Demo - Setup Guide

## 📋 Overview
This demo shows how to use the Firebase Toolkit Save System to save and load game state to the cloud.

## ✅ Prerequisites
Before running this demo, ensure you have:
1. ✅ Firebase SDK imported (Auth + Realtime Database)
2. ✅ `google-services.json` configured
3. ✅ User must be signed in (use AuthLeaderboardDemo first to register)

## 🎮 What This Demo Shows
- **Save game state** to Firebase cloud storage
- **Load game state** from any device
- **Multiple save slots** (slot1, slot2, autosave, etc.)
- **Auto-save** functionality
- **Cloud sync** across devices

## 🏗️ Scene Setup

### Step 1: Create Scene
1. Create new scene: `SaveSystemDemo`
2. Save to `Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scenes/`

### Step 2: Add FirebaseManager
1. Create empty GameObject: `FirebaseManager`
2. Add `FirebaseManager.cs` script
3. This handles Firebase initialization

### Step 3: Create UI Canvas
Create a Canvas with these UI elements:

```
Canvas
├── GameStatePanel
│   ├── LevelText (TextMeshPro)
│   ├── CoinsText (TextMeshPro)
│   ├── HealthText (TextMeshPro)
│   └── ExperienceText (TextMeshPro)
├── ActionsPanel
│   ├── AddCoinsButton
│   ├── AddExpButton
│   ├── TakeDamageButton
│   └── HealButton
├── SaveLoadPanel
│   ├── SlotNameInput (TMP_InputField)
│   ├── SaveButton
│   └── LoadButton
└── StatusText (TextMeshPro)
```

### Step 4: Add SaveDemoManager
1. Create empty GameObject: `SaveDemoManager`
2. Add `SaveDemoManager.cs` script
3. Assign all UI references in Inspector

## 🎯 How to Use

### Basic Save/Load Flow:
1. **Sign in** first (required for cloud saves)
2. **Modify game state** using buttons:
   - Add Coins (+50)
   - Add Experience (+100, auto level up at 500 exp)
   - Take Damage (-20 HP)
   - Heal (+30 HP)
3. **Enter slot name** (e.g., "slot1", "mysave", "autosave")
4. **Click Save** to save to cloud
5. **Click Load** to load from cloud

### Testing Cloud Sync:
1. Save game on Device A
2. Sign in with same account on Device B
3. Load same save slot
4. Your progress is synced! 🎉

## 💾 Code Examples

### Save Game
```csharp
SaveSystem.SaveSystem saveSystem = new SaveSystem.SaveSystem();

SaveSystem.SaveData data = new SaveSystem.SaveData
{
    playerLevel = 5,
    coins = 1000,
    health = 80,
    experience = 250,
    playtime = 3600f, // 1 hour in seconds
    currentScene = "Level1"
};

saveSystem.SaveGame(data, "slot1", (success, message) =>
{
    if (success)
        Debug.Log("Game saved!");
    else
        Debug.LogError($"Save failed: {message}");
});
```

### Load Game
```csharp
saveSystem.LoadGame("slot1", (data, success) =>
{
    if (success)
    {
        // Restore game state
        playerLevel = data.playerLevel;
        coins = data.coins;
        health = data.health;
        Debug.Log("Game loaded!");
    }
});
```

### Auto-Save
```csharp
// Add AutoSave component to GameObject
AutoSave autoSave = gameObject.AddComponent<AutoSave>();

// Enable auto-save every 30 seconds
autoSave.SetAutoSaveEnabled(true);
autoSave.SetSaveInterval(30f);

// Listen for auto-save completion
autoSave.OnAutoSaveComplete += (success) =>
{
    if (success)
        ShowNotification("Game auto-saved");
};
```

### Get All Saves
```csharp
saveSystem.GetAllSaves((saves) =>
{
    foreach (var save in saves)
    {
        Debug.Log($"Slot: {save.slotName}");
        Debug.Log($"Level: {save.playerLevel}");
        Debug.Log($"Playtime: {save.GetFormattedPlaytime()}");
        Debug.Log($"Date: {save.GetDateTime()}");
    }
});
```

### Delete Save
```csharp
saveSystem.DeleteSave("slot2", (success) =>
{
    if (success)
        Debug.Log("Save deleted");
});
```

## 📊 Firebase Database Structure
```
firebase-database/
└── saves/
    └── {userId}/
        ├── slot1/
        │   ├── playerLevel: 5
        │   ├── coins: 1000
        │   ├── health: 80
        │   ├── playtime: 3600
        │   └── timestamp: 1701388800
        ├── slot1_metadata/ (for quick loading)
        ├── slot2/
        ├── autosave/
        └── ...
```

## 🔒 Security Rules
Add these Firebase Database rules:
```json
{
  "rules": {
    "saves": {
      "$userId": {
        ".read": "$userId === auth.uid",
        ".write": "$userId === auth.uid"
      }
    }
  }
}
```

## 🎨 Extending SaveData
Create your own save data structure:

```csharp
using FirebaseToolkit.SaveSystem;

[System.Serializable]
public class MyGameSaveData : SaveData
{
    // Add custom fields
    public string[] unlockedWeapons;
    public Dictionary<string, bool> achievements;
    public int skillPoints;
    public Vector3 checkpointPosition;
    
    // Override CollectGameData in AutoSave to use your custom data
}
```

## 🐛 Troubleshooting

### "SaveSystem not initialized"
- ✅ Make sure user is signed in
- ✅ Check Firebase is initialized (OnFirebaseReady event)

### "Save failed: Permission denied"
- ✅ Verify Firebase Database rules allow user access
- ✅ Check user authentication status

### Saves not syncing across devices
- ✅ Confirm same user account on both devices
- ✅ Check internet connection
- ✅ Verify Firebase Database is online

### Data not persisting
- ✅ Make sure using cloud save (not local PlayerPrefs)
- ✅ Check Firebase Console → Database for saved data
- ✅ Verify slot names match exactly (case-sensitive)

## 📱 Platform Notes

### Mobile (iOS/Android)
- Auto-save triggers on app pause
- Auto-save triggers on app quit
- Consider showing "Saving..." indicator

### WebGL
- Cloud saves work seamlessly
- No local storage needed
- Perfect for web games

### Desktop
- Auto-save on quit
- Can implement manual save reminder

## ⚡ Performance Tips
1. **Use metadata** for slot listings (faster than loading full data)
2. **Limit auto-save frequency** to avoid excessive writes
3. **Compress large data** if game state is huge
4. **Cache current save** in memory, save periodically

## 🎓 Best Practices
- ✅ Always check `IsSignedIn` before using SaveSystem
- ✅ Show loading indicators during save/load
- ✅ Provide visual feedback on success/failure
- ✅ Implement proper error handling
- ✅ Auto-save before critical events (boss fights, level transitions)
- ✅ Allow multiple manual save slots
- ✅ Display save metadata (timestamp, playtime) to users

## 📚 Next Steps
- Integrate SaveSystem into your game
- Customize SaveData for your game's needs
- Add save slot UI with thumbnails
- Implement cloud save conflict resolution
- Add achievements/statistics tracking

## 🎉 You're Ready!
Your cloud save system is now fully functional. Players can save progress and continue on any device! 🚀
