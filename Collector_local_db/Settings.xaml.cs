using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressUpload.IsIndeterminate = true;

            Blogs.ItemsSource =  ProjectClasses.AllCategories = ((List<ProjectClasses.Category>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Category>>(
                    "Categories/", "GET", null)).ToList();
            ProgressUpload.IsIndeterminate = false;
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            

          

            if (ProjectClasses.AllCategories.Any(o => o.cname == CategoryBox.Text))
            {
                var msgbox = new MessageDialog("This category already exists");
                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "OK", Id = 1 });
                await msgbox.ShowAsync();
            }
            else
            {

                var userCategories =
               (ProjectClasses.Category)
                   await
                       SerwerFunction.Getfromserver<ProjectClasses.Category>(
                           "Categories/", "POST", new ProjectClasses.Category { cname = CategoryBox.Text });


                Blogs.ItemsSource = ProjectClasses.AllCategories = ((List<ProjectClasses.Category>)await
               SerwerFunction.Getfromserver<List<ProjectClasses.Category>>(
                   "Categories/", "GET", null)).ToList();

            }

        }

        private void CategoryBox_OnTextChanged(object sender, TextChangedEventArgs e) { 
            CategoryButton.IsEnabled = (CategoryBox.Text != "")? true : false;
        
              
        }
    }
}
