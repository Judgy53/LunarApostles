using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.ScavMonster;
using EntityStates.BrotherMonster;
using EntityStates.VagrantMonster.Weapon;
using UnityEngine;

namespace LunarApostles
{
  public class DrainSit : BaseDrainSitState
  {

    public override void OnEnter()
    {
      base.OnEnter();
      FireWave(this.characterBody, this.GetAimRay(), this.damageStat);
      this.outer.SetNextState((EntityState)new ExitDrainSit());
    }

    private void FireWave(CharacterBody body, Ray aimRay, float damageStat)
    {
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

      for (int i = 0; i < 12; i++)
      {
        float angle = i * Mathf.PI * 2 / 12;
        float x = Mathf.Cos(angle) * 5;
        float z = Mathf.Sin(angle) * 5;
        Vector3 pos = transform.position + new Vector3(x, 0, z);
        float angleDegrees = -angle * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
        ProjectileManager.instance.FireProjectile(JellyBarrage.projectilePrefab, pos, rot, this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 55);
      }
    }
  }
}