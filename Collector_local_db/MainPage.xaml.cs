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

        
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<Button> buttons = new List<Button>();
          
            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries.Where(o => o.Type.TypeId == 1);
                Lend_list.ItemsSource = db.Entries.Where(o => o.Type.TypeId == 2);
                

                for (int i = 0; i < db.Categories.Count(); i++)
                {
                    RowDefinition gridRow1 = new RowDefinition();
                    gridRow1.Height = new GridLength(35);

                    grid_borrow.RowDefinitions.Add(gridRow1);


                    Button newButton = new Button();
                    newButton.Width = 1014;
                    newButton.Height = 30;
              
                    //newButton.Padding = new Thickness(10, 30 * i + 5, 10,10 );
                    newButton.Content = db.Categories.ToList().ElementAt(i).Cname;

                    newButton.Click += fill_categories;
                    buttons.Add(newButton);
                    Grid.SetRow(newButton, i);
                    grid_borrow.Children.Add(newButton);



                }


            }


        }

        private void fill_categories(object sender, RoutedEventArgs e)
        {

            Button button;

           button = sender as Button;
           

            using (var db = new CollectorContext())
            {
                Borrow_list_object.ItemsSource = db.Entries.Where(o => o.Object.Category.Cname == (string)button.Content).ToList();
                
            }

            if (Borrow_list_object.Visibility == Visibility.Visible)
                Borrow_list_object.Visibility = Visibility.Collapsed;
            else
                Borrow_list_object.Visibility = Visibility.Visible;
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

      

        private void moneyBorrowButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries.Where(o => o.Object == null).ToList();
            }
            

            if (Borrow_list.Visibility == Visibility.Visible)
                Borrow_list.Visibility = Visibility.Collapsed;
            else
            {
                Borrow_list.Visibility = Visibility.Visible;
               
            }

        }

        private void moneyLendButton_Click(object sender, RoutedEventArgs e)
        {
            if (Lend_list.Visibility == Visibility.Visible)
                Lend_list.Visibility = Visibility.Collapsed;
            else
            {
                Lend_list.Visibility = Visibility.Visible;
                
            }
        }

        private void objectLendButton_Click(object sender, RoutedEventArgs e)
        {
           

        }

        private void objectBorrowButton_Click(object sender, RoutedEventArgs e)
        {
            grid_borrow.Visibility = Visibility.Visible;

            using (var db = new CollectorContext())
            {
                Borrow_list.ItemsSource = db.Entries.Where(o => o.Object != null).ToList();
            }

        }
    }
    }


