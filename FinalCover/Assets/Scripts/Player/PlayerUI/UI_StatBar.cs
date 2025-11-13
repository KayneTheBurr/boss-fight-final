using UnityEngine;
using UnityEngine.UI;


public class UI_StatBar : MonoBehaviour
{
    protected Slider slider;
    protected RectTransform rectTransform;

    [Header("Bar Option")]
    [SerializeField] protected bool scaleBarSizeWithStats = true;
    [SerializeField] protected float widthScaleBarMultiplier = 1f;
    //scale size of bar depending on stats(health/stamina/flux/etc)

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }
    protected virtual void Start()
    {

    }

    public virtual void SetStat(float newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(float maxValue)
    {
        
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarSizeWithStats)
        {
            //scale the rect transform as the stat that increases your resource bars is increased 
            rectTransform.sizeDelta = new Vector2(widthScaleBarMultiplier * maxValue, rectTransform.sizeDelta.y);

        }
        //refresh player ui hud in their layer group 
        if(PlayerUIManager.instance) PlayerUIManager.instance.playerHUDManager.RefreshHUD();
    }
}
