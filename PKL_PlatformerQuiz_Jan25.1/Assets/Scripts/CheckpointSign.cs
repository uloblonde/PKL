using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSign : MonoBehaviour
{
    private CheckpointMaster cm;

    void Start()
    {
        cm = GameObject.FindGameObjectWithTag("CM").GetComponent<CheckpointMaster>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cm.lastCheckpointPos = transform.position;
        }
    }
}
