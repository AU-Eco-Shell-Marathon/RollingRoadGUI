namespace RollingRoad.Core.ApplicationServices
{
    public interface IPidControl
    {
        double Kp { get; set; }
        double Ki { get; set; }
        double Kd { get; set; }
    }
}
