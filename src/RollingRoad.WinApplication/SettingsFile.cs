using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace RollingRoad.WinApplication
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        public static ISettingsProvider ColorSettings { get; } = new SettingsFile("GraphColors.Settings");
        public static ISettingsProvider DefaultSettings { get; } = new SettingsFile("Settings.settings");
    }

    /// <summary>
    /// Settings file originally used in the game "Norse"
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SettingsFile : ISettingsProvider
    {
        /// <summary>
        /// Structure used to store stats
        /// </summary>
        struct Stat
        {
            public string Name;
            public string Value;

            public Stat(string name, string value)
            {
                Name = name;
                Value = value;
            }
        };

        private readonly string _statsFile;
        private readonly List<Stat> _listOfStats = new List<Stat>();

        /// <summary>
        /// Try to load or create a new settings file
        /// </summary>
        /// <param name="path">The path of the file to open/create</param>
        public SettingsFile(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw  new ArgumentException("Path must not be null or empty");

            _statsFile = path;
            _listOfStats.Add(new Stat());
            LoadStats();
        }

        /// <summary>
        /// Load stats from file
        /// </summary>
        private void LoadStats()
        {
            if (!File.Exists(_statsFile))
            {
                File.Create(_statsFile).Close();
            }

            StreamReader sr = new StreamReader(_statsFile);

            int count = 0;
            string countString = sr.ReadLine();

            if (int.TryParse(countString, out count))
            {
                for (int i = 0; i < count; i++)
                {
                    string toParse = sr.ReadLine();

                    Stat temp;

                    temp.Name = toParse.Remove(toParse.IndexOf(": "));
                    temp.Value = toParse.Remove(0, toParse.IndexOf(": ") + 2);

                    CheckIfBigEnough(i);

                    _listOfStats[i] = temp;
                }
            }
            else
            {
                if (countString != null)
                    Debug.WriteLine("Invalid stat file @ " + _statsFile);
            }

            sr.Close();
        }

        /// <summary>
        /// Save stats to file
        /// </summary>
        public void SaveStats()
        {
            StreamWriter sw = new StreamWriter(_statsFile);

            if (_listOfStats != null)
            {
                sw.WriteLine(_listOfStats.Count.ToString());

                foreach (Stat tempStat in _listOfStats)
                {
                    sw.WriteLine(tempStat.Name + ": " + tempStat.Value);
                }
            }
            else
            {
                sw.WriteLine(0.ToString());
                sw.WriteLine("");
            }

            sw.Close();
        }

        /// <summary>
        /// Read int stats
        /// </summary>
        /// <param name="stat">The stat to retrieve</param>
        /// <returns>Value of the stat, if unable to load or parse 0 will be returned</returns>
        public int GetIntStat(string stat, int defaultValue = 0)
        {
            int value;

            return int.TryParse(GetStat(stat), out value) ? value : defaultValue;
        }

        public double GetDoubleStat(string stat, double defaultValue = 0)
        {
            double value;

            return double.TryParse(GetStat(stat), out value) ? value : defaultValue;
        }

        /// <summary>
        /// Write float stat to file
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">Value to write</param>
        public void SetFloatStat(string stat, float value)
        {
            SetStat(stat, value.ToString());
        }

        /// <summary>
        /// Get float stat from file or cache
        /// </summary>
        /// <param name="stat">The stat to retrieve</param>
        /// <returns>Value of the stat, if unable to load or parse 0 will be returned</returns>
        public float GetFloatStat(string stat)
        {
            try
            {
                return float.Parse(GetStat(stat));
            }
            catch (Exception)
            {
                // ignored
            }

            return 0;
        }

        /// <summary>
        /// Set int stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">Stat value</param>
        public void SetIntStat(string stat, int value)
        {
            if (GetIntStat(stat) == value)
                return;

            SetStat(stat, value.ToString());
        }


        /// <summary>
        /// Add to int stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">How much to add to key</param>
        public void AddToIntStat(string stat, int value)
        {
            SetIntStat(stat, GetIntStat(stat) + value);
        }

        /// <summary>
        /// Add to float stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">How much to add to key</param>
        public void AddToFloatStat(string stat, float value)
        {
            SetFloatStat(stat, GetFloatStat(stat) + value);
        }

        /// <summary>
        /// Get stat
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <returns>Returns empty string if not found, else it return the string value of the key</returns>
        public string GetStat(string stat)
        {
            if (_listOfStats == null)
                return "";

            foreach (Stat tempStat in _listOfStats.Where(tempStat => tempStat.Name == stat))
            {
                return tempStat.Value;
            }

            return "";
        }

        /// <summary>
        /// Set stat, auto saves to file
        /// </summary>
        /// <param name="stat">Stat key</param>
        /// <param name="value">Stat value</param>
        public void SetStat(string stat, string value)
        {
            int i = 0;
            foreach (Stat tempStat in _listOfStats)
            {
                if (tempStat.Name == stat)
                {
                    _listOfStats[i] = new Stat(_listOfStats[i].Name, value);
                    SaveStats();
                    return;
                }
                i++;
            }

            CheckIfBigEnough(i);

            _listOfStats[i] = new Stat(stat, value);

            SaveStats();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        private void CheckIfBigEnough(int count)
        {
            for (int i = _listOfStats.Count; i < count + 1; i++)
            {
                _listOfStats.Add(new Stat());
            }
        }
    }
}