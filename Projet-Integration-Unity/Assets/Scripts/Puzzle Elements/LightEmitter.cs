using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmitter : MonoBehaviour
{
    public LineRenderer linePrefab;
    public LineRenderer line;
    public Transform lightSource;
    public float distance;
    public LayerMask layerMask;
    public GameObject hitNew, hitSaved;
    public float nDensity;

    public float signedAngle;
    
    protected virtual void Start(){
        Physics2D.IgnoreLayerCollision(6, 7, true);
        Physics2D.IgnoreLayerCollision(6, 8, true);
    }
    public virtual Vector3 GetRotatedVector(float incomingAngle)
    {
        Quaternion rotation = Quaternion.AngleAxis(incomingAngle, Vector3.forward);
        Vector3 rotatedVector = rotation * transform.up;
        return rotatedVector;
    }
    public virtual Vector3 GetRayStartPos(float incomingAngle)
    {
        return transform.position + GetRotatedVector(incomingAngle) * 0.01f;
    }
    protected virtual void Update()
    {
        if(line) {
            line.GetComponent<LightLine>().nDensity = nDensity;

            float incomingAngle = Vector3.SignedAngle(transform.up, lightSource.position - transform.position, Vector3.forward);

            Ray2D ray = new Ray2D(GetRayStartPos(incomingAngle), GetRotatedVector(incomingAngle));
            RaycastHit2D hit;

            line.SetPosition(0, ray.origin);

            hit = Physics2D.Raycast(ray.origin, GetRotatedVector(incomingAngle), distance, layerMask);

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
                StartCoroutine(LightUp());
            }
        }else{
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
            hitNew = null;
            hitSaved = hitNew;
        }
    }

    IEnumerator LightUp()
    {
        yield return new WaitForSeconds(0.001f);
        try{
            if(hitSaved.GetComponent<Mirror>().line == null)
            {
                hitSaved.GetComponent<Mirror>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                hitSaved.GetComponent<Mirror>().nDensity = nDensity;
                hitSaved.GetComponent<Mirror>().line.GetComponent<LightLine>().waveLenght = line.GetComponent<LightLine>().waveLenght;
            }
            hitSaved.GetComponent<Mirror>().lightSource = transform;
        }catch{}
        try{
            if(hitSaved.GetComponent<Lens>().line == null)
            {
                hitSaved.GetComponent<Lens>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                hitSaved.GetComponent<Lens>().line.GetComponent<LightLine>().waveLenght = line.GetComponent<LightLine>().waveLenght;
            }
            hitSaved.GetComponent<Lens>().lightSource = transform;
        }catch{}
        try{
        hitSaved.GetComponent<LightReceiver>().hitByLight = true;
        hitSaved.GetComponent<LightReceiver>().waveLenghtReceived = line.GetComponent<LightLine>().waveLenghtInDensity;
        }catch{}
        try{
            if(hitSaved.transform.name == "light Collisions turret" && line.GetComponent<LightLine>().waveLenghtInDensity == 700)
            {
                hitSaved.transform.parent.gameObject.GetComponent<Turret>().Die();
            }
        }catch{}
    }
}
