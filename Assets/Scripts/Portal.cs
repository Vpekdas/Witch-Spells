using UnityEngine;

public class Portal : MonoBehaviour
{
    public void OnPortalAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
