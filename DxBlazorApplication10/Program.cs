using BlazorApp.Components;
using BlazorApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});

// 1. AddAuthentication을 호출하여 인증 서비스를 등록하고, 기본 인증 스킴을 JWT Bearer로 설정합니다.
//    이렇게 하면 프레임워크가 인증 챌린지가 필요할 때 사용할 기본 방식을 알게 됩니다.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 2. 권한 부여(Authorization) 서비스를 등록합니다. [Authorize] 특성 사용에 필요합니다.
builder.Services.AddAuthorization();

// 2. JWT 기반 인증 서비스 설정
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthService>();

// 3. 커스텀 AuthenticationStateProvider 등록
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();


// 앱 전체에서 단 하나의 라우트 맵만 유지하도록 Singleton으로 등록
builder.Services.AddSingleton<RouteTableService>();
// MDI 상태 관리를 위한 Scoped 서비스 등록
builder.Services.AddScoped<MdiStateService>();
builder.Services.AddScoped<MDIStateHelper>();

// --- 수정된 부분: MdiStateService를 Scoped로 변경하고 ThemeService를 Singleton으로 등록 ---
builder.Services.AddSingleton<ThemeService>();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMvc();

builder.Services.AddScoped<IDapperService, DapperService>();

// Scoped로 등록하여 사용자 세션별로 독립적인 툴바 상태를 유지합니다.
builder.Services.AddScoped<ToolbarStateService>();
// Scoped로 등록하여 사용자 세션별로 독립적인 메시지 박스 상태를 유지합니다.
builder.Services.AddScoped<MessageBoxService>();

// ✨ 사용자 상태 관리를 위한 Scoped 서비스 등록
builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<UserService>(); // UserService 등록

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();