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
        // 获取当前 NetworkManager 实例
        NetworkManager networkManager = NetworkManager.Singleton;

        // 检查 NetworkManager 是否可用且客户端已连接
        if (networkManager == null || !networkManager.IsListening)
        {
            Debug.LogWarning("Not accessable to player netobj");
            return null;
        }

        // 直接获取本地客户端的玩家对象
        NetworkObject tmp = networkManager.LocalClient.PlayerObject;
        //Debug.Log("get player: " + tmp?.name);
        return networkManager.LocalClient.PlayerObject;
    }

    public NetworkObject GetPlayerNetworkObject(ulong clientId)
    {
        // 获取当前 NetworkManager 实例（确保已初始化）
        NetworkManager networkManager = NetworkManager.Singleton;

        // 检查 NetworkManager 是否可用
        if (networkManager == null || !networkManager.IsListening)
        {
            return null;
        }

        // 通过 SpawnManager 获取客户端对应的玩家对象
        if (networkManager.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
        {
            return client.PlayerObject;
        }

        // 未找到对应客户端或玩家对象
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
