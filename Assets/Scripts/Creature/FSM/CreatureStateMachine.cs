using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CreatureStateMachine : MonoBehaviour
{
    #region Data
    [Header("Creature Data: ")]
    [SerializeField] private CreatureData _data;
    public CreatureData Data => _data;
    #endregion

    #region Private Components
    private CreatureStateFactory _states;
    private CreatureBaseState _currentState;
    #endregion

    #region Animation
    private Animator _anim;

    public static readonly int VertHash = Animator.StringToHash("Vert");
    public static readonly int StateHash = Animator.StringToHash("State");

    private Coroutine _stateTransitionCoroutine;
    private float _nextActionAllowedTime;
    public bool IsBusy { get; private set; } // Bandera de ocupado
    #endregion

    #region Navigation
    [Header("Navigation")]
    [HideInInspector]
    public NavMeshAgent Agent { get; private set; }

    public Transform FollowTarget { get; private set; }
    #endregion

    #region Strategies
    // Referencias a las estrategias (pueden ser Serialized o inyectadas)
    public IIdleStrategy IdleStrategy { get; set; }
    public IMovementStrategy PatrolStrategy { get; set; }
    public IMovementStrategy ChaseStrategy { get; set; }
    #endregion

    // Propiedades expuestas para los estados
    public CreatureBaseState CurrentState { get => _currentState; set => _currentState = value; }
    public Animator Anim { get => _anim; set => _anim = value; }

    #region Initialization
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        if (_anim == null) Debug.LogError("No Animator found on: " + this.name);

        Agent = GetComponent<NavMeshAgent>();
        if (Agent != null)
        {
            Agent.stoppingDistance = _data.stoppingDistance;
            Agent.acceleration = _data.acceleration;
        }
        else
        {
            Debug.LogError("No NavMeshAgent found on: " + this.name);
        }

        IdleStrategy = new GrazeBehaviorStrategy();
        PatrolStrategy = new GallopMovementStrategy();
        ChaseStrategy = new ChaseMovementStrategy();

        _states = new CreatureStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
    #endregion

    private void Update()
    {
        _currentState.UpdateStates();
    }

    #region Animation Transition
    /// <summary>
    /// Cambia gradualmente el parámetro "State", espera a que termine la animación y regresa.
    /// </summary>
    public void TriggerStateTransition(float targetValue, float transitionDuration, int paramHash, float cooldown, int timesToLoopAnimation = 1)
    {
        // 1. Validación de entrada y estado actual
        if (IsBusy || Time.time < _nextActionAllowedTime || _anim == null) return;

        // 2. Seguridad: Si por alguna razón hubiera una corriendo, la detenemos
        if (_stateTransitionCoroutine != null)
        {
            StopCoroutine(_stateTransitionCoroutine);
        }

        _stateTransitionCoroutine = StartCoroutine(StateRoutine(targetValue, transitionDuration, paramHash, cooldown, timesToLoopAnimation));
    }

    private IEnumerator StateRoutine(float targetValue, float transitionDuration, int paramHash, float cooldown, int timesToLoopAnimation)
    {
        IsBusy = true;

        try
        {
            // 1. Subir parámetro (Lerp inlined para evitar doble corrutina)
            yield return StartLerp(0, targetValue, transitionDuration, paramHash);

            // 2. Esperar la animación
            // Usamos null en lugar de WaitForEndOfFrame si no es estrictamente necesario (es más ligero)
            yield return null;

            float animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
            // Solo creamos el WaitForSeconds si el tiempo es mayor a 0
            if (animLength > 0)
            {
                yield return new WaitForSeconds(animLength * timesToLoopAnimation);
            }

            // 3. Bajar parámetro (Lerp inlined)
            yield return StartLerp(targetValue, 0, transitionDuration, paramHash);
        }
        finally
        {
            // 4. Limpieza final: Se ejecuta SIEMPRE, incluso si se detiene la corrutina
            _nextActionAllowedTime = Time.time + cooldown;
            IsBusy = false;
            _stateTransitionCoroutine = null;
        }
    }

    // Helper interno que no genera una nueva corrutina en el motor de Unity
    private IEnumerator StartLerp(float start, float end, float duration, int paramHash)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(start, end, elapsed / duration);
            _anim.SetFloat(paramHash, currentValue);
            yield return null; // Espera al siguiente frame
        }
        _anim.SetFloat(paramHash, end);
    }
    #endregion

    public Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _data.patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Usamos un radio de búsqueda pequeño (2m) para que el punto encontrado 
        // esté realmente cerca de nuestra dirección aleatoria.
        if (NavMesh.SamplePosition(randomDirection, out hit, 2.0f, NavMesh.AllAreas))
        {
            // Opcional: Alejar el punto un poco del borde manualmente
            // Pero con un stoppingDistance alto (>1) esto ya no suele ser necesario.
            return hit.position;
        }

        return transform.position;
    }

    public void OnPlayerInteracted(Transform playerTransform)
    {
        FollowTarget = playerTransform;
        // La transición la manejaremos en CheckSwitchStates de los estados
    }

    public void StopFollowing()
    {
        FollowTarget = null;
    }

    [Header("Debug Settings")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private Color _radiusColor = new Color(0, 1, 0, 0.2f); // Verde transparente
    [SerializeField] private Color _targetColor = Color.red;

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // 1. Dibujar el Radio de Patrulla (Esfera de alambre)
        // Se dibuja alrededor de la posición actual del animal
        Gizmos.color = _radiusColor;
        Gizmos.DrawWireSphere(transform.position, _data.patrolRadius);

        // 2. Dibujar el Punto de Destino
        if (Agent != null && Agent.hasPath)
        {
            // Dibujamos una línea desde el animal hasta su destino
            Gizmos.color = _targetColor;
            Gizmos.DrawLine(transform.position, Agent.destination);

            // Dibujamos una pequeña esfera sólida en el destino exacto
            Gizmos.DrawSphere(Agent.destination, 0.5f);
        }
    }
}

