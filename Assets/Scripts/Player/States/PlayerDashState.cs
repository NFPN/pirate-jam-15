using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTime;

    public PlayerDashState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void EnterState()
    {
        dashTime = 0;

        //lastDirection = player.directionVector;

        player.animator.SetFloat("directionX", 10);
        player.animator.SetFloat("directionY", 0);
    }

    public override void ExitState()
    {
        base.ExitState();
        player.rigidbody2D.velocity = Vector2.zero;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (dashTime > player.dashDuration)
            player.StateMachine.ChangeState(player.MoveState);

        dashTime += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.rigidbody2D.velocity = player.dashSpeed * player.lastDirectionVector.normalized;
    }
}
