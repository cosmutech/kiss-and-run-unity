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
            // 1. Pick a lane for an NPC encounter
            int npcLaneIndex = Random.Range(0, 3);
            float npcLaneX = lanePositions[npcLaneIndex];

            if (npcPrefabs != null && npcPrefabs.Length > 0)
            {
                GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
                Vector3 npcPos = new Vector3(npcLaneX, 0f, currentZ + chunkLength * 0.5f);
                Instantiate(npcPrefab, npcPos, Quaternion.Euler(0, 180, 0), chunk.transform);
            }
            else
            {
                CreateProceduralNPC(chunk.transform, npcLaneX, currentZ + chunkLength * 0.5f);
            }

            // 2. Place Obstacles, Ramps, or Props in other lanes
            for (int i = 0; i < 3; i++)
            {
                if (i == npcLaneIndex) continue; // Keep NPC lane clear to kiss

                float r = Random.value;
                float laneX = lanePositions[i];
                Vector3 obsPos = new Vector3(laneX, 0f, currentZ + chunkLength * 0.5f);

                if (r < 0.28f)
                {
                    // 3D Jump Ramp (Launches player into stunt trick!)
                    CreateProceduralJumpRamp(chunk.transform, laneX, currentZ + 12f);
                }
                else if (r < 0.55f)
                {
                    // Low Hurdle (Jump over!)
                    CreateProceduralHurdle(chunk.transform, laneX, currentZ + 15f);
                }
                else if (r < 0.80f)
                {
                    // Overhead Banner (Slide under!)
                    CreateProceduralOverheadSign(chunk.transform, laneX, currentZ + 15f);
                }
                else
                {
                    // Domino Crates (Crash prop!)
                    CreateProceduralDominoCrate(chunk.transform, laneX, currentZ + 15f);
                }
            }

            // 3. Place Coin Arcs in a lane
            int coinLaneIndex = Random.Range(0, 3);
            float coinX = lanePositions[coinLaneIndex];
            for (int c = 0; c < 4; c++)
            {
                float arcY = Mathf.Sin((c / 3f) * Mathf.PI) * 2f;
                Vector3 coinPos = new Vector3(coinX, 0.6f + arcY, currentZ + 5f + (c * 3f));
                CreateProceduralCoin(chunk.transform, coinPos);
            }

            // 4. Occasional Power-up Item (18% chance)
            if (Random.value < 0.18f)
            {
                int powerLane = Random.Range(0, 3);
                Vector3 powerPos = new Vector3(lanePositions[powerLane], 1.2f, currentZ + 22f);
                CreateProceduralPowerUp(chunk.transform, powerPos);
            }
        }

        private void CreateProceduralNPC(Transform parent, float x, float z)
        {
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcObj.name = "NPC_Pedestrian";
            npcObj.transform.parent = parent;
            npcObj.transform.position = new Vector3(x, 1f, z);
            npcObj.transform.rotation = Quaternion.Euler(0, 180, 0);

            Color[] npcColors = new Color[] {
                new Color(1f, 0.4f, 0.7f),  // Pink
                new Color(0.3f, 0.8f, 1f),  // Cyan
                new Color(1f, 0.85f, 0.2f), // Yellow
                new Color(0.6f, 0.3f, 0.9f) // Purple
            };
            Color chosenColor = npcColors[Random.Range(0, npcColors.Length)];
            npcObj.GetComponent<Renderer>().material = CreateToonMaterial(chosenColor, Color.white);

            // Head / Hair Sphere
            GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hair.transform.parent = npcObj.transform;
            hair.transform.localPosition = new Vector3(0, 0.8f, 0);
            hair.transform.localScale = new Vector3(0.9f, 0.7f, 0.9f);
            hair.GetComponent<Renderer>().material = CreateToonMaterial(new Color(0.2f, 0.12f, 0.05f), new Color(0.4f, 0.3f, 0.2f));
            Destroy(hair.GetComponent<Collider>());

            // Glowing Kiss Halo Prompt
            GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            halo.name = "KissHaloPrompt";
            halo.transform.parent = npcObj.transform;
            halo.transform.localPosition = new Vector3(0, 1.35f, 0);
            halo.transform.localScale = new Vector3(0.7f, 0.04f, 0.7f);
            halo.GetComponent<Renderer>().material = CreateToonMaterial(new Color(1f, 0.1f, 0.5f), Color.white, new Color(1f, 0.2f, 0.6f));
            Destroy(halo.GetComponent<Collider>());

            NPCController npcCtrl = npcObj.AddComponent<NPCController>();
            npcCtrl.npcName = "Stunner";
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
