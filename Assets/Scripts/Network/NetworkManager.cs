using System.Net;
using UnityEngine;
using Network;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public HostPacketSender HostSender => _packetDispatcher.HostSender;
    public ClientPacketSender ClientSender => _packetDispatcher.ClientSender;
    public HostPacketHandler HostHandler => _packetDispatcher.HostHandler;
    public ClientPacketHandler ClientHandler => _packetDispatcher.ClientHandler;

    private PacketDispatcher _packetDispatcher;

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

        _packetDispatcher = GetComponent<PacketDispatcher>();
    }

    public void StartAsHost()
    {
        _packetDispatcher.Setup(NetworkRole.Host);
        GameManager.Instance.AddPlayer(new PlayerModel(GameManager.Instance.GetNextPlayerID(), true));
    }

    public void StartAsClient(string hostIp)
    {
        _packetDispatcher.Setup(NetworkRole.Client, hostIp);
        _packetDispatcher.ClientSender.SendJoinRequest();
    }

}
