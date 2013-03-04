using UnityEngine;
using System;
using System.Collections;

public class NetworkRoomProperties
{
    
    public static void UpdateNoPlayers(string item)
    {
        PhotonNetwork.room.maxPlayers = Convert.ToInt32(item);
    }

    public static void UpdateDifficulty(string item)
    {
        var ht = new Hashtable();
        ht.Add("difficulty", item);

        PhotonNetwork.room.SetCustomProperties(ht);
    }

    public static void UpdateLevel(string item)
    {
        var ht = new Hashtable();
        ht.Add("level", item);

        PhotonNetwork.room.SetCustomProperties(ht);
    }
}
