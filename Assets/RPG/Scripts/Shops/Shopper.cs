using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        Shop activeItemShop = null;

        public event Action activeShopChange;
        public void SetActiveShop(Shop shop)
        {
            if (activeItemShop != null)
            {
                activeItemShop.SetShopper(null);
            }
            activeItemShop = shop;
            if (activeItemShop != null)
            {
                activeItemShop.SetShopper(this);
            }
            if (activeShopChange != null)
            {
                activeShopChange();
            }
        }


        public Shop GetActiveShop()
        {
            return activeItemShop;
        }

    }
}
