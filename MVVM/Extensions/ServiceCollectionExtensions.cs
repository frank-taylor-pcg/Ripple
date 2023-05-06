using System.Reflection;
using MVVM;

// This extension must exist in this namespace to perform properly
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// This class contains extension methods related to the <see cref="IServiceCollection"/> type.
/// </summary>
public static partial class ServiceCollectionExtensions
{
	public static IServiceCollection AddViewModels(this IServiceCollection serviceCollection)
	{
		ArgumentNullException.ThrowIfNull(serviceCollection, nameof(serviceCollection));
		
		AppDomain
			.CurrentDomain
			.GetAssemblies()
			.SelectMany(a =>
				a
					.GetTypes()
					.Where(type => type.IsClass)
					.Where(type => type.Name.EndsWith("ViewModel"))
					.Where(type => !type.Name.Equals("ViewModel"))
			)
			.ToList()
			.ForEach(viewModelType => RegisterViewModel(serviceCollection, viewModelType));

		return serviceCollection;
	}

	private static void RegisterViewModel(IServiceCollection serviceCollection, Type viewModelType)
	{
		// I don't understand this - therefore I don't trust it.
		Type? serviceType = viewModelType.FindInterfaces((type, obj) =>
			{
				return
					obj is Type[] typeArray &&
					typeArray
						.Where(x => x != type)
						.Any(x => x.IsAssignableFrom(type));
			},
			new[] { typeof(IViewModel) }
		).FirstOrDefault();

		if (serviceType is not null)
		{
			serviceCollection.AddScoped(serviceType, viewModelType);
		}
		else
		{
			serviceCollection.AddScoped(viewModelType);
		}
	}
}