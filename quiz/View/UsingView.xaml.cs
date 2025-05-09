using quiz.Service;
using quiz.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NavigationService = quiz.Service.NavigationService;


namespace quiz.View
{
    /// <summary>
    /// Logika interakcji dla klasy Page2.xaml
    /// </summary>
    public partial class UsingView : UserControl
    {
        public UsingView()
        {
            InitializeComponent();
            //this.DataContext = new UsingViewModel();
            
        }
       
    }
}
