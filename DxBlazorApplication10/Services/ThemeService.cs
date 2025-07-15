namespace BlazorApp.Services
{
    /// <summary>
    /// 애플리케이션의 DevExpress 테마를 관리하는 서비스입니다.
    /// </summary>
    public class ThemeService
    {
        /// <summary>
        /// 현재 적용된 테마의 이름입니다. 기본값은 'blazing-berry'입니다.
        /// </summary>
        public string CurrentTheme { get; private set; } = "blazing-berry";

        /// <summary>
        /// 테마가 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        public event Action OnThemeChanged;

        /// <summary>
        /// 애플리케이션의 테마를 설정합니다.
        /// </summary>
        /// <param name="themeName">적용할 테마의 이름입니다.</param>
        public void SetTheme(string themeName)
        {
            if (CurrentTheme != themeName)
            {
                CurrentTheme = themeName;
                OnThemeChanged?.Invoke();
            }
        }
    }
}
