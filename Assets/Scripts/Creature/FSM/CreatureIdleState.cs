using UnityEngine;

public class CreatureIdleState : CreatureBaseState
{
    private float _timer;
    public CreatureIdleState(CreatureStateMachine currentContext, CreatureStateFactory creatureStateFactory)
        : base(currentContext, creatureStateFactory) { }

    #region State Lifecycle
    public override void EnterState()
    {
        _timer = 0; // Reiniciar el reloj al entrar
        Debug.Log("Entrando en Idle. Esperando 1 minuto...");
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;
        if (Context.IdleStrategy != null)
        {
            Context.IdleStrategy.Idling(Context);
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }
    #endregion

    #region Logic
    public override void CheckSwitchStates()
    {
        if (_timer >= Context.Data.idleDuration && !Context.IsBusy)
        {
            SwitchState(Factory.Patrol());
            return;
        }

        if (Context.FollowTarget != null)
        {
            SwitchState(Factory.Chase());
            return;
        }
    }
    #endregion
}