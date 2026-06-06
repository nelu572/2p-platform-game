/// <summary>
/// 유령의 자율 이동 구현체가 따라야 하는 계약입니다.
/// </summary>
public interface IGhostMovementController
{
    public void Initialize();
    public void TickMovement(float deltaTime);
    public void StopMovement();
}
