using SOCMobile.Models;
using SOCMobile.PageModels;

namespace SOCMobile.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}