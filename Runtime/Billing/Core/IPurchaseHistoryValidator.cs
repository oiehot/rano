using System;
using System.Threading.Tasks;

namespace Rano.Billing
{
    public interface IPurchaseHistoryValidator
    {
        Action OnValidated { get; set; }
        
        Task ValidateAsync();
    }
}