using UnityEngine;
using CityBuilder.EventBus;

[RequireComponent(typeof(Entity))]
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHealth = 100f;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField]
    private float debugDamageAmount = 10f;
#endif

    private float currentHealth;
    private bool isDead;
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
        }

    }

}