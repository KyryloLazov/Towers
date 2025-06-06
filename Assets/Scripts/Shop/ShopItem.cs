using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private BaseTurretConfig _turretConfig;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _priceText;

    private void Awake()
    {
        _priceText.text = $"{_turretConfig.Cost}$";
        _buyButton.onClick.AddListener(OnBuyButtonPressed);
    }

    private void OnBuyButtonPressed()
    {
        BuildManager.Instance.SelectTurretToBuild(_turretConfig);
    }

    private void OnDestroy()
    {
        _buyButton.onClick.RemoveAllListeners();
    }
}
