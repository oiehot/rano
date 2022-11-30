#nullable enable

using System;
using System.Threading.Tasks;

namespace Rano.Billing
{
    public interface IPurchaseHistoryValidator
    {
        public event Action OnValidated;
        public Task ValidateAsync();
    }
}