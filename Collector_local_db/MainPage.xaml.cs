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
    /// 

    
   public class Choice
    {
        public bool Borrowed {  get;  set; }
        public bool Object {   get;  set; }  
    }


    public sealed partial class MainPage : Page
    {
        List<Button> buttons = new List<Button>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            PivotItem temp = (PivotItem)general_pivot.SelectedItem;
            String temp2 = (string)temp.Header;

            using (var db = new CollectorContext())
            {
               
                for (int i = 0; i < db.Categories.Count(); i++)
                {
                    RowDefinition gridRow1 = new RowDefinition();
                    gridRow1.Height = new GridLength(35);

                    grid_categories.RowDefinitions.Add(gridRow1);


                    Button newButton = new Button();
                    newButton.Width = 1014;
                    newButton.Height = 30;
                   
                    //newButton.Padding = new Thickness(10, 30 * i + 5, 10,10 );
                    newButton.Content = db.Categories.ToList().ElementAt(i).Cname;

                    newButton.Click += fill_categories;
                    buttons.Add(newButton);
                    Grid.SetRow(newButton, i);
                    grid_categories.Children.Add(newButton);


                  
                }


            }


        }

        private void hide_all(List<Button> buttons)
        {
           foreach(Button b in buttons)
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

            Button button;
            button = sender as Button;


            PivotItem temp = (PivotItem)general_pivot.SelectedItem;
            String temp2 = (string)temp.Header;

            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries.Where(o => o.Object.Category.Cname == (string)button.Content && o.Type.TypeId == 1).ToList();
                Lend_list.ItemsSource = db.Entries.Where(o => o.Object.Category.Cname == (string)button.Content && o.Type.TypeId == 2).ToList();
            }
            hide_all(buttons);
            objectBorrowButton.IsEnabled = false;
            Category_choosen.Visibility = Visibility.Visible;
            Category_choosen.Content = (string)button.Content;
            
                Borrow_list.Visibility = Visibility.Visible;
                Lend_list.Visibility = Visibility.Visible;
            

        }


        private void show_cat_click(object sender, RoutedEventArgs e)
        {
            PivotItem temp = (PivotItem)general_pivot.SelectedItem;
            String temp2 = (string)temp.Header;

            Button button;
            button = sender as Button;
            objectBorrowButton.IsEnabled = true;
            button.Visibility = Visibility.Collapsed;
            if(temp2=="Borrow")
            {
                Borrow_list.Visibility = Visibility.Collapsed;

            }
            else
            {
                Lend_list.Visibility = Visibility.Collapsed;
            }
            show_all(buttons);

        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Choice choice = new Choice();
            
            MessageDialog msgbox = new MessageDialog("What do you want to add ? ");

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
               var res2 = await msgbox2.ShowAsync();

                if ((int)res2.Id == 0)
                {
                    choice.Object = true;
                    choice.Borrowed = true;
                    
                    //Add_debt mynewPage = new Add_debt(choice);
                    //this.Content = mynewPage;
                    this.Frame.Navigate(typeof(Add_debt), choice);

                }

                if ((int)res2.Id == 1)
                {
                    choice.Object = false;
                    choice.Borrowed = true;
                    //Add_debt mynewPage = new Add_debt(choice);
                    //this.Content = mynewPage;
                    this.Frame.Navigate(typeof(Add_debt), choice);
                }

            }
            if ((int)res.Id == 1)
            {
                msgbox.Commands.Clear();
                MessageDialog msgbox3 = new MessageDialog("What did you Lend ?", "");
                msgbox3.Commands.Add(new UICommand { Label = "Object", Id = 0 });
                msgbox3.Commands.Add(new UICommand { Label = "Money", Id = 1 });
                var res2 = await msgbox3.ShowAsync();

                if ((int)res2.Id == 0)
                {
                    choice.Object = true;
                    choice.Borrowed = false;
                    //Add_debt mynewPage = new Add_debt(choice);
                    this.Frame.Navigate(typeof (Add_debt), choice);
                    //this.Content = mynewPage;

                }

                if ((int)res2.Id == 1)
                {
                    choice.Object = false;
                    choice.Borrowed = false;

                    //Add_debt mynewPage = new Add_debt(choice);
                    this.Frame.Navigate(typeof(Add_debt), choice);
                    //this.Content = mynewPage;

                }

              
                if ((int)res.Id == 2)
                {
                    MessageDialog msgbox2 = new MessageDialog("Nevermind then... :|", "");
                    await msgbox2.ShowAsync();
                }


            }


        }

        private void image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

      

        private void moneyButton_Click(object sender, RoutedEventArgs e)
        {

            Category_choosen.Visibility = Visibility.Collapsed;
            objectBorrowButton.IsEnabled = true;
            PivotItem  temp = (PivotItem)general_pivot.SelectedItem;
            String temp2 =(string) temp.Header;
            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries.Where(o => o.Object == null && o.Type.TypeId == 1).ToList();
                Lend_list.ItemsSource = db.Entries.Where(o => o.Object == null && o.Type.TypeId == 2).ToList();
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

            if (grid_categories.Visibility == Visibility.Visible)
            {
                grid_categories.Visibility = Visibility.Collapsed;
                

            }
            else
            {
                grid_categories.Visibility = Visibility.Visible;
                Lend_list.Visibility = Visibility.Collapsed;
                Borrow_list.Visibility = Visibility.Collapsed;
            }

            

        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {

            
            PivotItem temp = (PivotItem)general_pivot.SelectedItem;
            String temp2 = (string)temp.Header;
            if (temp2 == "Borrow")
            {
                using (var db = new CollectorContext())
                {



                    var type = new Type
                    {
                        TypeId = 1
                    };

                    var obj = new Object
                    {
                        ObjectId = ((Collector_local_db.Object)Borrow_list.SelectedItem).ObjectId,

                    };

                    var entry_edit = new Entry
                    {
                        Type = type,
                        
                        Title = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Title,
                        Who = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Who,
                        Desc = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Desc,
                        Priority = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Priority,
                        Amount = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Amount,
                        Date = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Date,
                        Deadline = ((Collector_local_db.Entry)Borrow_list.SelectedItem).Deadline
                        
                    };
                    this.Frame.Navigate(typeof(Add_debt), entry_edit);
                }

            }
            else
            {
                var type = new Type
                {
                    TypeId = 2
                };

                var entry_edit = new Entry
                {
                    Type = type,
                    Title = ((Collector_local_db.Entry)Lend_list.SelectedItem).Title,
                    Who = ((Collector_local_db.Entry)Lend_list.SelectedItem).Who,
                    Desc = ((Collector_local_db.Entry)Lend_list.SelectedItem).Desc,
                    Priority = ((Collector_local_db.Entry)Lend_list.SelectedItem).Priority,
                    Amount = ((Collector_local_db.Entry)Lend_list.SelectedItem).Amount,
                    Date = ((Collector_local_db.Entry)Lend_list.SelectedItem).Date,
                    Deadline = ((Collector_local_db.Entry)Lend_list.SelectedItem).Deadline
                };

                this.Frame.Navigate(typeof(Add_debt), entry_edit);
            }
                
            


            }

        private void Lend_list_GotFocus(object sender, RoutedEventArgs e)
        {
            edit_button.Visibility = Visibility.Visible;
        }

        private void Borrow_list_GotFocus(object sender, RoutedEventArgs e)
        {
           
            edit_button.Visibility = Visibility.Visible;
        }
    }
    }


