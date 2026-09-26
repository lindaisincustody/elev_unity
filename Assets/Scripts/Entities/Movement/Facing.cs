using UnityEngine;

public static class Facing
{
    public static Vector2 ToAxis(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return Vector2.zero;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            return new Vector2(Mathf.Sign(direction.x), 0);

        return new Vector2(0, Mathf.Sign(direction.y));
    }
}
