using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if (!Context.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Context.IsMovementPressed && !Context.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void EnterState()
    {
        Context.Anim.SetBool(PlayerStateMachine.IsRunningHash, true);
        Context.CurrentSpeed = Context.RunSpeed;
    }

    public override void ExitState()
    {
        Context.Anim.SetBool(PlayerStateMachine.IsRunningHash, false);
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}