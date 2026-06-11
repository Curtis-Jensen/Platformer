using System.Collections.Generic;
using UnityEngine;

// Central gating system for player actions.
// Any system can lock or unlock a category, and any system can ask whether
// a given action is currently permitted before attempting it.
// Uses a counter per type so multiple simultaneous locks compose correctly --
// movement stays locked until every system that locked it has unlocked it.
//
// Usage:
//   actions.Lock(ActionType.Attack);
//   if (actions.CanPerform(ActionType.Attack)) { ... }
//   actions.Unlock(ActionType.Attack);
public class PlayerActions : MonoBehaviour
{
    public enum ActionType
    {
        Movement,
        Attack,
        Dash,
    }

    private readonly Dictionary<ActionType, int> lockCounts = new Dictionary<ActionType, int>();

    // -------------------------------------------------------
    // CanPerform(type)
    // Returns true if the given action has no active locks.
    // -------------------------------------------------------
    public bool CanPerform(ActionType type)
    {
        return !lockCounts.TryGetValue(type, out int count) || count <= 0;
    }

    // -------------------------------------------------------
    // Lock(type)
    // Increments the lock counter for an action type.
    // The caller is responsible for a matching Unlock call.
    // -------------------------------------------------------
    public void Lock(ActionType type)
    {
        lockCounts.TryGetValue(type, out int count);
        lockCounts[type] = count + 1;
    }

    // -------------------------------------------------------
    // Unlock(type)
    // Decrements the lock counter. Clamps at zero so an extra
    // Unlock from a system that cleaned up early can't go negative.
    // -------------------------------------------------------
    public void Unlock(ActionType type)
    {
        lockCounts.TryGetValue(type, out int count);
        lockCounts[type] = Mathf.Max(0, count - 1);
    }
}
