using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay2D(Collision2D other) {
        if(Input.GetKey ("w") && Mathf.Round(other.transform.position.y) < Mathf.Round(transform.position.y) ) transform.position += new Vector3(0,1);
        if(Input.GetKey ("s") && Mathf.Round(other.transform.position.y) > Mathf.Round(transform.position.y) ) transform.position += new Vector3(0,-1);
        if(Input.GetKey ("d") && Mathf.Floor(other.transform.position.x) < Mathf.Floor(transform.position.x) ) transform.position += new Vector3(1,0);
        if(Input.GetKey ("a") && Mathf.Floor(other.transform.position.x) > Mathf.Floor(transform.position.x) ) transform.position += new Vector3(-1,0);
    }
}
