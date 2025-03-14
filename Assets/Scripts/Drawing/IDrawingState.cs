using System.Collections;
using UnityEngine;

public interface IDrawingState
{
    void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer);
}
