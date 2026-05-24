using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private PlayerStat Player1;
    [SerializeField] private PlayerStat Player2;

    void Update()
    {
        if (Player1.Hp <= 0) PlayerDead(Player1);
        if (Player2.Hp <= 0) PlayerDead(Player2);
    }

    public void PlayerDead(PlayerStat deadPlayer)
    {
        if (deadPlayer.Life > 0)
        {
            Debug.Log($"player{deadPlayer.TeamId}사망");
            /// TODO: 캐릭터에 맞는 유령 생성 로직 작성 필요 
            deadPlayer.Revive();
            Debug.Log("부활됨");
        }
        else
        {
            PlayerStat winnerPlayer = (deadPlayer == Player1) ? Player2 : Player1;
            SelectWinner(winnerPlayer, deadPlayer);
        }
    }


    public void SelectWinner(PlayerStat winnerPlayer, PlayerStat loserPlayer)
    {
        Debug.Log($"플레이어{winnerPlayer.TeamId} 승리");
    }
}
