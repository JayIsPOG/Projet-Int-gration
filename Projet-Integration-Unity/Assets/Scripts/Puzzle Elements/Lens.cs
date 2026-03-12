using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lens : LightEmitter
{
    public override Vector3 GetRotatedVector(float incomingAngle)
    {
        float outgoingAngle = Mathf.Asin((line.shadowBias * Mathf.Sin(incomingAngle * Mathf.Deg2Rad))/nDensity) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(outgoingAngle + 180f, Vector3.forward);
        Vector3 rotatedVector = rotation * transform.up;
        return rotatedVector;
    }
    public override Vector3 GetRayStartPos(float incomingAngle)
    {
        return transform.position + GetRotatedVector(incomingAngle) * 0.4f;
    }
}