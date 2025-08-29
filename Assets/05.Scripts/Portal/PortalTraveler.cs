using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PortalTraveler : MonoBehaviour
{
    // 포탈 별 진입시간 금지
    private readonly Dictionary<Portal, float> _blockedUntil = new();

    public bool IsBlocked(Portal p)
    {
        if (p != null && _blockedUntil.TryGetValue(p, out float until))
            return Time.time < until;
        return false;
    }

    public void SetCooldown(Portal p, float seconds)
    {
        if (p == null) return;
        _blockedUntil[p] = Time.time + Mathf.Max(0f, seconds);
    }

    private void OnDisable()
    {
        _blockedUntil.Clear();
    }
}
