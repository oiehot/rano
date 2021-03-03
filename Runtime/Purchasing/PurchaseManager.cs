// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;

namespace YOUR_PROJECT
{
    public class PurchaseManager : Singleton<PurchaseManager>
    {
        private IAPListener iapListener;

        void Awake()
        {
            Log.Info("Awake");

            var listener = this.gameObject.GetComponent<IAPListener>();
            if (listener == null)
            {
                this.iapListener = this.gameObject.AddComponent<IAPListener>();
                this.iapListener.onPurchaseComplete = new IAPListener.OnPurchaseCompletedEvent();
                this.iapListener.onPurchaseFailed = new IAPListener.OnPurchaseFailedEvent();                
            }
            else
            {
                this.iapListener = listener;
            }
            this.iapListener.consumePurchase = true;
            this.iapListener.dontDestroyOnLoad = false;
        }

        void OnEnable()
        {
            Log.Info("OnEnable");
            this.iapListener.onPurchaseComplete.AddListener(this.OnPurchaseComplete);
            this.iapListener.onPurchaseFailed.AddListener(this.OnPurchaseFailed);
        }

        void OnDisable()
        {
            Log.Info("OnDisable");
            this.iapListener.onPurchaseComplete.RemoveListener(this.OnPurchaseComplete);
            this.iapListener.onPurchaseFailed.RemoveListener(this.OnPurchaseFailed);
        }        

        public void OnPurchaseComplete(Product product)
        {
            if (product.definition.id == "com.oiehot.afo2.iap.bomb1")
            {
                GameManager.Instance.data.bombCount.Value += 10;
            }
            else if (product.definition.id == "com.oiehot.afo2.iap.bomb2")
            {
                GameManager.Instance.data.bombCount.Value += 25;
            }
            else if (product.definition.id == "com.oiehot.afo2.iap.bomb5")
            {
                GameManager.Instance.data.bombCount.Value += 70;
            }
            else if (product.definition.id == "com.oiehot.afo2.iap.bomb10")
            {
                GameManager.Instance.data.bombCount.Value += 150;
            } 
            else if (product.definition.id == "com.oiehot.afo2.iap.bomb20")
            {            
                GameManager.Instance.data.bombCount.Value += 350;
            } 
            else if (product.definition.id == "com.oiehot.afo2.iap.bomb50")
            {             
                GameManager.Instance.data.bombCount.Value += 1000;
            }
            Log.Important($"OnPurchaseCompleted: {product.definition.id}"); // TODO: id ? storeSpecificId
            
            // 중요한 데이터가 변경되었으므로 바로 데이터를 저장한다.
            // TODO: 아이템이 많은 경우 너무 많이 호출되는건 아닐까?
                // GameManager.Instance.SaveGame();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Log.Warning($"OnPurchaseFailed: product:{product.definition.storeSpecificId}, reason:{reason}");            
        }
    }
}

#endif