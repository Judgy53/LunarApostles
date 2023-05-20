using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.LunarExploderMonster.Weapon;
using EntityStates.LunarGolem;
using EntityStates.ScavMonster;
using UnityEngine;

namespace LunarApostles
{
  public class FireLuckyCannon : LuckyCannonState
  {
    private GameObject effectPrefab = FireTwinShots.effectPrefab;
    private GameObject projectilePrefab = FireTwinShots.projectilePrefab;
    private string attackSoundString = FireTwinShots.attackSoundString;
    private int refireIndex = 0;
    private float duration;

    public override void OnEnter()
    {
      base.OnEnter();
      this.duration = 0.15f;
      float recoil = FireEnergyCannon.recoilAmplitude / 2;
      int num1 = (int)Util.PlayAttackSpeedSound(FireEnergyCannon.sound, this.gameObject, this.attackSpeedStat);
      this.PlayCrossfade("Body", nameof(FireEnergyCannon), "FireEnergyCannon.playbackRate", this.duration, 0.1f);
      this.AddRecoil(-2f * recoil, -3f * recoil, -1f * recoil, 1f * recoil);
      if ((bool)(Object)FireEnergyCannon.effectPrefab)
        EffectManager.SimpleMuzzleFlash(FireEnergyCannon.effectPrefab, this.gameObject, EnergyCannonState.muzzleName, false);
      if (!this.isAuthority)
        return;
      Fire();
    }


    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge < (double)this.duration)
        return;
      if ((int)this.refireIndex < (int)3)
        this.outer.SetNextState((EntityState)new FireLuckyCannon()
        {
          refireIndex = (this.refireIndex + 1)
        });
      else
        this.outer.SetNextStateToMain();
    }


    private void Fire()
    {
      Ray ray = this.GetAimRay();
      int num = this.refireIndex;
      FireExploderShards shardInstance = new FireExploderShards();
      for (; this.refireIndex <= num; ++this.refireIndex)
      {
        ProjectileManager.instance.FireProjectile(shardInstance.projectilePrefab, ray.origin, Util.QuaternionSafeLookRotation(ray.direction), this.gameObject, this.damageStat * shardInstance.damageCoefficient, shardInstance.force, Util.CheckRoll(this.critStat, this.characterBody.master));
        Util.PlaySound(this.attackSoundString, this.gameObject);
        ProjectileManager.instance.FireProjectile(this.projectilePrefab, ray.origin, Util.QuaternionSafeLookRotation(ray.direction, Vector3.down), this.gameObject, this.damageStat * FireTwinShots.damageCoefficient, FireTwinShots.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 70);
        Util.PlaySound(this.attackSoundString, this.gameObject);
        ProjectileManager.instance.FireProjectile(this.projectilePrefab, ray.origin, Util.QuaternionSafeLookRotation(ray.direction, Vector3.up), this.gameObject, this.damageStat * FireTwinShots.damageCoefficient, FireTwinShots.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 70);
        Util.PlaySound(this.attackSoundString, this.gameObject);
        ProjectileManager.instance.FireProjectile(this.projectilePrefab, ray.origin, Util.QuaternionSafeLookRotation(ray.direction, Vector3.forward), this.gameObject, this.damageStat * FireTwinShots.damageCoefficient, FireTwinShots.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: 70);
      }
    }
  }
}
