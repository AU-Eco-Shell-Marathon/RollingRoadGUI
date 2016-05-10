namespace RollingRoad.Core.ApplicationServices
{
    public interface IMotorControl
    {
        int CruiseSpeed { get; set; }
        int MaxSpeed { get; set; }
    }
}
