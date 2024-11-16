using UnityEngine;
using UnityEngine.UI;

public class HealthOpponentWilson : MonoBehaviour
{
    public Slider healthSlider;         // Barre de vie verte
    public Image healthSliderRed;       // Image pour la barre de vie rouge
    public float maxHealth = 100f;
    public float currentHealthVerteOpponent;  // Vie pour la barre verte
    public float currentHealthRedOpponent;    // Vie pour la barre rouge

    public IaPlayerWilson iaPlayer;

    void Start()
    {
        // Initialiser les deux barres à leur maximum
        currentHealthVerteOpponent = maxHealth;
        currentHealthRedOpponent = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealthVerteOpponent;
        }

        if (healthSliderRed != null)
        {
            healthSliderRed.fillAmount = 1f;  // La barre rouge est pleine au départ
        }
    }

    public void TakeDamage(float damage)
    {
       

        // Réduire la vie de la barre verte
        if (currentHealthVerteOpponent > 0)
        {
            currentHealthVerteOpponent -= damage;
            if (currentHealthVerteOpponent < 0)
            {
                currentHealthVerteOpponent = 0;
            }
            UpdateHealthSlider();
        }
        // Réduire la vie de la barre rouge si la verte est vide
        else if (currentHealthRedOpponent > 0)
        {
            currentHealthRedOpponent -= damage;
            if (currentHealthRedOpponent < 0)
            {
                currentHealthRedOpponent = 0;
            }
            UpdateHealthSliderRed();
        }

        // Jouer l'animation de "Hit" de l'IA si elle est assignée
        if (iaPlayer != null)
        {
            iaPlayer.PlayHitReaction();
        }

        // Vérifier l'état de l'IA
        CheckHealthOpponentState();
    }

    void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealthVerteOpponent;
        }
    }

    void UpdateHealthSliderRed()
    {
        if (healthSliderRed != null)
        {
            healthSliderRed.fillAmount = currentHealthRedOpponent / maxHealth;
        }
    }

    void CheckHealthOpponentState()
    {
        if (iaPlayer == null) return;

        // Si la barre verte est vide et que l'IA n'est pas déjà en état rouge
        if (currentHealthVerteOpponent <= 0 && currentHealthRedOpponent > 0 && !iaPlayer.isRedState)
        {
            iaPlayer.TriggerRedState();
        }

        // Si les deux barres sont vides et que l'IA n'est pas déjà morte
        if (currentHealthVerteOpponent <= 0 && currentHealthRedOpponent <= 0 && !iaPlayer.isDead)
        {
            iaPlayer.TriggerDeath();
        }
    }
}
