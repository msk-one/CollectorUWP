using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                Blogs.ItemsSource = db.Categories.ToList();

            }


        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                var debt = new Category
                {
                    Cname = CategoryBox.Text
                };

                if (!db.Categories.Any(o => o.Cname == debt.Cname))
                {
                    db.Categories.Add(debt);
                    db.SaveChanges();
                    Blogs.ItemsSource = db.Categories.ToList();
                }
                else
                {
                    MessageDialog msgbox = new MessageDialog("This category already exists");
                    msgbox.Commands.Clear();
                    msgbox.Commands.Add(new UICommand { Label = "OK", Id = 1 });
                    var res = await msgbox.ShowAsync();
                }
            }
        }
    }
}
