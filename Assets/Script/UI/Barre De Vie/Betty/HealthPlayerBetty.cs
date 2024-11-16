using UnityEngine;
using UnityEngine.UI;

public class HealthBetty : MonoBehaviour
{
    public Slider BarreDeVieVerte;     // Slider pour la barre de vie verte
    public Image BarreDeVieRouge;      // Image pour la barre de vie rouge
    public float maxHealth = 100f;     // Vie maximale
    public float currentHealthVerte;   // Vie actuelle de la barre verte
    public float currentHealthRouge;   // Vie actuelle de la barre rouge

    public PlayerControllerBetty playerController; // Référence au PlayerController pour changer l'apparence et gérer la mort

    private bool hasTriggerRedState = false;
    public bool isInvincible = false;

    void Start()
    {
        // Initialiser les deux barres à leur maximum
        currentHealthVerte = maxHealth;
        currentHealthRouge = maxHealth;

        // Configure la valeur maximale du Slider de la barre verte
        if (BarreDeVieVerte != null)
        {
            BarreDeVieVerte.maxValue = maxHealth;
        }

        UpdateBarreDeVieVerte();
        UpdateBarreDeVieRouge();
    }

    // Mise à jour de la barre verte (Slider)
    void UpdateBarreDeVieVerte()
    {
        if (BarreDeVieVerte != null)
        {
            BarreDeVieVerte.value = currentHealthVerte;
            Debug.Log("Barre de vie verte mise à jour : " + BarreDeVieVerte.value);
        }
        else
        {
            Debug.LogError("BarreDeVieVerte n'est pas assignée dans l'inspecteur !");
        }
    }

    // Mise à jour de la barre rouge (Image)
    void UpdateBarreDeVieRouge()
    {
        if (BarreDeVieRouge != null)
        {
            BarreDeVieRouge.fillAmount = currentHealthRouge / maxHealth;
            Debug.Log("Barre de vie rouge mise à jour : " + BarreDeVieRouge.fillAmount);
        }
        else
        {
            Debug.LogError("BarreDeVieRouge n'est pas assignée dans l'inspecteur !");
        }
    }

   public void TakeDamage(float damage)
{
    if (currentHealthVerte > 0)
    {
        currentHealthVerte -= damage;
        if (currentHealthVerte < 0)
        {
            currentHealthVerte = 0;
        }
        UpdateBarreDeVieVerte();
    }
    else if (currentHealthRouge > 0)
    {
        currentHealthRouge -= damage;
        if (currentHealthRouge < 0)
        {
            currentHealthRouge = 0;
        }
        UpdateBarreDeVieRouge();
    }

    // Appeler l'animation de HitStun
    playerController.PlayHitReaction();
    CheckHealthState();
}




    // Vérifie l'état de la vie pour changement d'apparence et mort
    void CheckHealthState()
    {
        // Si la barre verte est vide, on passe à l'état rouge
        if (currentHealthVerte <= 0 && !playerController.isRedState && !hasTriggerRedState)
        {
            hasTriggerRedState = true; // Marque que l'état rouge a été déclenché une fois
            playerController.TriggerRedState(); // Passe à l'etat rouge
        }

        // Si la barre rouge est vide, on tue le personnage
        if (currentHealthRouge <= 0 && !playerController.isDead)
        {
            playerController.isDead = true; // Marque le personnage comme mort
            playerController.TriggerDeath(); // Change l'apparence en mort
        }
    }

    public void SetInvincibility(bool value)
{
    isInvincible = value;
    Debug.Log("Invincibilité du joueur : " + isInvincible);
}

}
