using BepInEx;
using RoR2;
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

    private static GameObject kipkipBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ScavLunar/ScavLunar1Body.prefab").WaitForCompletion();
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

      // Kipkip changes
      CharacterBody body1 = kipkipBody.GetComponent<CharacterBody>();
      body1.baseMaxHealth = 3800;
      body1.levelMaxHealth = 1140;

      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;

      kipkipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepTriJawCannon));
      new CircularToss();
      new CrystalSit();

      CharacterBody body2 = kipkipBody.GetComponent<CharacterBody>();
      body2.baseMaxHealth = 3800;
      body2.levelMaxHealth = 1140;

      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Secondary).First<AISkillDriver>().maxUserHealthFraction = 0.95f;
      kipkipMaster.GetComponents<AISkillDriver>().Where<AISkillDriver>(x => x.skillSlot == SkillSlot.Utility).First<AISkillDriver>().maxUserHealthFraction = 0.90f;

      kipkipBody.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(PrepTriJawCannon));
      new CircularToss();
      new CrystalSit();

      // Hooks
      On.RoR2.SceneDirector.Start += SceneDirector_Start;
    }

    private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
    {
      if (SceneManager.GetActiveScene().name == "limbo")
        LunarApostles.timeCrystals = new();
      orig(self);
    }

  }
}