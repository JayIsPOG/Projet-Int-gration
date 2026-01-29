using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public LineRenderer line;
    public Transform lightSource;
    public float distance;
    public LayerMask layerMask;

    public float signedAngle;
    
    // Update is called once per frame
    void Update()
    {
        if(line) {
            signedAngle = Vector3.SignedAngle(transform.up, lightSource.position - transform.position, Vector3.forward);

            Quaternion rotation = Quaternion.AngleAxis(-signedAngle, Vector3.forward);
            Vector3 rotatedVector = rotation * transform.up;

            Ray2D ray = new Ray2D(transform.position + rotatedVector * 0.01f, rotatedVector);
            RaycastHit2D hit;

            line.SetPosition(0, ray.origin);

            hit = Physics2D.Raycast(ray.origin, rotatedVector, distance, layerMask);

            if (hit.collider)
            {
                line.SetPosition(1, hit.point);
            }
            else
                line.SetPosition(1, ray.GetPoint(distance));
        }
    }
}
