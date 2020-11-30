using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapScripts : MonoBehaviour
{
    public GameObject spikes;
    public Transform spikeSpawner;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ON BALANCE LES PICS");
        Instantiate(spikes, spikeSpawner.position, spikeSpawner.rotation);
    }
}
