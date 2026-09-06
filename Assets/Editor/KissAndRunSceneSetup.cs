#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KissAndRun.Editor
{
    public static class KissAndRunSceneSetup
    {
        [MenuItem("Kiss & Run/⚡ Auto-Build 3D Runner Scene")]
        public static void BuildScene()
        {
            // 1. Create New Empty Scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Setup Lighting, Warm Cartoon Sun, Shadows & Atmospheric Fog
            GameObject sun = new GameObject("Directional Light (Sun)");
            Light lightComp = sun.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = new Color(1f, 0.96f, 0.88f);
            lightComp.intensity = 1.35f;
            lightComp.shadows = LightShadows.Soft;
            lightComp.shadowStrength = 0.70f;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Atmospheric Fog & Ambient Cartoon Lighting
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialWithDistance;
            RenderSettings.fogColor = new Color(0.70f, 0.85f, 1f);
            RenderSettings.fogDensity = 0.0065f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.75f, 0.88f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.92f, 0.82f, 0.70f);
            RenderSettings.ambientGroundColor = new Color(0.35f, 0.28f, 0.22f);

            // 3. Setup Ground / Road (3-Lanes) with Street Lamps
            GameObject roadHolder = new GameObject("=== TRACK SEGMENTS ===");
            for (int chunkIndex = 0; chunkIndex < 7; chunkIndex++)
            {
                CreateDetailedRoadChunk(roadHolder.transform, chunkIndex * 30f);
            }

            // 4. Create Player Rig (Romeo)
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player (Romeo)";
            player.transform.position = new Vector3(0f, 1f, 0f);
            player.tag = "Player";

            // High-Graphic Stylized Toon Material
            player.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(1f, 0.25f, 0.55f), Color.white, new Color(0.2f, 0.05f, 0.1f));

            // Character Controller & Movement
            Object.DestroyImmediate(player.GetComponent<Collider>());
            CharacterController charController = player.AddComponent<CharacterController>();
            charController.center = new Vector3(0, 1, 0);
            charController.height = 2f;
            charController.radius = 0.45f;

            player.AddComponent<SwipeDetector>();
            PlayerController playerCtrl = player.AddComponent<PlayerController>();
            player.AddComponent<HoverboardSystem>();
            player.AddComponent<JetpackSystem>();
            player.AddComponent<StuntTrickSystem>();

            // 5. Setup Main Camera with Third-Person Follow
            GameObject cameraObj = new GameObject("Main Camera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.3f;
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();
            ThirdPersonCameraFollow camFollow = cameraObj.AddComponent<ThirdPersonCameraFollow>();
            camFollow.SetTarget(player.transform);

            // 6. Setup Pursuer (Chaser Rig)
            GameObject chaser = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            chaser.name = "Chaser (Police / Angry Guy)";
            chaser.transform.position = new Vector3(0f, 1f, -15f);
            chaser.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(0.12f, 0.35f, 0.9f), Color.cyan);
            ChaserController chaserCtrl = chaser.AddComponent<ChaserController>();

            // 7. Setup Game Managers & Cinematic Directors
            GameObject managers = new GameObject("=== GAME MANAGERS ===");
            GameManager gameManager = managers.AddComponent<GameManager>();
            managers.AddComponent<SoundManager>();
            managers.AddComponent<CartoonVFXFactory>();
            KissManager kissManager = managers.AddComponent<KissManager>();
            KissCinematicDirector cinematicDirector = managers.AddComponent<KissCinematicDirector>();
            TrackSpawner trackSpawner = managers.AddComponent<TrackSpawner>();
            managers.AddComponent<CharacterWardrobeCatalog>();
            managers.AddComponent<CharacterSkinManager>();

            // Wire references
            SerializedObject gmSerial = new SerializedObject(gameManager);
            gmSerial.FindProperty("player").objectReferenceValue = playerCtrl;
            gmSerial.FindProperty("chaser").objectReferenceValue = chaserCtrl;
            gmSerial.ApplyModifiedProperties();

            SerializedObject trackSerial = new SerializedObject(trackSpawner);
            trackSerial.FindProperty("playerTransform").objectReferenceValue = player.transform;
            trackSerial.ApplyModifiedProperties();

            SerializedObject kissSerial = new SerializedObject(kissManager);
            kissSerial.FindProperty("playerTransform").objectReferenceValue = player.transform;
            kissSerial.ApplyModifiedProperties();

            // 8. Setup UI Canvas & Comic Banners
            SetupHUDCanvas(playerCtrl, chaserCtrl);

            // 9. Save Scene
            string scenePath = "Assets/Scenes/MainGame.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("<color=green><b>[Kiss & Run]</b> Complete High-Graphic 3D Runner Scene successfully generated and saved to: " + scenePath + "</color>");
            EditorUtility.DisplayDialog("Kiss & Run 3D Scene Ready!", "High-fidelity 3D Runner Scene has been automatically built!\n\nIncludes:\n• 3-Lane Road & Street Lamps\n• Stylized Cel-Shaded Toon Shaders\n• Atmospheric Horizon Fog & Sun Shadows\n• Hoverboard Riding & Crash Shield\n• High-Altitude Jetpack Flight\n• Aerial Stunt Tricks (Spins, Flips)\n• Cinematic Bullet-Time Kiss Director (Slow-Mo & Decals)\n• Third-Person Camera Follow & Dynamic Screen Shake\n• Chaser System & Domino Props\n• Character Wardrobe & Customizations\n\nPress PLAY (▶) to test immediately!", "Awesome!");
        }

        private static Material CreateStylizedMaterial(Color baseColor, Color rimColor, Color emissionColor = default)
        {
            Shader toonShader = Shader.Find("KissAndRun/StylizedToon");
            if (toonShader == null) toonShader = Shader.Find("Standard");

            Material mat = new Material(toonShader);
            mat.color = baseColor;
            if (mat.HasProperty("_RimColor")) mat.SetColor("_RimColor", rimColor);
            if (emissionColor != default && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor);
            }
            return mat;
        }

        private static void CreateDetailedRoadChunk(Transform parent, float zPos)
        {
            GameObject chunk = new GameObject("RoadChunk_Z" + zPos);
            chunk.transform.parent = parent;
            chunk.transform.position = new Vector3(0, 0, zPos);

            // Asphalt Road
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Asphalt";
            road.transform.parent = chunk.transform;
            road.transform.localPosition = new Vector3(0, -0.25f, 15f);
            road.transform.localScale = new Vector3(9f, 0.5f, 30f);
            road.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(0.18f, 0.22f, 0.26f), new Color(0.4f, 0.45f, 0.5f));

            // Lane Divider Dashes
            for (float z = 2.5f; z < 30f; z += 6f)
            {
                CreateLaneStripe(chunk.transform, -1.25f, z);
                CreateLaneStripe(chunk.transform, 1.25f, z);
            }

            // Sidewalk Curbs
            CreateCurb(chunk.transform, -4.75f, 15f);
            CreateCurb(chunk.transform, 4.75f, 15f);

            // Street Lamps on sidewalks with real lights
            CreateStreetLamp(chunk.transform, -5.4f, 10f);
            CreateStreetLamp(chunk.transform, 5.4f, 25f);

            // Spawn Multiple NPCs and Sidewalk Spectators on active chunks
            if (zPos >= 30f)
            {
                NPCType typeA = (NPCType)(((int)zPos / 30) % 10);
                NPCType typeB = (NPCType)((((int)zPos / 30) + 3) % 10);

                CreateSceneNPC(chunk.transform, -2.5f, zPos + 10f, typeA);
                CreateSceneNPC(chunk.transform, 2.5f, zPos + 22f, typeB);

                CreateSceneSpectator(chunk.transform, -4.85f, zPos + 8f);
                CreateSceneSpectator(chunk.transform, 4.85f, zPos + 18f);
            }
        }

        private static void CreateLaneStripe(Transform parent, float x, float z)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = "Stripe";
            stripe.transform.parent = parent;
            stripe.transform.localPosition = new Vector3(x, 0.02f, z);
            stripe.transform.localScale = new Vector3(0.2f, 0.05f, 2.5f);

            stripe.GetComponent<Renderer>().material = CreateStylizedMaterial(Color.white, Color.white);
            Object.DestroyImmediate(stripe.GetComponent<Collider>());
        }

        private static void CreateCurb(Transform parent, float x, float z)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = "Curb";
            curb.transform.parent = parent;
            curb.transform.localPosition = new Vector3(x, 0.15f, z);
            curb.transform.localScale = new Vector3(0.6f, 0.8f, 30f);

            curb.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(0.9f, 0.28f, 0.42f), new Color(1f, 0.6f, 0.7f));
        }

        private static void CreateStreetLamp(Transform parent, float x, float z)
        {
            GameObject lamp = new GameObject("StreetLamp");
            lamp.transform.parent = parent;
            lamp.transform.localPosition = new Vector3(x, 0f, z);

            // Pole
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.parent = lamp.transform;
            pole.transform.localPosition = new Vector3(0, 3f, 0);
            pole.transform.localScale = new Vector3(0.15f, 3f, 0.15f);

            // Bulb & Light
            GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.transform.parent = lamp.transform;
            bulb.transform.localPosition = new Vector3(x < 0 ? 0.6f : -0.6f, 6.2f, 0);
            bulb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            bulb.GetComponent<Renderer>().material = CreateStylizedMaterial(Color.yellow, Color.white, new Color(1f, 0.9f, 0.4f));

            Light pLight = bulb.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(1f, 0.92f, 0.7f);
            pLight.range = 8f;
            pLight.intensity = 1.2f;
        }

        private static void CreateSceneNPC(Transform parent, float x, float z, NPCType type)
        {
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcObj.name = "NPC_" + type.ToString();
            npcObj.transform.parent = parent;
            npcObj.transform.position = new Vector3(x, 1f, z);
            npcObj.transform.rotation = Quaternion.Euler(0, 180, 0);

            NPCController npcCtrl = npcObj.AddComponent<NPCController>();
            npcCtrl.ConfigureArchetype(type);

            Color outfitColor = type == NPCType.GymBro ? Color.red : (type == NPCType.BusinessExec ? new Color(0.12f, 0.18f, 0.32f) : new Color(1f, 0.25f, 0.6f));
            npcObj.GetComponent<Renderer>().material = CreateStylizedMaterial(outfitColor, Color.white);

            // Hair
            GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hair.transform.parent = npcObj.transform;
            hair.transform.localPosition = new Vector3(0, 0.8f, 0);
            hair.transform.localScale = new Vector3(0.9f, 0.7f, 0.9f);
            hair.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(0.2f, 0.12f, 0.05f), Color.white);
            Object.DestroyImmediate(hair.GetComponent<Collider>());

            // Halo prompt
            GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            halo.name = "KissHaloPrompt";
            halo.transform.parent = npcObj.transform;
            halo.transform.localPosition = new Vector3(0, 1.35f, 0);
            halo.transform.localScale = new Vector3(0.7f, 0.04f, 0.7f);
            Color haloColor = (type == NPCType.GymBro || type == NPCType.PoliceCadet) ? Color.red : new Color(1f, 0.1f, 0.5f);
            halo.GetComponent<Renderer>().material = CreateStylizedMaterial(haloColor, Color.white, haloColor);
            Object.DestroyImmediate(halo.GetComponent<Collider>());
        }

        private static void CreateSceneSpectator(Transform parent, float x, float z)
        {
            GameObject spectator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            spectator.name = "Spectator_Crowd";
            spectator.transform.parent = parent;
            spectator.transform.position = new Vector3(x, 1f, z);
            spectator.transform.localScale = new Vector3(0.8f, 0.85f, 0.8f);
            spectator.transform.rotation = Quaternion.Euler(0, x < 0 ? 90f : -90f, 0);

            spectator.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(0.2f, 0.7f, 0.95f), Color.white);
            Object.DestroyImmediate(spectator.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.parent = spectator.transform;
            head.transform.localPosition = new Vector3(0, 0.75f, 0);
            head.transform.localScale = new Vector3(0.85f, 0.7f, 0.85f);
            head.GetComponent<Renderer>().material = CreateStylizedMaterial(new Color(0.3f, 0.2f, 0.1f), Color.white);
            Object.DestroyImmediate(head.GetComponent<Collider>());

            spectator.AddComponent<SpectatorCheer>();
        }

        private static void SetupHUDCanvas(PlayerController player, ChaserController chaser)
        {
            GameObject canvasObj = new GameObject("HUD Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasObj.AddComponent<GraphicRaycaster>();

            // HUD Controller
            canvasObj.AddComponent<HUDController>();
        }
    }
}
#endif
