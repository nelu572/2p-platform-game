public interface IChargeable
{
    public bool IsCharging { get; set; }
    void ReleaseCharge(string actionName);
}