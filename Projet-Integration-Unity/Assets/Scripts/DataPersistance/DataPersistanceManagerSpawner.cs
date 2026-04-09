using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistanceManagerSpawner : MonoBehaviour
{
    [SerializeField] public GameObject Manager;
    void Start()
    {
        if(FindObjectOfType<DataPersistanceManager>() == null)
        {
            Instantiate(Manager, gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
