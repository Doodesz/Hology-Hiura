using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLockManager : MonoBehaviour
{
    [System.Serializable]
    public struct LevelLock
    {
        public string key;
        [Tooltip("For Debugging Purpose")] public int keyValue;
        public GameObject lockObj;

        public int GetKeyValue()
        {
            SetKeyValue(PlayerPrefs.GetInt(key, 0));
            return PlayerPrefs.GetInt(key, 0);
        }
        public void SetKeyValue(int inputKeyValue)
        {
            PlayerPrefs.SetInt(key, inputKeyValue);
        }
    }

    [System.Serializable]
    public struct LevelHiddenLock
    {
        public List<string> keysNeeded;
        public int collectibleKeysCollected;
        public GameObject lockObj;

        public bool IsUnlocked()
        {
            collectibleKeysCollected = 0;

            foreach (string key in keysNeeded)
            {
                if (PlayerPrefs.GetInt(key, 0) == 1)
                    collectibleKeysCollected++;
            }

            if (collectibleKeysCollected >= keysNeeded.Count)
                return true;
            else 
                return false;
        }
        public void UnlockLevelHidden()
        {
            foreach (string key in keysNeeded)
            {
                PlayerPrefs.SetInt(key, 1);
            }
        }
    }

    public List<LevelLock> levelLocks = new List<LevelLock>();
    public LevelHiddenLock levelHiddenLock = new LevelHiddenLock();

    public static LevelLockManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Unlocks level
        foreach(var levelLock in levelLocks)
        {
            if (levelLock.GetKeyValue() == 1)
            {
                levelLock.lockObj.SetActive(false);
            }
        }

        if (levelHiddenLock.IsUnlocked())
            levelHiddenLock.lockObj.SetActive(false);
    }

    public void UnlockAllLevels()
    {
        foreach (var levelLock in levelLocks)
        {
            levelLock.SetKeyValue(1);
        }

        levelHiddenLock.UnlockLevelHidden();

        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
