using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHealth = 100f;

    [Tooltip("If true, the GameObject is destroyed when health reaches 0 (e.g. units). " +
             "If false it stays in the scene (e.g. buildings/player).")]
    [SerializeField]
    private bool destroyOnDeath = false;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField]
    #pragma warning disable CS0414 // read via SerializedProperty in HealthEditor
    private float debugDamageAmount = 10f;
    #pragma warning restore CS0414
#endif

    private float currentHealth;
    public bool isDead;
    [HideInInspector]
    public Entity Entity;

    private void Awake()
    {
        currentHealth = maxHealth;
        Entity = GetComponent<Entity>();
    }

    public void TakeDamage(Damage damage)
    {
        if (isDead) return;

        currentHealth -= damage.Amount;

        if (currentHealth <= 0f)
        {
            isDead = true;
            EventBus<EntityDiedEvent>.Raise(new EntityDiedEvent(Entity));

            if (destroyOnDeath)
                Destroy(gameObject);
        }

    }

}