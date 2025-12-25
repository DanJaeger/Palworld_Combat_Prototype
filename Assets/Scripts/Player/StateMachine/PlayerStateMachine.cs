using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador principal del jugador que actúa como el "Contexto" en el patrón State Machine.
/// Gestiona los componentes, la entrada de usuario y las variables físicas compartidas entre estados.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerStateMachine : MonoBehaviour
{
    #region Componentes Cheados
    private CharacterController _characterController;
    private Animator _anim;
    private PlayerInput _playerInput;
    #endregion

    #region Configuración de Movimiento
    [Header("Ajustes de Velocidad")]
    [SerializeField] private float _walkSpeed = 4.0f;
    [SerializeField] private float _runSpeed = 8.0f;
    [SerializeField] private float _rotationFactorPerFrame = 15.0f;

    // Propiedades públicas para acceso desde los Estados
    public float CurrentSpeed { get; set; }
    public float WalkSpeed => _walkSpeed;
    public float RunSpeed => _runSpeed;

    // Datos de entrada y movimiento
    public Vector2 CurrentMovementInput { get; set; }
    public Vector3 CurrentMovement { get; set; } // Representa la dirección deseada
    public Vector3 AppliedMovement;              // Representa el vector final (x, y, z) aplicado al CharacterController

    public bool IsMovementPressed { get; set; }
    public bool IsRunPressed { get; set; }
    #endregion

    #region Configuración de Salto y Gravedad
    [Header("Ajustes de Salto")]
    [SerializeField] private float _maxJumpHeight = 2.0f;
    [SerializeField] private float _maxJumpTime = 0.7f;

    [Header("Estado de Salto")]
    public bool IsJumpPressed { get; set; }
    public bool IsJumping { get; set; }
    public bool HoldJump { get; set; } // Útil para saltos de altura variable

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    #endregion

    #region Diccionario de Animaciones (Hashes)
    // Usar Hashes es mucho más eficiente que usar Strings en cada frame
    public static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    public static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    public static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    public static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    #endregion

    #region State Machine
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }
    public PlayerStateFactory States => _states;
    #endregion

    #region Acceso a Componentes (Propiedades)
    public Animator Anim => _anim;
    public CharacterController CharacterController => _characterController;
    #endregion

    private void Awake()
    {
        // Inicialización de componentes
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _playerInput = new PlayerInput();

        // Configuración inicial
        SetupJumpVariables();

        // Inicialización de la Máquina de Estados
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // Suscripción a inputs
        SetupInputCallbacks();
    }

    /// <summary>
    /// Calcula la gravedad y la velocidad inicial de salto basándose en la altura deseada
    /// y el tiempo que debe tardar el jugador en alcanzar el punto más alto (Apex).
    /// </summary>
    private void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        // Fórmulas físicas: Gravedad = (-2 * h) / t^2  | Velocidad inicial = (2 * h) / t
        Gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        InitialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    private void Update()
    {
        HandleRotation();

        // Delega la lógica de actualización al estado actual
        _currentState.UpdateStates();

        // Aplicamos la velocidad horizontal basándonos en la velocidad actual (Walk/Run)
        // La Y se mantiene como la calcula la gravedad en los estados de Jump/Grounded
        AppliedMovement.x = CurrentMovement.x * CurrentSpeed;
        AppliedMovement.z = CurrentMovement.z * CurrentSpeed;

        // Movimiento físico final
        _characterController.Move(AppliedMovement * Time.deltaTime);
    }

    /// <summary>
    /// Rota al personaje suavemente hacia la dirección del movimiento.
    /// </summary>
    private void HandleRotation()
    {
        if (IsMovementPressed)
        {
            Vector3 targetDirection = new Vector3(CurrentMovement.x, 0, CurrentMovement.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    #region Input Handling
    private void SetupInputCallbacks()
    {
        _playerInput.Locomotion.Move.started += OnMovementInput;
        _playerInput.Locomotion.Move.canceled += OnMovementInput;
        _playerInput.Locomotion.Move.performed += OnMovementInput;

        _playerInput.Locomotion.Run.started += OnRunInput;
        _playerInput.Locomotion.Run.canceled += OnRunInput;

        _playerInput.Locomotion.Jump.started += OnJumpInput;
        _playerInput.Locomotion.Jump.canceled += OnJumpInput;
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        CurrentMovementInput = context.ReadValue<Vector2>();
        IsMovementPressed = CurrentMovementInput.x != 0 || CurrentMovementInput.y != 0;

        // Mapeamos el Vector2 del input al Vector3 de movimiento (X, Z)
        CurrentMovement = new Vector3(CurrentMovementInput.x, 0, CurrentMovementInput.y);
    }

    private void OnJumpInput(InputAction.CallbackContext context) => IsJumpPressed = context.ReadValueAsButton();
    private void OnRunInput(InputAction.CallbackContext context) => IsRunPressed = context.ReadValueAsButton();

    private void OnEnable() => _playerInput.Locomotion.Enable();
    private void OnDisable() => _playerInput.Locomotion.Disable();
    #endregion
}