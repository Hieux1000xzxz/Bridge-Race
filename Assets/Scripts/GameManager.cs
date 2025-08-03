using Lean.Pool;
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
    public Color maturalColor => naturalColor;


    private List<ContestantAI> contestantList;
    private int randomContestantCount;
    private void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
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
        randomContestantCount = Random.Range(2, Colors.Length);

        player.Init(PlayerColor);
        foreach (var brickSpawner in brickSpawners)
        {
            brickSpawner.Init();
        }

        for (int i = randomContestantCount - 1; i >= 1; i--)
        {
            ContestantAI contestant = LeanPool.Spawn(GameAssets.Instance.Contestant, Vector3.right * i, Quaternion.identity);
            contestant.Init(Colors[i]);
            contestantList.Add(contestant);
        }

    }

    private void Clear()
    {
        foreach (ContestantAI contestant in contestantList)
        {
            contestant.Despawn();
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
            contestant.Stop();
        }

        vcam.Follow = winner.transform;
        vcam.LookAt = winner.transform;

        player.Stop();
        VictoryManager.SetWinner(winner);
        Invoke(nameof(LoadVictoryScene), 3f);

    }
    public BridgeController FindBestBridge(Color characterColor, int currentFloor)
    {
        BridgeController bestBridge = bridges[Random.Range(0, bridges.Length)];
        int maxCount = 0;
        foreach (BridgeController bridge in bridges)
        {
            if (bridge.FloorLevel != currentFloor) continue;
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