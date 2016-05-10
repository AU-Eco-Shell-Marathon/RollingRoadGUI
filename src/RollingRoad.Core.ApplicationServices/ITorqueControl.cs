namespace RollingRoad.Core.ApplicationServices
{
    /// <summary>
    /// Allows the control of Rolling Road
    /// </summary>
    public interface ITorqueControl
    {
        /// <summary>
        /// Sets the torque
        /// </summary>
        /// <param name="torque">Must be higher than zero</param>
        void SetTorque(double torque);
    }
}
