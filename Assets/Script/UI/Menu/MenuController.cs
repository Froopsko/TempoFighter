using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject cursorIcon;     // Icône qui suit la souris
    [SerializeField] private GameObject settingsButton; // Bouton Settings
    [SerializeField] private GameObject fightButton;    // Bouton Fight
    [SerializeField] private GameObject quitButton;     // Bouton Quit

    private void Start()
    {
        // Vérifie que toutes les références sont assignées
        if (cursorIcon == null || settingsButton == null || fightButton == null || quitButton == null)
        {
            Debug.LogError("Assurez-vous que toutes les références sont assignées dans l'inspecteur !");
        }
    }

    private void Update()
    {
        // Déplacer l'icône du curseur selon la position de la souris
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Distance par rapport à la caméra
        cursorIcon.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);

        // Détecter le clic gauche de la souris
        if (Input.GetMouseButtonDown(0))
        {
            // Vérifier sur quel bouton la souris est positionnée
            if (IsMouseOver(settingsButton))
            {
                Debug.Log("La scène des paramètres n'est pas encore disponible.");
            }
            else if (IsMouseOver(fightButton))
            {
                // Charger la scène de sélection des personnages
                if (Application.CanStreamedLevelBeLoaded("CharacterSelection"))
                {
                    SceneManager.LoadScene("CharacterSelection");
                }
                else
                {
                    Debug.LogError("La scène 'CharacterSelection' n'est pas dans les Build Settings !");
                }
            }
            else if (IsMouseOver(quitButton))
            {
                // Quitter le jeu
                Application.Quit();
                Debug.Log("Quitter le jeu."); // Visible uniquement dans l'exécutable, pas dans l'éditeur Unity
            }
        }
    }

    // Fonction pour vérifier si la souris est au-dessus d'un bouton
    private bool IsMouseOver(GameObject button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        Vector2 localMousePosition = rect.InverseTransformPoint(Input.mousePosition);
        return rect.rect.Contains(localMousePosition);
    }
}
