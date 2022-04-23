#if USE_ESSENTIAL_KIT_BILLING_SERVICES && false

using System;
using System.Collections.Generic;
using UnityEngine;
using Rano;

namespace Rano.PlatformServices.Billing.EssentialKit
{
    public abstract class PurchaseRewarder : MonoBehaviour
    {
        private bool _activated;

        private Dictionary<string, Action> _rewardActions = new Dictionary<string, Action>();

        protected virtual void Awake()
        {
        }

        protected virtual void OnEnable()
        {
            _activated = true;
            if (PurchaseManager.Instance != null)
            {
                PurchaseManager.Instance.OnPurchaseComplete += HandlePurchaseComplete;
                PurchaseManager.Instance.OnPurchaseFailed += HandlePurchaseFailed;
                PurchaseManager.Instance.OnRestorePurchase += HandleRestorePurchase;
            }
        }

        protected virtual void OnDisable()
        {
            _activated = false;
            if (PurchaseManager.Instance != null)
            {
                PurchaseManager.Instance.OnPurchaseComplete -= HandlePurchaseComplete;
                PurchaseManager.Instance.OnPurchaseFailed -= HandlePurchaseFailed;
                PurchaseManager.Instance.OnRestorePurchase -= HandleRestorePurchase;
            }
        }

        protected virtual void HandlePurchaseComplete(string productId)
        {
            Reward(productId);
        }

        protected virtual void HandlePurchaseFailed(string productId, string errorMessage)
        {
        }

        protected virtual void HandleRestorePurchase(string productId)
        {
            Log.Info($"구매했던 상품을 복구합니다. ({productId})");
            Reward(productId);
        }

        protected void AddRewardAction(string productId, Action action)
        {
            _rewardActions.Add(productId, action);
        }

        protected void RemoveRewardAction(string productId)
        {
            _rewardActions.Remove(productId);
        }

        protected void RemoveAllRewardActions()
        {
            _rewardActions.Clear();
        }

        private void Reward(string productId)
        {
            if (_activated == false)
            {
                throw new Exception($"컴포넌트가 꺼져있어서 {productId} 상품의 보상을 처리할 수 없습니다.");
            }

            if (_rewardActions.ContainsKey(productId) == true)
            {
                _rewardActions[productId]();
            }
            else
            {
                throw new Exception($"{productId} 상품보상을 처리하는 함수가 추가되어있지 않습니다.");
            }
        }
    }
}
#endif