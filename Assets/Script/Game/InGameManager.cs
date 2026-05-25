using System.Collections;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private PlayerStat Player1;
    [SerializeField] private PlayerStat Player2;

    private bool isRoundOver;


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
        StartCoroutine(RoundEnd(winnerPlayer, loserPlayer));
    }
    private IEnumerator RoundEnd(PlayerStat winner, PlayerStat loser)
    {
        isRoundOver = true;

        //  슬로우모션 시작
        Time.timeScale = 0.2f;        // 속도를 20%로 줄임
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 실제 시간 기준으로 3초 대기 (Unscaled = timeScale 영향 안 받음)
        yield return new WaitForSecondsRealtime(3f);

        //  속도 원복
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // TODO: 결과 UI 표시
        StartCoroutine(CreateResultUI());
    }
    private IEnumerator CreateResultUI()
    {
        yield return null;
        //TODO 결과 화면 SetActive(true) & 결과 화면에 들어가야될 정보 추가
    }
}
