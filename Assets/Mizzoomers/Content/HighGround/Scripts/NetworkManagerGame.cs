using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerGame : NetworkManager
{
    // private bool _gameStarted = false;

    public override void Start()
    {
        base.Start();

#if !UNITY_SERVER
        // StartHost();
        // StartClient();
#endif
    }

    // public override void OnServerConnect(NetworkConnection conn)
    // {
    //     base.OnServerConnect(conn);
    //
    //     if (!_gameStarted)
    //     {
    //         ServerStartGame();
    //     }
    // }
    //
    // [Server]
    // private void ServerStartGame()
    // {
    //     if (_gameStarted)
    //     {
    //         return;
    //     }
    //
    //     _gameStarted = true;
    // }
}