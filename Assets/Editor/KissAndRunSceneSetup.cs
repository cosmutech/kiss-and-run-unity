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
            // 1. Create New Scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Setup Lighting & Sun
            GameObject sun = new GameObject("Directional Light (Sun)");
            Light lightComp = sun.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = new Color(1f, 0.95f, 0.85f);
            lightComp.intensity = 1.2f;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // 3. Setup Ground / Road (3-Lanes)
            GameObject roadHolder = new GameObject("=== TRACK SEGMENTS ===");
            for (int chunkIndex = 0; chunkIndex < 5; chunkIndex++)
            {
                CreateRoadChunk(roadHolder.transform, chunkIndex * 30f);
            }

            // 4. Create Player
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player (Romeo)";
            player.transform.position = new Vector3(0f, 1f, 0f);
            player.tag = "Player";

            // Color player pink/blue
            Material playerMat = new Material(Shader.Find("Standard"));
            playerMat.color = new Color(1f, 0.25f, 0.5f);
            player.GetComponent<Renderer>().material = playerMat;

            // Add Components
            Object.DestroyImmediate(player.GetComponent<Collider>());
            CharacterController charController = player.AddComponent<CharacterController>();
            charController.center = new Vector3(0, 1, 0);
            charController.height = 2f;
            charController.radius = 0.45f;

            player.AddComponent<SwipeDetector>();
            PlayerController playerCtrl = player.AddComponent<PlayerController>();

            // 5. Setup Main Camera
            GameObject cameraObj = new GameObject("Main Camera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.fieldOfView = 60f;
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();
            ThirdPersonCameraFollow camFollow = cameraObj.AddComponent<ThirdPersonCameraFollow>();
            camFollow.SetTarget(player.transform);

            // 6. Setup Pursuer (Chaser)
            GameObject chaser = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            chaser.name = "Chaser (Police / Angry Guy)";
            chaser.transform.position = new Vector3(0f, 1f, -15f);
            Material chaserMat = new Material(Shader.Find("Standard"));
            chaserMat.color = new Color(0.1f, 0.3f, 0.9f);
            chaser.GetComponent<Renderer>().material = chaserMat;
            ChaserController chaserCtrl = chaser.AddComponent<ChaserController>();

            // 7. Setup Game Managers
            GameObject managers = new GameObject("=== GAME MANAGERS ===");
            GameManager gameManager = managers.AddComponent<GameManager>();
            managers.AddComponent<SoundManager>();
            managers.AddComponent<KissManager>();
            TrackSpawner trackSpawner = managers.AddComponent<TrackSpawner>();

            // Wire references
            SerializedObject gmSerial = new SerializedObject(gameManager);
            gmSerial.FindProperty("player").objectReferenceValue = playerCtrl;
            gmSerial.FindProperty("chaser").objectReferenceValue = chaserCtrl;
            gmSerial.ApplyModifiedProperties();

            // 8. Setup UI Canvas
            SetupHUDCanvas(playerCtrl, chaserCtrl);

            // 9. Save Scene
            string scenePath = "Assets/Scenes/MainGame.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("<color=green><b>[Kiss & Run]</b> 3D Runner Scene successfully generated and saved to: " + scenePath + "</color>");
            EditorUtility.DisplayDialog("Kiss & Run Scene Ready!", "3D Subway Surfers style runner scene has been automatically built with 3-lane road, player, camera follow, and HUD!\n\nPress PLAY (▶) to test!", "Awesome!");
        }

        private static void CreateRoadChunk(Transform parent, float zPos)
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
            roadMat.color = new Color(0.15f, 0.18f, 0.22f);
            road.GetComponent<Renderer>().material = roadMat;

            // Curbs (Sidewalk borders)
            CreateCurb(chunk.transform, -4.75f, 15f);
            CreateCurb(chunk.transform, 4.75f, 15f);
        }

        private static void CreateCurb(Transform parent, float x, float z)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = "Curb";
            curb.transform.parent = parent;
            curb.transform.localPosition = new Vector3(x, 0.1f, z);
            curb.transform.localScale = new Vector3(0.5f, 0.8f, 30f);

            Material curbMat = new Material(Shader.Find("Standard"));
            curbMat.color = new Color(0.9f, 0.3f, 0.4f);
            curb.GetComponent<Renderer>().material = curbMat;
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
