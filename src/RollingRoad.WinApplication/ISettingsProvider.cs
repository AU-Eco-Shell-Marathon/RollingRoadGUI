namespace RollingRoad.WinApplication
{
    public interface ISettingsProvider
    {
        /// <summary>
        /// Read int stats
        /// </summary>
        /// <param name="stat">The stat to retrieve</param>
        /// <param name="defaultValue">Default value, used if no settings could be found</param>
        /// <returns>Value of the stat, if unable to load or parse 0 will be returned</returns>
        int GetIntStat(string stat, int defaultValue = 0);

        double GetDoubleStat(string stat, double defaultValue = 0);

        /// <summary>
        /// Write float stat to file
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">Value to write</param>
        void SetFloatStat(string stat, float value);

        /// <summary>
        /// Get float stat from file or cache
        /// </summary>
        /// <param name="stat">The stat to retrieve</param>
        /// <returns>Value of the stat, if unable to load or parse 0 will be returned</returns>
        float GetFloatStat(string stat);

        /// <summary>
        /// Set int stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">Stat value</param>
        void SetIntStat(string stat, int value);


        /// <summary>
        /// Add to int stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">How much to add to key</param>
        void AddToIntStat(string stat, int value);

        /// <summary>
        /// Add to float stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">How much to add to key</param>
        void AddToFloatStat(string stat, float value);

        /// <summary>
        /// Get stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <returns>Returns empty string if not found, else it return the string value of the key</returns>
        string GetStat(string stat);

        /// <summary>
        /// Set stat, auto saves to file
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">Stat value</param>
        void SetStat(string stat, string value);

    }
}
