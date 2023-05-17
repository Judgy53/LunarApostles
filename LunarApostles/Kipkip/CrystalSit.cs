using RoR2;
using RoR2.Navigation;
using System.Collections.Generic;
using EntityStates;
using EntityStates.ScavMonster;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace LunarApostles
{
  public class CrystalSit
  {
    private static GameObject timeCrystal = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WeeklyRun/TimeCrystalBody.prefab").WaitForCompletion();
    public CrystalSit()
    {
      On.EntityStates.ScavMonster.FindItem.OnEnter += FindItem_OnEnter;
      On.EntityStates.ScavMonster.FindItem.OnExit += FindItem_OnExit;
      On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
    }

    private void FindItem_OnEnter(On.EntityStates.ScavMonster.FindItem.orig_OnEnter orig, EntityStates.ScavMonster.FindItem self)
    {
      if (self.characterBody.name == "ScavLunar1Body(Clone)")
      {
        SpawnCrystals(self.characterBody);
        self.outer.SetState((EntityState)new ExitSit());
      }
      else
        orig(self);
    }

    private void FindItem_OnExit(On.EntityStates.ScavMonster.FindItem.orig_OnExit orig, EntityStates.ScavMonster.FindItem self)
    {
      if (self.characterBody.name != "ScavLunar1Body(Clone)")
        orig(self);
    }

    private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
    {
      orig(self);
      if (self && self.name == "TimeCrystalBody(Clone)" && SceneManager.GetActiveScene().name == "limbo")
      {
        int count = LunarApostles.timeCrystals.Count;
        if (count != 1)
          LunarApostles.timeCrystals.RemoveAt(0);
        else
        {
          LunarApostles.timeCrystals.RemoveAt(0);
          GameObject.Find("ScavLunar1Body(Clone)").GetComponent<CharacterBody>().RemoveBuff(RoR2Content.Buffs.Immune);
        }
      }
    }

    private void SpawnCrystals(CharacterBody body)
    {
      body.AddBuff(RoR2Content.Buffs.Immune);
      LunarApostles.timeCrystals = new();
      for (int i = 0; i < 3; i++)
      {
        NodeGraph groundNodes = SceneInfo.instance.groundNodes;
        if (!(bool)(Object)groundNodes)
          return;
        List<NodeGraph.NodeIndex> withFlagConditions = groundNodes.GetActiveNodesForHullMaskWithFlagConditions(HullMask.Golem, NodeFlags.None, NodeFlags.NoCharacterSpawn);
        NodeGraph.NodeIndex nodeIndex = withFlagConditions[Random.Range(0, withFlagConditions.Count)];
        Vector3 position;
        groundNodes.GetNodePosition(nodeIndex, out position);
        LunarApostles.timeCrystals.Add(GameObject.Instantiate(timeCrystal, position, Quaternion.identity));
      }

    }
  }
}