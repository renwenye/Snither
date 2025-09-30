using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatAudio : MonoBehaviour
{
    private AudioSource eat;



    private void OnEnable()
    {
        eat = GetComponent<AudioSource>();
        PlayerLength.OnChangeLength += EatSound;
    }

    private void OnDisable()
    {
        PlayerLength.OnChangeLength -= EatSound;
    }

    private void EatSound(ushort obj)
    {
        eat.Play();
    }
}
