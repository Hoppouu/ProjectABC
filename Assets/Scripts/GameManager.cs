using Manager;
using System.Collections.Generic;
using System.Data;
using System.Net;
using UnityEngine;

namespace Manager
{
    public class PlayerEntry
    {
        public int id;
        public IPEndPoint ipEndPoint;
        public GameObject gameObject;

        public PlayerEntry(int id, IPEndPoint ipEndPoint, GameObject gameObject)
        {
            this.id = id;
            this.ipEndPoint = ipEndPoint;
            this.gameObject = gameObject;
        }
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject prefab;
    
    private Dictionary<int, PlayerEntry> players;
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

    void Update()
    {
        players = new Dictionary<int, PlayerEntry>();
    }

    public void AddPlayer(int playerID, IPEndPoint ipEndPoint, bool isMine)
    {
        players[playerID] = new PlayerEntry(playerID, ipEndPoint, CreatePlayerPrefab(isMine));
    }

    public GameObject CreatePlayerPrefab(bool isMine)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.GetComponent<PlayerInput>().isMine = isMine;

        return gameObject;
    }
}
