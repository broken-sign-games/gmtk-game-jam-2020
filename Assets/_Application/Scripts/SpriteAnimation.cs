using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] frames = null;
    [SerializeField] private Sprite finalFrame = null;
    [SerializeField] private int framesPerSecond = 20;

    private bool isIntro = true;

    SpriteRenderer splashRenderer;

    private void Start()
    {
        splashRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isIntro)
        {
            return;
        }

        var index = (int)(Time.time * framesPerSecond) % frames.Length;
        var currentFrame = splashRenderer.sprite;
        splashRenderer.sprite = frames[index];

        if (currentFrame == frames[frames.Length - 1])
        {
            isIntro = false;
            splashRenderer.DOFade(0.0f, 1.0f).OnComplete(() =>
            {
                splashRenderer.sprite = finalFrame;
                splashRenderer.DOFade(1.0f, 0.25f);
            });
            return;
        }
    }
}
