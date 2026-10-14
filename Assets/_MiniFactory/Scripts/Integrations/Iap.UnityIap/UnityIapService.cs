using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniFactory.Core.Iap;
using UnityEngine;
using UnityEngine.Purchasing;

namespace MiniFactory.Iap.UnityIAP
{
    public sealed class UnityIapService : IIapService
    {
        public event Action<string> PurchaseSucceeded;
        public event Action<string, IapFailureReason> PurchaseFailed;
        public event Action Initialized;

        private readonly List<string> _productIds = new List<string>();

        private TaskCompletionSource<bool> _initializationSource;
        private IStoreService _storeService;
        private IProductService _productService;
        private IPurchaseService _purchaseService;

        public bool IsInitialized { get; private set; }
        public bool IsAvailable { get; private set; }

        public UnityIapService(IReadOnlyList<string> productIds)
        {
            if (productIds == null)
            {
                return;
            }

            for (int i = 0; i < productIds.Count; i++)
            {
                if (!string.IsNullOrEmpty(productIds[i]))
                {
                    _productIds.Add(productIds[i]);
                }
            }
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized)
            {
                return;
            }

            if (_initializationSource != null)
            {
                await _initializationSource.Task;
                return;
            }

            _initializationSource = new TaskCompletionSource<bool>();

            _storeService = UnityIAPServices.DefaultStore();
            _productService = UnityIAPServices.DefaultProduct();
            _purchaseService = UnityIAPServices.DefaultPurchase();

            _storeService.OnStoreConnected += OnStoreConnected;
            _storeService.OnStoreDisconnected += OnStoreDisconnected;
            _productService.OnProductsFetchFailed += OnProductsFetchFailed;
            _purchaseService.OnPurchasePending += OnPurchasePending;
            _purchaseService.OnPurchaseFailed += OnPurchaseFailed;

            try
            {
                await _storeService.Connect();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("IAP connection failed: " + exception.Message);
                IsInitialized = false;
                IsAvailable = false;
                _initializationSource.TrySetResult(false);
                Initialized?.Invoke();
                return;
            }

            IsInitialized = true;
            IsAvailable = true;
            _initializationSource.TrySetResult(true);
            Initialized?.Invoke();

            FetchProducts();
        }

        public void Purchase(string productId)
        {
            if (!IsInitialized || _purchaseService == null || _productService == null)
            {
                PurchaseFailed?.Invoke(productId, IapFailureReason.NotInitialized);
                return;
            }

            Product product = _productService.GetProductById(productId);

            if (product == null)
            {
                PurchaseFailed?.Invoke(productId, IapFailureReason.ProductNotFound);
                return;
            }

            _purchaseService.PurchaseProduct(product);
        }

        public IReadOnlyList<string> GetProducts()
        {
            return _productIds;
        }

        private void FetchProducts()
        {
            List<ProductDefinition> definitions = new List<ProductDefinition>();

            for (int i = 0; i < _productIds.Count; i++)
            {
                definitions.Add(new ProductDefinition(_productIds[i], ProductType.Consumable));
            }

            _productService.FetchProductsWithNoRetries(definitions);
        }

        private void OnStoreConnected()
        {
            IsAvailable = true;
        }

        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            IsAvailable = false;
        }

        private void OnProductsFetchFailed(ProductFetchFailed failure)
        {
            Debug.LogWarning("IAP product fetch failed: " + failure.FailureReason);
        }

        private void OnPurchasePending(PendingOrder order)
        {
            string productId = GetProductId(order);
            _purchaseService.ConfirmPurchase(order);
            PurchaseSucceeded?.Invoke(productId);
        }

        private void OnPurchaseFailed(FailedOrder order)
        {
            string productId = GetProductId(order);
            PurchaseFailed?.Invoke(productId, MapFailure(order.FailureReason));
        }

        private static string GetProductId(Order order)
        {
            if (order == null || order.CartOrdered == null)
            {
                return string.Empty;
            }

            IReadOnlyList<CartItem> items = order.CartOrdered.Items();

            if (items == null || items.Count == 0)
            {
                return string.Empty;
            }

            CartItem item = items[0];

            if (item.Product != null && item.Product.definition != null)
            {
                return item.Product.definition.id;
            }

            return item.CatalogListingId ?? string.Empty;
        }

        private static IapFailureReason MapFailure(PurchaseFailureReason reason)
        {
            switch (reason)
            {
                case PurchaseFailureReason.UserCancelled:
                    return IapFailureReason.PurchaseCancelled;
                case PurchaseFailureReason.PaymentDeclined:
                    return IapFailureReason.PaymentDeclined;
                case PurchaseFailureReason.ProductUnavailable:
                    return IapFailureReason.ProductNotFound;
                case PurchaseFailureReason.PurchasingUnavailable:
                    return IapFailureReason.NotAvailable;
                default:
                    return IapFailureReason.Unknown;
            }
        }
    }
}
