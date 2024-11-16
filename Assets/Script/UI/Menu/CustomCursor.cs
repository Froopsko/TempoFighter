using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private GameObject cursorIcon; // L'icône du curseur (image UI)

    private void Start()
    {
        // Masquer le curseur par défaut de Unity
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        // Suivre la position de la souris
        Vector3 mousePosition = Input.mousePosition;
        cursorIcon.transform.position = mousePosition;
    }
}
