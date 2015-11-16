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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                Blogs.ItemsSource = db.Entries.ToList();
                
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            //    using (var db = new CollectorContext())
            //    {
            //        //var blog = new Blog { Url = NewBlogUrl.Text };
            //        //db.Blogs.Add(blog);
            //        //db.SaveChanges();

            //        //Blogs.ItemsSource = db.Blogs.ToList();


            //    }

            //Add_debt mynewPage = new Add_debt(); //newPage is the name of the newPage.xaml file
            //this.Content = mynewPage;

            MessageDialog msgbox = new MessageDialog("What do you want to add ? ");

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = "Borrow", Id = 0 });
            msgbox.Commands.Add(new UICommand { Label = "Lend", Id = 1 });
            msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });

            var res = await msgbox.ShowAsync();

            if ((int)res.Id == 0)
            {
                msgbox.Commands.Clear();
                MessageDialog msgbox2 = new MessageDialog("What did you borrowed ?", "User Response");
                msgbox2.Commands.Add(new UICommand { Label = "Object", Id = 0 });
                msgbox2.Commands.Add(new UICommand { Label = "Money", Id = 1 });
                await msgbox2.ShowAsync();

                if ((int)res.Id == 0)
                {
                    Add_debt mynewPage = new Add_debt();
                    this.Content = mynewPage;

                }

                if ((int)res.Id == 1)
                {
                    Add_debt mynewPage = new Add_debt();
                    this.Content = mynewPage;

                }

            }
            if ((int)res.Id == 1)
            {
                msgbox.Commands.Clear();
                MessageDialog msgbox3 = new MessageDialog("What did you Lend ?", "User Response");
                msgbox3.Commands.Add(new UICommand { Label = "Object", Id = 1 });
                msgbox3.Commands.Add(new UICommand { Label = "Money", Id = 2 });
                await msgbox3.ShowAsync();

                if ((int)res.Id == 0)
                {
                    Add_debt mynewPage = new Add_debt();
                    this.Content = mynewPage;

                }

                if ((int)res.Id == 1)
                {
                    Add_debt mynewPage = new Add_debt();

                    this.Content = mynewPage;

                }

                using (var db = new CollectorContext())
                {
                    //var blog = new Blog { Url = NewBlogUrl.Text };
                    //db.Blogs.Add(blog);
                    //db.SaveChanges();

                    //Blogs.ItemsSource = db.Blogs.ToList();

                }

                if ((int)res.Id == 2)
                {
                    MessageDialog msgbox2 = new MessageDialog("Nevermind then... :|", "User Response");
                    await msgbox2.ShowAsync();
                }


            }


        }
    }
}
