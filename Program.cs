using ILearn.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Only load .env in Development (local)
if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load();
    Console.WriteLine("✅ .env file loaded (Development)");
}
else
{
    Console.WriteLine("✅ Using Railway environment variables (Production)");
}

// 🔹 MVC
builder.Services.AddControllersWithViews();

// 🔹 Supabase
builder.Services.AddSingleton<SupabaseService>();

// 🔹 Chat client
builder.Services.AddHttpClient<ChatService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();