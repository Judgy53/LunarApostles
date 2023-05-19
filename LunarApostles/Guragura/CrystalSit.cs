using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.BrotherMonster;
using UnityEngine;

namespace LunarApostles
{
  public class CrystalSit : BaseCrystalSitState
  {

    public override void OnEnter()
    {
      base.OnEnter();
      CharacterBody body = this.characterBody;
      float num = 360f / (float)12;
      Vector3 vector3 = Vector3.ProjectOnPlane(body.inputBank.aimDirection, Vector3.up);
      Vector3 footPosition = body.footPosition;
      for (int index = 0; index < 12; ++index)
      {
        Vector3 forward = Quaternion.AngleAxis(num * (float)index, Vector3.up) * vector3;
        ProjectileManager.instance.FireProjectile(ExitSkyLeap.waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(forward), body.gameObject, body.damage * ExitSkyLeap.waveProjectileDamageCoefficient, ExitSkyLeap.waveProjectileForce, Util.CheckRoll(body.crit, body.master));
      }

      float num2 = 360f / 8;
      for (int index = 0; index < 8; ++index)
      {
        Vector3 forward = Quaternion.AngleAxis(num2 * (float)index, Vector3.up) * vector3;
        ProjectileManager.instance.FireProjectile(FistSlam.waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(forward), body.gameObject, body.damage * FistSlam.waveProjectileDamageCoefficient, FistSlam.waveProjectileForce, Util.CheckRoll(body.crit, body.master));
      }
      this.characterBody.AddTimedBuff(RoR2Content.Buffs.Warbanner, 5);
      this.outer.SetNextState((EntityState)new ExitCrystalSit());
    }
  }
}