using UnityEngine;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RotateTransform : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 90f;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    } 
}
