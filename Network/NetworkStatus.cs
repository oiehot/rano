using UnityEngine;
using System; // TimeSpan
using System.Collections;
using System.Threading.Tasks; // Task.Delay

// Network API를 사용한다면 안드로이드 빌드시 AndroidManifest.xml 에 네트워크 엑세스 권한 요구사항이 삽입된다.
// 하지만, 네트워킹 API를 사용하지 않는다면, 이 권한은 없어지기 때문에 아래의 인터넷 접속여부를 확인하는 코드가 비정상적으로 작동된다.
// 따라서, 플레이어 세팅의 Internet Access 을 Auto가 아닌 Require로 바꿔야 한다.
// 참고: https://docs.unity3d.com/kr/current/Manual/android-manifest.html
// 참고: https://docs.unity3d.com/kr/current/Manual/class-PlayerSettingsAndroid.html

namespace Rano.Network
{
    public static class NetworkStatus
    {
        public static bool internetAvailable = false;
        
        public static async Task<bool> GetInternetAvailableAsync()
        {
            const string pingAddress = "8.8.8.8"; // 구글 공개 DNS 서버 IP 주소
            const float waitingTime = 0.5f; // 최대 핑 대기 시간.
            Ping ping;
            
            ping = new Ping(pingAddress);
            float pingStartTime = Time.time;
            
            await Task.Delay(TimeSpan.FromSeconds(waitingTime));
        
            if (ping.isDone)
            {
                // Ping이 실패하면 -1을 리턴한다.
                if (ping.time >=0)
                {
                    Debug.Log("* Internet Available");
                    internetAvailable = true;    
                }
                else
                {
                    Debug.Log("* Internet Unavailable (Disconnected)");
                    internetAvailable = false;    
                }
            }
            else
            {
                Debug.Log("* Internet Unavailable (Timeout)");
                internetAvailable = false;
            }
            return internetAvailable;
        }
    }
}