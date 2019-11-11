using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBeat : MonoBehaviour
{
    public bool invokeBeat = false;
    public float invokeDelay = 1.0f;
    public string invokeMessage = "InvokeBeat";
    
    public bool updateBeat = true;
    private float updateCount = 0.0f;
    public float updateDelay = 1.0f;
    public string updateMessage = "UpdateBeat";
    
    // TODO: Coroutine Beat
    
    void Awake()
    {
        if (invokeBeat)
        {
            InvokeRepeating("Print", 0.000001f, invokeDelay);
        }
    }
    
    void Update()
    {
        if (updateBeat)
        {
            updateCount += Time.deltaTime;
            if (updateCount >= updateDelay)
            {
                updateCount = 0.0f;
                Debug.Log(updateMessage);
            }
        }
    }
    
    void Print()
    {
        Debug.Log(invokeMessage);
    }
}