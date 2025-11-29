using System.Collections;
using UnityEngine;

public class BossEnemyCharacterManager : EnemyCharacterManager
{
    public string bossID = "";
    [SerializeField] bool hasBeenDefeated = false;

    protected override void Start()
    {
        base.Start();

        if(WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
        }
        else
        {
            hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
            if(hasBeenDefeated)
            {
                isActive.SetBool(false);
            }
        }
    }

    public override IEnumerator HandleDeathEvents(bool manuallySelectDeathAnim = false)
    {
        characterStatManager.currentHealth.SetFloat(0);
        isDead = true;

        //reset all flags that need to be reset

        //if we are not grounded, play arial death animation
        if (!manuallySelectDeathAnim)
        {
            characterAnimationManager.PlayTargetActionAnimation("Dead_01", true);
        }
        //play death vfx/sfx

        //if our save file does NOT contain this boss, add it now
        if(!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
        }
        else
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
        }

        WorldSaveGameManager.instance.SaveGame();

        yield return new WaitForSeconds(5);

        //award players some currency for slaying enemy 

        //disable the character
    }
}
