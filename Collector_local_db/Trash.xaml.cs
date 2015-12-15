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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                Trach_list.ItemsSource = db.Entries.Where(o => o.Is_active == false).ToList();

                removeButton.Visibility = Visibility.Collapsed;
                reviveButton.Visibility = Visibility.Collapsed;
            }
        }


        private void Trach_list_GotFocus(object sender, RoutedEventArgs e)
        {

            removeButton.Visibility = Visibility.Visible;
            reviveButton.Visibility = Visibility.Visible;
        }

        private void remove_button(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Entry)Trach_list.SelectedItem;
            if (selectedItem == null) return;
            var id = selectedItem.EntryId;

            using (var db = new CollectorContext())
            {


                var ent = (from d in db.Entries
                    .Include(usr => usr.Object)
                    .Include(usr => usr.Currency)
                    .Include(usr => usr.Type)
                    .Include(usr => usr.Object.Category)
                    where d.EntryId == id
                    select d).FirstOrDefault();


                db.Remove(ent);
                db.SaveChanges();
                Trach_list.ItemsSource = db.Entries.Where(o => o.Is_active == false).ToList();


            }
        }

        private void revive_button(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Entry)Trach_list.SelectedItem;
            if (selectedItem == null) return;

            var id = selectedItem.EntryId;

            using (var db = new CollectorContext())
            {

                var ent = (from d in db.Entries
                    .Include(usr => usr.Object)
                    .Include(usr => usr.Currency)
                    .Include(usr => usr.Type)
                    .Include(usr => usr.Object.Category)
                    where d.EntryId == id
                    select d).FirstOrDefault();

                ent.Is_active = true;

                db.SaveChanges();

                Trach_list.ItemsSource = db.Entries.Where(o => o.Is_active == false).ToList();

            }
        }
    }


    }
