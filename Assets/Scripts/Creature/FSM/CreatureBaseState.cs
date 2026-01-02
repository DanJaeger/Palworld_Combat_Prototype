using UnityEngine;

/// <summary>
/// Clase base abstracta para el sistema de M?quina de Estados Jer?rquica (HFSM).
/// Gestiona la l?gica de entrada, salida y la relaci?n entre Super-Estados y Sub-Estados.
/// </summary>
public abstract class CreatureBaseState
{
    #region Fields
    private bool _isRootState = false;
    private CreatureStateMachine _context;
    private CreatureStateFactory _factory;
    private CreatureBaseState _currentSubState;
    private CreatureBaseState _currentSuperState;
    #endregion

    #region Properties
    protected bool IsRootState { get => _isRootState; set => _isRootState = value; }
    protected CreatureStateMachine Context => _context;
    protected CreatureStateFactory Factory => _factory;
    #endregion

    /// <summary>
    /// Constructor base para inicializar las referencias del contexto y la factory.
    /// </summary>
    public CreatureBaseState(CreatureStateMachine currentContext, CreatureStateFactory playerStateFactory)
    {
        _context = currentContext;
        _factory = playerStateFactory;
    }

    #region Abstract Methods
    /// <summary> Se ejecuta una vez al entrar al estado. </summary>
    public abstract void EnterState();

    /// <summary> Se ejecuta cada frame (llamado desde FixedUpdate en este proyecto). </summary>
    public abstract void UpdateState();

    /// <summary> Se ejecuta una vez antes de cambiar a un nuevo estado. </summary>
    public abstract void ExitState();

    /// <summary> Contiene las condiciones necesarias para transicionar a otros estados. </summary>
    public abstract void CheckSwitchStates();
    #endregion

    #region State Logic
    /// <summary>
    /// Actualiza el estado actual y recursivamente su sub-estado activo.
    /// </summary>
    public void UpdateStates()
    {
        UpdateState();
    }

    /// <summary>
    /// Gestiona la transici?n hacia un nuevo estado, notificando al contexto o al super-estado.
    /// </summary>
    /// <param name="newState">El estado al que se desea transicionar.</param>
    protected void SwitchState(CreatureBaseState newState)
    {
        // 1. Salir del estado actual
        ExitState();

        // 2. Entrar en el nuevo estado
        newState.EnterState();

        //3. Asignamos nuevo estado
        _context.CurrentState = newState;

    }
    #endregion
}