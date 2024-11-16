using UnityEngine;

public class RhythmBonus : MonoBehaviour
{
    public float bpm = 68f; // Battements par minute
    public float toleranceWindow = 0.2f; // Tolérance autour du beat (en secondes)
    public float damageMultiplier = 2.0f; // Multiplicateur de dégâts quand le joueur est dans le rythme
    public AudioSource rhythmHitSound; // AudioSource pour le son de rythme

    private float timePerBeat; // Temps entre chaque beat
    private float nextBeatTime; // Le temps auquel le prochain beat se produit

    private bool isInvincible = false; // Indicateur d'invincibilité

    void Start()
    {
        timePerBeat = 60f / bpm; // Calculer le temps entre les beats en secondes
        nextBeatTime = Time.time; // Initialiser le temps du premier beat
    }

    void Update()
    {
        // Calculer quand le prochain beat devrait arriver
        if (Time.time >= nextBeatTime)
        {
            nextBeatTime += timePerBeat;
        }
    }

    public float CheckRhythm()
    {
        float timeSinceLastBeat = Mathf.Abs(Time.time - nextBeatTime);

        // Si le joueur attaque dans la fenêtre de tolérance autour du beat
        if (timeSinceLastBeat <= toleranceWindow || Mathf.Abs(nextBeatTime - Time.time) <= toleranceWindow)
        {
            Debug.Log("Attaque dans le rythme ! Bonus de dégâts activé.");
            
            // Jouer le son de rythme
            if (rhythmHitSound != null && !rhythmHitSound.isPlaying)
            {
                rhythmHitSound.Play();
            }

            return damageMultiplier; // Retourne le multiplicateur de dégâts si dans le rythme
        }
        else
        {
            Debug.Log("Attaque hors du rythme. Dégâts normaux.");
            return 1.0f; // Retourne 1.0 pour des dégâts normaux
        }
    }

    // Méthode pour définir l'invincibilité
    public void SetInvincibility(bool value)
    {
        isInvincible = value;
        Debug.Log("Invincibilité activée : " + isInvincible);
    }

    // Méthode pour vérifier si le joueur est invincible
    public bool IsInvincible()
    {
        return isInvincible;
    }
}
