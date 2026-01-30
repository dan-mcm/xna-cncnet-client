using System;
using ClientCore;
using ClientCore.Enums;
using ClientCore.Extensions;
using Rampastring.Tools;


namespace DTAClient.Domain
{
    /// <summary>
    /// A Tiberian Sun mission listed in Battle(E).ini.
    /// </summary>
    public class Mission
    {
        public Mission(IniFile iniFile, string sectionName, int index)
        {
            Index = index;
            CD = iniFile.GetIntValue(sectionName, nameof(CD), 0);
            Side = GetSideFromIniOrSection(iniFile, sectionName);
            CampaignID = GetCampaignIdFromIniOrSection(iniFile, sectionName);
            Scenario = iniFile.GetStringValue(sectionName, nameof(Scenario), string.Empty);
            UntranslatedGUIName = iniFile.GetStringValue(sectionName, "Description", "Undefined mission");
            GUIName = UntranslatedGUIName
                .L10N($"INI:Missions:{sectionName}:Description");

            IconPath = iniFile.GetStringValue(sectionName, "SideName", string.Empty);
            GUIDescription = iniFile.GetStringValue(sectionName, "LongDescription", string.Empty)
                .FromIniString()
                .L10N($"INI:Missions:{sectionName}:LongDescription");
            FinalMovie = iniFile.GetStringValue(sectionName, nameof(FinalMovie), "none");
            RequiredAddon = iniFile.GetBooleanValue(sectionName, nameof(RequiredAddon),
               ClientConfiguration.Instance.ClientGameType == ClientType.YR ||
               ClientConfiguration.Instance.ClientGameType == ClientType.Ares ?
                true :  // In case of YR this toggles Ra2Mode instead which should not be default
                false
            );
            Enabled = iniFile.GetBooleanValue(sectionName, nameof(Enabled), true);
            BuildOffAlly = iniFile.GetBooleanValue(sectionName, nameof(BuildOffAlly), false);
            PlayerAlwaysOnNormalDifficulty = iniFile.GetBooleanValue(sectionName, nameof(PlayerAlwaysOnNormalDifficulty), false);
        }

        /// <summary>
        /// Gets side (house): for D2K from Battle.ini section name (ATR=0, HAR=1, ORD=2); for other games from INI (default 0).
        /// </summary>
        private static int GetSideFromIniOrSection(IniFile iniFile, string sectionName)
        {
            if (ClientConfiguration.Instance.LocalGame.Equals("d2k", StringComparison.OrdinalIgnoreCase))
            {
                if (sectionName.Length >= 3)
                {
                    switch (sectionName.Substring(0, 3).ToUpperInvariant())
                    {
                        case "ATR": return 0;
                        case "HAR": return 1;
                        case "ORD": return 2;
                    }
                }
                return 0;
            }
            return iniFile.GetIntValue(sectionName, nameof(Side), 0);
        }

        /// <summary>
        /// Gets campaign mission number: for D2K from Battle.ini section name (e.g. ATR01 → 1); for other games from INI (default -1).
        /// </summary>
        private static int GetCampaignIdFromIniOrSection(IniFile iniFile, string sectionName)
        {
            if (ClientConfiguration.Instance.LocalGame.Equals("d2k", StringComparison.OrdinalIgnoreCase))
            {
                if (sectionName.Length >= 5 && char.IsDigit(sectionName[3]) && char.IsDigit(sectionName[4])
                    && int.TryParse(sectionName.Substring(3, 2), out int id))
                    return id;
                return -1;
            }
            int fromIni = iniFile.GetIntValue(sectionName, nameof(CampaignID), -1);
            if (fromIni < 0)
                fromIni = iniFile.GetIntValue(sectionName, "MissionNumber", -1);
            return fromIni;
        }

        public int Index { get; }
        public int CD { get; }
        public int CampaignID { get; } = -1;
        public int Side { get; }
        public string Scenario { get; }
        public string GUIName { get; }
        public string UntranslatedGUIName { get; }
        public string IconPath { get; }
        public string GUIDescription { get; }
        public string FinalMovie { get; }
        public bool RequiredAddon { get; }
        public bool Enabled { get; }
        public bool BuildOffAlly { get; }
        public bool PlayerAlwaysOnNormalDifficulty { get; }
    }
}
