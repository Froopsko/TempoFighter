using UnityEngine;

public class ScissorBetty : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private LayerMask enemyLayer;
    private float timer;

    private void Start()
    {
        // Réinitialiser le timer lorsque le projectile est activé
        timer = 0f;
    }

    private void Update()
    {
        // Déplacer le projectile sur l'axe local X (horizontal)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Désactiver le projectile après lifeTime secondes
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si le projectile touche un ennemi
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyLayer"))
        {
            HealthOpponentWilson health = other.GetComponentInChildren<HealthOpponentWilson>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Dégâts infligés par les ciseaux : " + damage);
            }

            // Détruire le projectile après avoir touché l'ennemi
            Destroy(gameObject);
        }
    }
}
