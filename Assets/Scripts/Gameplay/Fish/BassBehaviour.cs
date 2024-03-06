using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BassBehaviour : MonoBehaviour
{
    private Vector2 direction;

    private float currentRotationDeg;

    private float prevRotationDeg;
    private float targetRotationDeg;

    [SerializeField, Range(0f, 720f)] private float maxAngleRange = 360f;
    [SerializeField, Range(0f, 360f)] private float angleOffset = 0f;

    [Space]

    [SerializeField, Range(0f, 10f)] private float maxHoldDuration = 1f;
    [SerializeField, Range(0f, 10f)] private float minHoldDuration = 0.2f;

    private float targetHoldDuration;
    private float currentHoldDuration;

    void Update()
    {
        currentHoldDuration += Time.deltaTime;
        if (currentHoldDuration >= targetHoldDuration)
        {
            GetNewRotation();
        }

        LerpRotation();
        FaceRotation(currentRotationDeg + angleOffset);
    }

    private void FaceRotation(float angleDeg)
    {
        direction = GeneralUtils.DegreesToVector(angleDeg);

        Debug.DrawRay(transform.position, direction.OnZAxis());
    }

    private void GetNewRotation()
    {
        prevRotationDeg = currentRotationDeg;

        targetRotationDeg = Random.value * maxAngleRange;
        targetHoldDuration = Random.Range(minHoldDuration, maxHoldDuration);
        currentHoldDuration = 0f;
    }

    private void LerpRotation()
    {
        float t = currentHoldDuration / targetHoldDuration;

        currentRotationDeg = Mathf.Lerp(prevRotationDeg, targetRotationDeg, t);
    }
}
