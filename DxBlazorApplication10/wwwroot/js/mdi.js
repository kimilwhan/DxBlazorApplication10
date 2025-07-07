function addContextMenuHandler(container, tabClass, text, dotNetObject) {
    var tabElement = container.getElementsByClassName(tabClass)[0];
    if (!tabElement || tabElement.hasAttribute("cp_ctx")) return;
    tabElement.setAttribute("cp_ctx", true);
    tabElement.addEventListener('contextmenu', (event) => {
        event.preventDefault();
        let eventArgs = {
            clientX: event.clientX,
            clientY: event.clientY,
            screenX: event.screenX,
            screenY: event.screenY,
            offsetX: event.offsetX,
            offsetY: event.offsetY,
            pageX: event.pageX,
            pageY: event.pageY,
            button: event.button,
            buttons: event.buttons,
            ctrlKey: event.ctrlKey,
            shiftKey: event.shiftKey,
            altKey: event.altKey,
            metaKey: event.metaKey,
            detail: event.detail,
            type: event.type
        };
        dotNetObject.invokeMethodAsync("ShowContextMenu", eventArgs, text);
    });
};

// --- 추가된 부분: Blazor 컴포넌트 인스턴스를 전역 변수에 저장 ---
window.setMdiContainerHelper = (dotNetHelper) => {
    window.mdiContainerHelper = dotNetHelper;
};

// --- 추가된 부분: 새 창에서 부모 창의 .NET 메서드를 실행 ---
window.attachToParentTab = function (tabId) {
    if (window.opener && window.opener.mdiContainerHelper) {
        // 부모 창에 등록된 .NET 객체의 메서드를 호출
        window.opener.mdiContainerHelper.invokeMethodAsync('ReAttachTab', tabId);
        // 메서드 호출 후 자신(팝업창)을 닫음
        window.close();
    } else {
        alert("부모 창을 찾을 수 없어 탭으로 되돌릴 수 없습니다.");
    }
}

// --- 추가된 부분: 열린 팝업창의 참조를 저장하는 객체 ---
window.openPopups = {};

// --- 기존 함수 ---
window.openAsPopup = function (url, name) {
    const popup = window.open(url, name, "width=800,height=600,resizable=yes,scrollbars=yes");
    // --- 추가된 부분: 열린 창의 참조를 저장 ---
    if (popup) {
        window.openPopups[name] = popup;
    }
}

// 팝업창의 'beforeunload' 이벤트 핸들러입니다.
function handleBeforeUnload(e) {
    // 부모 창의 .NET 메서드를 호출하여 팝업이 닫혔음을 알립니다.
    if (window.opener && window.opener.mdiContainerHelper) {
        window.opener.mdiContainerHelper.invokeMethodAsync('NotifyPopupWindowClosed', window.popupTabId);
    }
}

// 팝업 페이지가 로드될 때 이 함수를 호출하여 이벤트 리스너를 등록합니다.
window.registerUnloadHandler = function (tabId) {
    // tabId를 전역 변수에 저장하여 unload 핸들러에서 접근할 수 있도록 합니다.
    window.popupTabId = tabId;
    window.addEventListener('beforeunload', handleBeforeUnload);
};

// 팝업 페이지 컴포넌트가 소멸될 때(예: 탭으로 복원될 때) 호출하여 불필요한 이벤트 발생을 막습니다.
window.unregisterUnloadHandler = function () {
    window.removeEventListener('beforeunload', handleBeforeUnload);
};

// --- 추가된 함수: 모든 팝업창을 닫습니다. ---
function closeAllPopups() {
    for (const key in window.openPopups) {
        const popup = window.openPopups[key];
        if (popup && !popup.closed) {
            popup.close();
        }
    }
}

// --- 추가된 함수: 부모창의 unload 이벤트를 등록합니다. ---
window.registerParentUnloadHandler = function () {
    window.addEventListener('beforeunload', closeAllPopups);
}