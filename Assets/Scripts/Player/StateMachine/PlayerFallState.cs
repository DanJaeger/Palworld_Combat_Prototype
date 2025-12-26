using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
       : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void CheckSwitchStates()
    {
        if (Context.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public void HandleGravity()
    {
        float previousYVelocity = Context.CurrentMovement.y;
        Context.CurrentMovement = new Vector3(Context.CurrentMovement.x, Context.CurrentMovement.y + Context.Gravity * Time.deltaTime, Context.CurrentMovement.z);
        Context.AppliedMovement.y = Mathf.Max((previousYVelocity + Context.CurrentMovement.y) * 0.5f, -20.0f);
    }
}