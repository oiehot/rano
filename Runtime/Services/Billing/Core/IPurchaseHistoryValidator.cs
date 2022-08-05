using System;
using System.Threading.Tasks;

namespace Rano.Services.Billing
{
    public interface IPurchaseHistoryValidator
    {
        Action OnValidated { get; set; }
        
        Task ValidateAsync();
    }
}