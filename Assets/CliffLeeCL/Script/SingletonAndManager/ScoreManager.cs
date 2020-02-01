using UnityEngine;
using UnityEngine.SceneManagement;

namespace CliffLeeCL
{
    public class ScoreManager : SingletonMono<ScoreManager>
    {
        /// <summary>
        /// For 0~1, add gainLoveSpeed per second.
        /// </summary>
        public float gainLoveSpeed = 0.2f;
        public float comboResetTime = 2.5f;

        public int CurrentScore
        {
            get
            {
                return currentScore;
            }
        }
        public int CurrentKillCount
        {
            get
            {
                return currentKillCount;
            }
        }
        public int CurrentCombo
        {
            get
            {
                return currentCombo;
            }
        }
        public float CurrentLove
        {
            get
            {
                return currentLove;
            }
        }
        public int MaxCombo
        {
            get
            {
                return maxCombo;
            }
        }

        int currentScore;
        int currentKillCount;
        int currentCombo;
        float currentLove;
        int maxCombo;

        public void AddScore(int amount)
        {
            currentScore += (int)(amount * (1.0f + currentCombo * 0.1f));
        }

        public void AddKillCount(int amount)
        {
            currentKillCount += amount;
        }

        public void AddCombo(int amount)
        {
            currentCombo += amount;
        }

        public void AddLove(float amount)
        {
            currentLove = Mathf.Clamp01(currentLove + amount);
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Update()
        {
            if (currentLove < 1.0f && GameManager.Instance.isInGame)
                AddLove(gainLoveSpeed * Time.deltaTime);

            if (currentCombo > maxCombo)
                maxCombo = currentCombo;

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                print("<color=brown>" + "Score: " + currentScore + ", " + "KillCount: " + currentKillCount + "</color>");
                print("<color=brown>" + "Combo: " + currentCombo + ", " + "Love: " + currentLove + "</color>");
            }
#endif
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentScore = 0;
            currentKillCount = 0;
            currentCombo = 0;
            currentLove = 0.4f;
            maxCombo = 0;
        }

        void Start()
        {
            EventManager.Instance.onGameOver += OnGameOver;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventManager.Instance.onGameOver -= OnGameOver;
        }

        void OnComboTimerIsUp()
        {
            currentCombo = 0;
        }

        public void OnGameOver()
        {
            UpdateHighestRecord();
        }

        public void UpdateHighestRecord()
        {
            int highestScore = PlayerPrefs.GetInt("ScoreRecord", 0);
            int highestKillCount = PlayerPrefs.GetInt("KillRecord", 0);
            int highestCombo = PlayerPrefs.GetInt("ComboRecord", 0);

            if (currentScore > highestScore)
                PlayerPrefs.SetInt("ScoreRecord", currentScore);
            if (currentKillCount > highestKillCount)
                PlayerPrefs.SetInt("KillRecord", currentKillCount);
            if (maxCombo > highestCombo)
                PlayerPrefs.SetInt("ComboRecord", maxCombo);
        }

    }
}
