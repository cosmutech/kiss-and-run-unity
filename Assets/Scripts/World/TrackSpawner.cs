using System.Collections.Generic;
using UnityEngine;

namespace KissAndRun
{
    public class TrackSpawner : MonoBehaviour
    {
        [Header("Prefabs (Optional - Will procedurally generate if empty)")]
        [SerializeField] private GameObject[] trackChunkPrefabs;
        [SerializeField] private GameObject[] npcPrefabs;
        [SerializeField] private GameObject roadblockPrefab;
        [SerializeField] private GameObject overheadSignPrefab;
        [SerializeField] private GameObject bananaPeelPrefab;
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject jumpRampPrefab;

        [Header("Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float chunkLength = 30f;
        [SerializeField] private int initialChunks = 7;
        [SerializeField] private float safeZoneLength = 40f;

        private float spawnZ = 0f;
        private readonly List<GameObject> activeChunks = new List<GameObject>();
        private readonly float[] lanePositions = new float[] { -2.5f, 0f, 2.5f };

        private void Start()
        {
            if (playerTransform == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTransform = p.transform;
            }

            // Spawn initial safe track segments
            for (int i = 0; i < initialChunks; i++)
            {
                SpawnChunk(spawnWithObstacles: i > 1);
            }
        }

        private void Update()
        {
            if (playerTransform == null) return;

            // Spawn next chunk ahead when player moves forward
            if (playerTransform.position.z + (chunkLength * (initialChunks - 2)) > spawnZ)
            {
                SpawnChunk(spawnWithObstacles: true);
                RecycleOldestChunk();
            }
        }

        private void SpawnChunk(bool spawnWithObstacles)
        {
            GameObject chunk;
            if (trackChunkPrefabs != null && trackChunkPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, trackChunkPrefabs.Length);
                chunk = Instantiate(trackChunkPrefabs[randomIndex], Vector3.forward * spawnZ, Quaternion.identity, transform);
            }
            else
            {
                chunk = CreateProceduralChunk(spawnZ);
            }

            activeChunks.Add(chunk);

            if (spawnWithObstacles)
            {
                PopulateChunk(chunk, spawnZ);
            }

            spawnZ += chunkLength;
        }

        private GameObject CreateProceduralChunk(float zPos)
        {
            GameObject chunk = new GameObject("TrackChunk_Z" + zPos);
            chunk.transform.parent = transform;
            chunk.transform.position = new Vector3(0, 0, zPos);

            // Asphalt road bed
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Asphalt";
            road.transform.parent = chunk.transform;
            road.transform.localPosition = new Vector3(0, -0.25f, 15f);
            road.transform.localScale = new Vector3(9f, 0.5f, 30f);
            road.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.18f, 0.22f, 0.26f), new Color(0.4f, 0.45f, 0.5f));

            // Lane Divider Dashes
            for (float z = 2.5f; z < 30f; z += 6f)
            {
                CreateLaneStripe(chunk.transform, -1.25f, z);
                CreateLaneStripe(chunk.transform, 1.25f, z);
            }

            // Sidewalk Curbs
            CreateCurb(chunk.transform, -4.75f, 15f);
            CreateCurb(chunk.transform, 4.75f, 15f);

            // Street Lamps with soft illumination
            CreateStreetLamp(chunk.transform, -5.4f, 8f);
            CreateStreetLamp(chunk.transform, 5.4f, 22f);

            return chunk;
        }

        private void CreateLaneStripe(Transform parent, float x, float z)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = "Stripe";
            stripe.transform.parent = parent;
            stripe.transform.localPosition = new Vector3(x, 0.02f, z);
            stripe.transform.localScale = new Vector3(0.2f, 0.05f, 2.5f);

            stripe.GetComponent<Renderer>().material = CreateToonMaterial(Color.white, Color.white);
            Destroy(stripe.GetComponent<Collider>());
        }

        private void CreateCurb(Transform parent, float x, float z)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = "Curb";
            curb.transform.parent = parent;
            curb.transform.localPosition = new Vector3(x, 0.15f, z);
            curb.transform.localScale = new Vector3(0.6f, 0.8f, 30f);

            curb.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.9f, 0.28f, 0.42f), new Color(1f, 0.6f, 0.7f));
        }

        private void CreateStreetLamp(Transform parent, float x, float z)
        {
            GameObject lamp = new GameObject("StreetLamp");
            lamp.transform.parent = parent;
            lamp.transform.localPosition = new Vector3(x, 0f, z);

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.parent = lamp.transform;
            pole.transform.localPosition = new Vector3(0, 3f, 0);
            pole.transform.localScale = new Vector3(0.12f, 3f, 0.12f);
            pole.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.25f, 0.25f, 0.28f), Color.gray);
            Destroy(pole.GetComponent<Collider>());

            GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.transform.parent = lamp.transform;
            bulb.transform.localPosition = new Vector3(x < 0 ? 0.6f : -0.6f, 6.2f, 0);
            bulb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Destroy(bulb.GetComponent<Collider>());

            bulb.GetComponent<Renderer>().material = CreateToonMaterial(new Color(1f, 0.95f, 0.5f), Color.white, new Color(1f, 0.9f, 0.4f));

            Light pLight = bulb.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(1f, 0.92f, 0.7f);
            pLight.range = 8f;
            pLight.intensity = 1.2f;
        }

        private void PopulateChunk(GameObject chunk, float currentZ)
        {
            // 1. Pick lanes for Multiple NPC Encounters in this segment
            int firstNpcLane = Random.Range(0, 3);
            int secondNpcLane = (firstNpcLane + Random.Range(1, 3)) % 3;

            // Spawn Primary NPC at Z+9
            NPCType type1 = (NPCType)Random.Range(0, System.Enum.GetValues(typeof(NPCType)).Length);
            CreateProceduralNPC(chunk.transform, lanePositions[firstNpcLane], currentZ + 9f, type1);

            // Spawn Secondary NPC at Z+22 (Different archetype!)
            NPCType type2 = (NPCType)Random.Range(0, System.Enum.GetValues(typeof(NPCType)).Length);
            CreateProceduralNPC(chunk.transform, lanePositions[secondNpcLane], currentZ + 22f, type2);

            // 2. Sidewalk Spectators (Cheering crowds lining the road!)
            CreateSidewalkSpectator(chunk.transform, -4.85f, currentZ + 6f);
            CreateSidewalkSpectator(chunk.transform, -4.85f, currentZ + 20f);
            CreateSidewalkSpectator(chunk.transform, 4.85f, currentZ + 12f);
            CreateSidewalkSpectator(chunk.transform, 4.85f, currentZ + 26f);

            // 3. Place Obstacles, Ramps, or Props in lanes that don't directly overlap NPCs
            for (int i = 0; i < 3; i++)
            {
                float r = Random.value;
                float laneX = lanePositions[i];

                if (i != firstNpcLane && r < 0.35f)
                {
                    // 3D Jump Ramp (Launches player into stunt trick!)
                    CreateProceduralJumpRamp(chunk.transform, laneX, currentZ + 9f);
                }
                else if (i != secondNpcLane && r < 0.65f)
                {
                    // Low Hurdle (Jump over!)
                    CreateProceduralHurdle(chunk.transform, laneX, currentZ + 16f);
                }
                else if (r < 0.85f)
                {
                    // Overhead Banner (Slide under!)
                    CreateProceduralOverheadSign(chunk.transform, laneX, currentZ + 16f);
                }
                else
                {
                    // Domino Crates (Crash prop!)
                    CreateProceduralDominoCrate(chunk.transform, laneX, currentZ + 16f);
                }
            }

            // 4. Place Coin Arcs in a free lane
            int coinLaneIndex = Random.Range(0, 3);
            float coinX = lanePositions[coinLaneIndex];
            for (int c = 0; c < 4; c++)
            {
                float arcY = Mathf.Sin((c / 3f) * Mathf.PI) * 2f;
                Vector3 coinPos = new Vector3(coinX, 0.6f + arcY, currentZ + 3f + (c * 3.2f));
                CreateProceduralCoin(chunk.transform, coinPos);
            }

            // 5. Occasional Power-up Item (20% chance)
            if (Random.value < 0.20f)
            {
                int powerLane = Random.Range(0, 3);
                Vector3 powerPos = new Vector3(lanePositions[powerLane], 1.2f, currentZ + 27f);
                CreateProceduralPowerUp(chunk.transform, powerPos);
            }
        }

        private void CreateProceduralNPC(Transform parent, float x, float z, NPCType type)
        {
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcObj.name = "NPC_" + type.ToString();
            npcObj.transform.parent = parent;
            npcObj.transform.position = new Vector3(x, 1f, z);
            npcObj.transform.rotation = Quaternion.Euler(0, 180, 0);

            // Configure Archetype Logic & Dialogues
            NPCController npcCtrl = npcObj.AddComponent<NPCController>();
            npcCtrl.ConfigureArchetype(type);

            // Visual Styling based on Archetype
            Color outfitColor = Color.magenta;
            Color hairColor = new Color(0.2f, 0.12f, 0.05f);

            switch (type)
            {
                case NPCType.Cheerleader:
                    outfitColor = new Color(1f, 0.25f, 0.6f);
                    hairColor = new Color(1f, 0.85f, 0.3f); // Blonde
                    break;
                case NPCType.GymBro:
                    outfitColor = new Color(0.9f, 0.15f, 0.15f);
                    hairColor = new Color(0.1f, 0.1f, 0.1f);
                    npcObj.transform.localScale = new Vector3(1.15f, 1.05f, 1.15f); // Muscular build
                    break;
                case NPCType.GothGirl:
                    outfitColor = new Color(0.15f, 0.12f, 0.2f);
                    hairColor = new Color(0.4f, 0.1f, 0.5f); // Purple
                    break;
                case NPCType.Influencer:
                    outfitColor = new Color(0.1f, 0.9f, 0.85f);
                    hairColor = new Color(0.9f, 0.5f, 0.3f);
                    break;
                case NPCType.BusinessExec:
                    outfitColor = new Color(0.12f, 0.18f, 0.32f); // Navy suit
                    hairColor = new Color(0.3f, 0.3f, 0.35f);
                    break;
                case NPCType.Teacher:
                    outfitColor = new Color(0.15f, 0.5f, 0.25f);
                    hairColor = new Color(0.45f, 0.25f, 0.15f);
                    break;
                case NPCType.Granny:
                    outfitColor = new Color(0.7f, 0.5f, 0.8f);
                    hairColor = new Color(0.9f, 0.9f, 0.92f); // White hair
                    break;
                case NPCType.Skater:
                    outfitColor = new Color(1f, 0.5f, 0.05f);
                    hairColor = new Color(0.6f, 0.35f, 0.1f);
                    break;
                case NPCType.PoliceCadet:
                    outfitColor = new Color(0.08f, 0.25f, 0.7f);
                    hairColor = new Color(0.2f, 0.15f, 0.1f);
                    break;
                case NPCType.Sweetheart:
                    outfitColor = new Color(1f, 0.4f, 0.8f);
                    hairColor = new Color(0.85f, 0.55f, 0.2f);
                    break;
            }

            npcObj.GetComponent<Renderer>().material = CreateToonMaterial(outfitColor, Color.white);

            // Head / Hair Mesh
            GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hair.transform.parent = npcObj.transform;
            hair.transform.localPosition = new Vector3(0, 0.8f, 0);
            hair.transform.localScale = new Vector3(0.9f, 0.7f, 0.9f);
            hair.GetComponent<Renderer>().material = CreateToonMaterial(hairColor, Color.white);
            Destroy(hair.GetComponent<Collider>());

            // Glowing 💋 Kiss Halo Prompt
            GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            halo.name = "KissHaloPrompt";
            halo.transform.parent = npcObj.transform;
            halo.transform.localPosition = new Vector3(0, 1.35f, 0);
            halo.transform.localScale = new Vector3(0.7f, 0.04f, 0.7f);
            Color haloColor = (type == NPCType.GymBro || type == NPCType.PoliceCadet) ? Color.red : new Color(1f, 0.1f, 0.5f);
            halo.GetComponent<Renderer>().material = CreateToonMaterial(haloColor, Color.white, haloColor);
            Destroy(halo.GetComponent<Collider>());
        }

        private void CreateSidewalkSpectator(Transform parent, float x, float z)
        {
            GameObject spectator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            spectator.name = "Spectator_Crowd";
            spectator.transform.parent = parent;
            spectator.transform.position = new Vector3(x, 1f, z);
            spectator.transform.localScale = new Vector3(0.8f, 0.85f, 0.8f);

            // Face toward road
            spectator.transform.rotation = Quaternion.Euler(0, x < 0 ? 90f : -90f, 0);

            Color[] crowdColors = new Color[] {
                new Color(0.95f, 0.35f, 0.2f),
                new Color(0.2f, 0.7f, 0.95f),
                new Color(0.95f, 0.8f, 0.15f),
                new Color(0.4f, 0.85f, 0.3f),
                new Color(0.8f, 0.3f, 0.85f)
            };
            Color shirtColor = crowdColors[Random.Range(0, crowdColors.Length)];
            spectator.GetComponent<Renderer>().material = CreateToonMaterial(shirtColor, Color.white);
            Destroy(spectator.GetComponent<Collider>());

            // Head
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.parent = spectator.transform;
            head.transform.localPosition = new Vector3(0, 0.75f, 0);
            head.transform.localScale = new Vector3(0.85f, 0.7f, 0.85f);
            head.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.3f, 0.2f, 0.1f), Color.white);
            Destroy(head.GetComponent<Collider>());

            // Cheering bob motion component
            spectator.AddComponent<SpectatorCheer>();
        }

        private void CreateProceduralJumpRamp(Transform parent, float x, float z)
        {
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "JumpRamp";
            ramp.transform.parent = parent;
            ramp.transform.position = new Vector3(x, 0.4f, z);
            ramp.transform.rotation = Quaternion.Euler(22f, 0, 0);
            ramp.transform.localScale = new Vector3(2.2f, 0.3f, 2.8f);

            ramp.GetComponent<Renderer>().material = CreateToonMaterial(new Color(1f, 0.75f, 0.05f), Color.yellow, new Color(0.7f, 0.5f, 0f));
            ramp.GetComponent<Collider>().isTrigger = true;
            ramp.AddComponent<JumpRamp>();
        }

        private void CreateProceduralHurdle(Transform parent, float x, float z)
        {
            GameObject hurdle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hurdle.name = "Hurdle_Low";
            hurdle.transform.parent = parent;
            hurdle.transform.position = new Vector3(x, 0.5f, z);
            hurdle.transform.localScale = new Vector3(2.2f, 0.9f, 0.4f);

            hurdle.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.9f, 0.15f, 0.15f), Color.white);
            hurdle.GetComponent<Collider>().isTrigger = true;
            Obstacle obs = hurdle.AddComponent<Obstacle>();
            obs.obstacleType = ObstacleType.RoadblockLow;
        }

        private void CreateProceduralOverheadSign(Transform parent, float x, float z)
        {
            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Overhead_Barrier";
            sign.transform.parent = parent;
            sign.transform.position = new Vector3(x, 2.1f, z);
            sign.transform.localScale = new Vector3(2.4f, 0.7f, 0.3f);

            sign.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.95f, 0.55f, 0.1f), Color.yellow);
            sign.GetComponent<Collider>().isTrigger = true;
            Obstacle obs = sign.AddComponent<Obstacle>();
            obs.obstacleType = ObstacleType.OverheadBarrier;
        }

        private void CreateProceduralDominoCrate(Transform parent, float x, float z)
        {
            GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crate.name = "DominoCrate";
            crate.transform.parent = parent;
            crate.transform.position = new Vector3(x, 0.6f, z);
            crate.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            crate.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.6f, 0.4f, 0.2f), new Color(0.8f, 0.6f, 0.3f));
            crate.GetComponent<Collider>().isTrigger = true;
            crate.AddComponent<DominoCrashProp>();
        }

        private void CreateProceduralCoin(Transform parent, Vector3 pos)
        {
            GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.name = "HeartCoin";
            coin.transform.parent = parent;
            coin.transform.position = pos;
            coin.transform.rotation = Quaternion.Euler(90f, 0, 0);
            coin.transform.localScale = new Vector3(0.5f, 0.08f, 0.5f);

            coin.GetComponent<Renderer>().material = CreateToonMaterial(new Color(1f, 0.85f, 0.1f), Color.white, new Color(0.9f, 0.7f, 0.05f));
            coin.GetComponent<Collider>().isTrigger = true;
            coin.AddComponent<HeartCoin>();
        }

        private void CreateProceduralPowerUp(Transform parent, Vector3 pos)
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "PowerUp_Orb";
            orb.transform.parent = parent;
            orb.transform.position = pos;
            orb.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            orb.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.1f, 0.9f, 1f), Color.white, new Color(0.1f, 0.8f, 1f));
            orb.GetComponent<Collider>().isTrigger = true;
            PowerUpItem item = orb.AddComponent<PowerUpItem>();
        }

        private void RecycleOldestChunk()
        {
            if (activeChunks.Count > 0)
            {
                GameObject oldChunk = activeChunks[0];
                activeChunks.RemoveAt(0);
                Destroy(oldChunk);
            }
        }

        private Material CreateToonMaterial(Color baseColor, Color rimColor, Color emissionColor = default)
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
    }
}
