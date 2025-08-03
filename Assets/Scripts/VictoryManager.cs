using UnityEngine;

public static class VictoryManager
{
    public static Color WinnerColor { get; private set; }
    public static string WinnerName { get; private set; }

    public static void SetWinner(Character winner)
    {
        WinnerColor = winner.Color;
        WinnerName = winner.name;
    }
}
