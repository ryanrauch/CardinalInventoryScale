using CardinalInventoryScale.ViewModels;
using Xamarin.Forms.Xaml;

namespace CardinalInventoryScale.Views.ContentPages
{
    public class InitialViewBase : ViewPageBase<InitialViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InitialView : InitialViewBase
	{
		public InitialView ()
		{
			InitializeComponent ();
		}
	}
}