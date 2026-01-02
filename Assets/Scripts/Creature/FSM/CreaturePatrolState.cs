using UnityEngine;

public class CreaturePatrolState : CreatureBaseState
{
    public CreaturePatrolState(CreatureStateMachine currentContext, CreatureStateFactory creatureStateFactory)
        : base(currentContext, creatureStateFactory) { }

    #region State Lifecycle
    public override void EnterState()
    {
        Context.PatrolStrategy.StartMovement(Context);
    }

    public override void UpdateState()
    {
        Context.PatrolStrategy.Move(Context);
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Limpiamos la ruta al salir para que no queden remanentes
        Context.Agent.updateRotation = true;

        if (Context.Agent.isOnNavMesh) Context.Agent.ResetPath();
        Context.Anim.SetFloat(CreatureStateMachine.VertHash, 0);
    }
    #endregion

    #region Logic
    public override void CheckSwitchStates()
    {

        // 1. Si aún está calculando la ruta, no hacemos nada.
        if (Context.Agent.pathPending) return;

        // 2. ¿Hemos llegado o el agente ya no tiene a dónde ir?
        bool hasReachedDestination = Context.Agent.remainingDistance <= Context.Agent.stoppingDistance;
        bool hasLostPath = !Context.Agent.hasPath;

        if (hasReachedDestination || hasLostPath)
        {
            // 3. Verificamos que realmente la velocidad sea baja para evitar saltos bruscos
            if (Context.Agent.velocity.sqrMagnitude < 0.1f)
            {
                Debug.Log("Destino alcanzado con éxito. Cambiando a Idle.");
                SwitchState(Factory.Idle());
                return;
            }
        }
        
        if (Context.FollowTarget != null)
        {
            SwitchState(Factory.Chase());
            return;
        }
    }
    #endregion
}