using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class MoleculeSpawner : MonoBehaviour
    {
        [SerializeField] private int initialMolecules = 10;
        [SerializeField] private float spawnRate = 0.1f;
        [SerializeField] private float spawnOffset = 50;
        [SerializeField] private BackgroundMolecule[] moleculePrefabs = null;
        
        private RectTransform rectTransform;

        private float lastSpawnTime = 0f;
        private Random rng;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            rng = new Random();
        }

        private void Start()
        {
            Vector2 canvasSize = rectTransform.rect.size;

            for (int i = 0; i < initialMolecules; ++i)
            {
                BackgroundMolecule molecule = Instantiate(moleculePrefabs[rng.Next(moleculePrefabs.Length)], rectTransform);

                var moleculeTransform = molecule.GetComponent<RectTransform>();
                moleculeTransform.anchoredPosition = new Vector2((float)rng.NextDouble(), (float)rng.NextDouble()) * canvasSize;

                float direction = (float)rng.NextDouble() * Mathf.PI * 2;
                molecule.Direction = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction));
            }
        }

        private void Update()
        {
            if (Time.time - lastSpawnTime < spawnRate)
                return;

            Vector2 canvasSize = rectTransform.rect.size;

            BackgroundMolecule molecule = Instantiate(moleculePrefabs[rng.Next(moleculePrefabs.Length)], rectTransform);

            float spawnLocationAlongBorder = (float)(rng.NextDouble() * 2 * (canvasSize.x + canvasSize.y));

            Vector2 spawnPosition;
            float direction;

            if (spawnLocationAlongBorder < canvasSize.x)
            {
                spawnPosition = new Vector2(spawnLocationAlongBorder, -spawnOffset);
                direction = (float)rng.NextDouble() * 90 + 45;
            }
            else if (spawnLocationAlongBorder < canvasSize.x + canvasSize.y)
            {
                spawnPosition = new Vector2(canvasSize.x + spawnOffset, spawnLocationAlongBorder - canvasSize.x);
                direction = (float)rng.NextDouble() * 90 + 135;
            }
            else if (spawnLocationAlongBorder < 2 * canvasSize.x + canvasSize.y)
            {
                spawnPosition = new Vector2(spawnLocationAlongBorder - canvasSize.x - canvasSize.y, canvasSize.y + spawnOffset);
                direction = (float)rng.NextDouble() * 90 + 225;
            }
            else
            {
                spawnPosition = new Vector2(-spawnOffset, spawnLocationAlongBorder - 2 * canvasSize.x - canvasSize.y);
                direction = (float)rng.NextDouble() * 90 + 315;
            }

            var moleculeTransform = molecule.GetComponent<RectTransform>();
            moleculeTransform.anchoredPosition = spawnPosition;

            molecule.Direction = new Vector2(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad));

            lastSpawnTime = Time.time;
        }
    }
}
