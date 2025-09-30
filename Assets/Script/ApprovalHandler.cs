using UnityEngine;
using Unity.Netcode;
using System;

public class ApprovalHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
        {
            response.Approved = false;
            response.Reason = "Server is full";
        }
        response.PlayerPrefabHash = null;
        response.Pending = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
