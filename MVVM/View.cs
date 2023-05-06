using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace MVVM;

public abstract class View<T> : ComponentBase, IDisposable
	where T : class, IViewModel
{
	[Inject]
	protected T? ViewModel { get; set; }

	public virtual void Dispose()
	{
		if (ViewModel != null)
		{
			ViewModel.PropertyChanged -= OnPropertyChanged;
		}
	}

	private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		await InvokeAsync(StateHasChanged);
	}

	protected override async Task OnInitializedAsync()
	{
		if (ViewModel is not null)
		{
			ViewModel.PropertyChanged += async (sender, e) =>
			{
				await InvokeAsync(StateHasChanged);
			};
		}

		if (ViewModel is ViewModel vmBase)
		{
			await vmBase.OnInitializedAsync().ConfigureAwait(false);
		}

		await base.OnInitializedAsync().ConfigureAwait(false);
	}
}