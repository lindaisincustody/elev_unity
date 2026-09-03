using UnityEngine;

public abstract class PaintingDrawingState : IDrawingState
{
    protected LetterDrawing letterDrawing;

    public abstract DrawingMode Mode { get; }
    public abstract DrawingWorld World { get; }

    public virtual void Enter(LetterDrawing drawing)
    {
        letterDrawing = drawing;

        letterDrawing.CameraRig.SetActive(false);
        letterDrawing.ShowDrawZone(false);
        letterDrawing.lineRenderer.enabled = false;
    }

    public virtual void Exit()
    {
    }

    public bool CanStartStrokeAt(Vector2 screenPosition)
    {
        return true;
    }

    public Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Camera worldCamera = Camera.main;
        float depth = Mathf.Abs(worldCamera.transform.position.z);

        return worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
    }

    public abstract void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer);
}
