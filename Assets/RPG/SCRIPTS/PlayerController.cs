using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayControls controls;

    [Header("Movimento")]
    [SerializeField] private Vector2 moveInput;
    private Vector3 movementDirection;

    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Animação")]
    [SerializeField] private bool isWalk;
    [SerializeField] private bool isAttack;

    [Header("Ataque")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private InputAction attackAction;

    private void Awake()
    {
        controls = new PlayControls();

        controls.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => moveInput = Vector2.zero;

        attackAction = controls.FindAction("Attack");
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimation();

        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            AttackPlayer();
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void MovePlayer()
    {
        if (isAttack)
        {
            isWalk = false;
            return;
        }

        movementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (movementDirection != Vector3.zero)
        {
            characterController.Move(movementDirection * walkSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("IsWalk", isWalk);
    }

    private void AttackPlayer()
    {
        if (isAttack) return;

        isAttack = true;
        isWalk = false;

        animator.SetTrigger("isAttack1");

        Invoke(nameof(HitEnemy), 0.3f);
        Invoke(nameof(EndAttack), attackTime);
    }

    private void HitEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void EndAttack()
    {
        isAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}