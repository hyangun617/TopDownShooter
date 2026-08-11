using UnityEngine;

public class UICanvasRoot : MonoBehaviour
{
    void Start()
    {
        SetCanvasRoot();
    }

    private void SetCanvasRoot()
    {
        UIManager.Instance.SetCanvasRoot(transform);
    }
}
