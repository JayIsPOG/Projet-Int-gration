using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lens : MonoBehaviour
{
    public LineRenderer linePrefab;
    public LineRenderer line;
    public Transform lightSource;
    public float distance;
    public LayerMask layerMask;
    public GameObject hitNew, hitSaved;

    [Range(1, 1.84f)]
    public float nDensity = 1;

    public float incomingAngle;
    
    void Start(){
        Physics2D.IgnoreLayerCollision(6, 7, true);
        Physics2D.IgnoreLayerCollision(6, 8, true);
    }
    void Update()
    {
        if(line) {
            line.GetComponent<LightLine>().nDensity = nDensity;

            incomingAngle = Vector3.SignedAngle(transform.up, lightSource.position - transform.position, Vector3.forward);

            float outgoingAngle = Mathf.Asin((line.shadowBias * Mathf.Sin(incomingAngle * Mathf.Deg2Rad))/nDensity) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(outgoingAngle + 180f, Vector3.forward);
            Vector3 rotatedVector = rotation * transform.up;

            Ray2D ray = new Ray2D(transform.position + rotatedVector * 0.01f, rotatedVector);
            RaycastHit2D hit;

            line.SetPosition(0, ray.origin);

            hit = Physics2D.Raycast(ray.origin, rotatedVector, distance, layerMask);

            if (hit.collider)
            {
                line.SetPosition(1, hit.point);
                hitNew = hit.collider.gameObject;
            }
            else
                line.SetPosition(1, ray.GetPoint(distance));

            if(hitNew != hitSaved)
            {
                try{
                    Destroy(hitSaved.GetComponent<Mirror>().line.gameObject);
                    //hitSaved.GetComponent<Mirror>().lightSource = null;
                }catch{}
                try{
                    Destroy(hitSaved.GetComponent<Lens>().line.gameObject);
                }catch{}
                try{
                hitSaved.GetComponent<LightReceiver>().hitByLight = false;
                }catch{}
                hitSaved = hitNew;
                try{
                    if(hitSaved.GetComponent<Mirror>().line == null)
                    {
                        hitSaved.GetComponent<Mirror>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                    }
                    hitSaved.GetComponent<Mirror>().lightSource = transform;
                }catch{}
                try{
                    if(hitSaved.GetComponent<Lens>().line == null)
                    {
                        hitSaved.GetComponent<Lens>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                        hitSaved.GetComponent<Lens>().line.GetComponent<LightLine>().nDensity = nDensity;
                    }
                    hitSaved.GetComponent<Lens>().lightSource = transform;
                }catch{}
                try{
                hitSaved.GetComponent<LightReceiver>().hitByLight = true;
                }catch{}
            }
        }else{
            try{
                Destroy(hitSaved.GetComponent<Mirror>().line.gameObject);
                //hitSaved.GetComponent<Mirror>().lightSource = null;
            }catch{}
            try{
            hitSaved.GetComponent<LightReceiver>().hitByLight = false;
            }catch{}
            hitNew = null;
            hitSaved = hitNew;
        }
    }
}
