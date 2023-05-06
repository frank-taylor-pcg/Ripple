using System.ComponentModel;

namespace MVVM;

// This wrapper allows me to correctly inject the view models
public interface IViewModel : INotifyPropertyChanged
{
}