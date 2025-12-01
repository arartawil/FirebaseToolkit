# SaveSystemDemo Scene - Complete Setup Guide

## 🎯 What You'll Build
A complete demo showing:
- Game state management (level, coins, XP, playtime)
- Manual save to 3 different slots
- Load from any slot
- Auto-save toggle with 30s interval
- Visual feedback for all operations

## 📋 Step-by-Step Scene Setup

### Step 1: Create New Scene
1. In Unity: `File → New Scene`
2. Save as: `Assets/FirebaseToolkit/Examples/SaveSystemDemo/Scenes/SaveSystemDemo.unity`

### Step 2: Add FirebaseManager
1. Create empty GameObject: `FirebaseManager`
2. Add component: `FirebaseManager.cs` (from Core)
3. This handles Firebase initialization automatically

### Step 3: Add AuthUI (Required)
1. Create Canvas: `AuthCanvas`
2. Add `AuthUI` prefab or component
3. Users **must be signed in** to use SaveSystem
4. Configure login/register panels

> **Note:** Users must authenticate before SaveSystem works!

### Step 4: Create Main Canvas

#### 4.1 Create Canvas
```
GameObject → UI → Canvas
```
- Name: `GameCanvas`
- Canvas Scaler: Scale With Screen Size
- Reference Resolution: 1920x1080

#### 4.2 Add Panel Background
```
Right-click GameCanvas → UI → Panel
```
- Name: `GamePanel`
- Anchor: Stretch (all corners)
- Color: Semi-transparent dark

### Step 5: Create Title
```
Right-click GamePanel → UI → Text - TextMeshPro
```
- Name: `TitleText`
- Text: "Save System Demo"
- Font Size: 48
- Alignment: Center
- Position: Top center

### Step 6: Create Stats Panel

#### 6.1 Stats Container
```
Right-click GamePanel → UI → Vertical Layout Group
```
- Name: `StatsPanel`
- Padding: 10
- Spacing: 5
- Child Alignment: Upper Left

#### 6.2 Add Stat Texts (create 4 TextMeshPro texts):
1. **LevelText**
   - Text: "Level: 1"
   - Font Size: 24

2. **CoinsText**
   - Text: "Coins: 0"
   - Font Size: 24

3. **ExperienceText**
   - Text: "XP: 0/100"
   - Font Size: 24

4. **PlaytimeText**
   - Text: "Playtime: 0:00"
   - Font Size: 24

### Step 7: Create Actions Panel

```
Right-click GamePanel → UI → Horizontal Layout Group
```
- Name: `ActionsPanel`
- Spacing: 10
- Child Control Size: Width

Add 3 Buttons (UI → Button - TextMeshPro):

1. **EarnCoinsButton**
   - Text: "💰 Earn Coins"
   - Width: 150, Height: 50

2. **GainXPButton**
   - Text: "⭐ Gain XP"
   - Width: 150, Height: 50

3. **LevelUpButton**
   - Text: "🎯 Level Up"
   - Width: 150, Height: 50

### Step 8: Create Save Panel

```
Right-click GamePanel → UI → Panel
```
- Name: `SavePanel`
- Background color: Slightly lighter

Add Title:
```
UI → Text - TextMeshPro
```
- Text: "💾 SAVE GAME"
- Font Size: 32
- Bold

Add 3 Save Buttons (Vertical Layout):

1. **SaveSlot1Button**
   - Text: "Save to Slot 1"
   - Color: Green tint

2. **SaveSlot2Button**
   - Text: "Save to Slot 2"
   - Color: Green tint

3. **SaveSlot3Button**
   - Text: "Save to Slot 3"
   - Color: Green tint

### Step 9: Create Load Panel

```
Right-click GamePanel → UI → Panel
```
- Name: `LoadPanel`

Add Title:
```
UI → Text - TextMeshPro
```
- Text: "📂 LOAD GAME"
- Font Size: 32
- Bold

Add 3 Load Buttons:

1. **LoadSlot1Button**
   - Text: "Load Slot 1"
   - Color: Blue tint

2. **LoadSlot2Button**
   - Text: "Load Slot 2"
   - Color: Blue tint

3. **LoadSlot3Button**
   - Text: "Load Slot 3"
   - Color: Blue tint

### Step 10: Add Auto-Save Toggle

```
Right-click GamePanel → UI → Toggle
```
- Name: `AutoSaveToggle`
- Label Text: "🔄 Enable Auto-Save (30s)"
- Background color: Yellow tint when checked

### Step 11: Add Status Text

```
Right-click GamePanel → UI → Text - TextMeshPro
```
- Name: `StatusText`
- Text: ""
- Font Size: 20
- Alignment: Center
- Position: Bottom center
- Enable Rich Text
- Color: White (will change dynamically)

### Step 12: Create GameManager

```
GameObject → Create Empty
```
- Name: `SaveSystemGameManager`
- Add component: `SaveSystemGameManager.cs`

### Step 13: Assign References in Inspector

Select `SaveSystemGameManager` and assign:

**UI References - Stats:**
- Level Text → `LevelText`
- Coins Text → `CoinsText`
- Experience Text → `ExperienceText`
- Playtime Text → `PlaytimeText`

**UI References - Action Buttons:**
- Earn Coins Button → `EarnCoinsButton`
- Gain XP Button → `GainXPButton`
- Level Up Button → `LevelUpButton`

**UI References - Save Buttons:**
- Save Slot1 Button → `SaveSlot1Button`
- Save Slot2 Button → `SaveSlot2Button`
- Save Slot3 Button → `SaveSlot3Button`

**UI References - Load Buttons:**
- Load Slot1 Button → `LoadSlot1Button`
- Load Slot2 Button → `LoadSlot2Button`
- Load Slot3 Button → `LoadSlot3Button`

**UI References - Status:**
- Status Text → `StatusText`
- Auto Save Toggle → `AutoSaveToggle`

### Step 14: Configure Initial Values

In `SaveSystemGameManager` Inspector:
- Player Level: 1
- Coins: 0
- Experience: 0
- Playtime: 0

## 🎮 Final Hierarchy

```
SaveSystemDemo (Scene)
├── FirebaseManager
├── AuthCanvas
│   └── AuthUI (login/register panels)
├── GameCanvas
│   └── GamePanel
│       ├── TitleText
│       ├── StatsPanel
│       │   ├── LevelText
│       │   ├── CoinsText
│       │   ├── ExperienceText
│       │   └── PlaytimeText
│       ├── ActionsPanel
│       │   ├── EarnCoinsButton
│       │   ├── GainXPButton
│       │   └── LevelUpButton
│       ├── SavePanel
│       │   ├── TitleText
│       │   ├── SaveSlot1Button
│       │   ├── SaveSlot2Button
│       │   └── SaveSlot3Button
│       ├── LoadPanel
│       │   ├── TitleText
│       │   ├── LoadSlot1Button
│       │   ├── LoadSlot2Button
│       │   └── LoadSlot3Button
│       ├── AutoSaveToggle
│       └── StatusText
└── SaveSystemGameManager

```

## ✅ Testing Checklist

### Pre-Flight Checks:
- [ ] Firebase SDK imported
- [ ] `google-services.json` in Assets/
- [ ] User is signed in via AuthUI
- [ ] All UI references assigned in Inspector

### Test Sequence:

1. **Sign In**
   - Use AuthUI to register or login
   - Wait for "Save System ready!" message

2. **Test Game Actions**
   - Click "Earn Coins" → Should add 10-100 coins
   - Click "Gain XP" → Should add 5-50 XP
   - Click "Level Up" → Should level up if XP ≥ 100
   - Watch playtime increment

3. **Test Manual Save**
   - Click "Save to Slot 1"
   - Should show "✓ Saved to slot1!"
   - Check Firebase Console → Database → `saves/{userId}/slot1`

4. **Test Load**
   - Change stats (earn coins, gain XP)
   - Click "Load Slot 1"
   - Should restore previous stats
   - Should show "✓ Loaded from slot1!"

5. **Test Multiple Slots**
   - Save different states to slot1, slot2, slot3
   - Load each slot to verify different states

6. **Test Auto-Save**
   - Enable "Auto-Save" toggle
   - Wait 30 seconds
   - Should see "Auto-saved!" message
   - Check Firebase Console → `saves/{userId}/autosave`

7. **Test Cross-Device Sync**
   - Save on Device A
   - Sign in with same account on Device B
   - Load same slot
   - Stats should match! 🎉

## 🐛 Troubleshooting

### "Save System not initialized!"
- ✅ Check user is signed in
- ✅ Verify FirebaseManager is in scene
- ✅ Wait for "Save System ready!" message

### Save buttons do nothing
- ✅ Check button references in Inspector
- ✅ Check Console for errors
- ✅ Verify Firebase Database rules allow writes

### Stats not updating after load
- ✅ Check UpdateUI() is called after load
- ✅ Verify TextMeshPro references are assigned
- ✅ Check Console for load errors

### Auto-save not working
- ✅ Verify toggle is enabled
- ✅ Check 30 second interval has passed
- ✅ Look for "Auto-saved!" message
- ✅ Verify user is still signed in

## 🎨 Optional Enhancements

### Visual Polish:
- Add save/load animations
- Show slot preview (level, coins, timestamp)
- Add confirmation dialog for load (overwrite warning)
- Display loading spinner during save/load
- Add sound effects for actions

### Gameplay:
- Add health system
- Implement inventory items
- Track achievements
- Add quest progress
- Include settings (volume, graphics)

### Advanced:
- Implement save slot thumbnails (screenshots)
- Add cloud conflict resolution
- Implement save file versioning
- Add backup/restore functionality
- Export/import save files

## 🎉 You're Done!

Your SaveSystemDemo scene is complete! Players can now:
- ✅ Earn coins and gain experience
- ✅ Save progress to multiple slots
- ✅ Load progress from any slot
- ✅ Enable auto-save for convenience
- ✅ Sync progress across devices

## 📚 Next Steps

1. **Customize SaveData** - Add your game-specific data
2. **Integrate into your game** - Use SaveSystem in your real project
3. **Add UI Polish** - Make it look beautiful
4. **Test extensively** - Verify cloud sync works
5. **Deploy** - Push to production!

Happy saving! 💾🎮
