using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.ScavMonster;
using UnityEngine;

namespace LunarApostles
{
  public class FireLuckyCannon : LuckyCannonState
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

    public override void OnEnter()
    {
      base.OnEnter();
      // Lucky part of the cannon
      if ((double)UnityEngine.Random.value < (double)0.25)
        this.currentRefire--;
      this.duration = FireEnergyCannon.baseDuration / this.attackSpeedStat;
      this.refireDuration = FireEnergyCannon.baseRefireDuration / this.attackSpeedStat;
      int num1 = (int)Util.PlayAttackSpeedSound(FireEnergyCannon.sound, this.gameObject, this.attackSpeedStat);
      this.PlayCrossfade("Body", nameof(FireEnergyCannon), "FireEnergyCannon.playbackRate", this.duration, 0.1f);
      this.AddRecoil(-2f * FireEnergyCannon.recoilAmplitude, -3f * FireEnergyCannon.recoilAmplitude, -1f * FireEnergyCannon.recoilAmplitude, 1f * FireEnergyCannon.recoilAmplitude);
      if ((bool)(Object)FireEnergyCannon.effectPrefab)
        EffectManager.SimpleMuzzleFlash(FireEnergyCannon.effectPrefab, this.gameObject, EnergyCannonState.muzzleName, false);
      if (!this.isAuthority)
        return;
      float num2 = this.currentRefire % 2 == 0 ? 1f : -1f;
      float num3 = Mathf.Ceil((float)this.currentRefire / 2f) * FireEnergyCannon.projectileYawBonusPerRefire;
      for (int index = 0; index < FireEnergyCannon.projectileCount; ++index)
      {
        Ray aimRay = this.GetAimRay();
        aimRay.direction = Util.ApplySpread(aimRay.direction, FireEnergyCannon.minSpread, FireEnergyCannon.maxSpread, 1f, 1f, num2 * num3, FireEnergyCannon.projectilePitchBonus);
        ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master));
      }
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge >= (double)this.refireDuration && this.currentRefire + 1 < FireEnergyCannon.maxRefireCount && this.isAuthority)
        this.outer.SetNextState((EntityState)new FireEnergyCannon()
        {
          currentRefire = (this.currentRefire + 1)
        });
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }
  }
}
