using System.Collections;
using UnityEngine;
using Spine.Unity;

public class IaPlayerWilson : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, attackFast, attackHeavy, parry, deadState, redState, Gimmick, HitStun1,HitStun2;
    public string currentState;
    private string currentAnimation;

    public float speed;
    private float movement;
    private bool isAttacking;
    private bool isParrying;
    public float gimmickCooldown = 10f;
    private Rigidbody rb;
    public PlayerControllerBetty playerController;
    public Transform player;
    public bool forceIdleAfterRedState = false;
    public bool isRedState = false;
    public bool isDead = false;
    public bool isGimmick = false;
    private bool isGimmickOnCooldown = false;
    private bool isRedStatePlaying = false; // Indicateur pour savoir si l'animation RedState est déjà jouée
    private bool redStateAnimationPlayed = false; // Indicateur pour savoir si RedState a déjà été joué


    
    
    private float previousHealth;


    public RhythmBonus rhythmBonus;
    public float attackDistance = 2f;
    private bool isInHitStun = false;
    public float blockChance = 0.3f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    public float health = 100f;
    public HealthOpponentWilson healthOpponent;
    public float attackDamage = 4f;
    public float attackRange = 1f;
    public Transform playerTransform;
    public HealthBetty playerHealth;
    public float redStateDuration = 1.5f; // Durée en secondes pour l'état rouge
    



    public bool enableDamage = false;
    private bool hasPlayedRedStateAnimation = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = "Idle";
        SetCharacterState("Idle");
        isAttacking = false;
        isParrying = false;

        if (rhythmBonus == null)
        {
            rhythmBonus = GetComponent<RhythmBonus>();
        }

        if (healthOpponent == null)
        {
            healthOpponent = GetComponent<HealthOpponentWilson>();
            if (healthOpponent == null)
            {
                Debug.LogError("Le script HealthOpponent est introuvable !");
            }
        }
        previousHealth = healthOpponent.currentHealthVerteOpponent + healthOpponent.currentHealthRedOpponent;

    }

    void Update()
{
    if (isDead || isInHitStun) return; // Ajoutez isInHitStun ici pour empêcher toute action

    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    if (player.position.x > transform.position.x)
    {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    else
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    if (!isAttacking && !isParrying && !isGimmick && !isRedStatePlaying)
    {
        if (distanceToPlayer > attackDistance)
        {
            MoveTowardsPlayer();
        }
        else if (distanceToPlayer <= attackDistance && Time.time >= lastAttackTime + attackCooldown)
        {
            rb.velocity = Vector3.zero;
            DecideAttackOrBlock();
        }
    }

    // Forcer l'animation Idle si l'IA est immobile
    if (rb.velocity.magnitude == 0 && currentState != "Idle" && !isRedStatePlaying && !isAttacking && !isParrying && !isGimmick)
    {
        Debug.Log("Forcer retour à Idle dans Update");
        SetCharacterState("Idle");
    }
}





    void MoveTowardsPlayer()
{
    if (isDead || isRedStatePlaying || isInHitStun) return; // Ajoutez isInHitStun ici

    Vector3 direction = (player.position - transform.position).normalized;
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    RaycastHit hit;
    if (Physics.Raycast(transform.position, direction, out hit, distanceToPlayer))
    {
        if (hit.collider.CompareTag("Wall"))
        {
            Debug.Log("Obstacle détecté entre l'IA et le joueur.");
            rb.velocity = Vector3.zero;
            SetCharacterState("Idle");
            return;
        }
    }

    rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, rb.velocity.z);
    SetCharacterState("Walking");
}



    void DecideAttackOrBlock()
{
    if (isDead || isInHitStun) return; // Ajoutez isInHitStun ici

    float randomValue = Random.Range(0f, 1f);

    if (randomValue < blockChance)
    {
        StartParry();
    }
    else
    {
        StartCoroutine(PerformAttack());
    }
}


    public void StartParry()
    {
        isParrying = true;
        SetCharacterState("Parry");
        rb.velocity = Vector3.zero;

        StartCoroutine(StopParryAfterDelay(1f));

    }

    IEnumerator StopParryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isParrying = false;
        SetCharacterState("Idle");

    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        rb.velocity = Vector3.zero;

        float randomAttack = Random.Range(0f, 1f);
        if (randomAttack < 0.5f)
        {
            SetCharacterState("AttackFast");
            yield return new WaitForSeconds(0.5f);


        }
        else
        {
            SetCharacterState("AttackHeavy");
            yield return new WaitForSeconds(1.5f);

        }

        AttackPlayer();

        isAttacking = false;
        lastAttackTime = Time.time;
        SetCharacterState("Idle");

    }

    


    

   void AttackPlayer()
{
    if (enableDamage && Vector3.Distance(transform.position, playerTransform.position) < attackRange)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("IA a infligé " + attackDamage + " points de dégâts au joueur");
        }
        // Supprimez l'appel à PlayHitReaction ici
        // PlayHitReaction(); <-- Enlevez cette ligne
    }
}



    public void PlayHitReaction()
{
    // Ne joue pas HitStun si l'IA est morte, ou déjà en HitStun
    if (isDead || isInHitStun) return;

    // Si RedState a déjà été joué, on autorise HitStun
    if (redStateAnimationPlayed)
    {
        Debug.Log("RedState a déjà été joué, transition vers HitStun.");
        isRedStatePlaying = false;
        isRedState = false;
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
    if (!isRedStatePlaying)
    {
        SetCharacterState("Idle");
    }
}







    IEnumerator ReturnToIdleAfterHit()
    {
        yield return new WaitForSeconds(1.3f);
        isInHitStun = false;
        SetCharacterState("Idle");
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
        Debug.Log("L'IA est en HitStun, ignore l'état : " + state);
        return;
    }

    // Réinitialiser l'état si on passe à Idle
    if (state == "Idle")
    {
        SetAnimation(idle, true, 1f);
        currentState = "Idle";
        Debug.Log("Transition vers Idle");
        rb.velocity = Vector3.zero; // Arrêter le mouvement
    }
    else if (state == "Walking")
    {
        SetAnimation(walking, true, 1f);
        currentState = "Walking";
        Debug.Log("Transition vers Walking");
    }
    else if (state == "AttackFast")
    {
        SetAnimation(attackFast, false, 1f);
        currentState = "AttackFast";
    }
    else if (state == "AttackHeavy")
    {
        SetAnimation(attackHeavy, false, 1f);
        currentState = "AttackHeavy";
    }
    else if (state == "Parry")
    {
        SetAnimation(parry, false, 1f);
        currentState = "Parry";
    }
    else if (state == "Gimmick")
    {
        SetAnimation(Gimmick, false, 1f);
        currentState = "Gimmick";
    }
    else if (state == "HitStun1")
    {
        SetAnimation(HitStun1, false, 1f);
        currentState = "HitStun1";
    }
    else if (state == "HitStun2")
    {
        SetAnimation(HitStun2, false, 1f);
        currentState = "HitStun2";
    }
}









    public void SetAnimation(AnimationReferenceAsset animationAsset, bool loop, float timeScale)
    {
        if (animationAsset == null) return;

        Spine.Animation animation = animationAsset.Animation;
        if (animation == null || currentAnimation == animation.Name) return;

        skeletonAnimation.state.ClearTracks();
        skeletonAnimation.skeleton.SetToSetupPose();
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;

        currentAnimation = animation.Name;
    }

    public void TriggerDeath()
    {
        if (!isDead)
        {
            isDead = true;
            ResetRedStateAnimation();
            SetCharacterState("DeadState");
            Debug.Log("L'IA est maintenant morte");
        }
    }

    private Spine.TrackEntry redStateTrackEntry;

    public void TriggerRedState()
{
    if (!isRedState && !isDead && !hasPlayedRedStateAnimation && !redStateAnimationPlayed)
    {
        isRedState = true;
        isRedStatePlaying = true;
        hasPlayedRedStateAnimation = true;
        redStateAnimationPlayed = true; // Marque l'animation comme jouée une fois

        SetAnimation(redState, false, 1f);
        redStateTrackEntry = skeletonAnimation.state.SetAnimation(0, redState, false);
        redStateTrackEntry.Complete += OnRedStateComplete;

        Debug.Log("L'IA entre en état rouge !");
        StartCoroutine(EndRedStateAfterDelay());
    }
}



    IEnumerator EndRedStateAfterDelay()
    {
        yield return new WaitForSeconds(redStateDuration);
        isRedState = false;
        SetCharacterState("Idle");

    }

    IEnumerator PerformGimmick()
{
    if (isGimmickOnCooldown)
    {
        Debug.Log("La saisie (grab) est en cooldown !");
        yield break;
    }

    isGimmickOnCooldown = true;
    isGimmick = true;
    SetCharacterState("Gimmick");

    yield return new WaitForSeconds(0.57f); // Durée de l'animation avant de vérifier la portée

    // Vérifier si le joueur est à portée pour le grab
    if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
    {
        Debug.Log("Grab réussi ! Inflige des dégâts doublés.");
        ApplyGrabDamage(); // Inflige des dégâts doublés
    }
    else
    {
        Debug.Log("Le grab a échoué, le joueur est hors de portée.");
    }

    yield return new WaitForSeconds(0.5f); // Attente avant de revenir à l'état Idle

    isGimmick = false;
    SetCharacterState("Idle");
    StartCoroutine(GimmickCooldown());
}

void ApplyGrabDamage()
{
    if (playerHealth != null)
    {
        float grabDamage = attackDamage * 2; // Dégâts doublés
        playerHealth.TakeDamage(grabDamage);
        Debug.Log("IA a infligé " + grabDamage + " points de dégâts au joueur avec le grab !");
    }
}

IEnumerator GimmickCooldown()
{
    yield return new WaitForSeconds(gimmickCooldown);
    isGimmickOnCooldown = false;
    Debug.Log("La saisie (grab) est réutilisable !");
}





   private void OnRedStateComplete(Spine.TrackEntry trackEntry)
{
    if (trackEntry == redStateTrackEntry)
    {
        isRedStatePlaying = false;
        isRedState = false;
        hasPlayedRedStateAnimation = false;
        isInHitStun = false;

        Debug.Log("Animation RedState terminée, ne sera plus jouée.");

        // Réinitialiser l'état pour permettre les animations HitStun
        rb.velocity = Vector3.zero;
        SetCharacterState("Idle");
    }
}








    public void UpdatePreviousHealth(float health)
    {
    previousHealth = health;
    }


    public void ResetRedStateAnimation()
    {
        hasPlayedRedStateAnimation = false;
        isRedState = false;
        
        isRedStatePlaying = false;


    }
}
