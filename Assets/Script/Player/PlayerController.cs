using System;
using System.Collections;
using System.Reflection;
using TMPro;
using Unity.Netcode;
using Unity.Networking;
using UnityEngine;

public enum Body_Part
{
    Head,
    Tail
}

public class PlayerController : NetworkBehaviour
{
    public static event Action GameOverEvent;

    [SerializeField] private float MoveSpeed = 3f;
    private Camera m_Camera;
    private Vector3 mousePos;
    private bool canCollide;
    [SerializeField] private float colCheckTime = .5f;

    private PlayerGameOverCheck gameoverCheck;
    private SpriteRenderer[] sprites;
    private Collider2D col;

    private void Awake()
    {
        canCollide = true;
        gameoverCheck = GetComponent<PlayerGameOverCheck>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameoverCheck.isGameOver.Value)
        {
            DespawnPlayer();
            return;
        }
        SetHeadVisible(true);
        if (!IsOwner || !Application.isFocused)
            return;
        mousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        //mousePos += m_Camera.transform.position;
        mousePos.z = 0;
        Vector2 dir = mousePos - transform.position;
        if ( dir != Vector2.zero )
        {
            transform.position = Vector2.MoveTowards( transform.position, mousePos, MoveSpeed * Time.deltaTime );
            transform.up = dir;

            m_Camera.transform.position = new Vector3(transform.position.x, transform.position.y, -20);
            //m_Camera.transform.up = new Vector3(0, 1,0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canCollide == false)
            return;

        if ( collision.CompareTag("Player") && IsOwner == true )
        {
            StartCoroutine(CollideTimer(colCheckTime));
            CollisionData me = new CollisionData()
            {
                id = OwnerClientId,
                bodyPart = Body_Part.Head,
                pos = transform.position,
                up = transform.up,
            };
            if (collision.TryGetComponent<Tail>(out Tail tail))
            {
                CollisionData him = new CollisionData()
                {
                    id = tail.GetOwner().OwnerClientId,
                    bodyPart = Body_Part.Tail,
                    pos = collision.transform.position,
                    up = collision.transform.up,
                };
                DeterminWinnerServerRpc(me, him);
            }
            if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                NetworkObject thatPlayer = playerController.NetworkObject;
                CollisionData him = new CollisionData()
                {
                    id = thatPlayer.OwnerClientId,
                    bodyPart = Body_Part.Head,
                    pos = thatPlayer.transform.position,
                    up = thatPlayer.transform.up,
                };
                DeterminWinnerServerRpc(me, him);
            }
        }
        if (collision.CompareTag("Ground") && IsOwner)
        {
            StartCoroutine(CollideTimer(colCheckTime));
            GameOverEvent?.Invoke();
        }
    }

    [ServerRpc]
    private void DeterminWinnerServerRpc(CollisionData player1, CollisionData player2)
    {
        if (player1.bodyPart == Body_Part.Tail && player2.bodyPart == Body_Part.Head)
        {
            CompeteInformation(player1.id, player2.id);
            return;
        }
        if (player1.bodyPart == Body_Part.Head && player2.bodyPart == Body_Part.Tail)
        {
            CompeteInformation(player2.id, player1.id);
            return;
        }
        if (player1.bodyPart == Body_Part.Head && player2.bodyPart == Body_Part.Head)
        {
            Vector3 dist = player1.pos - player2.pos;
            float dot1 = Vector3.Dot(dist, Vector3.Normalize(player1.pos) );
            float dot2 = Vector3.Dot(dist, Vector3.Normalize(player2.pos) );
            if (dot1 > dot2)
            {
                Debug.Log("player " + player2.id + " win");
                CompeteInformation(player2.id, player1.id);
            }
            //else
            //{
            //    Debug.Log("player "+ player1.id + "lose");
            //    CompeteInformation(player2.id, player1.id);
            //}
        }
    }

    private void CompeteInformation(ulong winner, ulong loser)
    {
        ulong[] targetArray = new ulong[1];
        targetArray[0] = loser;
        ClientRpcSendParams clientRpcSendParams = new ClientRpcSendParams()
        {
            TargetClientIds = targetArray,
        };
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = clientRpcSendParams,
        };
        GameOverClientRpc(clientRpcParams);

        targetArray[0] = winner;
        clientRpcSendParams = new ClientRpcSendParams()
        {
            TargetClientIds = targetArray,
        };
        clientRpcParams = new ClientRpcParams()
        {
            Send = clientRpcSendParams,
        };
        WinInfoClientRpc(clientRpcParams);


    }

    [ClientRpc]
    private void WinInfoClientRpc(ClientRpcParams clientRpcParams = default)
    {
        //if (IsOwner == false) return;
        //Debug.Log(clientRpcParams.Send.TargetClientIds + " eat a snake, " + "and called by: " + OwnerClientId);
    }

    [ClientRpc]
    private void GameOverClientRpc(ClientRpcParams clientRpcParams = default)
    {
        GameOverEvent?.Invoke();
        //if (IsOwner == false) return;
        //Debug.Log(clientRpcParams.Send.TargetClientIds + "Game Over " + "and called by: " + OwnerClientId);
        //DespawnPlayerClientRpc();
    }

    private void DespawnPlayer()
    {
        if (TryGetComponent<PlayerLength>(out  var playerLength))
        {
            playerLength.DespawnTails();
        }
        SetHeadVisible(false);
    }

    public void SetHeadVisible(bool visibility)
    {
        foreach (var sprite in sprites)
        {
            sprite.enabled = visibility;
            col.enabled = visibility;
        }

    }


    struct CollisionData : INetworkSerializable
    {


        public ulong id;
        public Body_Part bodyPart;
        public Vector3 pos;
        public Vector3 up;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref bodyPart);
            serializer.SerializeValue(ref pos);
            serializer.SerializeValue(ref up);
        }

    }

    IEnumerator CollideTimer(float _seconds)
    {
        canCollide = false;
        yield return new WaitForSeconds(_seconds);
        canCollide = true;
    }
}
