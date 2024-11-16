using UnityEngine;
using Spine.Unity;
using System.Collections;

public class PlayerControllerBetty : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, attackFast, attackHeavy, parry, redState, deadState, BackWalking, Gimmick,HitStun1,HitStun2,Fall;
    public RythmManager rhythmManager; // Référence au RhythmManager
    public RhythmBonus rhythmBonus; // Référence au script de bonus de rythme
    public string currentState;
    private string currentAnimation;
    public float speed;
    private float movement;
    private bool isAttacking;
    private bool isParrying;
    private bool isGimmickOnCooldown = false;
    public float gimmickCooldown = 20f; // Cooldown de 20 secondes
    public bool isInvincible = false;
    private bool isInHitStun = false;
    private bool isInFall = false;


    public bool isGimmick = false;
    public float teleportDistance = 3.0f;
    private Rigidbody rb;
    public bool isRedState = false;
    private bool isRedStatePlaying = false;
    public bool isDead = false;
    public float attackRange = 1.5f;
    public Transform enemyTransform;
    [SerializeField] private LayerMask enemyLayer;


    private bool isBlockedLeft = false;
    private bool isBlockedRight = false;
   
   
    

   void Start()
{
    rb = GetComponent<Rigidbody>();
    currentState = "Idle";
    SetCharacterState(currentState);
    isAttacking = false;
    isParrying = false;
    

    if (rhythmBonus == null)
    {
        rhythmBonus = GetComponent<RhythmBonus>();
        if (rhythmBonus == null)
        {
            Debug.LogError("RhythmBonus n'est pas assigné ou n'est pas trouvé sur ce GameObject !");
        }
    }

    // Abonnement à l'event Spine
    
}

void OnSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
{
   
}










    void Update()
{
    if (isDead || isInHitStun) return; // Empêche toute action si le personnage est en HitStun ou mort

    if (!isAttacking && !isParrying)
    {
        Move();
    }
    else
    {
        rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
    }

    if (Input.GetKeyDown(KeyCode.J) && !isAttacking && !isParrying)
    {
        StartCoroutine(PerformFastAttack());
    }

    if (Input.GetKeyDown(KeyCode.K) && !isAttacking && !isParrying)
    {
        StartCoroutine(PerformHeavyAttack());
    }

    if (Input.GetKey(KeyCode.O))
    {
        StartParry();
    }

    if (Input.GetKeyUp(KeyCode.O))
    {
        StopParry();
    }

    if (Input.GetKeyDown(KeyCode.P) && !isAttacking && !isParrying)
    {
        StartCoroutine(PerformGimmick());
    }
}


    public void SetAnimation(AnimationReferenceAsset animationAsset, bool loop, float timeScale)
    {
    if (animationAsset == null) return;

    Spine.Animation animation = animationAsset.Animation;
    if (animation == null || currentAnimation == animation.Name) return;

    // Ne pas réinitialiser la pose à chaque fois
    skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    currentAnimation = animation.Name;
    }




   public void SetCharacterState(string state)
{
    if (isDead)
    {
        SetAnimation(deadState, false, 1f);
        return;
    }

    if (isInHitStun)
    {
        Debug.Log("Le joueur est en HitStun, aucune autre animation ne peut être jouée.");
        return;
    }

    if (isRedStatePlaying)
    {
        Debug.Log("L'animation RedState est en cours, aucun changement d'état n'est permis.");
        return;
    }

    if (state.Equals("Idle"))
    {
        SetAnimation(idle, true, 1f);
    }
    else if (state.Equals("Walking"))
    {
        SetAnimation(walking, true, 1f);
    }
    else if (state.Equals("BackWalking"))
    {
        SetAnimation(BackWalking, true, 1f);
    }
    else if (state.Equals("AttackFast"))
    {
        SetAnimation(attackFast, false, 1f);
    }
    else if (state.Equals("AttackHeavy"))
    {
        SetAnimation(attackHeavy, false, 1f);
    }
    else if (state.Equals("Parry"))
    {
        SetAnimation(parry, false, 1f);
    }
    else if (state.Equals("Gimmick"))
    {
        SetAnimation(Gimmick, false, 1f);
    }
}





  private void OnRedStateComplete(Spine.TrackEntry trackEntry)
{
    if (trackEntry == redStateTrackEntry)
    {
        isRedStatePlaying = false;
        isRedState = false;

        // Réactiver le Rigidbody et remettre le mouvement
        rb.isKinematic = false;

        Debug.Log("Animation RedState terminée, retour à Idle.");
        SetCharacterState("Idle");

        // Autoriser le mouvement à nouveau
        isAttacking = false;
        isParrying = false;
        isGimmick = false;
        isInHitStun = false;
    }
}

public void TryFallAnimation()
{
    if (isDead || isInHitStun || isRedStatePlaying || isInFall || isAttacking)
        return;

    // 10% de chance de déclencher l'animation "Fall"
    float randomChance = Random.Range(0f, 1f);
    if (randomChance <= 0.1f)
    {
        PlayFallAnimation();
    }
}

public void PlayFallAnimation()
{
    isInFall = true;
    isInvincible = true;

    Debug.Log("Animation Fall activée !");
    Spine.TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, Fall.Animation, false);
    trackEntry.Complete += OnFallComplete;

    
}

void OnFallComplete(Spine.TrackEntry trackEntry)
{
    isInFall = false;
    StartCoroutine(InvincibilityBlink());

    // Désabonner l'événement pour éviter des appels multiples
    trackEntry.Complete -= OnFallComplete;

    // Retour à Idle
    SetCharacterState("Idle");
}

IEnumerator InvincibilityBlink()
{
    float blinkDuration = 1.0f; // Durée totale de l'invincibilité
    float blinkInterval = 0.1f; // Intervalle de clignotement

    float elapsedTime = 0f;
    while (elapsedTime < blinkDuration)
    {
        skeletonAnimation.gameObject.SetActive(!skeletonAnimation.gameObject.activeSelf);
        yield return new WaitForSeconds(blinkInterval);
        elapsedTime += blinkInterval;
    }

    // Réinitialiser l'état du personnage
    skeletonAnimation.gameObject.SetActive(true);
    isInvincible = false;
    Debug.Log("Fin de l'invincibilité après Fall.");
}











    

    




    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Vector3 direction = other.transform.position - transform.position;
            direction = direction.normalized;

            // Bloquer à droite
            if (direction.x > 0)
            {
                isBlockedRight = true;
            }
            // Bloquer à gauche
            else if (direction.x < 0)
            {
                isBlockedLeft = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Vector3 direction = other.transform.position - transform.position;
            direction = direction.normalized;

            // Débloquer à droite
            if (direction.x > 0)
            {
                isBlockedRight = false;
            }
            // Débloquer à gauche
            else if (direction.x < 0)
            {
                isBlockedLeft = false;
            }
        }
    }

   public void Move()
{
    if (isAttacking || isParrying || isDead || isGimmick || isRedState || isRedStatePlaying || isInHitStun)
        return;

    movement = 0;

    bool isFacingRight = transform.rotation.eulerAngles.y == 0;

    if (Input.GetKey(KeyCode.D) && !isBlockedRight)
    {
        movement = 1;
        SetCharacterState(isFacingRight ? "Walking" : "BackWalking");
    }
    else if (Input.GetKey(KeyCode.A) && !isBlockedLeft)
    {
        movement = -1;
        SetCharacterState(isFacingRight ? "BackWalking" : "Walking");
    }
    else
    {
        SetCharacterState("Idle");
    }

    rb.velocity = new Vector3(movement * speed, rb.velocity.y, rb.velocity.z);
}






    public void StartParry()
    {
        isParrying = true;
        SetCharacterState("Parry");
    }

    public void StopParry()
    {
        if (isParrying)
        {
            isParrying = false;
            SetCharacterState("Idle");
        }
    }

    IEnumerator PerformFastAttack()
{
    if (isInHitStun) yield break; // Empêche l'attaque si le joueur est en HitStun

    isAttacking = true;
    SetCharacterState("AttackFast");

    float damageMultiplier = rhythmBonus.CheckRhythm();

    ApplyDamage(3f * damageMultiplier);

    Spine.TrackEntry trackEntry = skeletonAnimation.state.GetCurrent(0);
    if (trackEntry != null)
    {
        trackEntry.Complete -= OnAttackComplete;
        trackEntry.Complete += OnAttackComplete;
    }

    yield return new WaitForSeconds(0.3f);

    ApplyDamage(3f * damageMultiplier);
}


void OnAttackComplete(Spine.TrackEntry trackEntry)
{
    if (isAttacking)
    {
        isAttacking = false;
        SetCharacterState("Idle");
        Debug.Log("Animation d'attaque terminée. Retour à Idle.");

        // Désabonner l'événement pour éviter des appels multiples
        trackEntry.Complete -= OnAttackComplete;
    }
}



private bool hasMidAttackEvent = false;






    IEnumerator PerformHeavyAttack()
{
    if (isInHitStun) yield break; // Empêche l'attaque si le joueur est en HitStun

    isAttacking = true;
    SetCharacterState("AttackHeavy");

    float damageMultiplier = rhythmBonus.CheckRhythm();
    ApplyDamage(5f * damageMultiplier);

    yield return new WaitForSeconds(1.5f);

    isAttacking = false;
    SetCharacterState("Idle");
}

   IEnumerator PerformGimmick()
{
    if (isGimmickOnCooldown)
    {
        Debug.Log("La téléportation est en cooldown !");
        yield break;
    }

    isGimmickOnCooldown = true;
    isAttacking = true;
    isGimmick = true;
    SetCharacterState("Gimmick");

    // Activer l'invincibilité
    if (rhythmBonus != null)
    {
        rhythmBonus.SetInvincibility(true);
    }

    yield return new WaitForSeconds(0.57f);

    Vector3 currentPosition = transform.position;
    float offset = teleportDistance;

    Vector3 newPosition = currentPosition;

    if (enemyTransform.position.x < currentPosition.x)
    {
        newPosition.x = currentPosition.x - offset;
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    else
    {
        newPosition.x = currentPosition.x + offset;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    transform.position = newPosition;
    Debug.Log("Téléportation effectuée à 0,375 secondes !");

    yield return new WaitForSeconds(0.5f);

    isAttacking = false;
    isGimmick = false;
    SetCharacterState("Idle");

    // Désactiver l'invincibilité
    if (rhythmBonus != null)
    {
        rhythmBonus.SetInvincibility(false);
    }

    StartCoroutine(GimmickCooldown());
}




IEnumerator GimmickCooldown()
{
    Debug.Log("Cooldown de la téléportation commencé !");
    yield return new WaitForSeconds(gimmickCooldown);
    isGimmickOnCooldown = false;
    Debug.Log("Téléportation réutilisable !");
}





private void OnTeleportEvent(Spine.TrackEntry trackEntry, Spine.Event e)
{
    if (e.Data.Name == "Teleport")
    {
        Debug.Log("Déclenchement de la téléportation !");
        Vector3 currentPosition = transform.position;
        float offset = teleportDistance;

        // Déterminer la nouvelle position
        Vector3 newPosition;
        if (enemyTransform.position.x < currentPosition.x)
        {
            // Téléporter vers la gauche
            newPosition = new Vector3(currentPosition.x - offset, currentPosition.y, currentPosition.z);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // Téléporter vers la droite
            newPosition = new Vector3(currentPosition.x + offset, currentPosition.y, currentPosition.z);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Appliquer la nouvelle position
        transform.position = newPosition;
        Debug.Log("Téléportation effectuée à la 6ème frame !");
    }
}











    public void ApplyDamage(float damage)
{
    if (isInvincible)
    {
        Debug.Log("Le joueur est invincible et ne subit pas de dégâts !");
        return;
    }

    Debug.Log("Dégâts infligés : " + damage);
    Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

    foreach (Collider enemy in hitEnemies)
    {
        HealthOpponentWilson healthOpponent = enemy.GetComponentInChildren<HealthOpponentWilson>();
        if (healthOpponent != null)
        {
            healthOpponent.TakeDamage(damage);
        }
    }

    // Jouer l'animation de hit pour le joueur
    PlayHitReaction();
}



    private Spine.TrackEntry redStateTrackEntry;

public void TriggerRedState()
{
    if (!isRedState && !isRedStatePlaying)
    {
        isRedState = true;
        isRedStatePlaying = true;

        // Désactiver le mouvement et rendre le Rigidbody cinématique
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        Debug.Log("TriggerRedState appelé. État rouge activé.");
        redStateTrackEntry = skeletonAnimation.state.SetAnimation(0, redState, false);
        redStateTrackEntry.Complete += OnRedStateComplete;
    }
}




public void PlayHitReaction()
{
    // Si le joueur est mort, en état rouge, invincible, déjà en HitStun, ou en train d'attaquer, ne pas jouer l'animation
    if (isDead || isRedStatePlaying || isInvincible || isInHitStun || isAttacking)
    {
        Debug.Log("Le joueur est invincible, en état spécial ou en train d'attaquer, l'animation de HitStun n'est pas jouée.");
        return;
    }

    isInHitStun = true;

    float randomValue = Random.Range(0f, 1f);
    Spine.TrackEntry trackEntry;

    if (randomValue < 0.5f)
    {
        trackEntry = skeletonAnimation.state.SetAnimation(0, HitStun1.Animation, false);
        Debug.Log("Joue l'animation HitStun1");
    }
    else
    {
        trackEntry = skeletonAnimation.state.SetAnimation(0, HitStun2.Animation, false);
        Debug.Log("Joue l'animation HitStun2");
    }

    trackEntry.Complete += OnHitStunComplete;
}




void OnHitStunComplete(Spine.TrackEntry trackEntry)
{
    isInHitStun = false;
    SetCharacterState("Idle");
    Debug.Log("Animation de HitStun terminée. Retour à Idle.");
}






    public void TriggerDeath()
    {
        if (!isDead)
        {
            isDead = true;
            SetCharacterState("DeadState");
        }
    }

    

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void LateUpdate()
    {
        if (isDead) return;

        if (enemyTransform != null)
        {
            if (enemyTransform.position.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}