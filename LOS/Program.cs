using LOS.Data;
using LOS.Interfaces;
using LOS.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog for file logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/app_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 1. CORE MVC & DATABASE SERVICES

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 2. DISTRIBUTED CACHE & SESSION MANAGEMENT

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// 3. AUTHENTICATION & JWT CONFIGURATION
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>

{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

// 4. DEPENDENCY INJECTION / BUSINESS SERVICES

builder.Services.AddScoped<IApplyLoanService, ApplyLoanService>();
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICibilService, CibilService>();
builder.Services.AddScoped<IEligibilityService, EligibilityService>();
builder.Services.AddScoped<IScoreCardService, ScoreCardService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IKycService, KycService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITrackStatusService, TrackStatusService>();
builder.Services.AddScoped<ISanctionService, SanctionService>();
builder.Services.AddScoped<IDisbursementService, DisbursementService>();

var app = builder.Build();


// 5. MIDDLEWARE PIPELINE

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
// Global exception middleware only for API endpoints
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
    appBuilder => appBuilder.UseMiddleware<LOS.Middlewares.ExceptionMiddleware>());

// MVC error page handler (always active)
app.UseExceptionHandler("/Home/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// CRITICAL ORDER: Session -> Authentication -> Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

app.Run();