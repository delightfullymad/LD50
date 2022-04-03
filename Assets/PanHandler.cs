using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanHandler : MonoBehaviour
{
    public void EndPanRight()
    {
        GameManager.gameManager.NewDay();
    }

    public void EndPanLeft()
    {
        GameManager.gameManager.EndDay();
    }
}
