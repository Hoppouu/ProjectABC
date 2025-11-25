using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public NetworkManager Instance { get; private set; }
    public PacketDispatcher packetDispatcher;

    private int _nextPlayerID = 2;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnJoinRequest(IPEndPoint sender)
    {
        packetDispatcher.PacketSender.JoinResponse(_nextPlayerID, sender);
        GameManager.Instance.AddPlayer(_nextPlayerID, sender, false);
        _nextPlayerID++;
    }

    public void StartAsHost()
    {
        packetDispatcher.Setup(NetworkRole.HOST);
        GameManager.Instance.CreatePlayerPrefab(true);
        packetDispatcher.PacketHandler.OnJoinRequest += OnJoinRequest;
    }

    public void StartAsClient(string hostIp)
    {
        packetDispatcher.Setup(NetworkRole.CLIENT, hostIp);
        packetDispatcher.PacketHandler.OnJoinResponse += (int id) => { GameManager.Instance.CreatePlayerPrefab(true); };
        packetDispatcher.PacketSender.JoinRequest();
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
