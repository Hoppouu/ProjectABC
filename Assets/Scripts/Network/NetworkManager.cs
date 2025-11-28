using System.Net;
using UnityEngine;
using Network;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    public PacketDispatcher packetDispatcher;


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

    public void StartAsHost()
    {
        packetDispatcher.Setup(NetworkRole.HOST);
        GameManager.Instance.AddPlayer(GameManager.Instance.GetNextPlayerID(), true);
    }

    public void StartAsClient(string hostIp)
    {
        packetDispatcher.Setup(NetworkRole.CLIENT, hostIp);
        packetDispatcher.ClientSender.SendJoinRequest();
    }

}
