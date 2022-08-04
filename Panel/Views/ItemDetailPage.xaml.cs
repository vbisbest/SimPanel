using System.ComponentModel;
using Xamarin.Forms;
using Panel.ViewModels;

namespace Panel.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}