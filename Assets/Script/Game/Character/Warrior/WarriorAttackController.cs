using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WarriorAttackController : MonoBehaviour, IAttackController
{
    [Header("공격 설정")]
    [SerializeField] private Vector2 _attackBoxSize = new Vector2(2f, 1.5f);
    [SerializeField] private Transform _attackPoint;
    //감지 레이어
    [SerializeField] private LayerMask _attackLayerMask;

    [Header("스킬 설정 - 검기")]
    // 검기 이동 속도
    [SerializeField] private float _slashWaveSpeed = 10f;
    // 시전자 반동 힘
    [SerializeField] private float _selfKnockbackForce = 4f;
    // 검기 프리팹
    [SerializeField] private GameObject _slashWavePrefab;
    // 검기 최대 사거리
    [SerializeField] private float _slashWaveMaxDistance = 15f;
    // 검기 크기
    [SerializeField] private Vector2 _slashWaveScale = new Vector2(4.5f, 8f);

    //델리게이트에 함수를 가져오기 위한 참조 변수
    private PlayerController _playerController;
    //쿨타임 공격력 가져오는 참조 변수
    private PlayerStat _playerStat;
    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator _animator;
    private BoxCollider2D _attackOffset;

    //GC부하를 줄이기 위해 미리 Collider2D<>버퍼 생성
    //리스트도 동적 배열이라서 크기가 변경되면 재할당이 발생하지만
    //이미 존재하는 리스트를 사용하기에 OverlapBoxAll보다는 성능면에서는 좋다
    List<Collider2D> _hitBuffer = new List<Collider2D>(15);
    private ContactFilter2D _contactFilter;
    void Awake()
    {

        //스크립트 가져오기
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();
        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator = GetComponent<Animator>();
        _attackOffset = GetComponent<BoxCollider2D>();

        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;

        // LayerMask를 ContactFilter2D로 변환
        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_attackLayerMask);
        _contactFilter.useTriggers = true;
    }

    public void Attack()
    {
        if (_playerStat.AttackCooltime > 0f)
            return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //if (_playerController.IsGrounded)
        //    _animator.SetTrigger("NormalAttack");
        //else
        //    _animator.SetTrigger("JumpAttack");
        PerformAttackHit();
        _playerStat.AttackCooltime = _playerStat.AttackCooltimeMax;
    }
    
    public void PerformAttackHit()
    {
        Vector2 facingDir = transform.localScale.x > 0f ? Vector2.right : Vector2.left;
        float offsetX = (_attackOffset.offset.x + _attackOffset.size.x / 2f + _attackBoxSize.x / 2f) * facingDir.x;
        Vector2 origin = (Vector2)transform.position + new Vector2(offsetX, 0f);

        _hitBuffer.Clear();
        Physics2D.OverlapBox(origin, _attackBoxSize, 0f, _contactFilter, _hitBuffer);

        for (int i = 0; i < _hitBuffer.Count; i++)
        {
            Collider2D enemy = _hitBuffer[i];
            if (enemy.gameObject == gameObject) continue;

            if (enemy.TryGetComponent<PlayerStat>(out var enemyStat))
            {
                if (enemyStat.TeamId != _playerStat.TeamId)
                    enemyStat.TakeDamage(_playerStat.AttackDamage);
            }
        }
        //인터페이스에서 프로퍼티 값으로 적과 자신(아군)을 구별하고 있습니다
        ///TODO InGameManager에서 캐릭터에게 TeamId 부여하는 기능 필요
        //다만 이건 의존성이 필요해서 따로 인터페이스를 분리해야 합니다
        //물론 2인용이 아니라 다인용일때 합니다 지금하면 오버엔지니어링에 걸릴 수 있습니다
    }

    public void Skill()
    {
        if (_playerStat.SkillCooltime > 0f)
            return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator.SetTrigger("Skill");

        FireSlashWave();
        _playerStat.SkillCooltime = _playerStat.SkillCooltimeMax;
    }

    private void FireSlashWave()
    {
        if (_slashWavePrefab == null)
        {
            Debug.LogWarning("SlashWave 프리팹이 없습니다!");
            return;
        }

        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

        // 검기 생성
        GameObject slashWaveObj = Instantiate(
            _slashWavePrefab,
            transform.position,
            Quaternion.identity

        );

        // 방향에 따라 스프라이트 반전
        Vector3 waveScale = new Vector3(_slashWaveScale.x * (isFacingRight ? 1f : -1f), _slashWaveScale.y, 1f);
        slashWaveObj.transform.localScale = waveScale;

        // 검기 초기화
        if (slashWaveObj.TryGetComponent<SlashWave>(out var slashWave))
            slashWave.Initialize(_playerStat.SkillDamage, _playerStat.TeamId, direction, _slashWaveSpeed, _slashWaveMaxDistance, _attackLayerMask);

        // 시전자 반동
        _playerController.ApplyKnockback(-direction, _selfKnockbackForce);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 facingDir = transform.localScale.x > 0f ? Vector2.right : Vector2.left;
        float offsetX = (_attackOffset.offset.x + _attackOffset.size.x / 2f + _attackBoxSize.x / 2f) * facingDir.x;
        Vector2 origin = (Vector2)transform.position + new Vector2(offsetX, 0f);
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(origin, _attackBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, _attackBoxSize);

    }
#endif
}