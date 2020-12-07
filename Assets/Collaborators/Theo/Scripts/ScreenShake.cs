using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    static Transform cam;
    static float shakeAmount;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>().transform;
        shakeAmount = 0;
    }


    void Update()
    {
        if (shakeAmount>0)
        {
            cam.localPosition = Random.insideUnitSphere * shakeAmount;
            shakeAmount -= shakeAmount/100f + Time.deltaTime;
            if (shakeAmount<0) shakeAmount = 0;
        }
        else
        {
            cam.localPosition = Vector3.zero;
        }
    }


    public static void Shake(float amount)
    {
        amount /= 10f;
        if (shakeAmount < amount)
        shakeAmount += amount;
    }
}
