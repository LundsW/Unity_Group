using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayControls controls;

    [SerializeField] private Vector2 _moveInput;

    private Vector3                 _movementDirection;
    private CharacterController     _characterController;
    public float                    _walkSpeed;
    public Animator                 _animator;
    [SerializeField] private bool   _isWalk;

    private void Awake()
    {
        controls = new PlayControls();

        controls.Player.Move.performed += context => _moveInput = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => _moveInput = Vector2.zero;
    }


    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

        updateAnimation();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void MovePlayer()
    {
        _movementDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;

        if (_movementDirection != Vector3.zero)
        {
            _characterController.Move(_movementDirection * _walkSpeed * Time.deltaTime);
            _isWalk = true;
        }

        else
        {
            _isWalk = false;
        }
        
    }


    void updateAnimation()
    {
        _animator.SetBool("isWalk", _isWalk);
    }
}