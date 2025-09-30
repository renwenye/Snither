using System.Net;
using System.Text;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Collections.Generic;

public class DebugIP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private StringBuilder sb;

    public static DebugIP instance;

    private void Awake()
    {
        sb = new StringBuilder();

        instance = this;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        //ShowServerSettings();
        sb.Clear();
        text.text = "";
    }

    //private void OnApplicationQuit()
    //{
    //    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    //}

    public void ShowServerSettings()
    {
        //sb.Clear();
        List<string> localIPs = GetLocalIPAddress();
        foreach (string ip in localIPs)
        {
            string localIP = "Local IP: " + ip;
            sb.AppendLine(localIP);
        }
        string HostIP = "Host IP: " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
        string Port = "Host Port: " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port.ToString();
        sb.AppendLine(HostIP);
        sb.AppendLine(Port);
        text.text = sb.ToString();
    }

    void Update()
    {
        text.text = sb.ToString();
        // Optionally display this on the UI
    }

    private void OnClientConnected(ulong clientId)
    {
        // ���ͻ�������ʱ����
        if (NetworkManager.Singleton.IsServer)
        {
            // ��������¼�пͻ�������
            sb.AppendLine($"Client connected with ID: {clientId}");
            // ������Լ������������ӣ�clientIdΪ0��ע�⣺����Ҳ�ǿͻ��ˣ�������ʱ�����������clientId
        }
        else
        {
            // �ͻ��˼�¼�Լ����ӳɹ�
            sb.AppendLine("Connected to server successfully.");
        }
    }

    void OnClientDisconnectCallback(ulong clientId)
    {
        sb.AppendLine("Host Disconnected: " + clientId);

        if (clientId == NetworkManager.ServerClientId)
        {
            sb.AppendLine("Host has disconnected!");
        }
    }

    List<string> GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        List<string> allIps = new List<string>();
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                allIps.Add(ip.ToString());

            }
        }
        return allIps;
    }
}