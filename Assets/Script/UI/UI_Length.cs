using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_Length : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    private void OnEnable()
    {
        //Debug.Log("register text event");
        PlayerLength.OnChangeLength += UpdateText;
    }

    private void OnDisable()
    {
        //Debug.Log("unregister text event");
        PlayerLength.OnChangeLength -= UpdateText;
    }

    private void UpdateText(ushort value)
    {
        //Debug.Log("update text");
        text.text = "Length: " + value.ToString();
    }
}
