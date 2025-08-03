using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class VictorySpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera vcam;

    private void Start()
    {
        GameObject winnerObj = Instantiate(playerPrefab, spawnPoint.position, Quaternion.Euler(0, 90f, 0));


        var winnerChar = winnerObj.GetComponent<Character>();
        if (winnerChar != null)
        {
            winnerChar.Init(VictoryManager.WinnerColor);
            winnerChar.name = VictoryManager.WinnerName;
        }

        vcam.Follow = null;
        vcam.LookAt = winnerObj.transform;
        vcam.transform.position = winnerObj.transform.position + winnerObj.transform.forward * 3f + Vector3.up * 2f;
        float startFOV = vcam.Lens.FieldOfView;
        DOTween.To(() => vcam.Lens.FieldOfView, x => vcam.Lens.FieldOfView = x, 93f, 2f).SetEase(Ease.InOutSine);
    }
}

