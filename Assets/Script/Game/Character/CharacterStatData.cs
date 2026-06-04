using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "Stat/CharacterStat")]
public class CharacterStatData : ScriptableObject
{
    //참조해서 변수의 값을 가져오기 위해 public으로 설정
    [Header("목숨")]
    public int life = 3;
    [Header("체력")]
    public int _maxHp = 100;

    [Header("공격")]
    public int _attackDamage = 20;
    public float _attackCooltimeMax = 0.8f;

    [Header("스킬")]
    public int _skillDamage = 40;
    public float _skillCooltimeMax = 10f;

    [Header("이동")]
    public float _moveSpeed = 5f;

    [Header("유령")]
    public string ghostPoolKey;
    public GameObject ghostPrefab;
}
