using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.BrotherMonster;
using EntityStates.VagrantMonster.Weapon;
using EntityStates.ScavMonster;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LunarApostles
{
  public class CrystalSit : BaseCrystalSitState
  {
    private float stopwatch;
    private float shockwaveStopwatch;
    private float missileStopwatch;
    public static float stormDuration;
    public static float stormToIdleTransitionDuration;
    public static float missileSpawnFrequency = 20f; // 25
    public static int missileTurretCount; // 4
    public static float missileTurretYawFrequency;
    public static float missileTurretPitchFrequency;
    public static float missileTurretPitchMagnitude;
    public static float missileSpeed;
    public static float damageCoefficient;
    public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/LunarMissileProjectile.prefab").WaitForCompletion();
    public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Heretic/HereticSpawnEffect.prefab").WaitForCompletion();

    public override void OnEnter()
    {
      base.OnEnter();
      int num = (int)Util.PlaySound("Play_moonBrother_phase4_transition", this.gameObject);
      EffectManager.SimpleMuzzleFlash(effectPrefab, this.gameObject, FireEnergyCannon.muzzleName, false);
      FireShockwave();
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      this.stopwatch += Time.fixedDeltaTime;
      this.shockwaveStopwatch += Time.fixedDeltaTime;
      this.missileStopwatch += Time.fixedDeltaTime;
      if (shockwaveStopwatch >= 2)
      {
        this.shockwaveStopwatch -= 2;
        FireShockwave();
      }
      if ((double)this.missileStopwatch >= 1.0 / (double)missileSpawnFrequency)
      {
        Ray aimRay = this.GetAimRay();
        this.missileStopwatch -= 1f / missileSpawnFrequency;
        for (int index = 0; index < JellyStorm.missileTurretCount; ++index)
        {
          float bonusYaw = (float)(360.0 / (double)JellyStorm.missileTurretCount * (double)index + 360.0 * (double)0.75 * (double)this.stopwatch);
          this.FireBlob(new Ray()
          {
            origin = aimRay.origin + new Vector3(0, 5, 0),
            direction = aimRay.direction
          }, Mathf.Sin(6.283185f * JellyStorm.missileTurretPitchFrequency * this.stopwatch) * JellyStorm.missileTurretPitchMagnitude, bonusYaw, 75);
        }
      }
      if ((double)this.stopwatch < (double)JellyStorm.stormDuration / 2)
        return;
      this.outer.SetNextState((EntityState)new ExitCrystalSit());
    }

    private void FireShockwave()
    {
      CharacterBody body = this.characterBody;
      float num = 360f / (float)12;
      Vector3 vector3 = Vector3.ProjectOnPlane(body.inputBank.aimDirection, Vector3.up);
      Vector3 footPosition = body.footPosition;
      for (int index = 0; index < 12; ++index)
      {
        Vector3 forward = Quaternion.AngleAxis(num * (float)index, Vector3.up) * vector3;
        ProjectileManager.instance.FireProjectile(ExitSkyLeap.waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(forward), body.gameObject, body.damage * ExitSkyLeap.waveProjectileDamageCoefficient, ExitSkyLeap.waveProjectileForce, Util.CheckRoll(body.crit, body.master));
      }
    }

    private void FireBlob(Ray aimRay, float bonusPitch, float bonusYaw, float speed)
    {
      Vector3 forward = Util.ApplySpread(aimRay.direction, 0.0f, 0.0f, 1f, 1f, bonusYaw, bonusPitch);
      ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, 0.0f, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: speed);
    }
  }
}