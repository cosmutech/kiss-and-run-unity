# 💋 Kiss & Run — High-Fidelity 3D Unity Engine Project

A complete, feature-rich 3D endless runner built in **Unity Engine (C# & URP)** matching the visual perspective, 3-lane physics, and mobile swipe controls of **Subway Surfers** and **Temple Run**, with cinematic bullet-time kiss encounters and dynamic comic visual effects.

---

## 🏗️ Enhanced Project Architecture & C# Systems

```
kiss_and_run_unity/
└── Assets/
    ├── Scripts/
    │   ├── Cinematics/
    │   │   ├── KissCinematicDirector.cs # Matrix/Anime bullet-time slow-mo, dynamic 3/4 camera close-up, lipstick stamp
    │   │   └── ComicCallout3D.cs        # 3D floating comic impact text ("MWAH! 💋", "WHACK! 💥", "POW! ⭐")
    │   ├── VFX/
    │   │   └── CartoonVFXFactory.cs     # Procedural particle bursts (fluttering hearts, slap stars, siren spotlights)
    │   ├── Customization/
    │   │   └── CharacterSkinManager.cs  # Dressing room & character skins (Romeo, Juliet, Cyber Ninja, Auras)
    │   ├── Core/
    │   │   ├── GameManager.cs           # Master game loop, score, combo multiplier, health hearts
    │   │   ├── SaveManager.cs           # PlayerPrefs persistence (high score, coin balance, audio)
    │   │   └── SoundManager.cs          # AudioSource pooling (jump, slide, kiss, slap, siren, crash)
    │   ├── Player/
    │   │   ├── PlayerController.cs      # 3-Lane movement, lerp interpolation, jump/slide physics
    │   │   ├── SwipeDetector.cs         # Mobile touch swipe gestures (Left, Right, Up, Down) & keyboard fallback
    │   │   └── ThirdPersonCameraFollow.cs # Smooth over-the-shoulder runner camera with directional trauma shake
    │   ├── NPCs/
    │   │   ├── NPCController.cs         # Pedestrians with 3D models, facial reactions, speech bubbles
    │   │   └── KissManager.cs           # Proximity detection & 💋 KISS trigger execution
    │   ├── Pursuer/
    │   │   └── ChaserController.cs      # Behind-the-back angry NPC & Police Inspector (Subway Surfers style)
    │   ├── World/
    │   │   ├── TrackSpawner.cs          # Endless modular 3D track chunks pooling & recycling
    │   │   ├── Obstacle.cs              # Roadblocks (jump), overhead signs (slide), and banana peels (slip)
    │   │   ├── DominoCrashProp.cs       # 3D PhysX domino chain reaction scatter
    │   │   ├── PowerUpItem.cs           # Shields, Jet Boosts, Magnets, Slow-Mo
    │   │   └── HeartCoin.cs             # Rotating 3D heart coins with magnet physics
    │   └── UI/
    │       └── HUDController.cs         # Health hearts, score, coins, chaser proximity bar, pulsating 💋 KISS
    └── Editor/
        └── KissAndRunSceneSetup.cs      # ⚡ 1-Click Auto-Scene Builder
```

---

## 🎬 Dramatic & Graphic Kiss Scenes

When the player approaches an eligible pedestrian and taps **💋 KISS**, the game triggers a cinematic sequence:
1. **Bullet-Time Slow Motion**: Time scale smoothly drops to `0.20f` (Matrix/anime bullet-time effect).
2. **Cinematic 3/4 Camera Close-Up**: The camera smoothly swoops to a dynamic close-up of both faces, zooming in with focused FOV.
3. **Graphic Visual Impacts**:
   - **Lipstick Stamp Decal**: A glowing red/pink lipstick kiss mark (`💋`) stamps directly on the NPC's cheek.
   - **Heart Explosion Burst**: 25+ fluttering 3D hearts burst radially toward the lens.
   - **Floating 3D Comic Impact Callouts**: Elastic, bouncing comic text (`"MWAH!! 💋"`, `"SMOOCH! ❤️"`, `"WHACK!! 💥"`).
   - **Comic Slap Sequence**: If the NPC reacts negatively, a high-impact slap triggers screen trauma shake, comic stars orbiting their head, and turns the chaser active.
4. **Seamless Transition**: Accelerates back into real-time 3D running with an explosive speed boost or slap recoil.

---

## 🕹️ 1-Click Scene Setup in Unity:
1. Open **Unity Hub** and add the project:
   `C:\Users\User\Desktop\Cosmutech\kiss_and_run_unity`
2. Click the top toolbar menu:
   **`Kiss & Run` ➔ `⚡ Auto-Build 3D Runner Scene`**
3. Press **Play (▶️)** to experience the high-graphic 3D runner directly in the editor!
