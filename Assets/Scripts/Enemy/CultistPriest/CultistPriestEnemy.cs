using UnityEngine;

public class CultistPriestEnemy : MonoBehaviour
{
    [Header("Cultist Priest")]
    [SerializeField] private float target_range;
    private float takeHitDuration = 2;
    private float takeHitTimer = Mathf.Infinity;
    private float distanceToPlayer;
    private float minDistanceToChange;
    private bool movingLeft = true;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float idleDuration;
    private float idleTimer = 0;

    [Header("Attack Parameters")]
    [SerializeField] private GameObject attack1_object;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    private bool isAttacking = false;

    [Header("Skill 1 Rober")]
    [SerializeField] private GameObject skill1TransPrefab;
    [SerializeField] private float skill1RoberRange;
    [SerializeField] private float skill1RoberDistance;
    [SerializeField] private float skill1RoberCooldown;
    private float skill1RoberTimer = Mathf.Infinity;
    private bool isSkill1Rober = false;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;

    // References
    private Animator anim;
    private Health playerHealth;
    private Health health;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        health = GetComponent<Health>();
        playerHealth = playerMovement.GetComponent<Health>();
    }

    private void Update()
    {
        if (health.IsDead())
        {
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 4);
            return;
        }

        cooldownTimer += Time.deltaTime;
        skill1RoberTimer += Time.deltaTime;
        takeHitTimer += Time.deltaTime;

        // Kiểm tra Skill1Rober trước, nếu đang trong thời gian cooldown hoặc thực thi thì không cho phép Attack
        if (skill1RoberTimer >= skill1RoberCooldown && PlayerInSight_Skill1Rober() && !isAttacking)
        {
            isSkill1Rober = true;
            skill1RoberTimer = 0;

            skill1TransPrefab.transform.localScale = new Vector3(
                Mathf.Abs(skill1TransPrefab.transform.localScale.x) * Mathf.Sign(transform.localScale.x),
                skill1TransPrefab.transform.localScale.y,
                skill1TransPrefab.transform.localScale.z
            );

            GameObject skill1Trans = Instantiate(
                skill1TransPrefab,
                new Vector3(
                    transform.position.x,
                    transform.position.y - 2f,
                    transform.position.z),
                Quaternion.identity);

            Destroy(skill1Trans, 1.5f);
            SoundManager.instance.PlaySound(attackSound);
        }

        if (skill1RoberTimer <= skill1RoberCooldown)
        {
            isSkill1Rober = false;
        }

        // Chỉ cho phép Attack nếu không trong trạng thái Skill1Rober
        if (PlayerInSight_Attack() && cooldownTimer >= attackCooldown && playerHealth.currentHealth > 0 && !isSkill1Rober)
        {
            isAttacking = true;
            cooldownTimer = 0;
            GameObject Attack1 = Instantiate(
                attack1_object,
                new Vector3(
                    transform.position.x + transform.localScale.x * 1f,
                    transform.position.y + 2f,
                    transform.position.z),
                Quaternion.identity);

            Attack1.transform.localScale = new Vector3(5, 5, 1);
            Destroy(Attack1, 1f);
            anim.SetTrigger("cultistpriest_attack");
            SoundManager.instance.PlaySound(attackSound);
        }

        if (cooldownTimer <= attackCooldown)
        {
            isAttacking = false;
        }

        // Ngăn di chuyển nếu đang trong thời gian skill1Rober (bao gồm duration và cooldown) hoặc các trạng thái khác
        if (IsTakeHit() || health.IsDead() || PlayerInSight_Attack() || isAttacking || skill1RoberTimer <= 2f)
        {
            anim.SetBool("cultistpriest_moving", false);
        }
        else
        {
            if (PlayerInSight())
            {
                distanceToPlayer = playerMovement.transform.position.x - transform.position.x;
                minDistanceToChange = 0.1f;
                if (movingLeft)
                {
                    if (distanceToPlayer < -minDistanceToChange)
                    {
                        ChasePlayer(-1);
                    }
                    else
                    {
                        DirectionChange();
                    }
                }
                else
                {
                    if (distanceToPlayer > minDistanceToChange)
                    {
                        ChasePlayer(1);
                    }
                    else
                    {
                        DirectionChange();
                    }
                }
            }
        }
    }

    private bool PlayerInSight_Attack()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }
        return hit.collider != null;
    }

    private void DirectionChange()
    {
        anim.SetBool("cultistpriest_moving", false);

        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            idleTimer = 0;
            movingLeft = !movingLeft;
        }
    }

    private void ChasePlayer(int _direction)
    {
        anim.SetBool("cultistpriest_moving", true);

        transform.position = new Vector3(
            transform.position.x + Time.deltaTime * _direction * moveSpeed,
            transform.position.y,
            transform.position.z);

        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * _direction,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            new Vector3(
                boxCollider.bounds.size.x * target_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z),
            0,
            Vector2.left,
            0,
            playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private bool PlayerInSight_Skill1Rober()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * skill1RoberRange * transform.localScale.x * skill1RoberDistance,
            new Vector3(
                boxCollider.bounds.size.x * skill1RoberRange,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z),
            0,
            Vector2.left,
            0,
            playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z)
            );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center,
            new Vector3(
                boxCollider.bounds.size.x * target_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z)
            );

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * skill1RoberRange * transform.localScale.x * skill1RoberDistance,
            new Vector3(
                boxCollider.bounds.size.x * skill1RoberRange,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z)
            );
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeHit()
    {
        takeHitTimer = 0;
    }

    public bool IsTakeHit()
    {
        return takeHitTimer <= takeHitDuration;
    }
}