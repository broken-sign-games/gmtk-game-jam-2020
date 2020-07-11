using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] frames = null;
    [SerializeField] private int framesPerSecond = 10;

    void Update()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        var index = (int)(Time.time * framesPerSecond) % frames.Length;
        var currentFrame = renderer.sprite;
        if (currentFrame == frames[frames.Length - 1])
        {
            return;
        }
        GetComponent<SpriteRenderer>().sprite = frames[index];
    }
}
