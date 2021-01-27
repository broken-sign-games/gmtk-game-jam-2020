using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class MoleculeSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnRate = 0.1f;
        [SerializeField] private float spawnOffset = 50;
        [SerializeField] private BackgroundMolecule[] moleculePrefabs = null;
        
        private RectTransform rectTransform;

        private float lastSpawnTime = 0f;
        private Random rng;
        private Vector2 canvasSize;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            rng = new Random();

            canvasSize = rectTransform.rect.size;
        }

        private void Update()
        {
            if (Time.time - lastSpawnTime < spawnRate)
                return;

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
                spawnPosition = new Vector2(canvasSize.x + spawnOffset, spawnLocationAlongBorder);
                direction = (float)rng.NextDouble() * 90 + 135;
            }
            else if (spawnLocationAlongBorder < 2 * canvasSize.x + canvasSize.y)
            {
                spawnPosition = new Vector2(spawnLocationAlongBorder, canvasSize.y + spawnOffset);
                direction = (float)rng.NextDouble() * 90 + 225;
            }
            else
            {
                spawnPosition = new Vector2(-spawnOffset, spawnLocationAlongBorder);
                direction = (float)rng.NextDouble() * 90 + 315;
            }

            Debug.Log(spawnPosition);

            var moleculeTransform = molecule.GetComponent<RectTransform>();
            moleculeTransform.anchoredPosition = spawnPosition;

            molecule.Direction = new Vector2(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad));

            lastSpawnTime = Time.time;
        }
    }
}
