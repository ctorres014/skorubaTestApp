using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuration
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "cookie";
    opt.DefaultChallengeScheme = "oidc";
}).AddCookie("cookie", c =>
{
    c.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    c.Cookie.Name = "test";

}).AddOpenIdConnect("oidc", opt =>
{
    //opt.Authority = "https://skoruba.bizitglobal.com";//"https://authgol.desarrollo.local";
    opt.Authority = "https://localhost:44310/";
    opt.ClientId = "WebApplicationClient";//"testIdentityPostaClient";
    opt.ClientSecret = "secret_f6b611d10ba1e729c204db8a941b40ddb2d1c1a5dc0770341c0803d7654644d5";//"3c5decf4-3143-3f21-e9ff-5160a7d2eab4";
    opt.ResponseType = "code";
    opt.UsePkce = true;
    opt.ResponseMode = "query";
    //opt.Scope.Add("address");
    opt.Scope.Add("email");
    opt.Scope.Add("openid");
    opt.Scope.Add("profile");
    //opt.Scope.Add("api.read");
    opt.SaveTokens = true;
    opt.GetClaimsFromUserInfoEndpoint = true;

    //opt.Events.OnRedirectToIdentityProvider = context =>
    //{
    //    var customValue = "21321321";
    //    context.ProtocolMessage.AcrValues = $"custom_param:{customValue}";
    //    return Task.CompletedTask;
    //};

    opt.Events.OnRedirectToIdentityProvider = context =>
    {
        var customValue = "21321321";
        // Agregarlo a la URL como parámetro extra
        context.ProtocolMessage.Parameters.Add("custom_param", customValue);
        return Task.CompletedTask;
    };

});

   


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
