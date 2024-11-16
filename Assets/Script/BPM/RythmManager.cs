using UnityEngine;
using System;

public class RythmManager : MonoBehaviour
{
    public float bpm = 86f;
    public event Action OnBeat;

    private float beatInterval;
    private float timer;

    void Start()
    {
        beatInterval = 60f / bpm; // 60 / 86 ≈ 0.6977 secondes par beat
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer -= beatInterval;
            OnBeat?.Invoke();
        }
    }
}


