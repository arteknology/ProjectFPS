using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScieCirculaireRotation : MonoBehaviour
{
    public bool isRightToLeft;
    private float _rightToLeftSpeed = 500f;
    private float _leftToRightSpeed = -500f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRightToLeft)
        {
            transform.Rotate(Vector3.up * _rightToLeftSpeed * Time.deltaTime);
        }
        
        if (!isRightToLeft)
        {
            transform.Rotate(Vector3.up * _leftToRightSpeed * Time.deltaTime);
        }
    }
}
