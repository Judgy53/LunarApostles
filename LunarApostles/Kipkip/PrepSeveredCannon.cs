using RoR2;
using UnityEngine;
using EntityStates;
using EntityStates.ScavMonster;

namespace LunarApostles
{
  public class PrepSeveredCannon : SeveredCannonState
  {
    public static float baseDuration;
    public static string sound;
    public static GameObject chargeEffectPrefab;
    private GameObject chargeInstance;
    private float duration;

    public override void OnEnter()
    {
      base.OnEnter();
      this.duration = PrepEnergyCannon.baseDuration / this.attackSpeedStat;
      this.PlayCrossfade("Body", nameof(PrepEnergyCannon), "PrepEnergyCannon.playbackRate", this.duration, 0.1f);
      int num = (int)Util.PlaySound(PrepEnergyCannon.sound, this.gameObject);
      if (!(bool)(Object)this.muzzleTransform || !(bool)(Object)PrepEnergyCannon.chargeEffectPrefab)
        return;
      this.chargeInstance = Object.Instantiate<GameObject>(PrepEnergyCannon.chargeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
      this.chargeInstance.transform.parent = this.muzzleTransform;
      ScaleParticleSystemDuration component = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
      if (!(bool)(Object)component)
        return;
      component.newDuration = this.duration;
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      this.StartAimMode(0.5f);
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextState((EntityState)new FireSeveredCannon());
    }

    public override void OnExit()
    {
      base.OnExit();
      if (!(bool)(Object)this.chargeInstance)
        return;
      EntityState.Destroy((Object)this.chargeInstance);
    }
  }
}