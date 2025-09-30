using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    public NetworkVariable<ushort> length = new NetworkVariable<ushort>(1, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public static event Action<ushort> OnChangeLength;

    [SerializeField] private GameObject tailPrefab;
    private List<Tail> tails = new List<Tail>();

    private Collider2D snakeHeadCollider;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        snakeHeadCollider = GetComponent<Collider2D>();
        if (!IsServer)
            length.OnValueChanged += LengthChange;

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        length.OnValueChanged -= LengthChange;
    }

    private void LengthChange(ushort previousValue, ushort newValue)
    {
        InstantiateTail();
    }


    private void InstantiateTail()
    {
        Transform lastTransform;
        if (tails.Count > 0)
            lastTransform = tails.Last().transform;
        else
            lastTransform = transform;
        GameObject newTail = Instantiate(tailPrefab, lastTransform.position, Quaternion.identity);
        if (newTail.TryGetComponent<Tail>(out Tail tail))
        {
            tail.AssignOwner(NetworkObject);
            tail.AssignLastTail(lastTransform);
            newTail.GetComponent<SpriteRenderer>().sortingOrder = length.Value;
            Physics2D.IgnoreCollision(newTail.GetComponent<Collider2D>(), snakeHeadCollider);
            tails.Add(tail);

            if (IsOwner)
            {
                //Debug.Log("invoke event " + OnChangeLength.ToString());
                OnChangeLength?.Invoke(length.Value);
            }
        }
    }

    // Only Called by server
    public void AddLength()
    {
        //Debug.Log("Add Length Called");
        length.Value++;
        InstantiateTail();
    }

    public void DespawnTails()
    {
        for (int i = 0; i< tails.Count; i++)
        {
            Tail tail = tails[i];
            tails.Remove(tail);
            Destroy(tail.gameObject);
        }
    }
}
