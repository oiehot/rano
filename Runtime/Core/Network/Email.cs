#nullable enable

using System;
using UnityEngine;

namespace Rano.Network
{
    public class Email
    {
        private string _receiverEmailAddress;
        private string _subject;
        private string _body;

        public string ReceiverEmailAddress
        {
            get => _receiverEmailAddress;
            set => _receiverEmailAddress = value; // TODO: 올바른 이메일 주소인지 체크하기.
        }

        public string Subject
        {
            get => _subject;
            set => _subject = value;
        }

        public string Body
        {
            get => _body;
            set => _body = value;
        }
        
        private string? GetMailToUrl()
        {
            // TODO: 올바른 이메일 주소인지 체크하기. 아니면 null 리턴.
            string receiverEmailAddress = _receiverEmailAddress;
            string subject = URLHelper.EscapeURL(_subject);
            string body = URLHelper.EscapeURL(_body);
            string url = $"mailto:{receiverEmailAddress}?subject={subject}&body={body}";
            return url;
        }
        
        public bool OpenSendForm()
        {
            Log.Info("이메일 폼 준비 중...");
            
            // Mailto URL 주소를 얻는다.
            string? url = GetMailToUrl();
            if (url == null)
            {
                Log.Warning("이메일 폼 열기 실패 (올바른 mailto url을 얻을 수 없음)");
                return false;
            }
            // Mailto URL을 브라우져로 연다.
#if UNITY_EDITOR
            Log.Info($"이메일 주소: {url}");
#endif
            Log.Info("이메일 폼 여는 중...");
            try
            {
                Application.OpenURL(url);
            }
            catch (Exception e)
            {
                Log.Warning("이메일 폼 열기 실패 (Mailto URL을 여는 중 예외 발생)");
                return false;
            }
            return true;
        }
    }
}