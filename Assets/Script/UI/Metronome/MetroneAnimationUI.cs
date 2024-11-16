using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image m_Image;
    public Sprite[] m_SpriteArray;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public float speedMultiplier = 1.0f; // Multiplicateur de vitesse (doit être > 0)

    private int m_IndexSprite;
    private float m_Timer;
    private float m_FrameInterval;

    void Start()
    {
        m_IndexSprite = 0;
        m_Timer = 0f;

        // Fixe l'intervalle pour 20 fps
        m_FrameInterval = 1f / 20f; // 20 fps = 0.05 secondes par frame
    }

    void Update()
    {
        // Augmente le timer en fonction du temps écoulé
        m_Timer += Time.deltaTime;

        // Change de sprite à chaque intervalle de frame ajusté par le multiplicateur de vitesse
        if (m_Timer >= m_FrameInterval / Mathf.Max(speedMultiplier, 1.0f))
        {
            m_Timer = 0f;

            // Change le sprite de l'image
            m_Image.sprite = m_SpriteArray[m_IndexSprite];
            m_IndexSprite++;

            // Boucle sur le tableau de sprites
            if (m_IndexSprite >= m_SpriteArray.Length)
            {
                m_IndexSprite = 0;

                // Joue le son de clic lorsque le cycle complet est terminé
                if (audioSource != null && clickSound != null)
                {
                    audioSource.PlayOneShot(clickSound);
                }
            }
        }
    }
}
