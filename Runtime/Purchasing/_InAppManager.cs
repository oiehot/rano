// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Rano;

namespace AFO2
{
    public class IAPManager : Purchaser
    {
        override void Initialize()
        {
            // TODO: 부모의 Awake실행할것.
            AddProduct(
                new InAppProduct(
                    Constants.IAP.Bomb1.id, Constants.IAP.Bomb2.type, 
                    this.OnPurchaseBomb2, this.OnPurchaseFailed
                );
            );

            AddProduct(
                new InAppProduct(
                    Constants.IAP.Bomb2.id, Constants.IAP.Bomb2.type,
                    this.OnPurchaseBomb2, this.OnPurchaseFailed
                );
            );

        }
        public void OnPurchaseBomb1()
        {
            GameManager.Instance.data.bombCount += 10;
        }

        public void OnPurchaseBomb2()
        {
            GameManager.Instance.data.bombCount += 25;
        }

        public void OnPurchaseFailed()
        {
            Log.Warning("OnPurchaseFailed");
        }
    }
}

#endif