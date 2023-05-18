using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.BrotherMonster;
using EntityStates.LunarWisp;
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

      for (int i = 0; i < 32; i++)
      {
        Ray projectileRay = new Ray();
        projectileRay.direction = aimRay.direction;
        float maxDistance = 160f;
        float randX = UnityEngine.Random.Range(-80f, 80f);
        float randY = UnityEngine.Random.Range(-1f, 1f);
        float randZ = UnityEngine.Random.Range(-80f, 80f);
        Vector3 randVector = new Vector3(randX, randY, randZ);
        Vector3 position = this.characterBody.corePosition + randVector;
        projectileRay.origin = position;
        RaycastHit hitInfo;
        {
          if (Physics.Raycast(aimRay, out hitInfo, maxDistance, (int)LayerIndex.world.mask))
          {
            projectileRay.direction = hitInfo.point - projectileRay.origin;
            EffectManager.SpawnEffect(LunarApostles.severPrefab, new EffectData { origin = projectileRay.origin, rotation = Util.QuaternionSafeLookRotation(projectileRay.direction) }, false);
            ProjectileManager.instance.FireProjectile(LunarApostles.wispBomb, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), this.gameObject, this.damageStat * SeekingBomb.bombDamageCoefficient, SeekingBomb.bombForce, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 0);
          }
        }
      }
    }
  }
}