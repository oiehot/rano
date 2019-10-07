using System; // Console

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System.Threading.Tasks; // for Task.Run
using System.Threading; // for Thread.Sleep

namespace Tests
{
    public class AppleAppstoreTest
    {
        [Test]
        public async void GetLastestVersion_Test()
        {
            string id = "1350067922";
            
            string version = await Rano.Appstore.AppleAppstore.GetLastestVersionAsync(id);
            
            // TODO: 버젼 업데이트가 되면 실패할 예정.
            // ! 유니티 2019.1 의 TestRunner(NUnit) 지원 문제로
            // ! 테스트는 실패했더라도 Pass 표시가 된다.
            // ! 그러나 예외 로그가 뜨기 때문에 대처가 가능할것임.
            Assert.AreEqual(version, "1.1.129");
        }
    }
}