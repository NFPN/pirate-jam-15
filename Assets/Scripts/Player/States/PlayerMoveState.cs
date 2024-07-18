using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMoveState : PlayerState
{
    private InputAction moveAction;

    public PlayerMoveState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        moveAction = player.input.actions["Move"];
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void FrameUpdate()
    {
        var direction = moveAction.ReadValue<Vector2>().normalized;
        player.transform.position = player.transform.position + Utils.GetVec3(direction) * player.moveSpeed * Time.deltaTime;

        player.animator.SetFloat("directionX", direction.x);
        player.animator.SetFloat("directionY", direction.y);
    }
}
