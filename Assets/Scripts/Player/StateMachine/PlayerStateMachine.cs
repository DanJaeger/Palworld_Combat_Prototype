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
    private Transform _cameraTransform;

    [Header("Character Stats: ")]
    [SerializeField] private CharacterStatsSO _characterStats;
    #endregion

    #region Configuración de Movimiento

    // Propiedades públicas para acceso desde los Estados
    public float CurrentSpeed { get; set; }
    public float WalkSpeed => _characterStats.WalkSpeed;
    public float RunSpeed => _characterStats.RunSpeed;

    // Datos de entrada y movimiento
    public Vector2 CurrentMovementInput { get; set; }
    public Vector3 CurrentMovement { get; set; } // Representa la dirección deseada
    [HideInInspector] public Vector3 AppliedMovement;              // Representa el vector final (x, y, z) aplicado al CharacterController

    public bool IsMovementPressed { get; set; }
    public bool IsRunPressed { get; set; }
    #endregion

    #region Configuración de Salto y Gravedad

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

        if (Camera.main != null) _cameraTransform = Camera.main.transform;

        // Configuración inicial
        SetupJumpVariables();

        // Inicialización de la Máquina de Estados
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // Suscripción a inputs
        SetupInputCallbacks();

        //Mouse Config
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Calcula la gravedad y la velocidad inicial de salto basándose en la altura deseada
    /// y el tiempo que debe tardar el jugador en alcanzar el punto más alto (Apex).
    /// </summary>
    private void SetupJumpVariables()
    {
        float timeToApex = _characterStats.MaxJumpTime / 2;
        // Fórmulas físicas: Gravedad = (-2 * h) / t^2  | Velocidad inicial = (2 * h) / t
        Gravity = (-2 * _characterStats.MaxJumpHeight) / Mathf.Pow(timeToApex, 2);
        InitialJumpVelocity = (2 * _characterStats.MaxJumpHeight) / timeToApex;
    }

    private void Update()
    {
        CalculateCameraRelativeMovement();

        HandleRotation();

        // Delega la lógica de actualización al estado actual
        _currentState.UpdateStates();

        // Aplicamos la velocidad horizontal basándonos en la velocidad actual (Walk/Run)
        // La Y se mantiene como la calcula la gravedad en los estados de Jump/Grounded
        AppliedMovement.x = CurrentMovement.x * CurrentSpeed;
        AppliedMovement.z = CurrentMovement.z * CurrentSpeed;
        // Y lleva la velocidad vertical directa (Salto/Gravedad)
        AppliedMovement.y = CurrentMovement.y;

        // Movimiento físico final
        _characterController.Move(AppliedMovement * Time.deltaTime);
    }

    /// <summary>
    /// Transforma el input local (WASD) en una dirección del mundo real basada en la cámara.
    /// </summary>
    private void CalculateCameraRelativeMovement()
    {
        if (_cameraTransform == null) return;

        // Obtenemos los vectores Forward y Right de la cámara
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        // Anulamos la inclinación vertical (Y) para que el personaje no intente "volar"
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Guardamos la dirección horizontal en un vector temporal
        Vector3 desiredRunDirection = (forward * CurrentMovementInput.y) + (right * CurrentMovementInput.x);
        // Creamos la dirección final combinando el input con la orientación de la cámara
        CurrentMovement = new Vector3(desiredRunDirection.x, CurrentMovement.y, desiredRunDirection.z);
    }

    /// <summary>
    /// Rota al personaje suavemente hacia la dirección del movimiento.
    /// </summary>
    private void HandleRotation()
    {
        // Solo rotamos si el jugador está moviendo el stick/teclas
        // Esto cumple tu condición: si solo mueves la cámara y no el input, el jugador no rota.
        if (IsMovementPressed)
        {
            // 1. Creamos un vector basado en el movimiento, pero forzamos Y a cero
            // Esto evita que el personaje se incline al saltar o caer.
            Vector3 rotationDirection = new Vector3(CurrentMovement.x, 0f, CurrentMovement.z);

            // 2. Solo rotamos si hay una dirección horizontal real
            if (rotationDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _characterStats.RotationFactorPerFrame * Time.deltaTime);
            }
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