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

        public PlayerEntry(int id, GameObject gameObject, IPEndPoint ipEndPoint)
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
    private int _nextPlayerID = 1;
    
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

    public void AddPlayer(int playerID, bool isMine, IPEndPoint ipEndPoint = null)
    {
        players[playerID] = new PlayerEntry(playerID, CreatePlayerPrefab(isMine), ipEndPoint);
    }

    private GameObject CreatePlayerPrefab(bool isMine)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.GetComponent<PlayerInput>().isMine = isMine;

        return gameObject;
    }

    public List<PlayerEntry> GetPlayers()
    {
        List<PlayerEntry> list = new List<PlayerEntry>();
        foreach(PlayerEntry player in players.Values)
        {
            list.Add(player);
        }

        return list;
    }

    public int GetNextPlayerID() { return _nextPlayerID++; }
}
