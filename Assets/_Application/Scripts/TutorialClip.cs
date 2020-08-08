using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TutorialClip : MonoBehaviour
{
    [SerializeField] private int framesPerSecond = 60;
    [SerializeField] private float delayBetweenLoops = 0f;
    
    public Sprite[] Frames;

    private Image splashRenderer;

    private float startTime;
    private float endTime;

    private void Start()
    {
        splashRenderer = GetComponent<Image>();
        
        StartAnimation();
    }

    public void StartAnimation()
    {
        startTime = Time.time;
        endTime = -1;
    }

    private void Update()
    {
        int index = (int)((Time.time - startTime) * framesPerSecond);
        
        if (index >= Frames.Length)
        {
            if (endTime < 0)
                endTime = Time.time;


            if (Time.time - endTime >= delayBetweenLoops)
            {
                StartAnimation();
                index = 0;
            }
            else
            {
                index = Frames.Length - 1;
            }
        }

        splashRenderer.sprite = Frames[index];
    }
}
