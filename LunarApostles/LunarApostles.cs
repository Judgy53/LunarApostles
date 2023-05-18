using BepInEx;
using RoR2;
using RoR2.Projectile;
using RoR2.CharacterAI;
using R2API;
using EntityStates;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LunarApostles
{
  [BepInPlugin("com.Nuxlar.LunarApostles", "LunarApostles", "1.0.0")]

  public class LunarApostles : BaseUnityPlugin
  {
    public static List<GameObject> timeCrystals;
    public static GameObject severPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/MoonExitArenaOrbEffect.prefab").WaitForCompletion();
    public static GameObject wispBomb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab").WaitForCompletion();
    public static GameObject trijawProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Scav/ScavEnergyCannonProjectile.prefab").WaitForCompletion().InstantiateClone("TriJawProjectile");

    private static GameObject trijawProjectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Scav/ScavEnergyCannonGhost.prefab").WaitForCompletion().InstantiateClone("TriJawProjectileGhost");
    private static GameObject kipkipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Body.prefab").WaitForCompletion().InstantiateClone("KipkipApostle");
    private static GameObject wipwipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Body.prefab").WaitForCompletion().InstantiateClone("WipwipApostle");
    private static GameObject kipkipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Master.prefab").WaitForCompletion();
    private static GameObject wipwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Master.prefab").WaitForCompletion();
    private static GameObject twiptwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar3Master.prefab").WaitForCompletion();
    private static GameObject guraguraMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar4Master.prefab").WaitForCompletion();

    public void Awake()
    {
      GivePickupsOnStart[] pickups = kipkipMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);
      pickups = wipwipMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);
      pickups = twiptwipMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);
      pickups = guraguraMaster.GetComponents<GivePickupsOnStart>();
      foreach (GivePickupsOnStart pickup in pickups)
        GameObject.Destroy(pickup);

      // kms
      CharacterBody body1 = wipwipBody.GetComponent<CharacterBody>();
      body1.baseMaxHealth = 3800;
      body1.levelMaxHealth = 1140;

      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;

      wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepTriJawCannon));
      // wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(OrbBarrage));
      // wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterShockwaveSit));

      // Wipwip Changes
      wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepTriJawCannon));

      // Hooks
      On.RoR2.SceneDirector.Start += SceneDirector_Start;
    }

    private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
    {
      if (SceneManager.GetActiveScene().name == "limbo")
        LunarApostles.timeCrystals = new();
      orig(self);
    }

    private void SetupClones()
    {
      ContentAddition.AddBody(kipkipBody);
      ContentAddition.AddBody(wipwipBody);
    }

    private void SetupProjectiles()
    {
      trijawProjectile.transform.localScale = new Vector3(1, 1, 1);
      trijawProjectileGhost.transform.localScale = new Vector3(10, 10, 10);
      ProjectileController projectileController = trijawProjectile.GetComponent<ProjectileController>();
      projectileController.cannotBeDeleted = true;
      projectileController.ghost = trijawProjectileGhost.GetComponent<ProjectileGhostController>();
      projectileController.ghostPrefab = trijawProjectileGhost;
      ContentAddition.AddProjectile(trijawProjectile);
    }

  }
}