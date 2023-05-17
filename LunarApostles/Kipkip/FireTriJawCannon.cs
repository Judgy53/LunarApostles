using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.ScavMonster;
using UnityEngine;

namespace LunarApostles
{
  public class FireTriJawCannon : TriJawCannonState
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
    private float angle;
    private float speedOverride;
    private float refireDurationBase;
    private bool firstThreshold;
    private bool secondThreshold;

    public override void OnEnter()
    {
      base.OnEnter();
      angle = 45f;
      speedOverride = 75;
      refireDurationBase = 0.75f;
      firstThreshold = this.healthComponent.health <= (this.healthComponent.fullHealth * 0.75); // 75% HP
      secondThreshold = this.healthComponent.health <= (this.healthComponent.fullHealth * 0.5); // 50% HP
      Debug.LogWarning(firstThreshold);
      Debug.LogWarning(secondThreshold);
      if (firstThreshold)
      {
        angle = 30f;
        speedOverride = 85;
        refireDurationBase = 0.5f;
      }
      if (secondThreshold)
      {
        angle = 30f;
        speedOverride = 95;
        refireDurationBase = 0.25f;
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
      for (int index = 0; index < FireEnergyCannon.projectileCount; ++index)
      {
        Ray aimRay = this.GetAimRay();
        aimRay.direction = TweakedApplySpread(aimRay.direction, 0, 1, FireEnergyCannon.projectilePitchBonus);
        ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: speedOverride);
      }
      for (int idx = 0; idx < 2; ++idx)
      {
        for (int index = 0; index < FireEnergyCannon.projectileCount; ++index)
        {
          Ray aimRay = this.GetAimRay();
          aimRay.direction = TweakedApplySpread(aimRay.direction, angle, 1, FireEnergyCannon.projectilePitchBonus);
          ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: speedOverride);
        }
      }
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge >= (double)this.refireDuration && this.currentRefire + 1 < FireEnergyCannon.maxRefireCount && this.isAuthority)
        this.outer.SetNextState((EntityState)new FireTriJawCannon()
        {
          currentRefire = (this.currentRefire + 1)
        });
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }

    private Vector3 TweakedApplySpread(Vector3 direction, float angle, float bonusYaw, float bonusPitch)
    {
      Vector3 up = Vector3.up;
      Vector3 axis1 = Vector3.Cross(up, direction);
      float x = UnityEngine.Random.Range(1, 2);
      Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360f)) * (Quaternion.Euler(angle, 0.0f, 0.0f) * Vector3.forward);
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
