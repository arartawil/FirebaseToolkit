# Leaderboard Demo Scene Setup Guide

## Quick Setup Instructions

Follow these steps to set up the LeaderboardDemo scene in Unity:

### 1. Open the Scene
- Navigate to `Assets/FirebaseToolkit/Examples/LeaderboardDemo/Scenes/`
- Double-click `LeaderboardDemo.unity` to open it

### 2. Create FirebaseManager GameObject
1. In **Hierarchy** window: Right-click → **Create Empty**
2. Name it: `FirebaseManager`
3. In **Inspector**: Click **Add Component**
4. Search for: `FirebaseManager`
5. Add the component (it will show as a script)

### 3. Create UI Canvas
1. In **Hierarchy**: Right-click → **UI** → **Canvas**
2. This automatically creates:
   - Canvas
   - EventSystem

### 4. Create Submit Score Panel
1. Right-click **Canvas** → **UI** → **Panel**
2. Rename to: `SubmitPanel`
3. Add these child elements:
   - **UI** → **Input Field** → Name: `PlayerNameInput`
     - Set placeholder text: "Enter Player Name"
   - **UI** → **Input Field** → Name: `ScoreInput`
     - Set placeholder text: "Enter Score"
     - Set Content Type: Integer Number
   - **UI** → **Button** → Name: `SubmitButton`
     - Change Text child to: "Submit Score"
   - **UI** → **Text** → Name: `StatusText`
     - Set text: "Ready"
     - Set color to white or yellow for visibility

### 5. Create Leaderboard Display Panel
1. Right-click **Canvas** → **UI** → **Panel**
2. Rename to: `LeaderboardPanel`
3. Add these child elements:
   - **UI** → **Text** → Name: `Title`
     - Set text: "Top Scores"
     - Set font size: 24
     - Set alignment: Center
   - **UI** → **Scroll View** → Name: `ScoresList`
     - This creates Viewport and Content automatically
   - **UI** → **Button** → Name: `RefreshButton`
     - Change Text child to: "Refresh Leaderboard"

### 6. Add LeaderboardDemoUI Script
1. Create a new empty GameObject in Hierarchy
2. Name it: `LeaderboardDemoController`
3. Add Component: `LeaderboardDemoUI`
4. In the Inspector, assign the references:
   - **Player Name Input**: Drag `PlayerNameInput` from Hierarchy
   - **Score Input**: Drag `ScoreInput` from Hierarchy
   - **Submit Button**: Drag `SubmitButton` from Hierarchy
   - **Status Text**: Drag `StatusText` from Hierarchy
   - **Refresh Button**: Drag `RefreshButton` from Hierarchy
   - **Scores List Content**: Drag the **Content** object from inside ScoresList/Viewport

### 7. Position UI Elements
Arrange the UI nicely in the Scene view:

**SubmitPanel** (Left or Top):
- Position: Top-left or top section
- Contains input fields and submit button stacked vertically

**LeaderboardPanel** (Right or Bottom):
- Position: Right side or bottom section
- Contains title, scroll view, and refresh button

**Recommended Layout:**
```
Canvas
├── SubmitPanel (Top: 200px height)
│   ├── PlayerNameInput
│   ├── ScoreInput
│   ├── SubmitButton
│   └── StatusText
└── LeaderboardPanel (Bottom: Fill remaining space)
    ├── Title
    ├── ScoresList (Scroll View)
    └── RefreshButton
```

### 8. Configure Canvas Settings
1. Select **Canvas** in Hierarchy
2. Set **Canvas Scaler** component:
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: **1920 x 1080**
   - Match: **0.5** (balance between width and height)

### 9. Test the Scene
1. Make sure your Firebase is properly configured
2. Press **Play** in Unity Editor
3. The status should show "Ready to submit scores!"
4. Enter a player name and score
5. Click Submit Score
6. The leaderboard should refresh automatically

## Troubleshooting

**Firebase not initializing:**
- Check that `google-services.json` is in `Assets/` folder
- Verify Firebase SDK is properly imported
- Check Console for error messages

**UI not visible:**
- Make sure Canvas is set to Screen Space - Overlay
- Check that EventSystem exists in scene
- Verify UI elements are not behind the camera

**Buttons not responding:**
- Ensure EventSystem is present
- Check that buttons have the Graphic Raycaster component
- Verify button OnClick events are properly set up in the script

## Advanced Customization

### Create Custom Score Entry Prefab
1. Create a new **UI** → **Panel** prefab
2. Add Text components for rank, name, and score
3. Save as prefab in `Assets/FirebaseToolkit/Examples/LeaderboardDemo/Prefabs/`
4. Assign to **Score Entry Prefab** field in LeaderboardDemoUI

### Change Leaderboard ID
- In LeaderboardDemoUI component, change **Leaderboard Id** field
- This allows multiple separate leaderboards (e.g., "global", "weekly", "level1")

### Adjust Number of Scores
- Change **Top Scores Count** in LeaderboardDemoUI component
- Default is 10, increase for longer leaderboards
