# 💋 Kiss & Run — Unity 3D Engine Project

A complete, high-performance 3D endless lane runner developed in **Unity Engine (C# & URP)** matching the gameplay and visual mechanics of **Subway Surfers** and **Temple Run**.

---

## 🏗️ Project Architecture & C# Scripts

```
kiss_and_run_unity/
└── Assets/
    └── Scripts/
        ├── Core/
        │   ├── GameManager.cs      # Master game loop, score, combo multiplier, health hearts
        │   ├── SaveManager.cs      # PlayerPrefs persistence (high score, coin balance, audio)
        │   └── SoundManager.cs     # AudioSource pooling (jump, slide, kiss, slap, siren, crash)
        ├── Player/
        │   ├── PlayerController.cs # 3-Lane movement, lerp interpolation, jump/slide physics
        │   └── SwipeDetector.cs    # Mobile touch swipe gestures (Left, Right, Up, Down) & keyboard fallback
        ├── NPCs/
        │   ├── NPCController.cs    # Pedestrians with 3D models, facial reactions, speech bubbles
        │   └── KissManager.cs      # Proximity detection & 💋 KISS trigger execution
        ├── Pursuer/
        │   └── ChaserController.cs # Behind-the-back angry NPC & Police Inspector (Subway Surfers style)
        ├── World/
        │   ├── TrackSpawner.cs     # Endless modular 3D track chunks pooling & recycling
        │   ├── Obstacle.cs         # Roadblocks (jump), overhead signs (slide), and banana peels (slip)
        │   └── HeartCoin.cs        # Rotating 3D heart coins with magnet physics
        └── UI/
            └── HUDController.cs    # Health hearts, score, coins, chaser proximity bar, pulsating 💋 KISS
```

---

## 🎮 How the 3D Systems Work

1. **3-Lane Running Grid (`PlayerController.cs` & `SwipeDetector.cs`)**:
   - 3 Lanes: Left (`X = -2.5`), Center (`X = 0`), Right (`X = 2.5`).
   - Swipe Left/Right smoothly lerps the character with banking tilt.
   - Swipe Up triggers high jump trajectory.
   - Swipe Down triggers fast slide (temporarily shrinking the character collider to slide under overhead signs).
   - In Unity Editor, you can test with `A/D` (or Left/Right Arrows), `W/Space` (Jump), and `S` (Slide).

2. **The Behind-the-Back Chaser (`ChaserController.cs`)**:
   - When an NPC reacts angrily or police are called, the pursuer runs right behind the player's back in the 3D viewport.
   - Real-time proximity slider shows how close the chaser is.
   - Tripping or hitting an obstacle causes the chaser to lunge forward.
   - Clean running pulls away until an escape bonus (+250 pts) is awarded!

3. **Endless Track Pooling (`TrackSpawner.cs`)**:
   - Generates 30m modular road chunks ahead.
   - Recycles chunks that pass behind the player to maintain solid **60/120 FPS**.
   - Spawns roadblocks, clearance signs, banana peels, and arched coin paths.

4. **Proximity 💋 Kiss (`KissManager.cs` & `NPCController.cs`)**:
   - Detects pedestrians in front of the player.
   - Lights up a neon heart halo under their feet and illuminates the **💋 KISS** button.
   - Triggers 15 distinct reactions with comic speech bubbles and sound effects.

---

## 🚀 How to Open in Unity

1. Open **Unity Hub**.
2. Click **Add** ➔ **Add project from disk**.
3. Select this folder: `C:\Users\User\Desktop\Cosmutech\kiss_and_run_unity`.
4. Choose **Unity 2022.3 LTS** or **Unity 6**.
5. Switch build platform to **Android** in *File ➔ Build Settings*.
6. Press **Play (▶️)** to test the full 3D gameplay immediately!
