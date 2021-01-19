using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameStore.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private static Dictionary<string, BaseViewModel> views = new Dictionary<string, BaseViewModel>();

        protected static BaseViewModel GetView(string viewName)
        {
            if (views.ContainsKey(viewName))
                return views[viewName];

            switch (viewName)
            {
                case "caro":
                    views.Add(viewName, new CaroGameViewModel());
                    break;

                case "puzzle":
                    views.Add(viewName, new PuzzleGameViewModel());
                    break;

                default:
                    return null;
            }

            return views[viewName];

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetBindableProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(property, value)) return;

            property = value;
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
