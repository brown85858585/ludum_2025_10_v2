using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public bool showCursor = false;
    public bool useUpdate = false;

    void Start()
    {
        UpdateCursor();
    }

    void Update()
    {
        if (!useUpdate) return;

        if (Cursor.visible != showCursor)
            UpdateCursor();
    }

    void OnEnable()
    {
        EventManagerv2.instance.StartListening("UpdateScreenCursor", SetCursorVisible);
    }

    void OnDisable()
    {
        if (!EventManagerv2.IsDestroyed)
        {
            EventManagerv2.instance.StopListening("UpdateScreenCursor", SetCursorVisible);
        }
    }

    private void UpdateCursor()
    {
        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void SetCursorVisible(EventParam eventParam)
    {
        string _showCursor = eventParam.data as string;
        showCursor = _showCursor.Contains("show");
        UpdateCursor();
    }

}