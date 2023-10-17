using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;

    
    
    public GameObject Init()
    {
        GameObject temp = null;
        
        if (PhotonNetwork.IsConnected)
        {
            // ���� �÷��̾ �����մϴ�.
            // PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, playerSpawnPoint.position, playerSpawnPoint.rotation);
            temp = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, playerSpawnPoint.position, playerSpawnPoint.rotation);
            
            // �� NPC�� �����մϴ�.
            // PhotonNetwork.Instantiate("Prefabs/" + enemyPrefab.name, enemySpawnPoint.position, enemySpawnPoint.rotation);
        }

        return temp;
    }
}
