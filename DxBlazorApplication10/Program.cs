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

// 1. AddAuthentication�� ȣ���Ͽ� ���� ���񽺸� ����ϰ�, �⺻ ���� ��Ŵ�� JWT Bearer�� �����մϴ�.
//    �̷��� �ϸ� �����ӿ�ũ�� ���� ç������ �ʿ��� �� ����� �⺻ ����� �˰� �˴ϴ�.
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

// 2. ���� �ο�(Authorization) ���񽺸� ����մϴ�. [Authorize] Ư�� ��뿡 �ʿ��մϴ�.
builder.Services.AddAuthorization();

// 2. JWT ��� ���� ���� ����
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthService>();

// 3. Ŀ���� AuthenticationStateProvider ���
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();


// MDI ���� ������ ���� Scoped ���� ���
builder.Services.AddScoped<MdiStateService>();
builder.Services.AddScoped<MDIStateHelper>();

// --- ������ �κ�: MdiStateService�� Scoped�� �����ϰ� ThemeService�� Singleton���� ��� ---
builder.Services.AddSingleton<ThemeService>();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMvc();

builder.Services.AddScoped<IDapperService, DapperService>();

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