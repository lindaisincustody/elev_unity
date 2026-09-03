using UnityEngine;

public interface IDrawingState
{
    DrawingMode Mode { get; }
    DrawingWorld World { get; }

    void Enter(LetterDrawing letterDrawing);
    void Exit();

    bool CanStartStrokeAt(Vector2 screenPosition);
    Vector3 ScreenToWorldPoint(Vector2 screenPosition);

    void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer);
}
