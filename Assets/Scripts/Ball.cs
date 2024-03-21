using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    AudioSource audioData;
    
    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioData.Play(0);
    }
}
//ANDRES