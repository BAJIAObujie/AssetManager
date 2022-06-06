using UnityEngine;

namespace Core.Asset
{
    public static class AssetWorkMode
    {
        private static string PlayerPrefKey = "Key-UseSimulate";
        private static int UseSimulateValue = 1;
        private static int UseAssetBundleValue = 0;

        private static bool _init;
        private static bool _useSimulate;
        
        public static bool UseAssetBundle => !UseSimulate;
        
        public static bool UseSimulate
        {
            get
            {
#if UNITY_EDITOR
                if (_init)
                    return _useSimulate;
                _init = true;
                _useSimulate = PlayerPrefs.GetInt(PlayerPrefKey, UseSimulateValue) == UseSimulateValue;
                return _useSimulate;
#else                
                return false;
#endif
            }
        }

        #region MenuItem

#if UNITY_EDITOR
        
        private const string FastButton_UseSimulate = "FastButton/UseSimulate";
        private const string FastButton_UseAssetBundle = "FastButton/UseAssetBundle";
        
        [UnityEditor.MenuItem(FastButton_UseSimulate)]
        private static void SetSimulateMode()
        {
            PlayerPrefs.SetInt(PlayerPrefKey, UseSimulateValue);
        }
        
        [UnityEditor.MenuItem(FastButton_UseSimulate, true)]
        private static bool SetSimulateModeCheck()
        {
            bool bMatch = PlayerPrefs.GetInt(PlayerPrefKey, UseSimulateValue) == UseSimulateValue;
            UnityEditor.Menu.SetChecked(FastButton_UseSimulate, bMatch);
            return true;
        }
        
        [UnityEditor.MenuItem(FastButton_UseAssetBundle)]
        private static void SetABMode()
        {
            PlayerPrefs.SetInt(PlayerPrefKey, UseAssetBundleValue);
        }
        
        [UnityEditor.MenuItem(FastButton_UseAssetBundle, true)]
        private static bool SetABModeCheck()
        {
            bool bMatch = PlayerPrefs.GetInt(PlayerPrefKey, UseSimulateValue) == UseAssetBundleValue;
            UnityEditor.Menu.SetChecked(FastButton_UseAssetBundle, bMatch);
            return true;
        }
        
#endif
        
        #endregion
        

    }
}