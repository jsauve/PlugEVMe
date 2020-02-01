using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using PlugEVMe.ViewModels;
using PlugEVMe.Exceptions;
using System.Diagnostics;

namespace PlugEVMe.Services
{
	public class XamarinFormsNavService : INavService
	{
		readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();

		public event PropertyChangedEventHandler CanGoBackChanged;

		public INavigation XamarinFormsNav { get; set; }

		public bool CanGoBack
		{
			get
			{
				return XamarinFormsNav.NavigationStack != null
					&& XamarinFormsNav.NavigationStack.Count > 0;
			}
		}

		public async Task GoBack()
		{
			if (CanGoBack)
			{
				await XamarinFormsNav.PopAsync(true);
			}

			OnCanGoBackChanged();
		}


		public async Task NavigateTo<TVM, TParameter>(TParameter parameter) where TVM : BaseViewModel
		{
			await NavigateToView(typeof(TVM));

			if (XamarinFormsNav.NavigationStack.Last().BindingContext is BaseViewModel)
			{
				try
				{
					var bindingContext = XamarinFormsNav.NavigationStack.Last().BindingContext;
					await (bindingContext as BaseViewModel).Init();
				}
				catch (AppException e)
				{
					Debug.WriteLine("In catch block of Main method.");
					Debug.WriteLine("Caught: {0}", e.Message);
					if (e.InnerException != null)
						Debug.WriteLine("Inner exception: {0}", e.InnerException);
				}

			}
		}

		public async Task RemoveLastView()
		{
			if (XamarinFormsNav.NavigationStack.Any())
			{
				var lastView = XamarinFormsNav.NavigationStack[XamarinFormsNav.NavigationStack.Count - 2];

                XamarinFormsNav.RemovePage(lastView);
			}
		}

		public async Task ClearBackStack()
		{
			if (XamarinFormsNav.NavigationStack.Count <= 1)
			{
				return;
			}

			for (var i = 0; i < XamarinFormsNav.NavigationStack.Count - 1; i++)
			{
				XamarinFormsNav.RemovePage(XamarinFormsNav.NavigationStack[i]);
			}
		}

		public async Task NavigateToUri(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentException("Invalid URI");
			}

			Device.OpenUri(uri);
		}


		public void RegisterViewMapping(Type viewModel, Type view)
		{
			_map.Add(viewModel, view);
		}

		async Task NavigateToView(Type viewModelType)
		{
            if (viewModelType is null)
            {
                throw new ArgumentNullException(nameof(viewModelType));
            }

            Type viewType;

			if (!_map.TryGetValue(viewModelType, out viewType))
			{
				throw new ArgumentException("No view found in view mapping for " + viewModelType.FullName + ".");
            }
			var constructor = viewType.GetTypeInfo()
									  .DeclaredConstructors
									  .FirstOrDefault(dc => dc.GetParameters().Count() <= 0);

			var view = constructor.Invoke(null) as Page;

            var vm = ((App)Application.Current).Kernel.GetService(viewModelType);

			view.BindingContext = vm;

			await XamarinFormsNav.PushAsync(view, true);
		}

		void OnCanGoBackChanged()
		{
			CanGoBackChanged?.Invoke(this, new PropertyChangedEventArgs("CanGoBack"));
		}

		public async Task NavigateTo<T>()
		{
			await NavigateToView(typeof(T));

			if (XamarinFormsNav.NavigationStack.Last().BindingContext is BaseViewModel)
			{
				await((BaseViewModel)(XamarinFormsNav.NavigationStack.Last().BindingContext)).Init();
			}
		}

		public void ThrowInner()
		{
			throw new AppException("Exception in ThrowInner method.");
		}

		public void CatchInner(object v)
		{
			try
			{
				this.ThrowInner();
			}
			catch (AppException e)
			{
				throw new AppException("Error in CatchInner caused by calling the ThrowInner method.", e);
			}
		}

	}
}
