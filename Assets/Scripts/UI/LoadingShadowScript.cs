using UnityEngine;
using Doozy.Engine.UI;
using Doozy.Engine.Progress;

public class LoadingShadowScript : MonoBehaviour
{
    public Progressor loadingBar;

    public void UpdateLoadingBar(float progressValue)
    {
        loadingBar.SetProgress(progressValue);
    }

    public void ResetLoadingBar()
    {
        loadingBar.ResetValueTo(0);
    }
}
