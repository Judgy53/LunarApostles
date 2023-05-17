using RoR2;
using RoR2.Projectile;
using EntityStates.ScavMonster;
using UnityEngine;

namespace LunarApostles
{
  public class CircularToss
  {
    public CircularToss()
    {
      On.EntityStates.ScavMonster.ThrowSack.Fire += ThrowSack_Fire;
    }

    private void ThrowSack_Fire(On.EntityStates.ScavMonster.ThrowSack.orig_Fire orig, EntityStates.ScavMonster.ThrowSack self)
    {
      if (self.characterBody.name == "ScavLunar1Body(Clone)")
      {
        Ray aimRay = self.GetAimRay();

        Ray ray1 = aimRay;
        Ray ray2 = aimRay;
        Vector3 point = aimRay.GetPoint(ThrowSack.minimumDistance);
        bool flag = false;
        GameObject gameObject = self.gameObject;
        Ray ray3 = ray1;
        RaycastHit raycastHit;
        LayerIndex layerIndex = LayerIndex.world;
        int mask1 = (int)layerIndex.mask;
        layerIndex = LayerIndex.entityPrecise;
        int mask2 = (int)layerIndex.mask;
        LayerMask layerMask = (LayerMask)(mask1 | mask2);
        if (Util.CharacterRaycast(gameObject, ray3, out raycastHit, 500f, layerMask, QueryTriggerInteraction.Ignore))
        {
          point = raycastHit.point;
          flag = true;
        }
        float speedOverride = ThrowSack.projectileVelocity;
        if (flag)
        {
          Vector3 vector3_1 = point - ray2.origin;
          Vector2 vector2_1 = new Vector2(vector3_1.x, vector3_1.z);
          float num1 = vector2_1.magnitude;
          Vector2 vector2_2 = vector2_1 / num1;
          if ((double)num1 < (double)ThrowSack.minimumDistance)
            num1 = ThrowSack.minimumDistance;
          float initialYspeed = Trajectory.CalculateInitialYSpeed(ThrowSack.timeToTarget, vector3_1.y);
          float num2 = num1 / ThrowSack.timeToTarget;
          Vector3 vector3_2 = new Vector3(vector2_2.x * num2, initialYspeed, vector2_2.y * num2);
          speedOverride = vector3_2.magnitude;
          ray2.direction = vector3_2;
        }

        float num = 360f / (float)(ThrowSack.projectileCount * 2);
        Vector3 vector3 = Vector3.ProjectOnPlane(self.inputBank.aimDirection, Vector3.up);
        for (int index = 0; index < ThrowSack.projectileCount * 2; ++index)
        {
          Vector3 forward = Quaternion.AngleAxis(num * (float)index, Vector3.up) * vector3;
          ProjectileManager.instance.FireProjectile(ThrowSack.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), self.gameObject, self.damageStat * ThrowSack.damageCoefficient, 0.0f, Util.CheckRoll(self.critStat, self.characterBody.master), speedOverride: speedOverride);
        }
        for (int index = 0; index < ThrowSack.projectileCount; ++index)
        {
          Quaternion rotation = Util.QuaternionSafeLookRotation(Util.ApplySpread(ray2.direction, ThrowSack.minSpread, ThrowSack.maxSpread, 1f, 1f));
          ProjectileManager.instance.FireProjectile(ThrowSack.projectilePrefab, ray2.origin, rotation, self.gameObject, self.damageStat * ThrowSack.damageCoefficient, 0.0f, Util.CheckRoll(self.critStat, self.characterBody.master), speedOverride: speedOverride);
        }
      }
      else
        orig(self);
    }
  }
}