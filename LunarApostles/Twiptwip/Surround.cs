using RoR2;
using RoR2.Projectile;
using RoR2.Navigation;
using EntityStates;
using EntityStates.ScavMonster;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LunarApostles
{
  public class Surround : BaseState
  {
    public static float baseDuration;
    public static string sound;
    public static float damageCoefficient;
    public static string attackSoundString;
    public static float timeToTarget = 3f;
    public static int projectileCount;
    private float duration;
    private ChildLocator childLocator;

    public override void OnEnter()
    {
      base.OnEnter();
      this.duration = (ThrowSack.baseDuration * 2) / this.attackSpeedStat;
      int num = (int)Util.PlayAttackSpeedSound(ThrowSack.sound, this.gameObject, this.attackSpeedStat);
      this.PlayAnimation("Body", nameof(ThrowSack), "ThrowSack.playbackRate", this.duration);
      Transform modelTransform = this.GetModelTransform();
      if (!(bool)(UnityEngine.Object)modelTransform)
        return;
      this.childLocator = modelTransform.GetComponent<ChildLocator>();
      if (!(bool)(UnityEngine.Object)this.childLocator)
        return;
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      RainFire(0.2f, 8f, 20f);
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }

    public static void RainFire(float meteorInterval, float meteorRadius, float meteorBaseDamage)
    {
      if (!NetworkServer.active)
        return;
      Vector2 vector2 = 150f * UnityEngine.Random.insideUnitCircle;
      Vector3 meteorPosition = new Vector3(vector2.x, 0f, vector2.y);
      RaycastHit hitInfo;
      if (Physics.Raycast(new Ray(meteorPosition, Vector3.down), out hitInfo, 500f, (int)LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
        meteorPosition.y = hitInfo.point.y;
      meteorPosition += Vector3.up * 0.2f;
      RoR2Application.fixedTimeTimers.CreateTimer(UnityEngine.Random.Range(0.0f, meteorInterval), (Action)(() =>
      {
        EffectManager.SpawnEffect(LunarApostles.meteorStormController.warningEffectPrefab, new EffectData()
        {
          origin = meteorPosition,
          scale = meteorRadius
        }, true);
        RoR2Application.fixedTimeTimers.CreateTimer(2f, (Action)(() =>
        {
          EffectManager.SpawnEffect(LunarApostles.meteorStormController.impactEffectPrefab, new EffectData()
          {
            origin = meteorPosition
          }, true);
          new BlastAttack()
          {
            baseDamage = (meteorBaseDamage * (float)(0.800000011920929 + 0.200000002980232 * (double)Run.instance.ambientLevelFloor)),
            crit = false,
            falloffModel = BlastAttack.FalloffModel.None,
            bonusForce = Vector3.zero,
            damageColorIndex = DamageColorIndex.Default,
            position = meteorPosition,
            procChainMask = new ProcChainMask(),
            procCoefficient = 1f,
            teamIndex = TeamIndex.Monster,
            radius = meteorRadius
          }.Fire();
        }));
      }));
    }

  }
}