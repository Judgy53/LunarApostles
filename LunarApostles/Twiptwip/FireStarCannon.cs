using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.ScavMonster;
using EntityStates.TitanMonster;
using UnityEngine;

namespace LunarApostles
{
  public class FireStarCannon : StarCannonState
  {
    public static float baseDuration;
    public static float baseRefireDuration;
    public static string sound;
    public static GameObject effectPrefab;
    public static GameObject projectilePrefab;
    public static float damageCoefficient;
    public static float force;
    public static float minSpread;
    public static float maxSpread;
    public static float recoilAmplitude = 1f;
    public static float projectilePitchBonus;
    public static float projectileYawBonusPerRefire;
    public static int projectileCount;
    public static int maxRefireCount;
    public int currentRefire;
    private float duration;
    private float refireDuration;
    private float speedOverride;
    private float refireDurationBase;
    private bool firstThreshold;
    private bool secondThreshold;

    public override void OnEnter()
    {
      base.OnEnter();
      speedOverride = 55;
      refireDurationBase = 1f;
      firstThreshold = this.healthComponent.health <= (this.healthComponent.fullHealth * 0.75); // 75% HP
      secondThreshold = this.healthComponent.health <= (this.healthComponent.fullHealth * 0.5); // 50% HP
      if (firstThreshold)
      {
        speedOverride = 65;
        refireDurationBase = 0.75f;
      }
      if (secondThreshold)
      {
        speedOverride = 65;
        refireDurationBase = 0.75f;
      }
      this.duration = FireEnergyCannon.baseDuration / this.attackSpeedStat;
      this.refireDuration = refireDurationBase / this.attackSpeedStat;
      int num1 = (int)Util.PlayAttackSpeedSound(FireEnergyCannon.sound, this.gameObject, this.attackSpeedStat);
      this.PlayCrossfade("Body", nameof(FireEnergyCannon), "FireEnergyCannon.playbackRate", this.duration, 0.1f);
      this.AddRecoil(-2f * FireEnergyCannon.recoilAmplitude, -3f * FireEnergyCannon.recoilAmplitude, -1f * FireEnergyCannon.recoilAmplitude, 1f * FireEnergyCannon.recoilAmplitude);
      if ((bool)(Object)FireEnergyCannon.effectPrefab)
        EffectManager.SimpleMuzzleFlash(FireEnergyCannon.effectPrefab, this.gameObject, EnergyCannonState.muzzleName, false);
      if (!this.isAuthority)
        return;

      Ray aimRay = this.GetAimRay();
      FireStarFormation(aimRay);
    }

    private void FireStarFormation(Ray aimRay)
    {
      float num3 = UnityEngine.Random.Range(0.0f, 360f);
      for (int index3 = 0; index3 < 6; ++index3)
      {
        for (int index4 = 0; index4 < 4; ++index4)
        {
          Vector3 vector3 = Quaternion.Euler(0.0f, num3 + 45f * (float)index3, 0.0f) * Vector3.forward;
          Vector3 position = aimRay.origin + vector3 * (FireGoldFist.distanceBetweenFists / 2) * (float)index4;
          Vector3 space = position + Vector3.forward;
          ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, space, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: speedOverride);
        }
      }
    }

    /* fires flat for some reason
        private void FireStarFormation(Ray aimRay)
        {
          float num3 = UnityEngine.Random.Range(0.0f, 360f);
          for (int index3 = 0; index3 < 8; ++index3)
          {
            for (int index4 = 0; index4 < 6; ++index4)
            {
              Vector3 vector3 = Quaternion.Euler(0.0f, num3 + 45f * (float)index3, 0.0f) * Vector3.forward;
              Vector3 position = aimRay.origin + vector3 * (FireGoldFist.distanceBetweenFists / 2) * (float)index4;
              Vector3 space = position + Vector3.up * 30;
              ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, space, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: speedOverride);
            }
          }
        }
    */

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge >= (double)this.refireDuration && this.currentRefire + 1 < FireEnergyCannon.maxRefireCount && this.isAuthority)
        this.outer.SetNextState((EntityState)new FireStarCannon()
        {
          currentRefire = (this.currentRefire + 1)
        });
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }
  }
}