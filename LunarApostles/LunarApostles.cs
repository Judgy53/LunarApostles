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
    public static MeteorStormController meteorStormController = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Meteor/MeteorStorm.prefab").WaitForCompletion().GetComponent<MeteorStormController>();

    private static GameObject kipkipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Body.prefab").WaitForCompletion();
    private static GameObject wipwipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Body.prefab").WaitForCompletion();
    private static GameObject kipkipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Master.prefab").WaitForCompletion();
    private static GameObject wipwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar2Master.prefab").WaitForCompletion();
    private static GameObject twiptwipMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar3Master.prefab").WaitForCompletion();
    private static GameObject guraguraMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar4Master.prefab").WaitForCompletion();

    public void Awake()
    {
      // Setup
      SetupSkillStates();
      // Hooks
      On.RoR2.SceneDirector.Start += SceneDirector_Start;

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

      //  Master Changes
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;
      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      wipwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;
      twiptwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      twiptwipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;
      guraguraMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      guraguraMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;
      // wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepTriJawCannon));
      // wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(OrbBarrage));
      // wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterShockwaveSit));

      // kms
      CharacterBody body1 = wipwipBody.GetComponent<CharacterBody>();
      body1.baseMaxHealth = 3800;
      body1.levelMaxHealth = 1140;
      // Wipwip Changes
      wipwipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepBlunderbuss));
      wipwipBody.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(ArtilleryBarrage));
      wipwipBody.GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EnterMineSit));
    }

    private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
    {
      if (SceneManager.GetActiveScene().name == "limbo")
        LunarApostles.timeCrystals = new();
      orig(self);
    }

    private void SetupSkillStates()
    {
      // Kipkip
      ContentAddition.AddEntityState<BaseShockwaveSitState>(out _);
      ContentAddition.AddEntityState<EnterShockwaveSit>(out _);
      ContentAddition.AddEntityState<ExitShockwaveSit>(out _);
      ContentAddition.AddEntityState<ShockwaveSit>(out _);
      ContentAddition.AddEntityState<SeveredCannonState>(out _);
      ContentAddition.AddEntityState<PrepSeveredCannon>(out _);
      ContentAddition.AddEntityState<FireSeveredCannon>(out _);
      ContentAddition.AddEntityState<OrbBarrage>(out _);
      // Wipwip
      ContentAddition.AddEntityState<BaseMineSitState>(out _);
      ContentAddition.AddEntityState<EnterMineSit>(out _);
      ContentAddition.AddEntityState<ExitMineSit>(out _);
      ContentAddition.AddEntityState<MineSit>(out _);
      ContentAddition.AddEntityState<BlunderbussState>(out _);
      ContentAddition.AddEntityState<PrepBlunderbuss>(out _);
      ContentAddition.AddEntityState<FireBlunderbuss>(out _);
      ContentAddition.AddEntityState<ArtilleryBarrage>(out _);
    }

  }
}