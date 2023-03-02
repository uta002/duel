using UnityEngine;

[CreateAssetMenu(fileName = "DuelerDataBase", menuName = "DuelerDataBase")]
public class Dueler_DataBase_SO : ScriptableObject
{
    public DuelerStatus_SO[] database;

    public DuelerStatus_SO GetDueler_SO(int id)
    {
        return database[id % database.Length];
    }

    public int GetRandomID() => Random.Range(0, database.Length);
}
