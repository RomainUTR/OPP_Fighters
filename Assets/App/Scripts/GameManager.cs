using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Fighter player1;
    public Fighter player2;

    public float delayBetweenTurns = 1.0f;

    private void Start()
    {
        StartCoroutine(FightRoutine());
    }

    IEnumerator FightRoutine()
    {
        int round = 0;

        while (player1.IsAlive() && player2.IsAlive())
        {
            round++;
            Debug.Log("--- ROUND " + round + " ---");

            yield return StartCoroutine(HandleTurn(player1, player2));

            if (!player2.IsAlive()) { EndBattle(player1); yield break; }

            yield return new WaitForSeconds(delayBetweenTurns);

            yield return StartCoroutine(HandleTurn(player2, player1));

            if (!player1.IsAlive()) { EndBattle(player2); yield break; }

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