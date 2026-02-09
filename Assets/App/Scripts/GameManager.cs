using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public EnemyGenerator enemyGenerator;

    public Fighter Player1;
    public Fighter Player2;

    public float delayBetweenTurns = 1.0f;

    private void Start()
    {
        StartCoroutine(FightRoutine());
    }

    IEnumerator FightRoutine()
    {
        int round = 0;
        enemyGenerator.GenerateEnemy(Player2);

        while (Player1.IsAlive())
        {
            round++;
            Debug.Log("--- ROUND " + round + " ---");

            yield return StartCoroutine(HandleTurn(Player1, Player2));

            if (!Player2.IsAlive())
            {
                Debug.Log("Ennemi vaincu !");
                yield return new WaitForSeconds(1.0f);

                Debug.Log("Un nouvel adversaire apparaît...");

                enemyGenerator.GenerateEnemy(Player2);
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            yield return new WaitForSeconds(delayBetweenTurns);

            yield return StartCoroutine(HandleTurn(Player2, Player1));

            if (!Player1.IsAlive()) { EndBattle(Player2); yield break; }

            yield return new WaitForSeconds(delayBetweenTurns);
        }
    }

    IEnumerator HandleTurn(Fighter source, Fighter target)
    {
        if (source.isPlayer)
        {
            Debug.Log("<b>A TOI DE JOUER !</b> (A = Attaque, H = Soin, D = Défense)");

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) ||
                                             Input.GetKeyDown(KeyCode.H) ||
                                             Input.GetKeyDown(KeyCode.D));

            if (Input.GetKeyDown(KeyCode.A))
            {
                source.ExecuteAction(0, target);
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                source.ExecuteAction(1, target);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                source.ExecuteAction(2, target);
            }
        }
        else
        {
            source.PerformAITurn(target);
        }
    }

    void EndBattle(Fighter winner)
    {
        Debug.Log("FIN DU COMBAT ! Vainqueur : " + winner.GetName());
    }
}