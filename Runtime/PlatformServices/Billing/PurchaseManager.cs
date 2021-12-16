// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rano;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Billing
{
    public enum PurchaseFailedReason
    {
        Unknown,
        AlreadyPurchased
    }

    public sealed class PurchaseManager : MonoSingleton<PurchaseManager>
    {
        /// <summary>
        /// PlatformId에 매칭되는 Product 객체를 담는 사전.
        /// PlatformId가 아닌 Settings에 적혀진 ProductId를 키로 사용한다.
        /// </summary>
        private Dictionary<string, IBillingProduct> _products;

        public UnityEvent<string> onPurchaseComplete;
        public UnityEvent<string, string> onPurchaseFailed;
        public UnityEvent<string> onRestorePurchase;

        void Awake()
        {
            _products = new Dictionary<string, IBillingProduct>();
            if (BillingServices.IsAvailable())
            {
                BillingServices.InitializeStore();
            }
            else
            {
                Log.Warning("인앱상품 결제서비스를 사용할 수 없습니다.");
            }
        }

        void OnEnable()
        {
            Log.Info("PurchaseManager Enabled");
            BillingServices.OnInitializeStoreComplete += OnInitializeStoreComplete;
            BillingServices.OnTransactionStateChange += OnTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete += OnRestorePurchasesComplete;
        }

        void OnDisable()
        {
            Log.Info("PurchaseManager Disabled");
            BillingServices.OnInitializeStoreComplete -= OnInitializeStoreComplete;
            BillingServices.OnTransactionStateChange -= OnTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete -= OnRestorePurchasesComplete;
        }

        private void OnInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
        {
            if (error == null)
            {
                IBillingProduct[] products = result.Products;
                Log.Info("인앱상품 결제서비스 초기화 완료.");
                Log.Info($"총 {products.Length} 개의 상품이 있습니다:");
                for (int i = 0; i < products.Length; i++)
                {
                    IBillingProduct product = products[i];
                    _products.Add(product.Id, product);
                    Log.Info($"[{i}] {product}");
                }
            }
            else
            {
                Log.Warning($"인앱상품 결제서비스 초기화 실패: {error}");
                return;
            }

            string[] invalidIds = result.InvalidProductIds;
            Log.Info($"총 {invalidIds.Length} 개의 잘못된 상품이 있습니다:");
            if (invalidIds.Length > 0)
            {
                for (int i = 0; i < invalidIds.Length; i++)
                {
                    Log.Info($"[{i}] {invalidIds[i]}");
                }
            }
        }

        /// <summary>
        /// 상품구매를 요청한뒤 결과가 리턴되었을 때 호출된다.
        /// </summary>
        /// <param name="result">결과</param>
        private void OnTransactionStateChange(BillingServicesTransactionStateChangeResult result)
        {
            IBillingTransaction[] transactions = result.Transactions;
            for (int i = 0; i < transactions.Length; i++)
            {
                IBillingTransaction transaction = transactions[i];
                switch (transaction.TransactionState)
                {
                    case BillingTransactionState.Purchased:
                        // TODO: 영수증 검증절차.
                        if (transaction.ReceiptVerificationState == BillingReceiptVerificationState.Success)
                        {
                            Log.Important($"상품구매 성공. ({transaction.Payment.ProductId})");
                            onPurchaseComplete.Invoke(transaction.Payment.ProductId);
                        }
                        else
                        {
                            Log.Warning($"상품구매 영수증 검증실패. ({transaction.Payment.ProductId})");
                        }
                        break;

                    case BillingTransactionState.Failed:
                        Log.Info($"상품구매 실패. ({transaction.Error.Description})");
                        onPurchaseFailed.Invoke(transaction.Payment.ProductId, transaction.Error.Description);
                        break;
                }
            }
        }

        /// <summary>
        /// 구매복구 요청 후 결과가 리턴되었을 때 호출된다.
        /// </summary>
        /// <param name="result">결과</param>
        /// <param name="error">에러</param>
        private void OnRestorePurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
        {
            if (error == null)
            {
                IBillingTransaction[] transactions = result.Transactions;
                Log.Info("구매복구 요청이 완료되었습니다.");
                Log.Info($"구매복구 할 상품은 총 {transactions.Length} 개 입니다:");
                for (int i = 0; i < transactions.Length; i++)
                {
                    IBillingTransaction transaction = transactions[i];
                    Log.Info($"[{i}]: {transaction.Payment.ProductId}");
                    onRestorePurchase.Invoke(transaction.Payment.ProductId);
                }
            }
            else
            {
                Log.Info($"구매복구 요청이 실패했습니다. ({error.Description})");
            }
        }

        /// <summary>
        /// 상품구매 요청.
        /// </summary>
        /// <param name="productId">상품 Id</param>
        /// <returns>구매요청 성공 여부</returns>
        public bool Purchase(string productId)
        {
            if (BillingServices.CanMakePayments() == false)
            {
                Log.Warning("결제서비스에서 지불기능을 사용할 수 없어 구매할 수 없습니다.");
                return false;
            }

            if (_products.ContainsKey(productId) == false)
            {
                Log.Warning($"구매에 실패했습니다. 상품 {productId}을 구매가능목록에서 찾을 수 없습니다.");
                return false;
            }

            IBillingProduct product = _products[productId];

            if (BillingServices.IsProductPurchased(product) == false)
            {
                Log.Info($"인앱상품 구매 요청: {product}");
                BillingServices.BuyProduct(product);
                return true;
            }
            else
            {
                Log.Warning($"이미 구매한 상품이어서 재구매할 수 없습니다: {product}");
                return false;
            }
        }

        /// <summary>
        /// 구매복구를 요청한다.
        /// </summary>
        [ContextMenu("Restore Purchases")]
        public void RestorePurchases()
        {
            Log.Info("구매복구 요청.");
            BillingServices.RestorePurchases();
        }

        /// <summary>
        /// 결제서비스 가능 여부 리턴.
        /// </summary>
        /// <returns>결제서비스 가능 여부</returns>
        public bool IsAvailable()
        {
            return BillingServices.IsAvailable();
        }        

        /// <summary>
        /// 상품의 구매가능 여부를 리턴한다.
        /// </summary>
        /// <param name="productId">상품Id</param>
        /// <returns></returns>
        public bool IsPurchased(string productId)
        {
            if (_products.ContainsKey(productId) == false)
            {
                throw new Exception($"구매여부 확인에 실패했습니다. 상품 {productId}을 구매가능목록에서 찾을 수 없습니다.");
            }

            IBillingProduct product = _products[productId];
            return BillingServices.IsProductPurchased(product);
        }

        public bool IsBuyable(string productId)
        {
            return _products.ContainsKey(productId);
        }
    }
}