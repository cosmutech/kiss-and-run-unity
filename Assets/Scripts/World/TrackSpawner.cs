using System.Collections.Generic;
using UnityEngine;

namespace KissAndRun
{
    public class TrackSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject[] trackChunkPrefabs;
        [SerializeField] private GameObject[] npcPrefabs;
        [SerializeField] private GameObject roadblockPrefab;
        [SerializeField] private GameObject overheadSignPrefab;
        [SerializeField] private GameObject bananaPeelPrefab;
        [SerializeField] private GameObject coinPrefab;

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
            if (trackChunkPrefabs == null || trackChunkPrefabs.Length == 0) return;

            int randomIndex = Random.Range(0, trackChunkPrefabs.Length);
            GameObject chunk = Instantiate(trackChunkPrefabs[randomIndex], Vector3.forward * spawnZ, Quaternion.identity, transform);
            activeChunks.Add(chunk);

            if (spawnWithObstacles)
            {
                PopulateChunk(chunk, spawnZ);
            }

            spawnZ += chunkLength;
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

            // 2. Place Obstacles in other lanes (Roadblock or Overhead barrier)
            for (int i = 0; i < 3; i++)
            {
                if (i == npcLaneIndex) continue; // Keep NPC lane clear to kiss

                float r = Random.value;
                float laneX = lanePositions[i];
                Vector3 obsPos = new Vector3(laneX, 0f, currentZ + chunkLength * 0.5f);

                if (r < 0.40f && roadblockPrefab)
                {
                    Instantiate(roadblockPrefab, obsPos, Quaternion.identity, chunk.transform);
                }
                else if (r < 0.75f && overheadSignPrefab)
                {
                    Instantiate(overheadSignPrefab, obsPos, Quaternion.identity, chunk.transform);
                }
                else if (r < 0.90f && bananaPeelPrefab)
                {
                    Instantiate(bananaPeelPrefab, obsPos, Quaternion.identity, chunk.transform);
                }
            }

            // 3. Place Coin Arcs in free lanes
            if (coinPrefab)
            {
                int coinLaneIndex = Random.Range(0, 3);
                float coinX = lanePositions[coinLaneIndex];

                for (int c = 0; c < 4; c++)
                {
                    float arcY = Mathf.Sin((c / 3f) * Mathf.PI) * 2f;
                    Vector3 coinPos = new Vector3(coinX, 0.5f + arcY, currentZ + 5f + (c * 3f));
                    Instantiate(coinPrefab, coinPos, Quaternion.identity, chunk.transform);
                }
            }
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
    }
}
