using Machine.Data;
using Machine.Profibus;
using Ripple;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Test();

app.Run();

void Test()
{
	bool? vacuumState = null;

	// Is there a way to make copying values out of the VM easier?
	CodeBlock cb = new();
	cb
		.DeclareVariable("state", false)
		.SetVacuumPumpOn(true, 32)
		.GetVacuumPumpState(32, "state");
		//.CSAction(() => vacuumState = cb.Mem.state);

	VirtualMachine vm = new()
	{
		CodeBlock = cb
	};
	vm.Run();

	System.Diagnostics.Debug.WriteLine($"[Final results] Vacuum State => {vacuumState}");
	System.Diagnostics.Debug.WriteLine($"[Final results] Vacuum State (direct) => {cb.Mem.state}");

}