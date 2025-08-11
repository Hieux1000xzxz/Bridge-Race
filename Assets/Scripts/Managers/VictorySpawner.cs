using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictorySpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera vcam;

    [Header("UI Elements")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button playAgainLoseButton;

    private void Start()
    {
        GameObject winnerObj = Instantiate(playerPrefab, spawnPoint.position, Quaternion.Euler(0, 90f, 0));

        var winnerChar = winnerObj.GetComponent<Character>();
        if (winnerChar != null)
        {
            winnerChar.Init(VictoryManager.WinnerColor);
            winnerChar.name = VictoryManager.WinnerName;
        }

        SetupUI();
        SetupButtons();

        vcam.Follow = null;
        vcam.LookAt = winnerObj.transform;
        vcam.transform.position = winnerObj.transform.position + winnerObj.transform.forward * 3f + Vector3.up * 2f;
        float startFOV = vcam.Lens.FieldOfView;
        DOTween.To(() => vcam.Lens.FieldOfView, x => vcam.Lens.FieldOfView = x, 93f, 2f).SetEase(Ease.InOutSine);
    }

    private void SetupUI()
    {
        if (VictoryManager.IsPlayerWinner)
        {
            if (winUI != null) winUI.SetActive(true);
            if (loseUI != null) loseUI.SetActive(false);

            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (winUI != null) winUI.SetActive(false);
            if (loseUI != null) loseUI.SetActive(true);

            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveAllListeners();
            playAgainButton.onClick.AddListener(PlayAgain);
        }
        if (playAgainLoseButton != null)
        {
            playAgainLoseButton.onClick.RemoveAllListeners();
            playAgainLoseButton.onClick.AddListener(PlayAgain);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(NextLevel);
        }
    }

    private void PlayAgain()
    {
        string currentLevelScene = "Level" + VictoryManager.CurrentLevel;
        SceneManager.LoadScene(currentLevelScene);
    }

    private void NextLevel()
    {
        if (VictoryManager.CurrentLevel < 2)
        {
            int nextLevel = VictoryManager.CurrentLevel + 1;
            string nextLevelScene = "Level" + nextLevel;
            SceneManager.LoadScene(nextLevelScene);
        }
        else
        {
            SceneManager.LoadScene("Level1");
        }
    }
}