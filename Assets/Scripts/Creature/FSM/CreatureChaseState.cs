using UnityEngine;

public class CreatureChaseState : CreatureBaseState
{
    public CreatureChaseState(CreatureStateMachine currentContext, CreatureStateFactory creatureStateFactory)
        : base(currentContext, creatureStateFactory) { }

    #region State Lifecycle
    public override void EnterState()
    {
        Debug.Log("Iniciando seguimiento del jugador.");
        // Cambiamos a la estrategia de seguimiento
        Context.ChaseStrategy.StartMovement(Context);
    }

    public override void UpdateState()
    {
        Context.ChaseStrategy.Move(Context);
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Context.Agent.ResetPath();
    }
    #endregion

    #region Logic
    public override void CheckSwitchStates()
    {
        // Si el objetivo desaparece, volvemos a Idle
        if (Context.FollowTarget == null)
        {
            SwitchState(Factory.Idle());
        }
    }
    #endregion
}