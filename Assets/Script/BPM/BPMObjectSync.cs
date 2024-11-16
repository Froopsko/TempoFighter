using UnityEngine;

public class BPMObjectSync : MonoBehaviour
{
    public RythmManager rhythmManager;
    public GameObject targetObject; // Le GameObject à synchroniser avec le BPM

    void Start()
    {
        if (rhythmManager != null)
        {
            rhythmManager.OnBeat += SyncWithBPM;
        }
        else
        {
            Debug.LogError("RhythmManager n'est pas assigné.");
        }
    }

    void SyncWithBPM()
    {
        // Exécuter une action rythmée sans effet de pulsation
        if (targetObject != null)
        {
            //Debug.Log("BPM beat detected"); // Pour vérifier le déclenchement au rythme de 86 BPM
        }
    }

    void OnDestroy()
    {
        if (rhythmManager != null)
        {
            rhythmManager.OnBeat -= SyncWithBPM;
        }
    }
}
