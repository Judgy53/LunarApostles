using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using EntityStates;
using EntityStates.BrotherMonster;
using EntityStates.LunarWisp;
using EntityStates.ScavMonster;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace LunarApostles
{
  public class CrystalSit
  {
    private static GameObject timeCrystal = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WeeklyRun/TimeCrystalBody.prefab").WaitForCompletion();
    public CrystalSit()
    {
      On.EntityStates.ScavMonster.FindItem.OnEnter += FindItem_OnEnter;
      On.EntityStates.ScavMonster.FindItem.OnExit += FindItem_OnExit;
    }

    private void FindItem_OnEnter(On.EntityStates.ScavMonster.FindItem.orig_OnEnter orig, EntityStates.ScavMonster.FindItem self)
    {
      if (self.characterBody.name == "ScavLunar1Body(Clone)")
      {
        FireWave(self.characterBody, self.GetAimRay(), self.damageStat);
        self.outer.SetState((EntityState)new ExitSit());
      }
      else
        orig(self);
    }

    private void FindItem_OnExit(On.EntityStates.ScavMonster.FindItem.orig_OnExit orig, EntityStates.ScavMonster.FindItem self)
    {
      if (!self.characterBody.name.Contains("ScavLunar"))
        orig(self);
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

      Ray projectileRay = new Ray();
      projectileRay.direction = aimRay.direction;
      float maxDistance = 1000f;
      float randY = UnityEngine.Random.Range(10f, 25f);
      Vector3 randVector = new Vector3(projectileRay.direction.x, randY, projectileRay.direction.z);
      Vector3 position = footPosition + randVector;
      projectileRay.origin = position;
      RaycastHit hitInfo;
      {
        if (Physics.Raycast(aimRay, out hitInfo, maxDistance, (int)LayerIndex.world.mask))
        {
          projectileRay.direction = hitInfo.point - projectileRay.origin;
          EffectManager.SpawnEffect(LunarApostles.severPrefab, new EffectData { origin = projectileRay.origin, rotation = Util.QuaternionSafeLookRotation(projectileRay.direction) }, false);
          ProjectileManager.instance.FireProjectile(LunarApostles.wispBomb, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), body.gameObject, damageStat * SeekingBomb.bombDamageCoefficient, SeekingBomb.bombForce, Util.CheckRoll(body.crit, body.master), speedOverride: 15);
        }
      }
    }
  }
}
// RoR2/Base/Brother/BrotherSunderWave.prefab
//RoR2/Base/Brother/BrotherRing.prefab