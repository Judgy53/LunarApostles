using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.ScavMonster;
using UnityEngine;

namespace LunarApostles
{
  public class FireSeveredCannon : SeveredCannonState
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
    private ChildLocator childLocator;
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
      speedOverride = 65;
      refireDurationBase = 0.75f;
      firstThreshold = this.healthComponent.health <= (this.healthComponent.fullHealth * 0.75); // 75% HP
      secondThreshold = this.healthComponent.health <= (this.healthComponent.fullHealth * 0.5); // 50% HP
      if (firstThreshold)
      {
        speedOverride = 75;
        refireDurationBase = 0.5f;
      }
      if (secondThreshold)
      {
        speedOverride = 85;
        refireDurationBase = 0.25f;
      }
      Transform modelTransform = this.GetModelTransform();
      if (!(bool)(Object)modelTransform)
        return;
      this.childLocator = modelTransform.GetComponent<ChildLocator>();
      if (!(bool)(Object)this.childLocator)
        return;
      this.duration = FireEnergyCannon.baseDuration / this.attackSpeedStat;
      this.refireDuration = refireDurationBase / this.attackSpeedStat;
      int num1 = (int)Util.PlayAttackSpeedSound(FireEnergyCannon.sound, this.gameObject, this.attackSpeedStat);
      this.PlayCrossfade("Body", nameof(FireEnergyCannon), "FireEnergyCannon.playbackRate", this.duration, 0.1f);
      this.AddRecoil(-2f * FireEnergyCannon.recoilAmplitude, -3f * FireEnergyCannon.recoilAmplitude, -1f * FireEnergyCannon.recoilAmplitude, 1f * FireEnergyCannon.recoilAmplitude);
      if ((bool)(Object)FireEnergyCannon.effectPrefab)
        EffectManager.SimpleMuzzleFlash(LunarApostles.severPrefab, this.gameObject, EnergyCannonState.muzzleName, false);
      if (!this.isAuthority)
        return;
      Transform child = this.childLocator.FindChild(EnergyCannonState.muzzleName);
      if ((bool)(Object)child)
      {
        Ray aimRay = this.GetAimRay();
        Ray projectileRay = new Ray();
        projectileRay.direction = aimRay.direction;
        float maxDistance = 1000f;
        float randX = UnityEngine.Random.Range(-25f, 25f);
        float randY = UnityEngine.Random.Range(10f, 25f);
        float randZ = UnityEngine.Random.Range(-25f, 25f);
        Vector3 randVector = new Vector3(randX, randY, randZ);
        Vector3 position = child.position + randVector;
        projectileRay.origin = position;
        RaycastHit hitInfo;
        {
          if (Physics.Raycast(aimRay, out hitInfo, maxDistance, (int)LayerIndex.world.mask))
          {
            projectileRay.direction = hitInfo.point - projectileRay.origin;
            FireCannon(projectileRay, 0.0f, 0.0f);
          }
        }
      }
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      if ((double)this.fixedAge >= (double)this.refireDuration && this.currentRefire + 1 < FireEnergyCannon.maxRefireCount && this.isAuthority)
        this.outer.SetNextState((EntityState)new FireSeveredCannon()
        {
          currentRefire = (this.currentRefire + 1)
        });
      if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
        return;
      this.outer.SetNextStateToMain();
    }

    private void FireCannon(Ray projectileRay, float bonusPitch, float bonusYaw)
    {
      EffectManager.SpawnEffect(LunarApostles.severPrefab, new EffectData { origin = projectileRay.origin, rotation = Util.QuaternionSafeLookRotation(projectileRay.direction) }, false);
      for (int index = 0; index < FireEnergyCannon.projectileCount; ++index)
      {
        projectileRay.direction = Util.ApplySpread(projectileRay.direction, FireEnergyCannon.minSpread, FireEnergyCannon.maxSpread, 1f, 1f, 1, FireEnergyCannon.projectilePitchBonus);
        ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), this.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, this.characterBody.master), speedOverride: speedOverride);
      }
    }

  }
}
