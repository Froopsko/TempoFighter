using UnityEngine;
using System;

public class MusicSync : MonoBehaviour
{
    public float bpm; // BPM de la musique
    public event Action OnBeat; // Événement pour chaque beat

    private float beatInterval; // Temps entre chaque beat
    private float timer;

    public Spine.AnimationState animationState; // À assigner dans l'inspecteur
    public int trackIndex = 0;
    public bool loop = true;

    void Start()
    {
        beatInterval = 60f / bpm;
        timer = 0f;

        // Obtenez l'instance de MusicSync et abonnez-vous à l'événement
        MusicSync musicSync = FindObjectOfType<MusicSync>();
        if (musicSync != null)
        {
            musicSync.OnBeat += TriggerSpineEvent;
        }
        else
        {
            Debug.LogError("MusicSync GameObject not found in the scene.");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer -= beatInterval; // Réinitialiser le timer pour le prochain beat
            OnBeat?.Invoke(); // Lancer l'événement pour le beat
        }
    }

    void TriggerSpineEvent()
{
    if (animationState != null)
    {
        Debug.Log("Beat Triggered");
        animationState.SetAnimation(trackIndex, "AnimationName", loop); // Remplace "AnimationName" par ton nom d'animation
    }
    else
    {
        Debug.LogError("AnimationState is not assigned. Please check the reference in the inspector.");
    }
}

}
