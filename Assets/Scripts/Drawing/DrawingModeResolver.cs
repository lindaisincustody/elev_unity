using System.Collections.Generic;
using UnityEngine;

public class DrawingModeResolver
{
    private readonly Dictionary<DrawingMode, IDrawingState> states;
    private readonly List<ZoneRequest> zoneRequests = new List<ZoneRequest>();

    private DrawingMode? abilityRequest;

    public DrawingModeResolver(Dictionary<DrawingMode, IDrawingState> states)
    {
        this.states = states;
    }

    public void PushZone(Object source, DrawingMode mode)
    {
        zoneRequests.Add(new ZoneRequest(source, mode));
    }

    public void PopZone(Object source)
    {
        zoneRequests.RemoveAll(request => request.Source == source);
    }

    public void RequestAbility(DrawingMode mode)
    {
        abilityRequest = mode;
    }

    public void ClearAbility()
    {
        abilityRequest = null;
    }

    public bool IsAbilityState(IDrawingState state)
    {
        return abilityRequest.HasValue && states[abilityRequest.Value] == state;
    }

    public IDrawingState Resolve(DrawingWorld world)
    {
        if (abilityRequest.HasValue)
        {
            IDrawingState ability = LegalOrNull(abilityRequest.Value, world);
            if (ability != null)
                return ability;
        }

        for (int i = zoneRequests.Count - 1; i >= 0; i--)
        {
            IDrawingState zone = LegalOrNull(zoneRequests[i].Mode, world);
            if (zone != null)
                return zone;
        }

        DrawingMode? fallback = DefaultFor(world);
        return fallback.HasValue ? states[fallback.Value] : null;
    }

    private static DrawingMode? DefaultFor(DrawingWorld world)
    {
        return world == DrawingWorld.Underworld ? DrawingMode.Predicting : (DrawingMode?)null;
    }

    private IDrawingState LegalOrNull(DrawingMode mode, DrawingWorld world)
    {
        IDrawingState state = states[mode];
        return state.World == world ? state : null;
    }

    private readonly struct ZoneRequest
    {
        public readonly Object Source;
        public readonly DrawingMode Mode;

        public ZoneRequest(Object source, DrawingMode mode)
        {
            Source = source;
            Mode = mode;
        }
    }
}
