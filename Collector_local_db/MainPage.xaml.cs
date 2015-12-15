using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    public class Choice
    {
        public bool Borrowed { get; set; }
        public bool Object { get; set; }
    }


    public sealed partial class MainPage
    {
        readonly List<Button> _pCategoryButtons = new List<Button>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

            using (var db = new CollectorContext())
            {

                for (var i = 0; i < db.Categories.Count(); i++)
                {
                    var gridRow1 = new RowDefinition {Height = new GridLength(35)};

                    grid_categories.RowDefinitions.Add(gridRow1);


                    var category = new Button
                    {
                        Width = double.NaN,
                        Height = 30,
                        Content = db.Categories.ToList().ElementAt(i).Cname
                    };
                    category.Click += fill_categories;


                    _pCategoryButtons.Add(category);
                    Grid.SetRow(category, i);
                    grid_categories.Children.Add(category);



                }


            }


        }

        private void hide_all(List<Button> buttons)
        {
            foreach (Button b in buttons)
            {
                b.Visibility = Visibility.Collapsed;

            }

        }

        private void show_all(List<Button> buttons)
        {
            foreach (Button b in buttons)
            {
                b.Visibility = Visibility.Visible;

            }

        }

        private void fill_categories(object sender, RoutedEventArgs e)
        {
            var itemInCategory = sender as Button;


            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries
                     .Include(usr => usr.Object)
                     .Include(usr => usr.Object.Category)
                     .Include(usr => usr.Type)
                     .Where(o => o.Object.Category.Cname == (string)itemInCategory.Content && o.Type.TypeId == 1 && o.Is_active)
                     .ToList();

                Lend_list.ItemsSource = db.Entries
                    .Include(usr => usr.Object)
                    .Include(usr => usr.Object.Category)
                    .Include(usr => usr.Type)
                    .Where(o => o.Object.Category.Cname == (string)itemInCategory.Content && o.Type.TypeId == 2 && o.Is_active)
                    .ToList();
            }
            hide_all(_pCategoryButtons);
            objectBorrowButton.IsEnabled = false;
            Category_choosen.Visibility = Visibility.Visible;
            Category_choosen.Content = (string)itemInCategory.Content;

            Borrow_list.Visibility = Visibility.Visible;
            Lend_list.Visibility = Visibility.Visible;


        }


        private void ShowElementsInCategory(object sender, RoutedEventArgs e)
        {
            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

            var clickedCategory = sender as Button;
            objectBorrowButton.IsEnabled = true;
            clickedCategory.Visibility = Visibility.Collapsed;
            if (temp2 == "Borrow")
            {
                Borrow_list.Visibility = Visibility.Collapsed;

            }
            else
            {
                Lend_list.Visibility = Visibility.Collapsed;
            }
            show_all(_pCategoryButtons);

        }



        private async void AddButton(object sender, RoutedEventArgs e)
        {
            var choice = new Choice();

            var msgbox = new MessageDialog("What do you want to add ? ");

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = "Borrow", Id = 0 });
            msgbox.Commands.Add(new UICommand { Label = "Lend", Id = 1 });
            msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });

            var res = await msgbox.ShowAsync();

            if ((int)res.Id == 0)
            {
                msgbox.Commands.Clear();
                MessageDialog msgbox2 = new MessageDialog("What did you borrowed ?", "");
                msgbox2.Commands.Add(new UICommand { Label = "Object", Id = 0 });
                msgbox2.Commands.Add(new UICommand { Label = "Money", Id = 1 });
                msgbox2.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });
                var res2 = await msgbox2.ShowAsync();

                if ((int)res2.Id == 0)
                {
                    choice.Object = true;
                    choice.Borrowed = true;

             
                    Frame.Navigate(typeof(AddDebt), choice);

                }

                if ((int)res2.Id == 1)
                {
                    choice.Object = false;
                    choice.Borrowed = true;
                 
                    Frame.Navigate(typeof(AddDebt), choice);
                }


                if ((int)res.Id == 2)
                {
                    var msgbox4 = new MessageDialog("Nevermind then... :|", "");
                    await msgbox2.ShowAsync();
                }

            }
            if ((int)res.Id == 1)
            {
                msgbox.Commands.Clear();
                var msgbox3 = new MessageDialog("What did you Lend ?", "");
                msgbox3.Commands.Add(new UICommand { Label = "Object", Id = 0 });
                msgbox3.Commands.Add(new UICommand { Label = "Money", Id = 1 });
                msgbox3.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });
                var res2 = await msgbox3.ShowAsync();

                if ((int)res2.Id == 0)
                {
                    choice.Object = true;
                    choice.Borrowed = false;
                    //AddDebt mynewPage = new AddDebt(choice);
                    Frame.Navigate(typeof(AddDebt), choice);
                    //this.Content = mynewPage;

                }

                if ((int)res2.Id == 1)
                {
                    choice.Object = false;
                    choice.Borrowed = false;

                    //AddDebt mynewPage = new AddDebt(choice);
                    Frame.Navigate(typeof(AddDebt), choice);
                    //this.Content = mynewPage;

                }


                if ((int)res.Id == 2)
                {
                    var msgbox2 = new MessageDialog("Nevermind then... :|", "");
                    await msgbox2.ShowAsync();
                }


            }


        }

        private void SettingsClick(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }

        private void TrachClick(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Trash));
        }

        



        private void moneyButton_Click(object sender, RoutedEventArgs e)
        {
            edit_button.Visibility = Visibility.Collapsed;
            remove_button.Visibility = Visibility.Collapsed;
            Category_choosen.Visibility = Visibility.Collapsed;
            objectBorrowButton.IsEnabled = true;
            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries.Where(o => o.Object == null && o.Type.TypeId == 1 && o.Is_active).ToList();
                Lend_list.ItemsSource = db.Entries.Where(o => o.Object == null && o.Type.TypeId == 2 && o.Is_active).ToList();
            }
            if (temp2 == "Borrow")
            {


                if (Borrow_list.Visibility == Visibility.Visible)
                {
                    Borrow_list.Visibility = Visibility.Collapsed;
                    grid_categories.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Borrow_list.Visibility = Visibility.Visible;
                    grid_categories.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (Lend_list.Visibility == Visibility.Visible)
                {
                    Lend_list.Visibility = Visibility.Collapsed;
                    grid_categories.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Lend_list.Visibility = Visibility.Visible;
                    grid_categories.Visibility = Visibility.Collapsed;
                }
            }


        }



        private void objectButton_Click(object sender, RoutedEventArgs e)
        {
            edit_button.Visibility = Visibility.Collapsed;
            remove_button.Visibility = Visibility.Collapsed;
            if (grid_categories.Visibility == Visibility.Visible)
            {
                grid_categories.Visibility = Visibility.Collapsed;
                Lend_list.Visibility = Visibility.Collapsed;
                Borrow_list.Visibility = Visibility.Collapsed;

            }
            else
            {
                grid_categories.Visibility = Visibility.Visible;
                Lend_list.Visibility = Visibility.Collapsed;
                Borrow_list.Visibility = Visibility.Collapsed;
            }



        }

        private void EditButton(object sender, RoutedEventArgs e)
        {
            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;
           
                using (var db = new CollectorContext())
                {
                var entry = (Entry)Lend_list.SelectedItem;
                var selectedItem = (Entry)Borrow_list.SelectedItem;
                    if (selectedItem == null && entry == null) return;
                  
                   
                    var id = (temp2 == "Borrow") ? selectedItem.EntryId : entry.EntryId;




                    var selectedInfo = (from d in db.Entries
                        .Include(usr => usr.Object)
                        .Include(usr => usr.Object.Category)
                        .Include(usr => usr.Type)
               
                        .Include(usr => usr.Currency)
                        where d.EntryId == id
                        select d).FirstOrDefault();


                    Frame.Navigate(typeof(AddDebt), selectedInfo);
                }
        }


            
        

        private void Lend_list_GotFocus(object sender, RoutedEventArgs e)
        {
            edit_button.Visibility = Visibility.Visible;
            remove_button.Visibility = Visibility.Visible;
        }

        private void Borrow_list_GotFocus(object sender, RoutedEventArgs e)
        {

            edit_button.Visibility = Visibility.Visible;
            remove_button.Visibility = Visibility.Visible;
        }



        private void remove_debt(object sender, RoutedEventArgs e)
        {
            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;
         
            Entry ent = null;

            using (var db = new CollectorContext())
            {
                var entry = (Entry)Lend_list.SelectedItem;
                var selectedItem = (Entry)Borrow_list.SelectedItem;
                if (selectedItem == null && entry == null) return;


                var id = (temp2 == "Borrow")? selectedItem.EntryId: entry.EntryId;

            
                ent = (from d in db.Entries
                   .Include(usr => usr.Object)
                   .Include(usr => usr.Currency)
                   .Include(usr => usr.Type)
                   .Include(usr => usr.Object.Category)
                       where d.EntryId == id
                       select d).FirstOrDefault();


                ent.Is_active = false;

                var typeid = ent.Type.TypeId;
                    
               
                db.SaveChanges();
            
                if(typeid == 1)
                Borrow_list.ItemsSource = (ent.Object != null) ? db.Entries.Where(o => o.Object != null && o.Type.TypeId == typeid && ent.Is_active).ToList() : db.Entries.Where(o => o.Object == null && o.Type.TypeId == typeid && ent.Is_active).ToList();
                else
                Lend_list.ItemsSource = (ent.Object != null) ? db.Entries.Where(o => o.Object != null && o.Type.TypeId == typeid && ent.Is_active).ToList() : db.Entries.Where(o => o.Object == null && o.Type.TypeId == typeid && ent.Is_active).ToList();

            }
        }
        }
    }



