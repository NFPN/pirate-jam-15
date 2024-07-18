using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerState
{
    private InputAction moveAction;
    private float dashStartTime;

    private Vector2 dashDirection;

    public PlayerDashState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        moveAction = player.input.actions["Move"];
    }

    public override void EnterState()
    {
        dashDirection = moveAction.ReadValue<Vector2>().normalized;
        if (dashDirection.magnitude == 0)
        {
            player.StateMachine.ChangeState(player.MoveState);
            return;
        }
        dashStartTime = Time.time;
        player.animator.SetFloat("directionX", 10);
        player.animator.SetFloat("directionY", 0);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        var position = player.transform.position;
        position += player.dashSpeed * Time.deltaTime * dashDirection.ToVector3();
        player.transform.position = position;
        if (dashStartTime + player.dashDuration < Time.time)
            player.StateMachine.ChangeState(player.MoveState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
