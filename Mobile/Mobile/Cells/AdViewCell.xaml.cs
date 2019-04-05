using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Cells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdViewCell : ViewCell
	{
		public AdViewCell ()
		{
			InitializeComponent ();
		}
	}
}