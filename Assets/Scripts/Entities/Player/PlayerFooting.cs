public class PlayerFooting : Component
{
    private bool overHole;
    private bool onPath;

    public void SetOverHole(bool value)
    {
        overHole = value;
        FallIfUnsupported();
    }

    public void SetOnPath(bool value)
    {
        onPath = value;
        FallIfUnsupported();
    }

    private void FallIfUnsupported()
    {
        if (overHole && !onPath)
            transform.position = Entity.Get<PlayerScenePosition>().GetSpawnPosition();
    }
}
