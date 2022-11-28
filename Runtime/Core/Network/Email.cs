#nullable enable

namespace Rano.Network
{
    public class Email
    {
        private string _receiver;
        private string _subject;
        private string _body;

        public string Receiver => _receiver;
        public string Subject => _subject;
        public string Body => _body;

        public Email(string receiver, string subject, string body)
        {
            _receiver = receiver;
            _subject = subject;
            _body = body;
        }
        
        public string? SendMailUrl
        {
            get
            {
                // TODO: 올바른 이메일 주소인지 체크하기. 아니면 null 리턴.
                string receiver = _receiver;
                string subject = URLHelper.EscapeURL(_subject);
                string body = URLHelper.EscapeURL(_body);
                string url = $"mailto:{receiver}?subject={subject}&body={body}";
                return url;
            }
        }

    }
}