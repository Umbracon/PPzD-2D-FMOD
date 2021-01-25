using FMODUnity;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField, EventRef] string dyingRef;
    [SerializeField, EventRef] string damagedRef;
    [SerializeField, EventRef] string attackRef;
    [SerializeField, EventRef] string footstepRef;

    [SerializeField] Bandit banditController;
    [SerializeField] float speed = .5f;
    [SerializeField] float attackRange = .25f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] float damage = 10;

    EnemyInput input;
    PlayerController target;
    float attackCooldownTimer;
    float distanceToTarget;

    void Awake() {
        input = new EnemyInput();
        banditController.Setup(input);
        banditController.AttackHitEvent += CheckIfTargetInRange;
        banditController.DiedEvent += Despawn;
        banditController.DiedEvent += HandleDeath;
        banditController.FootstepEvent += PlayFootstepSound;
    }

    void Despawn() {
        RuntimeManager.PlayOneShot(dyingRef);
        Destroy(gameObject, 3);
    }

    void CheckIfTargetInRange() {
        if (distanceToTarget <= attackRange)
            target.DealDamage(damage);
    }

    void Update() {
        if (banditController.IsDead) {
            input.horizontalMove = 0;
            return;
        }
        UpdateTarget();
        UpdateMovement();
        UpdateAttack();
    }

    void UpdateTarget() {
        if (!target)
            target = FindObjectOfType<PlayerController>();
        if(target)
            distanceToTarget = (target.transform.position - transform.position).magnitude;
    }

    void UpdateMovement() {
        if (target && !target.IsDead) {
            var targetPosition = target.transform.position;
            var movementDirection = (targetPosition - transform.position).normalized;
            input.horizontalMove = movementDirection.x * speed;
        } else {
            input.horizontalMove = 0;
        }
    }

    void UpdateAttack() {
        input.isAttacking = false;
        if (attackCooldownTimer > 0) {
            attackCooldownTimer -= Time.deltaTime;
            return;
        }
        if (target && !target.IsDead) {
            if (distanceToTarget <= attackRange) {
                RuntimeManager.PlayOneShot(attackRef);
                input.isAttacking = true;
                attackCooldownTimer = attackCooldown;
            }
        }
    }

    public void DealDamage(float damage) {
        banditController.TakeDamage(damage);

        if (!banditController.IsDead)
        {
            RuntimeManager.PlayOneShot(damagedRef);
        }
    }

    void HandleDeath() {
        GetComponent<Collider2D>().enabled = false;
    }

    void PlayFootstepSound() 
    {
        RuntimeManager.PlayOneShotAttached(footstepRef, gameObject);
    }
}