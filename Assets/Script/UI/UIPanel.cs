using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        OnShow();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        OnHide();
    }


    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}
