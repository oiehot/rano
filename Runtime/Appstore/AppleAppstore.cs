namespace Rano.Appstore
{
    using UnityEngine;
    using System.Net.Http;
    using System.Threading; // for Thread.Sleep
    using System.Threading.Tasks; // for Task.Run
    using LitJson;
    
    public static class AppleAppstore
    {
        // !
        // ! using: 구문이 종료되거나 중간에 문제가 생길 경우, 리소스를 스스로 정리하는 기능.
        // ! 예) 파일, 데이터베이스 사용.
        // !
        // ! using 구문에 사용하려는 class는 우선,
        // ! 리소스 정리를 위한 메소드를 만들어두어야 합니다.
        // ! IDisposable interface를 상속하고,
        // ! 리소스 정리를 위한 void Dispose()를 구현해 두어야 합니다.
        // !
        // ! using구문은 다음의 경우에 리소스를 정리할 수 있는 Dispose() 함수를 부릅니다:
        // ! using 구문이 정상적으로 완료, using 구문 수행 중 에러상황이 발생.
        // !
        public static async Task<string> GetLastestVersionAsync(string id )
        {
            // TODO: 인터넷 연결되지 않았을 때 실행하면 에러남.
            // ex) http://itunes.apple.com/lookup?id=1350067922
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetStringAsync($"http://itunes.apple.com/lookup?id={id}");
                    var data = JsonMapper.ToObject(result);
                    
                    string version = data["results"][0]["version"].ToString();
                    return version;
                }
            }
            catch
            {
                return ""; // TODO: Test Return Null;
            }
        }
        
        public static void Open(string id)
        {
            // ex) itms-apps://itunes.apple.com/app/id1350067922
            Application.OpenURL($"itms-apps://itunes.apple.com/app/id{id}");
        }
    }
}