using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       if(other.TryGetComponent<Character>(out Character player))
       {
           player.Stop();
           GameManager.Instance.GameOver(player);
        }
    }
}
