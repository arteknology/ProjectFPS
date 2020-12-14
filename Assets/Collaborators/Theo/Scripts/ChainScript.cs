using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainScript : MonoBehaviour
{
    private LineRenderer _chain;
    public Transform pointe;
    
    // Start is called before the first frame update
    void Start()
    {
        _chain = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //_chain.SetPosition(0, transform.localPosition);
        _chain.SetPosition(1, pointe.localPosition);
        
    }
}
