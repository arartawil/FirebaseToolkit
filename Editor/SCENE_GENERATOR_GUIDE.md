# Complete Save System Scene Generator

## Overview
The `CreateCompleteSaveSystemScene` editor script generates a fully functional Save System demo scene with all UI components, references, and scripts pre-configured. This is the fastest way to get started with Firebase Toolkit's Save System.

## How to Use

### Method 1: Menu Item (Recommended)
1. Open Unity Editor
2. Navigate to menu: **FirebaseToolkit → Create Complete Save System Scene**
3. The scene will be created at: `Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scenes/SaveSystemDemo.unity`
4. Open the created scene
5. **Important**: Manually assign the `SaveSystemGameManager` script to the GameManager GameObject
6. Press Play!

### Method 2: Custom Location
If you want to create the scene at a different location:

1. Open the `CreateCompleteSaveSystemScene.cs` script in Editor folder
2. Modify the `scenePath` variable in the `CreateScene()` method
3. Run from menu as above

## What Gets Created

### Scene Structure

#### AuthCanvas (Active by Default)
- **LoginPanel** (Active)
  - Title
  - Email Input Field
  - Password Input Field
  - Login Button
  - Show Register Button
  - Forgot Password Button
  - Status Text

- **RegisterPanel** (Inactive)
  - Title
  - Email Input Field
  - Password Input Field
  - Confirm Password Input Field
  - Register Button
  - Show Login Button
  - Status Text

- **ForgotPasswordPanel** (Inactive)
  - Title
  - Reset Email Input Field
  - Send Reset Button
  - Back To Login Button
  - Status Text

#### GameCanvas (Inactive by Default)
- **StatsPanel** - Displays player stats
  - Title Text: "SAVE SYSTEM DEMO"
  - Level Text
  - Coins Text
  - Experience Text
  - Playtime Text

- **ActionsPanel** - Game action buttons
  - Earn Coins Button
  - Gain XP Button
  - Level Up Button

- **SavePanel** - Save game slots
  - Title: "💾 SAVE GAME"
  - Save Slot 1 Button
  - Save Slot 2 Button
  - Save Slot 3 Button

- **LoadPanel** - Load game slots
  - Title: "📂 LOAD GAME"
  - Load Slot 1 Button
  - Load Slot 2 Button
  - Load Slot 3 Button

- **AutoSavePanel** - Auto-save toggle
  - Enable Auto-Save Toggle (Default: ON)
  - Label: "🔄 Enable Auto-Save (Every 30s)"

- **StatusText** - Feedback text at bottom

#### GameObjects
- **FirebaseManager** - Core Firebase integration (requires manual script assignment)
- **GameManager** - Game logic controller (requires manual script assignment)
- **EventSystem** - UI input handling

## Color Scheme
All panels use carefully chosen colors for visual distinction:
- **AuthCanvas Background**: Black with 80% opacity (`#000000CC`)
- **Login/Register/Forgot Panels**: Black with 53% opacity (`#00000088`)
- **Stats Panel**: Dark Blue (`#1E3A5F`)
- **Actions Panel**: Dark Gray (`#2C3E50`)
- **Save Panel**: Dark Orange (`#D35400`)
- **Load Panel**: Dark Blue (`#2980B9`)
- **Status Text**: Yellow (`#F1C40F`)
- **Buttons**: Various colors (Green, Teal, Orange, Gray, etc.)

## UI Positioning
All UI elements use **absolute positioning** with precise anchor points and pixel-perfect placement:
- **Anchors**: Elements are anchored to specific points (top, center, bottom, etc.)
- **Positions**: Exact X/Y coordinates ensure consistent layout
- **Sizes**: Fixed widths and heights for all elements
- **No Layout Groups**: Direct positioning for complete control

## Post-Generation Steps

### Critical: Script Assignment
After generating the scene, you **must** manually assign scripts:

1. Select the **FirebaseManager** GameObject
2. In Inspector, add the `FirebaseManager.cs` script component
3. Select the **GameManager** GameObject
4. In Inspector, add the `SaveSystemGameManager.cs` script component

### Why Manual Assignment?
Unity's Editor scripting API cannot programmatically assign MonoBehaviour scripts to GameObjects during scene creation. The scene generator creates all GameObjects and UI components but cannot attach your custom scripts. This is a Unity limitation, not a toolkit issue.

### Verifying Setup
After script assignment, verify:
- [ ] FirebaseManager has the FirebaseManager script
- [ ] GameManager has the SaveSystemGameManager script
- [ ] All UI references in GameManager are assigned (should auto-populate)
- [ ] AuthCanvas is active, GameCanvas is inactive
- [ ] EventSystem is present in scene

## Testing the Scene

1. **First Run**: Login or Register a new account
2. **After Login**: 
   - AuthCanvas hides
   - GameCanvas shows
   - You see your stats (Level, Coins, XP, Playtime)
3. **Game Actions**:
   - Click "💰 Earn Coins" to add coins
   - Click "⭐ Gain XP" to gain experience
   - Click "🎯 Level Up" to increase level
4. **Saving**:
   - Click any "Save Slot" button to save current progress
   - Auto-save runs every 30 seconds if enabled
5. **Loading**:
   - Click any "Load Slot" button to restore saved progress
   - Stats update immediately

## Customization

### Modifying Colors
All colors are defined using the `HexToColor()` helper method. To change colors:
1. Open `CreateCompleteSaveSystemScene.cs`
2. Find the panel creation method (e.g., `CreateStatsPanel()`)
3. Change the hex color code in `HexToColor("#...")` calls

### Adjusting Positions
All UI elements use the helper methods:
- `CreateFixedText()` - For text labels
- `CreatePositionedInputField()` - For input fields
- `CreatePositionedButton()` - For buttons

To adjust positions, modify the `position` parameter (Vector2) in these calls.

### Adding New Elements
Use the existing helper methods as templates:
```csharp
// Add a new text label
CreateFixedText(parent, "MyLabel", "My Text", 24, FontStyles.Normal, Color.white,
    TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(200, 40));

// Add a new button
CreatePositionedButton(parent, "MyButton", "Click Me", 
    HexToColor("#3498DB"), new Vector2(0, 0), new Vector2(200, 50));
```

## Troubleshooting

### Scene Not Appearing in Scenes Folder
- Check the console for errors
- Verify the path exists: `Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scenes/`
- If path doesn't exist, create it manually or modify `scenePath` variable

### "Script Not Assigned" Warning
- This is expected! See **Post-Generation Steps** above
- Manually assign `FirebaseManager.cs` and `SaveSystemGameManager.cs`

### UI Elements Overlapping
- The positioned layout is designed for 1920x1080 resolution
- If using different resolution, adjust the Canvas Scaler reference resolution

### Buttons Not Working
- Ensure `SaveSystemGameManager` script is assigned
- Check that all UI references in GameManager Inspector are populated
- Verify EventSystem is present in scene

## Next Steps

After generating and testing the scene:
1. Review the `SaveSystemGameManager.cs` script to understand the implementation
2. Customize the game logic for your specific needs
3. Modify the UI layout and colors to match your game's style
4. Add additional stats or save data fields in `SaveData.cs`
5. Implement your own game mechanics using the Save System

## Support

For more information:
- See `SETUP_GUIDE.md` for Save System integration
- See `SCENE_SETUP.md` for manual scene setup instructions
- See `RELEASE_v0.2.0.md` for feature overview
- Check `README.md` for general Firebase Toolkit documentation

---

**Note**: This scene generator is part of Firebase Toolkit v0.2.0 and demonstrates the complete Save System functionality with cloud storage via Firebase Realtime Database.
