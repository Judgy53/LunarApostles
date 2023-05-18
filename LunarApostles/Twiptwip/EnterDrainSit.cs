using RoR2;
using EntityStates;
using EntityStates.ScavMonster;

namespace LunarApostles
{
  public class EnterDrainSit : BaseDrainSitState
  {
    public static float baseDuration;
    public static string soundString;
    private float duration;

    public override void OnEnter()
    {
      base.OnEnter();
      this.duration = EnterSit.baseDuration / this.attackSpeedStat;
      int num = (int)Util.PlaySound(EnterSit.soundString, this.gameObject);
      this.PlayCrossfade("Body", nameof(EnterSit), "Sit.playbackRate", this.duration, 0.1f);
      this.modelLocator.normalizeToFloor = true;
      this.modelLocator.modelTransform.GetComponent<AimAnimator>().enabled = true;
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge < (double)this.duration)
        return;
      this.outer.SetNextState((EntityState)new DrainSit());
    }
  }
}
