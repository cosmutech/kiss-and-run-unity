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

            // 2. Setup Lighting, Warm Cartoon Sun, and Shadows
            GameObject sun = new GameObject("Directional Light (Sun)");
            Light lightComp = sun.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = new Color(1f, 0.95f, 0.85f);
            lightComp.intensity = 1.35f;
            lightComp.shadows = LightShadows.Soft;
            lightComp.shadowStrength = 0.65f;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

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

            // Stylized Material
            Material playerMat = new Material(Shader.Find("Standard"));
            playerMat.color = new Color(1f, 0.25f, 0.55f);
            player.GetComponent<Renderer>().material = playerMat;

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
            Material chaserMat = new Material(Shader.Find("Standard"));
            chaserMat.color = new Color(0.12f, 0.35f, 0.9f);
            chaser.GetComponent<Renderer>().material = chaserMat;
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
            EditorUtility.DisplayDialog("Kiss & Run 3D Scene Ready!", "High-fidelity 3D Runner Scene has been automatically built!\n\nIncludes:\n• 3-Lane Road & Street Lamps\n• Hoverboard Riding & Crash Shield\n• High-Altitude Jetpack Flight\n• Aerial Stunt Tricks (Spins, Flips)\n• Cinematic Bullet-Time Kiss Director (Slow-Mo & Decals)\n• Third-Person Camera Follow & Dynamic Screen Shake\n• Chaser System & Domino Props\n• Character Wardrobe & Customizations\n\nPress PLAY (▶) to test immediately!", "Awesome!");
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

            Material roadMat = new Material(Shader.Find("Standard"));
            roadMat.color = new Color(0.18f, 0.22f, 0.26f);
            road.GetComponent<Renderer>().material = roadMat;

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
        }

        private static void CreateLaneStripe(Transform parent, float x, float z)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = "Stripe";
            stripe.transform.parent = parent;
            stripe.transform.localPosition = new Vector3(x, 0.02f, z);
            stripe.transform.localScale = new Vector3(0.2f, 0.05f, 2.5f);

            Material stripeMat = new Material(Shader.Find("Standard"));
            stripeMat.color = Color.white;
            stripe.GetComponent<Renderer>().material = stripeMat;
            Object.DestroyImmediate(stripe.GetComponent<Collider>());
        }

        private static void CreateCurb(Transform parent, float x, float z)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = "Curb";
            curb.transform.parent = parent;
            curb.transform.localPosition = new Vector3(x, 0.15f, z);
            curb.transform.localScale = new Vector3(0.6f, 0.8f, 30f);

            Material curbMat = new Material(Shader.Find("Standard"));
            curbMat.color = new Color(0.9f, 0.28f, 0.42f);
            curb.GetComponent<Renderer>().material = curbMat;
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

            Material bulbMat = new Material(Shader.Find("Standard"));
            bulbMat.color = Color.yellow;
            bulb.GetComponent<Renderer>().material = bulbMat;

            Light pLight = bulb.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(1f, 0.92f, 0.7f);
            pLight.range = 8f;
            pLight.intensity = 1.2f;
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
