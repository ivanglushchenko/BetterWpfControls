using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetterWpfControls.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region .ctors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowModel();
        }

        #endregion .ctors

        #region Fields

        private ContentAdorner _adorner;

        #endregion Fields

        #region Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var lyr = AdornerLayer.GetAdornerLayer(btn);
            if (_adorner != null)
            {
                lyr.Remove(_adorner);
                _adorner = null;
            }
            else
            {
                _adorner = new ContentAdorner(new Button() { Content = "Here could be pretty much anything!" }, btn);
                _adorner.HorizontalOffset = btn.ActualWidth / 2.0;
                _adorner.VerticalOffset = btn.ActualHeight;
                lyr.Add(_adorner);
            }
        }

        #endregion Methods
    }
}
