using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpState : PlayerState
{
    private InputAction moveAction;
    private const float gConstant = -9.81f;

    private float jumpStartTime;
    private Vector3 jumpStartPosition;

    public PlayerJumpState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        moveAction = player.input.actions["Move"];
    }

    public override void EnterState()
    {
        jumpStartTime = Time.time;
        jumpStartPosition = player.transform.position;

        player.animator.SetFloat("directionY", 10);
    }

    public override void ExitState()
    {
        player.animator.SetFloat("directionX", 0);
        player.animator.SetFloat("directionY", 0);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void FrameUpdate()
    {
        // Jump Math
        var t = Time.time - jumpStartTime;
        float y = player.jumpForce * t + (gConstant * Mathf.Pow(t, 2)) / 2.0f;
        if (y < 0)
        {
            player.transform.position = jumpStartPosition;
            player.StateMachine.ChangeState(player.MoveState);
            return;
        }

        // Check if player is trying to move left or right
        var direction = moveAction.ReadValue<Vector2>().normalized;

        jumpStartPosition += player.moveSpeed * Time.deltaTime * direction.ToVector3();

        player.transform.position = jumpStartPosition + Vector3.up * y;

    }

    // Jump physics math y = vi*t + g*t^2/2
}
