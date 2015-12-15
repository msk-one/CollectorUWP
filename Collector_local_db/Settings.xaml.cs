using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
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
                    await msgbox.ShowAsync();
                }
            }
        }
    }
}
