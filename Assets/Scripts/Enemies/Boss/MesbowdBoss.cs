using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class MesbowdBoss : Enemy
{
    public HandControl leftHandControl;
    public HandControl rightHandControl;


    // false - right, true - left   
    private bool attackHand = false;

    [Header("Right Arm")]
    public Transform rightArm;
    public Transform rightForearm;
    public Transform rightHand;
    public Transform rightHandCenter;

    [Header("Left Arm")]
    public Transform leftArm;
    public Transform leftForearm;
    public Transform leftHand;
    public Transform leftHandCenter;

    public float handMoveSpeed = 1;
    public float minDistanceToTarget = 1;

    public float handMoveSpeedAttack = 1.5f;

    private Vector2 newLeftArmPosition;
    private Vector2 newRightArmPosition;

    private Vector2 currentLeftArmPosition;
    private Vector2 currentRightArmPosition;

    public float attackSpeed = 1;
    private float lastAttack;

    public float minRange = 0.4f;
    public float maxRange = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        HasKnockbackAnim = false;
        HasAttackAnim = false;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();

        UpdateHandTargetLocations();
        UpdateArmPositions();

        leftHandControl.SetAttackSpeed(handMoveSpeedAttack);
        leftHandControl.SetNormalSpeed(handMoveSpeed);
        leftHandControl.SetCooldown(attackSpeed);
        rightHandControl.SetAttackSpeed(handMoveSpeedAttack);
        rightHandControl.SetNormalSpeed(handMoveSpeed);
        rightHandControl.SetCooldown(attackSpeed);
    }

    private void UpdateHandTargetLocations()
    {
        if (!rightHandControl.IsAttacking)
        {
            if (Vector2.Distance(rightHandControl.CurrentPosition, rightHandControl.TargetPostion) < minDistanceToTarget)
            {
                print("new right hand location");
                newRightArmPosition = GetNewTarget(rightArm, GetRightArmRange(), 160, 270);
            }
        }
        if (!leftHandControl.IsAttacking)
        {
            if (Vector2.Distance(leftHandControl.CurrentPosition,leftHandControl.TargetPostion) < minDistanceToTarget)
            {
                print("new left hand location");
                newLeftArmPosition = GetNewTarget(leftArm, GetLeftArmRange(), 270, 360 + 50);
            }
        }
    }

    private void UpdateArmPositions()
    {
        MoveLeftArm(leftHandControl.CurrentPosition);
        MoveRightArm(rightHandControl.CurrentPosition);

    }

    private Vector2 GetNewTarget(Transform origin, float range, float angleMin, float angleMax)
    {
        range = Random.Range(minRange, maxRange) * range;
        float angle = Random.Range(Mathf.Deg2Rad * angleMin, Mathf.Deg2Rad * angleMax);
        float x = origin.position.x + range * MathF.Cos(angle);
        float y = origin.position.y + range * MathF.Sin(angle);
        return new Vector2(x, y);
    }

    private void Attack()
    {
        if(rightHandControl.CanAttack && leftHandControl.CanAttack)
        {
            int hand = Random.Range(0, 2);
            if (hand == 0)
                rightHandControl.Attack(player);
            else
                leftHandControl.Attack(player);
            

        }

        return;
        if (!rightHandControl.IsAttacking)
            rightHandControl.Attack(player);
        if (!leftHandControl.IsAttacking)
            leftHandControl.Attack(player);

    }
    private void MoveHandToPlayer()
    {
        MoveLeftArm(player.transform.position);
        MoveRightArm(player.transform.position);
    }

    private float GetLeftArmRange()
    {
        var a = Vector2.Distance(leftArm.position, leftForearm.position);
        var b = Vector2.Distance(leftForearm.position, leftHandCenter.position);
        return (a + b) * 0.95f;
    }
    private float GetRightArmRange()
    {
        var a = Vector2.Distance(rightArm.position, rightForearm.position);
        var b = Vector2.Distance(rightForearm.position, rightHandCenter.position);
        return (a + b) * 0.95f;
    }

    private void MoveRightArm(Vector2 position)
    {
        // Some math for calculating the angles between
        var distance = Vector2.Distance(position, rightArm.position);
        var a = Vector2.Distance(rightArm.position, rightForearm.position);
        var b = Vector2.Distance(rightForearm.position, rightHandCenter.position);

        distance = MathF.Min((a + b) * 0.95f, distance);

        var cosPhi = (Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(distance, 2)) / (2 * a * b);
        var sinPhi = Mathf.Sqrt(1 - Mathf.Pow(cosPhi, 2));

        var sinAlpha = b * sinPhi / distance;


        float alpha = MathF.Asin(sinAlpha);
        var theta = MathF.Asin(a * sinPhi / distance);

        float phi;

        // check if over 90 deg
        if (cosPhi > 0 && sinPhi > 0)
            phi = MathF.PI - MathF.Asin(sinPhi);
        else
            phi = MathF.Asin(sinPhi);


        Vector2 direction = new(position.x - rightArm.position.x, position.y - rightArm.position.y);
        float playerAngle = 0;

        // Check if over 90 deg
        if (direction.x > 0 && direction.y < 0)
            playerAngle = -MathF.PI + MathF.Asin(direction.y / new Vector2(direction.x, direction.y).magnitude);
        else
            playerAngle = -MathF.Asin(direction.y / new Vector2(direction.x, direction.y).magnitude);


        var playerAngleDeg = Mathf.Rad2Deg * playerAngle;
        var alphaDeg = Mathf.Rad2Deg * alpha;
        var phiDeg = Mathf.Rad2Deg * phi;

        var forearmRot = playerAngleDeg - alphaDeg;

        if (float.IsNaN(forearmRot) || float.IsNaN(phiDeg))
            return;
        rightArm.localRotation = Quaternion.Euler(rightArm.localRotation.x, rightArm.localRotation.y, forearmRot);
        rightForearm.localRotation = Quaternion.Euler(rightForearm.localRotation.x, rightForearm.localRotation.y, phiDeg);
    }

    private void MoveLeftArm(Vector2 position)
    {
        // Some math for calculating the angles between
        var distance = Vector2.Distance(position, leftArm.position);
        var a = Vector2.Distance(leftArm.position, leftForearm.position);
        var b = Vector2.Distance(leftForearm.position, leftHandCenter.position);

        distance = MathF.Min((a + b) * 0.95f, distance);

        var cosPhi = (Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(distance, 2)) / (2 * a * b);
        var sinPhi = Mathf.Sqrt(1 - Mathf.Pow(cosPhi, 2));

        var sinAlpha = b * sinPhi / distance;


        float alpha = MathF.Asin(sinAlpha);
        var theta = MathF.Asin(a * sinPhi / distance);

        float phi;

        // check if over 90 deg
        if (cosPhi > 0 && sinPhi > 0)
            phi = -MathF.PI + MathF.Asin(sinPhi);
        else
            phi = -MathF.Asin(sinPhi);


        var direction = new Vector2(position.x - leftArm.position.x, position.y - leftArm.position.y);
        float playerAngle = 0;

        // Check if over 90 deg
        if (direction.x < 0 && direction.y < 0)
            playerAngle = MathF.PI - MathF.Asin(direction.y / new Vector2(direction.x, direction.y).magnitude);
        else
            playerAngle = MathF.Asin(direction.y / new Vector2(direction.x, direction.y).magnitude);


        var playerAngleDeg = Mathf.Rad2Deg * playerAngle;
        var alphaDeg = Mathf.Rad2Deg * alpha;
        var phiDeg = Mathf.Rad2Deg * phi;

        var forearmRot = playerAngleDeg + alphaDeg;
        if (float.IsNaN(forearmRot) || float.IsNaN(phiDeg))
            return;
        leftArm.localRotation = Quaternion.Euler(leftArm.localRotation.x, leftArm.localRotation.y, forearmRot);
        leftForearm.localRotation = Quaternion.Euler(leftForearm.localRotation.x, leftForearm.localRotation.y, phiDeg);
    }
}
