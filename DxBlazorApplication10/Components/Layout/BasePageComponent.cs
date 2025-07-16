using BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Layout
{
    // 설명: 모든 페이지가 상속받는 기본 클래스입니다.
    // 툴바 상태 등록 및 리소스 정리를 자동으로 처리합니다.
    public abstract class BasePageComponent : ComponentBase, IDisposable
    {
        [Inject]
        protected ToolbarStateService ToolbarState { get; set; } = default!;

        // 페이지가 초기화될 때, 자식 페이지에서 재정의된 메서드들을 서비스에 등록합니다.
        protected override void OnInitialized()
        {
            ToolbarState.RegisterToolbarActions(
                search: OnSearch,
                add: OnAdd,
                save: OnSave,
                delete: OnDelete,
                print: OnPrint
            );
            base.OnInitialized();
        }

        // 자식 페이지에서 재정의(override)할 수 있도록 virtual 메서드로 선언합니다.
        // 기본적으로는 아무 동작도 하지 않습니다.
        protected virtual void OnSearch() { }
        protected virtual void OnAdd() { }
        protected virtual void OnSave() { }
        protected virtual void OnDelete() { }
        protected virtual void OnPrint() { }

        // 페이지가 소멸(탭이 닫히거나 다른 페이지로 이동)될 때,
        // 서비스에 등록된 액션들을 정리하여 메모리 누수를 방지합니다.
        public void Dispose()
        {
            ToolbarState.ResetToolbar();
        }
    }
}
