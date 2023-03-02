using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuelerManager_Mono : MonoBehaviour
{
    public static DuelerManager_Mono Instance => instance;
    static DuelerManager_Mono instance;

    public static List<Dueler_Mono> AllDuelers => allDuelers;
    static List<Dueler_Mono> allDuelers = new List<Dueler_Mono>();

    public DuelerEvent OnAddEvent;

    public static Dueler_Mono GetDueler(int id)
    {
        foreach(var d in AllDuelers)
        {
            if(d.ID == id)
            {
                return d;
            }
        }
        return null;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static void AddDueler(Dueler_Mono add)
    {
        allDuelers.Add(add);
        instance.OnAddEvent?.Invoke(add);
    }

    public static void RemoveDueler(Dueler_Mono remove)
    {
        allDuelers.Remove(remove);
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}

[System.Serializable]
public class DuelerEvent : UnityEvent<Dueler_Mono> { }
