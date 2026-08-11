using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    public virtual bool IsModal => true;

    public virtual void Open() { gameObject.SetActive(true); }
    public virtual void Close() { gameObject.SetActive(false); }
}
