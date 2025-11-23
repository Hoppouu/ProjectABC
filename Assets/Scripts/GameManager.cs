using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject prefab;
    [HideInInspector] public List<GameObject> players;

    public PacketDispatcher packetDispatcher;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {

    }

    private void PlayerJoin(int playerID)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.GetComponent<PlayerInput>().isMine = false;
    }

    public void StartAsHost()
    {
        packetDispatcher.Setup(NetworkRole.HOST);
        GameObject gameObject = Instantiate(prefab);
        gameObject.GetComponent<PlayerInput>().isMine = true;
        packetDispatcher.PacketHandler.OnPlayerJoin += PlayerJoin;
    }

    public void StartAsClient(string hostIp)
    {
        packetDispatcher.Setup(NetworkRole.CLIENT, hostIp);
        GameObject gameObject = Instantiate(prefab);
        gameObject.GetComponent<PlayerInput>().isMine = true;
        packetDispatcher.PacketSender.PlayerJoin(1);
    }
}
