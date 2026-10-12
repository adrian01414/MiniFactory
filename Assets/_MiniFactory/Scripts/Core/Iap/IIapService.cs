using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniFactory.Core.Iap
{
    public interface IIapService
    {
        event Action<string> PurchaseSucceeded;
        event Action<string, IapFailureReason> PurchaseFailed;

        bool IsInitialized { get; }
        bool IsAvailable { get; }

        Task InitializeAsync();
        void Purchase(string productId);
        IReadOnlyList<string> GetProducts();
    }
}
