using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] frames = null;
    [SerializeField] private int framesPerSecond = 20;

    SpriteRenderer splashRenderer;

    private void Start()
    {
        splashRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var index = (int)(Time.time * framesPerSecond) % frames.Length;
        var currentFrame = splashRenderer.sprite;
        if (currentFrame == frames[frames.Length - 1])
        {
            return;
        }
        splashRenderer.sprite = frames[index];
    }
}
