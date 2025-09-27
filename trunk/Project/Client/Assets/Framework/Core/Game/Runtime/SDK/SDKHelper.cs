using UnityEngine;

namespace Game
{
    public static class SDKHelper
    {
        public static bool IsMobileNetwork()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
        }
        
        
    }
}