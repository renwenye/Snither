using System.Collections;
using System.Collections.Generic;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    //private NetworkVariable<bool> eaten = new NetworkVariable<bool>(false, 
    //    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //private bool eaten;

    private void Start()
    {
        //eaten = false;
    }
    public GameObject prefab { get; set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false) return;
        if (IsServer == false) return;
        if (collision.TryGetComponent<PlayerLength>(out  var playerLength))
        {
            playerLength.AddLength();
        }
        //if (IsOwner)
        //NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
        NetworkObject.Despawn();
    }
}
