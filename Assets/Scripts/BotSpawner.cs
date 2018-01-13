using UnityEngine;
using UnityEngine.Networking;

public class BotSpawner : NetworkBehaviour
{
    [SerializeField] GameObject botPrefab;

    [ServerCallback]
    void Start()
    {
        GameObject obj = Instantiate(botPrefab, transform.position, transform.rotation);
        obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        obj.AddComponent<Bot>();
        /*
         * allows everyone to see the bot
         */
        NetworkServer.Spawn(obj);
    }
}
