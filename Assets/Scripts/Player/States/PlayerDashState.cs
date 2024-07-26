using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTime;

    private float lastDashTime;

    public PlayerDashState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void EnterState()
    {
        if(lastDashTime + player.CurrentDashLevel.cooldown >Time.time)
        {
            player.StateMachine.ChangeState(player.MoveState);
            return;
        }
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

        if (dashTime > player.CurrentDashLevel.dashDuration)
            player.StateMachine.ChangeState(player.MoveState);

        dashTime += Time.fixedDeltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (!player.isKnockback)
            player.rigidbody2D.velocity = player.CurrentDashLevel.dashSpeed * player.lastDirectionVector.normalized;

    }
}
