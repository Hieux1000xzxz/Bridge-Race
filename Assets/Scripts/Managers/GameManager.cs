using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;

    private static GameManager instance;
    public Color NaturalColor => naturalColor;
    public Color PlayerColor => Colors[0];
    public Color RandomColor => Colors[Random.Range(0, randomContestantCount)];
    public Color RandomColorAIColors => Colors[Random.Range(1, randomContestantCount)];

    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private Player player;
    [SerializeField] private BridgeController[] bridges;
    [SerializeField] private BrickSpawner[] brickSpawners;
    [SerializeField] private Color[] Colors;
    [SerializeField] private Color naturalColor;
    [SerializeField] private int currentLevel = 1;

    public Color maturalColor => naturalColor;

    private List<ContestantAI> contestantList;
    private int randomContestantCount;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Input.simulateMouseWithTouches = false;
        contestantList = new List<ContestantAI>();
        instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Clear();

        foreach (BridgeController bridge in bridges)
        {
            bridge.SetAllStepsToNatural();
        }

        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;
        randomContestantCount = Random.Range(3, Colors.Length);

        player.Init(PlayerColor);

        foreach (var brickSpawner in brickSpawners)
        {
            brickSpawner.Init();
        }

        for (int i = randomContestantCount - 1; i >= 1; i--)
        {
            GameObject contestantObj = ObjectPool.instance.Spawn("Contestant");
            if (contestantObj != null)
            {
                contestantObj.transform.position = Vector3.right * i;
                contestantObj.transform.rotation = Quaternion.identity;

                ContestantAI contestant = contestantObj.GetComponent<ContestantAI>();
                if (contestant != null)
                {
                    contestant.Init(Colors[i]);
                    contestantList.Add(contestant);
                }
                else
                {
                    Debug.LogError("ContestantAI component not found on spawned contestant object");
                    ObjectPool.instance.Despawn(contestantObj);
                }
            }
            else
            {
                Debug.LogWarning("Cannot spawn contestant from pool");
            }
        }
    }

    private void Clear()
    {
        foreach (ContestantAI contestant in contestantList)
        {
            if (contestant != null && contestant.gameObject != null)
            {
                ObjectPool.instance.Despawn(contestant.gameObject);
            }
        }
        contestantList.Clear();
    }

    public void Restart()
    {
        Initialize();
    }

    public void GameOver(Character winner)
    {
        foreach (ContestantAI contestant in contestantList)
        {
            if (contestant != null)
            {
                contestant.Stop();
            }
        }

        vcam.Follow = winner.transform;
        vcam.LookAt = winner.transform;

        player.Stop();
        VictoryManager.SetWinner(winner, currentLevel);
        Invoke(nameof(LoadVictoryScene), 3f);
    }

    public BridgeController FindBestBridge(Color characterColor, int currentFloor, BridgeController currentBridge, bool randomFirst)
    {
        if (currentBridge != null && !currentBridge.IsCompleted)
            return currentBridge;

        if (randomFirst)
        {
            List<BridgeController> availableBridges = new List<BridgeController>();
            foreach (BridgeController bridge in bridges)
            {
                if (bridge.FloorLevel != currentFloor) continue;
                if (bridge.IsCompleted) continue;
                availableBridges.Add(bridge);
            }
            if (availableBridges.Count > 0)
                return availableBridges[Random.Range(0, availableBridges.Count)];
        }

        BridgeController bestBridge = null;
        int maxCount = -1;

        foreach (BridgeController bridge in bridges)
        {
            if (bridge.FloorLevel != currentFloor) continue;
            if (bridge.IsCompleted) continue;

            int count = bridge.GetColorCount(characterColor);
            if (count > maxCount)
            {
                bestBridge = bridge;
                maxCount = count;
            }
        }
        return bestBridge;
    }

    private void LoadVictoryScene()
    {
        SceneManager.LoadScene("CheerScene");
    }
}