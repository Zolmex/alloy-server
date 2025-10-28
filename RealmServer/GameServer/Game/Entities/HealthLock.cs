using Common;

namespace GameServer.Game.Entities;

/// <summary>
/// Helper class for preventing a characte from dying for a specified period of time.
/// </summary>
public class HealthLock
{
    private bool locked = true;
    private bool lockTriggered;

    /// <summary>
    /// Sets the percentage at which the lock activate, 0 is 0%, 1 is 100%.
    /// </summary>
    public float LockAtPerc { private get; set; }

    /// <summary>
    /// Sets the duration of the lock in MS, use int.MaxValue for an infinite lock.
    /// </summary>
    public int LockDurationMs { private get; set; }

    /// <summary>
    /// Check if a lock is active for a given character.
    /// </summary>
    /// <param name="character">Character the lock is on.</param>
    /// <returns>Whether or not the lock is active.</returns>
    public bool IsLockActive(Character character)
    {
        if (locked && character.HpPerc < LockAtPerc && !lockTriggered)
        {
            lockTriggered = true;
            character.ApplyConditionEffect(ConditionEffectIndex.Invincible, LockDurationMs);
        }

        return locked && character.HpPerc < LockAtPerc;
    }
    
    /// <summary>
    /// Release an existing lock.
    /// </summary>
    /// <param name="character">Character the lock is on.</param>
    public void ReleaseLock(Character character)
    {
        locked = false;
        lockTriggered = true;
        character.RemoveConditionEffect(ConditionEffectIndex.Invincible);
    }
}
