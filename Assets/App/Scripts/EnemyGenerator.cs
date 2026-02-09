using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public List<SO_FighterData> PossibleEnemies;

    public void GenerateEnemy(Fighter enemyFighter)
    {
        int randomIndex = Random.Range(0, PossibleEnemies.Count);
        SO_FighterData randomData = PossibleEnemies[randomIndex];
        enemyFighter.Init(randomData);
        Debug.Log("Un nouvel ennemi apparaît : " + randomData.FighterName + "!");
    }
}
