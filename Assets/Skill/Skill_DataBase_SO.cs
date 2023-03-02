using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SkillDataBase", menuName ="SkillDataBase")]
public class Skill_DataBase_SO : ScriptableObject
{
    public Skill_Base_SO[] database;
    public Skill_Base_SO dummy;

    public Skill_Base_SO GetSkillSO(int id)
    {
        for (int n = 0; n < database.Length; n++)
        {
            if (database[n].Equals(id))
            {
                return database[n];

            }
        }
        return dummy;
    }

    public Skill_Base_SO GetRandomSkillSO()
    {

        int randomID = Random.Range(1, database.Length);

        return GetSkillSO(randomID);
    }
}
