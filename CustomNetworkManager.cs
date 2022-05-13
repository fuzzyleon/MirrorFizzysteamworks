using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    static int[] arr = { 1, 2, 3, 4, 5 };
    List<int> uniqueID_List = new List<int>(arr);

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.connectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

           
            int i = Random.Range(0, uniqueID_List.Count);
            GamePlayerInstance.UniqueID = uniqueID_List[i];
            uniqueID_List.RemoveAt(i);      

            /*GamePlayers.Add(GamePlayerInstance);*/

        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) //for leaving the server (add leave button in lobby)
    {

        foreach(PlayerObjectController GamePlayerInstance in GamePlayers)
        {
           if(GamePlayerInstance.connectionID == conn.connectionId)
           {
               GamePlayers.Remove(GamePlayerInstance);
               uniqueID_List.Add(GamePlayerInstance.UniqueID);
               break;
           }
        }
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
}
