using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int Current { get; private set; }
    public int Max => maxHealth;

    public UnityEvent onDamaged;
    public UnityEvent onDied;

    private void Awake()
    {
        Current = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (Current <= 0) return;

        Current = Mathf.Max(0, Current - amount);
        onDamaged.Invoke();

        if (Current == 0)
            onDied.Invoke();
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(maxHealth, Current + amount);
    }
}
