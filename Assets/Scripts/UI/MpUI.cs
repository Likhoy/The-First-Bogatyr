using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MpUI : MonoBehaviour
{
    [SerializeField] private Image mpBar;
    [SerializeField] private TMP_Text mpText;

    private GameObject player;

    void Awake()
    {
        player = GameManager.Instance.GetPlayer().gameObject;
    }

    void Update()
    {
        if (!player)
        {
            Destroy(gameObject);
            return;
        }
        Status stat = player.GetComponent<Status>();

        int maxMp = stat.totalStat.mana;
        float mp = stat.mana;
        float curMp = mp / maxMp;

        if (curMp > mpBar.fillAmount)
        {
            mpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
            if (mpBar.fillAmount > curMp)
            {
                mpBar.fillAmount = curMp;
            }
        }
        if (curMp < mpBar.fillAmount)
        {
            mpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
            if (mpBar.fillAmount < curMp)
            {
                mpBar.fillAmount = curMp;
            }
        }

        if (mpText)
        {
            mpText.text = mp.ToString() + "/" + maxMp.ToString();
        }
    }
}
