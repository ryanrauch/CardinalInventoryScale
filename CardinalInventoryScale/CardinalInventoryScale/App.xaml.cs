using Autofac;
using CardinalInventoryScale.Views.ContentPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace CardinalInventoryScale
{
	public partial class App : Application
	{
        public static IContainer Container { get; set; }

        public App ()
		{
			InitializeComponent();
            Container = AutoFacContainerBuilder.CreateContainer();
            MainPage = new InitialView();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
