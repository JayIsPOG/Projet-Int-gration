using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public GameObject signal;
    private bool lightOn;
    public LineRenderer linePrefab;
    private LineRenderer line;
    public float distance;
    public LayerMask layerMask;
    public Transform lightPoint;
    public float waveLenght = 700;
    public float nDensity = 1;

    private GameObject hitNew, hitSaved;
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
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
            //try{
            //    if(hitSaved.GetComponent<Mirror>().line == null)
            //    {
            //        hitSaved.GetComponent<Mirror>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
            //        hitSaved.GetComponent<Mirror>().nDensity = nDensity;
            //        hitSaved.GetComponent<Mirror>().line.GetComponent<LightLine>().waveLenght = waveLenght;
            //    }
            //    hitSaved.GetComponent<Mirror>().lightSource = transform;
            //}catch{}
            //try{if(hitSaved.GetComponent<Lens>().line == null)
            //    {
            //        hitSaved.GetComponent<Lens>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
            //        hitSaved.GetComponent<Lens>().line.GetComponent<LightLine>().waveLenght = waveLenght;
            //    }
            //    hitSaved.GetComponent<Lens>().lightSource = transform;
            //}catch{}
            //try{
            //    hitSaved.GetComponent<LightReceiver>().hitByLight = true;
            //    hitSaved.GetComponent<LightReceiver>().waveLenghtReceived = line.GetComponent<LightLine>().waveLenghtInDensity;
            //}catch{}
        }

        if(signal)
        {
            lightOn = signal.GetComponent<PressurePlate>().pressed;
        }else{
            lightOn = true;
            if(!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();
        }

        if(lightOn)
        {
            if(line == null)
            {
                GetComponent<AudioSource>().Play();
                line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                line.GetComponent<LightLine>().waveLenght = waveLenght;
            }


            Ray2D ray = new Ray2D(lightPoint.position, -transform.up);
            RaycastHit2D hit;

            line.SetPosition(0, ray.origin);

            hit = Physics2D.Raycast(ray.origin, -transform.up, distance, layerMask);

            if (hit.collider)
            {
                line.SetPosition(1, hit.point);
                hitNew = hit.collider.gameObject;
            }
            else
                line.SetPosition(1, ray.GetPoint(distance));
        }
        else
        {
            if(line != null){
                Destroy(line.gameObject);
                GetComponent<AudioSource>().Stop();
            }
            hitNew = null;
        }
    }

    IEnumerator LightUp()
    {
        yield return new WaitForSeconds(0.1f);
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
            if(hitSaved.GetComponent<LightReceiverCrystal>().line == null)
            {
                hitSaved.GetComponent<LightReceiverCrystal>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                hitSaved.GetComponent<LightReceiverCrystal>().nDensity = nDensity;
                hitSaved.GetComponent<LightReceiverCrystal>().line.GetComponent<LightLine>().waveLenght = line.GetComponent<LightLine>().waveLenght;
                hitSaved.GetComponent<LightReceiverCrystal>().hitByLight = true;
                hitSaved.GetComponent<LightReceiverCrystal>().waveLenghtReceived = line.GetComponent<LightLine>().waveLenghtInDensity;
            }
            hitSaved.GetComponent<LightReceiverCrystal>().lightSource = transform;
        }catch{}
        try{
            if(hitSaved.transform.name == "light Collisions turret" && line.GetComponent<LightLine>().waveLenghtInDensity == 700)
            {
                hitSaved.transform.parent.gameObject.GetComponent<Turret>().Die();
            }
        }catch{}
    }
}
