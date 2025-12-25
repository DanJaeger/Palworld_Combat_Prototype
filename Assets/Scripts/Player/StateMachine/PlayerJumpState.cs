using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerJumpState : PlayerBaseState, IRootState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
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
        HandleJump();
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
    void HandleJump()
    {
        Context.IsJumping = true;
        Context.CurrentMovement = new Vector3(Context.CurrentMovement.x, Context.InitialJumpVelocity, Context.CurrentMovement.z);
        Context.AppliedMovement.y = Context.InitialJumpVelocity;
        Context.Anim.SetTrigger(PlayerStateMachine.IsJumpingHash);
    }
    public void HandleGravity()
    {
        bool isFalling;
        float fallMultiplier = 2.0f;
        if (Context.HoldJump)
            isFalling = Context.CurrentMovement.y <= 0 || !Context.IsJumpPressed;
        else
            isFalling = Context.CurrentMovement.y <= 0;

        if (isFalling)
        {
            float previousYVelocity = Context.CurrentMovement.y;
            Context.CurrentMovement = new Vector3(Context.CurrentMovement.x, Context.CurrentMovement.y + (Context.Gravity * fallMultiplier * Time.deltaTime), Context.CurrentMovement.z);
            Context.AppliedMovement.y = Mathf.Max((previousYVelocity + Context.CurrentMovement.y) * 0.5f, -10.0f);
        }
        else
        {
            float previousYVelocity = Context.CurrentMovement.y;
            Context.CurrentMovement = new Vector3(Context.CurrentMovement.x, Context.CurrentMovement.y + (Context.Gravity * Time.deltaTime), Context.CurrentMovement.z);
            Context.AppliedMovement.y = (previousYVelocity + Context.CurrentMovement.y) * 0.5f;
        }
    }
}