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
        Debug.Log("Fight");
        Debug.Log(player1.GetName() + " est " + player1.IsAlive());
        Debug.Log(player2.GetName() + " est " + player2.IsAlive());
        int round = 0;

        while (player1.IsAlive() && player2.IsAlive())
        {
            round++;
            Debug.Log("--- ROUND " + round + " ---");
            player1.PerformTurn(player2);
            yield return new WaitForSeconds(delayBetweenTurns);

            if (!player2.IsAlive())
            {
                EndBattle(player1);
                yield break;
            }

            player2.PerformTurn(player1);

            yield return new WaitForSeconds(delayBetweenTurns);

            if (!player1.IsAlive())
            {
                EndBattle(player2);
                yield break;
            }
        }
    }

    void EndBattle(Fighter winner)
    {
        Debug.Log("FIN DU COMBAT ! Vainqueur : " + winner.GetName());
    }
}