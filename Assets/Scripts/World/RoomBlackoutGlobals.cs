using UnityEngine;

public class RoomBlackoutGlobals : MonoBehaviour
{
    private static readonly int PlayerWorldPosId = Shader.PropertyToID("_PlayerWorldPos");
    private static readonly int RoomSanityId = Shader.PropertyToID("_RoomSanity");

    private void Update()
    {
        if (!GameSession.Instance.IsRunning)
            return;

        Shader.SetGlobalVector(PlayerWorldPosId, Player.instance.transform.position);

        SanityManager sanity = SanityManager.Instance;
        Shader.SetGlobalFloat(RoomSanityId, sanity.CurrentSanity / (float)sanity.MaxSanity);
    }
}
