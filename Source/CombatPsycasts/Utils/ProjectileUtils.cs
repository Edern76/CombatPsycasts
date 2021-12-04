using UnityEngine;
using Verse;

namespace CombatPsycasts.Utils
{
    public static class ProjectileUtils
    {
        public static float RecalculateNewTicksToImpact(Projectile projectile, Vector3 newDestination)
        {
            float num = (projectile.ExactPosition - newDestination).magnitude / projectile.def.projectile.SpeedTilesPerTick;
            return num;
        }
    }
}