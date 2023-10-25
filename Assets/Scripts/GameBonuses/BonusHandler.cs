using System.Collections.Generic;
using System.Linq;

public static class BonusHandler
{
    public static List<BonusDetailsSO> GetRandomBonusesForWave(int waveNumber)
    {
        WaveDetailsSO currentWaveDetails = GameManager.Instance.allWavesDetails[waveNumber - 1];

        System.Random r = new System.Random();
        int[] bonusesNums = Enumerable.Range(0, currentWaveDetails.possibleBonuses.Count - 1).ToArray();
        r.Shuffle(bonusesNums);

        var chosenBonusDetails = new List<BonusDetailsSO>();
        for (int i = 0; i < Settings.waveBonusesNumber; i++)
        {
            chosenBonusDetails.Add(currentWaveDetails.possibleBonuses[bonusesNums[i]]);
        }
        return chosenBonusDetails;
    }

    public static bool ApplyBonus(BonusDetailsSO bonusDetails)
    {
        switch (bonusDetails)
        {
            case ItemBonusDetailsSO itemBonusDetails: 
                return ApplyItemBonus(itemBonusDetails);

            case PowerBonusDetailsSO powerBonusDetails: 
                return ApplyPowerBonus(powerBonusDetails);

            case ResurrectorBonusDetailsSO resurrectorBonusDetails:
                return ApplyResurrectorBonus(resurrectorBonusDetails);

            default: return false;
        }
    }

    private static bool ApplyResurrectorBonus(ResurrectorBonusDetailsSO bonusDetails)
    {
        return GameManager.Instance.GetPlayer().health.AddExtraLives(bonusDetails.livesReserve);
    }

    private static bool ApplyPowerBonus(PowerBonusDetailsSO bonusDetails)
    {
        switch (bonusDetails.bonusType)
        {
            case PowerBonusType.Armor:
                return GameManager.Instance.GetPlayer().health.AddArmorProtectionPercent(bonusDetails.bonusPercent);
            
            case PowerBonusType.DamageReflector:
                return GameManager.Instance.GetPlayer().health.AddChanceToAvoidDamage(bonusDetails.bonusPercent);

            case PowerBonusType.HealthBoost:
                return GameManager.Instance.GetPlayer().health.IncreaseMaxHealth(bonusDetails.bonusPercent);

            default: return false;
        }
    }

    private static bool ApplyItemBonus(ItemBonusDetailsSO bonusDetails)
    {
        for (int i = 0; i < bonusDetails.itemNumber; i++)
        {
            GameManager.Instance.GiveItem(bonusDetails.itemGiven);
        }
        return true;
    }
}
