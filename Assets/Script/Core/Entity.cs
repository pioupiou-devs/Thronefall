using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private Faction _faction = Faction.Neutral;
    public Faction Faction => _faction;

    // Default faction for concrete entity types; applied on Reset so the
    // Inspector is pre-filled and configuration is mostly unnecessary.
    protected virtual Faction DefaultFaction => Faction.Neutral;

    private void Reset()
    {
        _faction = DefaultFaction;
    }
}
