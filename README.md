# 💋 Kiss & Run — High-Fidelity 3D Unity Engine Project

A complete, feature-rich 3D endless runner built in **Unity Engine (C# & URP)** matching the visual perspective, 3-lane physics, and mobile swipe controls of **Subway Surfers** and **Temple Run**, with cinematic bullet-time kiss encounters, hoverboard cruising, aerial stunt tricks, jetpack flight, and dynamic comic visual effects.

---

## 🏗️ Enhanced Project Architecture & C# Systems

```
kiss_and_run_unity/
└── Assets/
    ├── Scripts/
    │   ├── Cinematics/
    │   │   ├── KissCinematicDirector.cs # Matrix/Anime bullet-time slow-mo, dynamic 3/4 camera close-up, glowing lipstick stamp
    │   │   └── ComicCallout3D.cs        # 3D floating comic impact text ("MWAH! 💋", "WHACK! 💥", "POW! ⭐")
    │   ├── VFX/
    │   │   └── CartoonVFXFactory.cs     # Procedural particle bursts (fluttering hearts, slap stars, siren spotlights)
    │   ├── Customization/
    │   │   ├── CharacterWardrobeCatalog.cs # Full catalog of Characters, Headgear, Trails & Hoverboards
    │   │   └── CharacterSkinManager.cs     # Dressing room & character skins (Romeo, Juliet, Cyber Ninja, Auras)
    │   ├── Core/
    │   │   ├── GameManager.cs           # Master game loop, score, combo multiplier, health hearts
    │   │   ├── SaveManager.cs           # PlayerPrefs persistence (high score, coin balance, audio)
    │   │   └── SoundManager.cs          # AudioSource pooling (jump, slide, kiss, slap, siren, crash)
    │   ├── Player/
    │   │   ├── PlayerController.cs      # 3-Lane movement, lerp interpolation, jump/slide physics, hoverboard summon
    │   │   ├── HoverboardSystem.cs      # Subway Surfers style hoverboard riding, neon underglow, crash shield protection
    │   │   ├── JetpackSystem.cs         # Sky altitude rocket flight, dual thrusters, aerial coin cruising
    │   │   ├── StuntTrickSystem.cs      # 360 spins, backflips, and barrel rolls during jump ramps
    │   │   ├── SwipeDetector.cs         # Mobile touch swipe gestures (Left, Right, Up, Down, Double-Tap) & keyboard fallback
    │   │   └── ThirdPersonCameraFollow.cs # Smooth over-the-shoulder runner camera with directional trauma shake
    │   ├── NPCs/
    │   │   ├── NPCController.cs         # Pedestrians with 3D models, facial reactions, speech bubbles
    │   │   └── KissManager.cs           # Proximity detection & 💋 KISS trigger execution
    │   ├── Pursuer/
    │   │   └── ChaserController.cs      # Behind-the-back angry NPC & Police Inspector (Subway Surfers style)
    │   ├── World/
    │   │   ├── TrackSpawner.cs          # Endless procedural 3D track chunks with asphalt, street lamps, ramps & curbs
    │   │   ├── JumpRamp.cs              # 3D launch ramps triggering aerial stunts & bonus score
    │   │   ├── Obstacle.cs              # Roadblocks (jump), overhead signs (slide), and banana peels (slip)
    │   │   ├── DominoCrashProp.cs       # 3D PhysX domino chain reaction scatter
    │   │   ├── PowerUpItem.cs           # Shields, Jet Boosts, Magnets, Slow-Mo
    │   │   └── HeartCoin.cs             # Rotating 3D heart coins with magnet physics
    │   └── UI/
    │       └── HUDController.cs         # Health hearts, score, coins, chaser proximity bar, hoverboard/jetpack gauges, pulsating 💋 KISS
    └── Editor/
        └── KissAndRunSceneSetup.cs      # ⚡ 1-Click Auto-Scene Builder
```

---

## 🚀 High-Fun Running Mechanics (Subway Surfers & Temple Run Inspired)

1. **🛹 Hoverboard Riding with Crash Shield**:
   - Double-tap screen or tap the HUD hoverboard icon to summon a hoverboard.
   - Illuminates the road with dynamic neon underglow and particle speed lines.
   - **Crash Protection**: If you hit a hurdle, roadblock, or obstacle while riding, the hoverboard shatters and absorbs 100% of the impact, saving your run!
2. **🚀 High-Altitude Jetpack Flight**:
   - Collect the Jetpack power-up to rocket up into the sky.
   - Cruise at high altitude over all traffic and roadblocks while sweeping arcs of floating coins with dual exhaust thrusters.
3. **🤸 Jump Ramps & Aerial Stunts**:
   - Run onto 3D angled launch ramps placed on lanes to propel high into the air.
   - Automatically executes radical acrobatics (360 spins, backflips, barrel rolls) with bonus callouts (`"RADICAL BACKFLIP! +250 🤸"`).
4. **📦 PhysX Domino Destruction**:
   - Food carts and wooden crates shatter into physical rigidbodies upon impact, scattering debris dynamically across the road.

---

## 👗 Character Wardrobe & Customizations

Players can unlock and equip a deep wardrobe of skins and accessories using collected Heart Coins:
- **Playable Characters**:
  - 🌹 *Romeo (Streetwear)* — Default urban runner
  - ⛸️ *Juliet (Speed Skater)* — Agile skater with custom animations
  - 🤖 *Cyber Neon* — Glowing cyberpunk aesthetic
  - 👑 *Golden King* — Luxurious gold trim and royal aura
  - 🪩 *Disco Superstar* — 70s retro style
  - 🤡 *Prankster Clown* — Comic street entertainer
- **Headgear & Glasses**: Thug Life Shades, DJ Kitty Headphones, Royal Crown, Neon Cyber Visor, Pink Bunny Ears, Ninja Bandana.
- **Particle Trails**: Fluttering Hearts, Fire Sprint, Cyber Matrix, Cash Storm.
- **Custom Hoverboards**: Cyber Cruiser, Flame Rocket, Heart Glider, Midas Goldboard.

---

## 🎬 Dramatic & Graphic Kiss Scenes

When the player approaches an eligible pedestrian and taps **💋 KISS**, the game triggers an authentic cinematic sequence:
1. **Bullet-Time Slow Motion**: Time scale smoothly drops to `0.20f` (Matrix/anime bullet-time effect).
2. **Cinematic 3/4 Camera Close-Up**: The camera swoops to a dynamic close-up of both faces, zooming in with focused FOV.
3. **Graphic Visual Impacts**:
   - **Lipstick Stamp Decal**: A glowing red/pink lipstick kiss mark (`💋`) stamps directly on the NPC's cheek.
   - **Heart Explosion Burst**: 25+ fluttering 3D hearts burst radially toward the lens.
   - **Floating 3D Comic Impact Callouts**: Elastic, bouncing comic text (`"MWAH!! 💋"`, `"SMOOCH! ❤️"`, `"WHACK!! 💥"`).
   - **Comic Slap Sequence**: If the NPC reacts negatively, a high-impact slap triggers screen trauma shake, comic stars orbiting their head, and alerts the chaser.
4. **Seamless Transition**: Accelerates back into real-time 3D running with an explosive speed boost or slap recoil.

---

## 🕹️ 1-Click Scene Setup in Unity:
1. Open **Unity Hub** and add the project:
   `C:\Users\User\Desktop\Cosmutech\kiss_and_run_unity`
2. Click the top toolbar menu:
   **`Kiss & Run` ➔ `⚡ Auto-Build 3D Runner Scene`**
3. Press **Play (▶️)** to experience the high-graphic 3D runner directly in the editor!
