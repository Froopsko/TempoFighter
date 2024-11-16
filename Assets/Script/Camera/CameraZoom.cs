using System;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    public Camera mainCamera;

    public float minZoom = 25.4f;
    public float maxZoom = 60f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    // Start is called before the first frame update
    void Update()
    {
        // Calculer la distance entre le joueur et l'ennemi
        float distance = Vector3.Distance(player.position, enemy.position);

        // Mapper la distance sur le champ de vision de la caméra
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, (distance - minDistance) / (maxDistance - minDistance));
        // Limiter la valeur du zoom entre minZoom et maxZoom
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Appliquer le zoom avec une transition lissée

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);

        
        
    }

    
    
}
