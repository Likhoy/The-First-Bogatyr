using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ProductBoughtEvent : MonoBehaviour
{
    public event Action<ProductBoughtEvent, ProductBoughtEventArgs> OnBuyProduct;

    public void CallProductBoughtEvent(int playerMoney)
    {
        OnBuyProduct?.Invoke(this, new ProductBoughtEventArgs() { playerMoney = playerMoney });
    }
}


public class ProductBoughtEventArgs : EventArgs
{
    public int playerMoney;
}
