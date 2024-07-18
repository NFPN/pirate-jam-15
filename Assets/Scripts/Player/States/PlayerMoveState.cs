using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerState
{
    private readonly InputAction moveAction;

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
        player.rigidbody2D.velocity = player.moveSpeed * Time.fixedDeltaTime * player.directionVector.normalized;
    }

    public override void FrameUpdate()
    {
        var direction = moveAction.ReadValue<Vector2>().normalized;

        player.animator.SetFloat("directionX", direction.x);
        player.animator.SetFloat("directionY", direction.y);

        if (direction.x != 0)
            player.UpdatePlayerDirection(direction.x < 0 ? Utils.Direction.Left : Utils.Direction.Right);
    }
}
