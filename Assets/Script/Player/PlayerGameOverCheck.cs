using System;
using System.Collections;
using System.Collections.Generic;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGameOverCheck : NetworkBehaviour
{
    private NetworkObject player;
    private PlayerController playerController;
    public NetworkVariable<bool> isGameOver = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        player = NetworkObject;
        playerController = GetComponent<PlayerController>();
        if (IsOwner)
        {
            PlayerController.GameOverEvent += GameOver;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
            PlayerController.GameOverEvent -= GameOver;

    }

    public void GameOver()
    {
        isGameOver.Value = true;
    }

    public void Reborn()
    {
        if (IsOwner)
        {
            isGameOver.Value = false;
            //SetPlayerActiveServerRpc(Player_Manager.instance.player.OwnerClientId);
        }
    }

    [ServerRpc]
    private void SetPlayerActiveServerRpc(ulong ClientId)
    {
        //SetPlayerActiveClientRpc(ClientId);
        NetworkObject player = Player_Manager.instance.GetPlayerNetworkObject(ClientId);
        SetPlayerActiveClientRpc(ClientId);
    }

    [ClientRpc]
    private void SetPlayerActiveClientRpc(ulong clientId)
    {
        //player.GetComponent<PlayerController>().SetVisible(true);
    }
}
