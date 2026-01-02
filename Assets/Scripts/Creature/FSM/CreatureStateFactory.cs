using System.Collections.Generic;

/// <summary>
/// Listado de estados disponibles para el jugador.
/// Facilita el acceso mediante claves en lugar de strings o comparaciones de clase.
/// </summary>
public enum CreatureStates
{
    Idle,
    Patrol,
    Chase
}

/// <summary>
/// F?brica encargada de instanciar y almacenar todos los estados del jugador.
/// Utiliza el patr?n Flyweight para reutilizar instancias y optimizar memoria.
/// </summary>
public class CreatureStateFactory
{
    private readonly CreatureStateMachine _context;
    private readonly Dictionary<CreatureStates, CreatureBaseState> _states = new();

    /// <summary>
    /// Inicializa la f?brica y pre-genera todos los estados posibles.
    /// </summary>
    /// <param name="currentContext">Referencia a la m?quina de estados principal.</param>
    public CreatureStateFactory(CreatureStateMachine currentContext)
    {
        _context = currentContext;

        // Registro de instancias
        _states[CreatureStates.Idle] = new CreatureIdleState(_context, this);
        _states[CreatureStates.Patrol] = new CreaturePatrolState(_context, this);
        _states[CreatureStates.Chase] = new CreatureChaseState(_context, this);
    }

    #region Accessors (Getters)
    // Estos m?todos permiten a los estados transicionar usando Factory.Idle(), etc.

    public CreatureBaseState Idle() => _states[CreatureStates.Idle];
    public CreatureBaseState Patrol() => _states[CreatureStates.Patrol];
    public CreatureBaseState Chase() => _states[CreatureStates.Chase];
    #endregion
}