using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_Gameover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameoverText;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        PlayerController.GameOverEvent += gameover;
    }

    private void OnDisable()
    {
        PlayerController.GameOverEvent -= gameover;
    }

    private void gameover()
    {
        canvas.enabled = true;
        gameoverText.text = "Game Over";
    }

    public void Reborn()
    {
        Player_Manager.instance.player.GetComponent<PlayerGameOverCheck>().Reborn();
    }
}
