using System;

namespace Rano.Services.Billing
{
    public class NotFoundProductException : Exception
    {
        private string _productId;
        public string ProductId => _productId;

        public NotFoundProductException(string productId)
        {
            _productId = productId;
        }
    }

    public class InvalidProductException : Exception
    {
        private string _productId;
        public string ProductId => _productId;

        public InvalidProductException(string productId)
        {
            _productId = productId;
        }
    }
}