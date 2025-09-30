using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player_Manager : NetworkBehaviour
{
    public static Player_Manager instance;
    public NetworkObject player;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            NetworkObject.Despawn();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //player = GetLocalPlayerNetworkObject();
        StartCoroutine(AssignPlayer());
    }

    IEnumerator AssignPlayer()
    {
        yield return new WaitForSeconds(.3f);
        player = GetLocalPlayerNetworkObject();
    }

    public NetworkObject GetLocalPlayerNetworkObject()
    {
        // ��ȡ��ǰ NetworkManager ʵ��
        NetworkManager networkManager = NetworkManager.Singleton;

        // ��� NetworkManager �Ƿ�����ҿͻ���������
        if (networkManager == null || !networkManager.IsListening)
        {
            Debug.LogWarning("Not accessable to player netobj");
            return null;
        }

        // ֱ�ӻ�ȡ���ؿͻ��˵���Ҷ���
        NetworkObject tmp = networkManager.LocalClient.PlayerObject;
        //Debug.Log("get player: " + tmp?.name);
        return networkManager.LocalClient.PlayerObject;
    }

    public NetworkObject GetPlayerNetworkObject(ulong clientId)
    {
        // ��ȡ��ǰ NetworkManager ʵ����ȷ���ѳ�ʼ����
        NetworkManager networkManager = NetworkManager.Singleton;

        // ��� NetworkManager �Ƿ����
        if (networkManager == null || !networkManager.IsListening)
        {
            return null;
        }

        // ͨ�� SpawnManager ��ȡ�ͻ��˶�Ӧ����Ҷ���
        if (networkManager.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
        {
            return client.PlayerObject;
        }

        // δ�ҵ���Ӧ�ͻ��˻���Ҷ���
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetPlayerServerRpc()
    {
        Debug.Log(NetworkManager.Singleton.ConnectedClients.Count);
        if (player == null && NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out NetworkClient _player))
        {
            Debug.Log("get ");
            player = _player.PlayerObject;
        }
    }
}
