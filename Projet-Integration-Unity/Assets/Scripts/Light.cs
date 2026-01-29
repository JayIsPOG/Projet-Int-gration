using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public LineRenderer linePrefab;
    private LineRenderer line;
    public float distance;
    public LayerMask layerMask;
    public Transform lightPoint;

    private GameObject hitNew, hitSaved;
    void Start()
    {
        line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
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
            hitSaved = hitNew;
            try{
                if(hitSaved.GetComponent<Mirror>().line == null)
                {
                    hitSaved.GetComponent<Mirror>().line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                }
                hitSaved.GetComponent<Mirror>().lightSource = transform;
            }catch{}
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
}
