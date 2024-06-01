using System.Collections.Generic;
using System.Linq;

public static class BonusHandler
{
    public static List<BonusDetailsSO> GetRandomBonusesForWave(int waveNumber)
    {
        WaveDetailsSO currentWaveDetails = GameManager.Instance.allWavesDetails[waveNumber - 1];

        // if no bonuses
        if (currentWaveDetails.possibleBonuses.Count == 0)
            return null;

        System.Random r = new System.Random();
        int[] bonusesNums = Enumerable.Range(0, currentWaveDetails.possibleBonuses.Count).ToArray();
        r.Shuffle(bonusesNums);

        var chosenBonusDetails = new List<BonusDetailsSO>();
        for (int i = 0; i < Settings.waveBonusesNumber; i++)
        {
            chosenBonusDetails.Add(currentWaveDetails.possibleBonuses[bonusesNums[i]]);
        }
        return chosenBonusDetails;
    }

    public static void ApplyBonus(BonusDetailsSO bonusDetails)
    {
        switch (bonusDetails)
        {
            case ItemBonusDetailsSO itemBonusDetails: 
                ApplyItemBonus(itemBonusDetails);
                break;

            case PowerBonusDetailsSO powerBonusDetails: 
                ApplyPowerBonus(powerBonusDetails);
                break;

            case ResurrectorBonusDetailsSO resurrectorBonusDetails:
                ApplyResurrectorBonus(resurrectorBonusDetails);
                break;
            
            default: break;
        }
    }

    private static void ApplyResurrectorBonus(ResurrectorBonusDetailsSO bonusDetails)
    {
        GameManager.Instance.GetPlayer().health.AddExtraLives(bonusDetails.livesReserve);
    }

    private static void ApplyPowerBonus(PowerBonusDetailsSO bonusDetails)
    {
        switch (bonusDetails.bonusType)
        {
            case PowerBonusType.Armour:
                Protection.AddProtection<Armour>(GameManager.Instance.GetPlayer().health, 
                    bonusDetails);
                break;

            case PowerBonusType.VirtualArmour:
                Protection.AddProtection<VirtualArmour>(GameManager.Instance.GetPlayer().health,
                    bonusDetails);
                break;

            case PowerBonusType.DamageReflector:
                Protection.AddProtection<DamageReflector>(GameManager.Instance.GetPlayer().health,
                    bonusDetails);
                break;

            case PowerBonusType.HealthBoost:
                GameManager.Instance.GetPlayer().health.IncreaseMaxHealth(bonusDetails.bonusPercent);
                break;

            default: break;
        }
    }

    private static void ApplyItemBonus(ItemBonusDetailsSO bonusDetails)
    {
        for (int i = 0; i < bonusDetails.itemNumber; i++)
        {
            GameManager.Instance.GiveItem(bonusDetails.itemGiven);
        }
    }
}
