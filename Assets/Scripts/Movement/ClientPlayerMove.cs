using Unity.Netcode;
using UnityEngine;

public class ClientPlayerMove : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private WASDMovement m_WASDMovement;
    [SerializeField] private Camera m_Camera;

    private void Awake()
    {
        m_WASDMovement.enabled = false;
        m_Camera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            m_Camera.enabled = true;
        }

        if (IsServer)
        {
            m_WASDMovement.enabled = true;
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateInputServerRpc(Vector2 move)
    {
        m_WASDMovement.Move(move);
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        UpdateInputServerRpc(new Vector2(moveX, moveZ));
    }
}
