using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private PlayerStat Player1;
    [SerializeField] private PlayerStat Player2;

    //일단 작성한것입니다 다시 집가서 다시 수정할 것입니다
    public void PlayerDead(PlayerStat deadPlayer)
    {
        Debug.Log($"player{deadPlayer.TeamId}사망");
        deadPlayer.Life--;
        deadPlayer.Hp = deadPlayer.MaxHp;
        Debug.Log("부활됨");
    }

    public void SelectWinner(PlayerStat winnerPlayer, PlayerStat loserPlayer)
    {
        Debug.Log($"플레이어{winnerPlayer.TeamId} 승리");
    }
}
