using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        SetFlag("Mercia_FirstMet", false);
        SetFlag("Sui_FirstMet", false);
        SetFlag("GotCoin", false);
    }

    public void SetFlag(string key, bool value)
    {
        if (flags.ContainsKey(key))
        {
            flags[key] = value;
        }
        else
        {
            flags.Add(key, value);
        }
    }

    public bool GetFlag(string key)
    {
        return flags.ContainsKey(key) ? flags[key] : false;
    }
}
