using RoR2;
using RoR2.Projectile;
using EntityStates.ScavMonster;
using UnityEngine;

namespace LunarApostles
{
  public class TriJawCannon
  {
    public TriJawCannon()
    {
      On.EntityStates.ScavMonster.FireEnergyCannon.OnEnter += FireEnergyCannon_OnEnter;
    }

    private void FireEnergyCannon_OnEnter(On.EntityStates.ScavMonster.FireEnergyCannon.orig_OnEnter orig, EntityStates.ScavMonster.FireEnergyCannon self)
    {
      orig(self);
      if (self.isAuthority && self.characterBody.name == "ScavLunar1Body(Clone)")
      {
        float num2 = self.currentRefire % 2 == 0 ? 1f : -1f;
        float num3 = Mathf.Ceil((float)self.currentRefire / 2f) * FireEnergyCannon.projectileYawBonusPerRefire;
        for (int index = 0; index < FireEnergyCannon.projectileCount; ++index)
        {
          Ray aimRay = self.GetAimRay();
          aimRay.direction = TweakedApplySpread(aimRay.direction, 30f, num2 * num3, FireEnergyCannon.projectilePitchBonus);
          ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), self.gameObject, self.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(self.critStat, self.characterBody.master));
        }
        for (int index = 0; index < FireEnergyCannon.projectileCount; ++index)
        {
          Ray aimRay = self.GetAimRay();
          aimRay.direction = TweakedApplySpread(aimRay.direction, 60f, num2 * num3, FireEnergyCannon.projectilePitchBonus);
          ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), self.gameObject, self.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(self.critStat, self.characterBody.master));
        }
      }
    }

    private Vector3 TweakedApplySpread(Vector3 direction, float angle, float bonusYaw, float bonusPitch)
    {
      Vector3 up = Vector3.up;
      Vector3 axis1 = Vector3.Cross(up, direction);
      float x = UnityEngine.Random.Range(0, 4);
      Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360f)) * (Quaternion.Euler(30, 0.0f, 0.0f) * Vector3.forward);
      Vector3 vector32 = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360f)) * (Quaternion.Euler(x, 0.0f, 0.0f) * Vector3.forward);
      float y = vector32.y;
      vector3.y = 0.0f;
      double angle1 = (double)Mathf.Atan2(vector3.z, vector3.x) * 57.2957801818848 - 90.0 + bonusYaw;
      float angle2 = (Mathf.Atan2(y, vector3.magnitude) * 57.29578f + bonusPitch) * 1;
      Vector3 axis2 = up;
      return Quaternion.AngleAxis((float)angle1, axis2) * (Quaternion.AngleAxis(angle2, axis1) * direction);
    }
  }
}