using System.Runtime.CompilerServices;

namespace Catalyst.Engine.Core;

public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

public interface INotifyPropertyChanged
{
    event PropertyChangedEventHandler? PropertyChanged;

    void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "");
}