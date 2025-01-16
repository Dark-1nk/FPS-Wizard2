using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int orbsCollected;
    public int money;

    void Awake()
    {
        // Ensure the GameManager persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerStats(int orbs, int money)
    {
        orbsCollected = orbs;
        this.money = money;
    }

    public void ResetStats()
    {
        orbsCollected = 0;
        money = 0;
    }
}
