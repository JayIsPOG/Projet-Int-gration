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
    public bool canUpdate;
    public float signedAngle;
    
    protected virtual void Start(){
        distance = 1000;
        canUpdate = true;
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
    public virtual void Update()
    {
        if(true)
        {
            if(line) {
                line.GetComponent<LightLine>().nDensity = nDensity;

                float incomingAngle = Vector3.SignedAngle(transform.up, lightSource.position - transform.position, Vector3.forward);
                signedAngle = incomingAngle;

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
                    //Debug.Log(hitSaved is LightEmitter); //this returns false, but it should be true right ?
                    DestroyChild();
                    StopCoroutine("LightUp");
                    StartCoroutine(LightUp());
                }
            }else{
                hitNew = null;
                DestroyChild();
            }
        }
    }

    void DestroyChild()
    {
        if(hitSaved)
        {
            if(hitSaved.GetComponent<Mirror>())
            {
                if(hitSaved.GetComponent<Mirror>().line)
                    Destroy(hitSaved.GetComponent<Mirror>().line.gameObject);
            }
            else if(hitSaved.GetComponent<Lens>())
                Destroy(hitSaved.GetComponent<Lens>().line.gameObject);
            else if(hitSaved.GetComponent<LightReceiver>())
                hitSaved.GetComponent<LightReceiver>().hitByLight = false;
            else if(hitSaved.GetComponent<LightReceiverCrystal>())
            {
                hitSaved.GetComponent<LightReceiverCrystal>().hitByLight = false;
                try{
                Destroy(hitSaved.GetComponent<LightReceiverCrystal>().line.gameObject);
                }catch{}
            }
        }
        hitSaved = hitNew;
    }

    IEnumerator LightUp()
    {
        yield return new WaitForSeconds(0.01f);
        //Debug.Log(hitSaved);
        if(hitSaved)
        {
            try{
                if(hitSaved.GetComponent<Mirror>()){
                    if(hitSaved.GetComponent<Mirror>().line == null)
                    {
                        hitSaved.GetComponent<Mirror>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                        hitSaved.GetComponent<Mirror>().nDensity = nDensity;
                        hitSaved.GetComponent<Mirror>().line.GetComponent<LightLine>().waveLenght = line.GetComponent<LightLine>().waveLenght;
                    }
                    hitSaved.GetComponent<Mirror>().lightSource = transform;
                }
                if(hitSaved.GetComponent<Lens>()){
                    if(hitSaved.GetComponent<Lens>().line == null)
                    {
                        hitSaved.GetComponent<Lens>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                        hitSaved.GetComponent<Lens>().line.GetComponent<LightLine>().waveLenght = line.GetComponent<LightLine>().waveLenght;
                    }
                    hitSaved.GetComponent<Lens>().lightSource = transform;
                }
                if(hitSaved.GetComponent<LightReceiver>()){
                    hitSaved.GetComponent<LightReceiver>().hitByLight = true;
                    hitSaved.GetComponent<LightReceiver>().waveLenghtReceived = line.GetComponent<LightLine>().waveLenghtInDensity;
                }
                if(hitSaved.GetComponent<LightReceiverCrystal>()){
                    if(hitSaved.GetComponent<LightReceiverCrystal>().line == null)
                    {
                        hitSaved.GetComponent<LightReceiverCrystal>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                        hitSaved.GetComponent<LightReceiverCrystal>().nDensity = nDensity;
                        hitSaved.GetComponent<LightReceiverCrystal>().line.GetComponent<LightLine>().waveLenght = line.GetComponent<LightLine>().waveLenght;
                        hitSaved.GetComponent<LightReceiverCrystal>().hitByLight = true;
                        hitSaved.GetComponent<LightReceiverCrystal>().waveLenghtReceived = line.GetComponent<LightLine>().waveLenghtInDensity;
                    }
                    hitSaved.GetComponent<LightReceiverCrystal>().lightSource = transform;
                }
                if(hitSaved.transform.name == "light Collisions turret"){
                    float w = line.GetComponent<LightLine>().waveLenghtInDensity;
                    if(700 <= w + 30 && 700 >= w - 30)
                    {
                        hitSaved.transform.parent.gameObject.GetComponent<Turret>().StartCoroutine("Die");
                    }
                }
            }catch{}
        }
    }
}
