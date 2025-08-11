using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeGate : MonoBehaviour
{
    [SerializeField] private Renderer gateRenderer;
    [SerializeField] private BrickSpawner[] brickSpawner;
    [SerializeField] private int Floor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character player))
        {
            gateRenderer.material.SetColor(GameStatic.BASE_COLOR, player.Color);
            foreach(BrickSpawner spawner in brickSpawner)
            {
                spawner.ShowBricksByColor(player.Color);
            }
            player.SetFloor(Floor);
        }
    }
}
