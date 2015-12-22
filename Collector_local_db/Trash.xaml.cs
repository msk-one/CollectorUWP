using System.Collections.Generic;
using Microsoft.Data.Entity;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Trash
    {
        public Trash()
        {
            InitializeComponent();
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            Trach_list.ItemsSource = ProjectClasses.AllArchivedEntrys =((List<ProjectClasses.Entry>)await
              SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                  "GetEntriesForUserArchived/"+SerwerFunction.Uid, "GET", null)).ToList();

           

            removeButton.Visibility = Visibility.Collapsed;
                reviveButton.Visibility = Visibility.Collapsed;
            
        }


        private void Trach_list_GotFocus(object sender, RoutedEventArgs e)
        {

            removeButton.Visibility = Visibility.Visible;
            reviveButton.Visibility = Visibility.Visible;
        }

        private async void remove_button(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ProjectClasses.Entry)Trach_list.SelectedItem;
            if (selectedItem == null) return;
            var id = selectedItem.id;

          var test = (ProjectClasses.Entry)await
             SerwerFunction.Getfromserver<ProjectClasses.Entry>(
                 "Entries/" + id, "DELETE", null);



            Trach_list.ItemsSource = ProjectClasses.AllArchivedEntrys = ((List<ProjectClasses.Entry>)await
              SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                  "GetEntriesForUserArchived/" + SerwerFunction.Uid, "GET", null)).ToList();



        }

        private async void revive_button(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ProjectClasses.Entry)Trach_list.SelectedItem;
            if (selectedItem == null) return;

            var id = selectedItem.id;

            var ent = ProjectClasses.AllArchivedEntrys.FirstOrDefault(o => o.id == id);


            ent.archived = 0;

            var received_debt = (ProjectClasses.Entry)await SerwerFunction.Getfromserver<ProjectClasses.Entry>("Entries/" + id, "PUT", ent);


            Trach_list.ItemsSource = ProjectClasses.AllArchivedEntrys = ((List<ProjectClasses.Entry>)await
              SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                  "GetEntriesForUserArchived/" + SerwerFunction.Uid, "GET", null)).ToList();




        }
    }


    }
