namespace Rano.Appstore
{
    using UnityEngine;
    using System.Net.Http;
    using System.Threading; // for Thread.Sleep
    using System.Threading.Tasks; // for Task.Run
    using LitJson;
    
    public static class GoogleAppstore
    {
        public static void Open(string id)
        {
            Application.OpenURL($"market://details?id={id}");
        }
    }
}