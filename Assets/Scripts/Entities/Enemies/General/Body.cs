using UnityEngine;

public class Body
{
    private Transform body;

    public Body(Transform body)
    {
        this.body = body;
    }

    public void UpdateBody(Vector3? target)
    {
        if (!target.HasValue)
            return;

        Vector2 targetPos = target.Value;
        Vector2 currentPos = body.position;

        if (targetPos.x < currentPos.x)
        {
            body.localScale = new Vector3(1, 1, 1);
        }
        else
        {

            body.localScale = new Vector3(-1, 1, 1);
        }
    }
}
