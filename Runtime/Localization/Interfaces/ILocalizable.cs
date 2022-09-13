using System.Globalization;

namespace Rano.Localization
{
    /// <summary>
    /// 국제화 지원 인터페이스.
    /// </summary>
    /// <remarks>
    /// LocalizationManager에 의해 Locale이 변경되는 시점에
    /// 이 인터페이스를 구현하는 모든 MonoBehaviour들을 찾아
    /// UpdateLocalizableString 메서드를 실행한다.
    /// </remarks>
    public interface ILocalizable
    {
        public void OnLocaleChanged(CultureInfo cultureInfo);
    }
}