using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartNetwork : MonoBehaviour
{
    private NetworkManager networkManager;
    private UnityTransport unityTransport;

    [SerializeField] private TMP_InputField serverIP;
    [SerializeField] private TMP_InputField port;

    [SerializeField] private Dropdown localIPs;


    private void Awake()
    {
        networkManager = NetworkManager.Singleton;
        // 获取UnityTransport组件
        unityTransport = networkManager.GetComponent<UnityTransport>();
    }

    public void StartHost()
    {
        SetServer();
        NetworkManager.Singleton.StartHost();
    }
    public void StartServer()
    {
        SetServer();
        NetworkManager.Singleton.StartServer();
    }
    public void StartClient()
    {
        SetServer();
        NetworkManager.Singleton.StartClient();
    }

    public void ShutDown()
    {
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }

    private void SetServer()
    {
        string ip = "127.0.0.1";
        ushort _port = (ushort)7777;
        if (serverIP.text.Length != 0)
            ip = serverIP.text;
        if (port.text.Length != 0)
            _port = ToUshort(port.text);
        unityTransport.SetConnectionData(ip, _port);
        DebugIP.instance.ShowServerSettings();
    }

    private ushort ToUshort(string text)
    {
        ushort res = 0;
        for (int i = 0; i < text.Length; i++)
        {
            res *= 10;
            res += (ushort)(text[i] - (char)'0');
        }
        return res;
    }

}
