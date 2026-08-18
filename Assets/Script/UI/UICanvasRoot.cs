using UnityEngine;

public class UICanvasRoot : MonoBehaviour
{
    void Awake()
    {
        SetCanvasRoot();
    }

    private void SetCanvasRoot()
    {
        UIManager.Instance.SetCanvasRoot(transform);
    }
}
