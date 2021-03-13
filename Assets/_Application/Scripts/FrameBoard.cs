using UnityEngine;

namespace GMTK2020
{
    [RequireComponent(typeof(Camera))]
    public class FrameBoard : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer boardSprite = null;

        private void Start()
        {
            float boardWidth = boardSprite.bounds.extents.x;

            var camera = GetComponent<Camera>();

            camera.orthographicSize = boardWidth / camera.aspect;
        }
    } 
}
