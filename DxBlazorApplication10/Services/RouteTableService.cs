using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazorApp.Services
{
    public class RouteTableService
    {
        // Key: 라우트 경로 (e.g., "/counter"), Value: 컴포넌트 타입 (e.g., typeof(Counter))
        public readonly IReadOnlyDictionary<string, Type> RouteMap;

        public RouteTableService()
        {
            // 현재 실행 중인 어셈블리에서 라우팅 가능한 모든 컴포넌트를 찾습니다.
            var routeMap = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(ComponentBase)) && t.GetCustomAttribute<RouteAttribute>() != null)
                .SelectMany(t => t.GetCustomAttributes<RouteAttribute>().Select(a => new { Route = a.Template, ComponentType = t }))
                .ToDictionary(x => x.Route.ToLower(), x => x.ComponentType); // 경로는 소문자로 통일하여 관리

            RouteMap = routeMap;
        }

        public Type? GetTypeForRoute(string route)
        {
            if (string.IsNullOrEmpty(route)) return null;

            return RouteMap.GetValueOrDefault(route.ToLower());
        }
    }
}
