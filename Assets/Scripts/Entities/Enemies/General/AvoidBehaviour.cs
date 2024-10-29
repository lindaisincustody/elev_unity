using UnityEngine;

public class AvoidBehaviour
{
    private Rigidbody2D rb;
    private Vector2 minBound;
    private Vector2 maxBound;

    public AvoidBehaviour(Rigidbody2D rigidbody, Vector2 minBound, Vector2 maxBound)
    {
        this.rb = rigidbody;
        this.minBound = minBound;
        this.maxBound = maxBound;
    }

    public Vector2 GetDirection(Vector2 directionAwayFromPlayer)
    {
        Vector2 horizontalWallInfluence = GetClosestHorizontalWall();
        Vector2 verticalWallInfluence = GetClosestVerticalWall();

        return GetAverage(directionAwayFromPlayer, horizontalWallInfluence, verticalWallInfluence);
    }

    private Vector2 GetClosestHorizontalWall()
    {
        float distanceToTop = maxBound.y - rb.position.y;   // Distance to the top wall
        float distanceToBottom = rb.position.y - minBound.y; // Distance to the bottom wall
        float cst = 3;
        float dist = Mathf.Min(distanceToTop, distanceToBottom);
        if (dist > 3)
            return Vector2.zero;

        if (distanceToTop < distanceToBottom)
            return (cst - dist) * -Vector2.up * 0.6f;
        else
            return (cst - dist) * Vector2.up * 0.6f;
    }

    private Vector2 GetClosestVerticalWall()
    {
        float distanceToLeft = rb.position.x - minBound.x;  // Distance to the left wall
        float distanceToRight = maxBound.x - rb.position.x;  // Distance to the right wall
        float cst = 3;
        float dist = Mathf.Min(distanceToLeft, distanceToRight);

        if (dist > 3)
            return Vector2.zero;

        if (distanceToLeft < distanceToRight)
            return (cst - dist) * -Vector2.right * 0.6f;
        else
            return (cst - dist) * Vector2.right * 0.6f;
    }

    private Vector2 GetAverage(Vector2 a, Vector2 b, Vector2 c)
    {
        float maxX = a.x;
        float maxY = a.y;

        if (Mathf.Abs(b.x) > Mathf.Abs(maxX))
            maxX = b.x;

        if (Mathf.Abs(c.x) > Mathf.Abs(maxX))
            maxX = c.x;

        if (Mathf.Abs(b.y) > Mathf.Abs(maxY))
            maxY = b.y;

        if (Mathf.Abs(c.y) > Mathf.Abs(maxY))
            maxY = c.y;

        return new Vector2(maxX, maxY);
    }
}
