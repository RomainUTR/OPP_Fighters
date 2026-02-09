using UnityEngine;

[CreateAssetMenu(fileName = "SO_FighterData", menuName = "Scriptable Objects/SO_FighterData")]
public class SO_FighterData : ScriptableObject
{
    public string FighterName;
    public int MaxHealth;
    public int CriticalChance;
    public int MinDamage;
    public int MaxDamage;
    public int HealAmount;
}
