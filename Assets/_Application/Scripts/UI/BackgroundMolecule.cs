using UnityEngine;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class BackgroundMolecule : MonoBehaviour
    {
        [SerializeField] private float speed = 100f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private Vector2 direction = Vector2.zero;

        public Vector2 Direction
        {
            get => direction;
            set => direction = value;
        }

        private RectTransform rectTransform;
        private Camera mainCamera;

        private bool wasVisible = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            mainCamera = Camera.main;

            if (Random.value < 0.5f)
                rotationSpeed = -rotationSpeed;
        }

        private void Update()
        {
            rectTransform.anchoredPosition += direction * speed * Time.deltaTime;
            rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            if (rectTransform.IsVisibleFrom(mainCamera))
                wasVisible = true;
            else if (wasVisible)
                Destroy(gameObject);
        }
    } 
}
