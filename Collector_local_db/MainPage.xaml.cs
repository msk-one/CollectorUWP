using Microsoft.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static Windows.UI.Xaml.Controls.ContentDialogResult;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

// 1- kosz 
// 0 - nie kosz


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

    internal class Login
    {
        public Login(string username, string password)
        {
            this.login = username;
            this.password = password;
        }

        public string login;
        public string password;

    }

    internal class BackLogin
    {
        public int uid { get; set; }
        public string login { get; set; }
        public string token { get; set; }
    }

    public sealed partial class MainPage
    {
        readonly List<Button> _pCategoryButtons = new List<Button>();

        public MainPage()
        {
            InitializeComponent();


            if (SerwerFunction.Uid == -1)
            {
                var dialog = new ContentDialog()
                {
                    Title = "Log in:",
                    RequestedTheme = ElementTheme.Dark,
                   
                    MaxWidth = this.ActualWidth
                };

                // Setup Content
                var panel = new StackPanel();
                
                var usrnName = new TextBox()
                {
                    Name = "UsernameBox",
                    PlaceholderText = "Username",
                    TextWrapping = TextWrapping.Wrap

                };

               

                panel.Children.Add(usrnName);
                var passwd = new PasswordBox
                {
                    PlaceholderText = "Password:",
                    Name = "PasswdBox"
                };

                usrnName.TextChanged += delegate
                {
                    dialog.IsPrimaryButtonEnabled = (passwd.Password != "" && usrnName.Text != "") ? true : false;
                };

                passwd.PasswordChanged += delegate
                {
                    dialog.IsPrimaryButtonEnabled = (passwd.Password != "" && usrnName.Text != "") ? true : false;
                };

                panel.Children.Add(passwd);


                var cb = new CheckBox {Name = "registercheck"};

                var dockTop = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = { new TextBlock { Text = "I want to register     " }, cb }
                };

                panel.Children.Add(dockTop);

                dialog.Content = panel;

                // Add Buttons
                dialog.PrimaryButtonText = "Log";
                dialog.IsPrimaryButtonEnabled = false;
                
                dialog.PrimaryButtonClick += async delegate
                {
                    var namepath = (cb.IsChecked.HasValue && cb.IsChecked.Value == true)
                        ? "RegisterUser"
                        : "LoginUser";
                    try
                    {
                        var respondLoginInfo =
                            (BackLogin)
                                await
                                    SerwerFunction.Getfromserver<BackLogin>(
                                        namepath, "POST",
                                        new Login(usrnName.Text, passwd.Password));
                        if (respondLoginInfo == null)
                            dialog.ShowAsync();



                        SerwerFunction.password = respondLoginInfo.token;
                        SerwerFunction.login = respondLoginInfo.login;
                        SerwerFunction.Uid = respondLoginInfo.uid;
                        TitleBlock.Text = "COLLECTOR - welcome: " + respondLoginInfo.login;

                    }
                    catch
                    {
                        dialog.Title = "This name is already taken or password is wrong";
                    }


                };
                dialog.SecondaryButtonText = "Cancel";
                dialog.SecondaryButtonClick += delegate
                {
                    Application.Current.Exit();
                };


                var result = dialog.ShowAsync();

            }
        }
        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

            

                //for (var i = 0; i < db.Categories.Count(); i++)
                //{
                //    var gridRow1 = new RowDefinition {Height = new GridLength(35)};

                //    grid_categories.RowDefinitions.Add(gridRow1);


                //    var category = new Button
                //    {
                //        Width = double.NaN,
                //        Height = 30,
                //        Content = db.Categories.ToList().ElementAt(i).cname
                //    };
                //    category.Click += fill_categories;


                //    _pCategoryButtons.Add(category);
                //    Grid.SetRow(category, i);
                //    grid_categories.Children.Add(category);



                }


            


        

        private static void hide_all(IEnumerable<Button> buttons)
        {
            foreach (var b in buttons)
            {
                b.Visibility = Visibility.Collapsed;

            }

        }

        private static void show_all(IEnumerable<Button> buttons)
        {
            foreach (var b in buttons)
            {
                b.Visibility = Visibility.Visible;

            }

        }

        private void fill_categories(object sender, RoutedEventArgs e)
        {
            var itemInCategory = sender as Button;


            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

           






                //Borrow_list.ItemsSource = db.Entries
                //     .Include(usr => usr.Object)
                //     .Include(usr => usr.Object.Category)
                //     .Include(usr => usr.Type)
                //     .Where(o => o.Object.Category.cname == (string)itemInCategory.Content && o.Type.tid == 1 && o.archived== 0)
                //     .ToList();

                //Lend_list.ItemsSource = db.Entries
                //    .Include(usr => usr.Object)
                //    .Include(usr => usr.Object.Category)
                //    .Include(usr => usr.Type)
                //    .Where(o => o.Object.Category.cname == (string)itemInCategory.Content && o.Type.tid == 2 && o.archived ==0)
                //    .ToList();
            
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
                Borrow_list.Visibility = Visibility.Collapsed;
            else
                Lend_list.Visibility = Visibility.Collapsed;

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
                var msgbox2 = new MessageDialog("What did you borrowed ?", "");
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
                   
                    Frame.Navigate(typeof(AddDebt), choice);
               

                }

                if ((int)res2.Id == 1)
                {
                    choice.Object = false;
                    choice.Borrowed = false;

                    Frame.Navigate(typeof(AddDebt), choice);
          

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

            
                //Borrow_list.ItemsSource = db.Entries.Where(o => o.Object == null && o.Type.tid == 1 && o.archived ==0).ToList();
                //Lend_list.ItemsSource = db.Entries.Where(o => o.Object == null && o.Type.tid == 2 && o.archived == 0).ToList();
            
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
           
               
                var entry = (ProjectClasses.Entry)Lend_list.SelectedItem;
                var selectedItem = (ProjectClasses.Entry)Borrow_list.SelectedItem;
                    if (selectedItem == null && entry == null) return;
                  
                   
                    var id = (temp2 == "Borrow") ? selectedItem.id : entry.id;




                    //var selectedInfo = (from d in db.Entries
                    //    .Include(usr => usr.Object)
                    //    .Include(usr => usr.Object.Category)
                    //    .Include(usr => usr.Type)
               
                    //    .Include(usr => usr.Currency)
                    //    where d.id == id
                    //    select d).FirstOrDefault();


                   // Frame.Navigate(typeof(AddDebt), selectedInfo);
                
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
         
            ProjectClasses.Entry ent = null;

            
                var entry = (ProjectClasses.Entry)Lend_list.SelectedItem;
                var selectedItem = (ProjectClasses.Entry)Borrow_list.SelectedItem;
                if (selectedItem == null && entry == null) return;


                var id = (temp2 == "Borrow")? selectedItem.id: entry.id;

            
                //ent = (from d in db.Entries
                //   .Include(usr => usr.Object)
                //   .Include(usr => usr.Currency)
                //   .Include(usr => usr.Type)
                //   .Include(usr => usr.Object.Category)
                //       where d.id == id
                //       select d).FirstOrDefault();


                //ent.archived = 1;

                //var typeid = ent.Type.tid;
                    
               
                //db.SaveChanges();
            
                //if(typeid == 1)
                //Borrow_list.ItemsSource = (ent.Object != null) ? db.Entries.Where(o => o.Object != null && o.Type.tid == typeid && ent.archived == 0).ToList() : db.Entries.Where(o => o.Object == null && o.Type.tid == typeid && ent.archived == 0).ToList();
                //else
                //Lend_list.ItemsSource = (ent.Object != null) ? db.Entries.Where(o => o.Object != null && o.Type.tid == typeid && ent.archived == 0).ToList() : db.Entries.Where(o => o.Object == null && o.Type.tid == typeid && ent.archived == 0).ToList();

            
        }

        private void send_debt(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog()
            {
                Title = "Send this to:",
                RequestedTheme = ElementTheme.Dark,
                //FullSizeDesired = true,
                MaxWidth = this.ActualWidth
            };

            // Setup Content
            var panel = new StackPanel();

            var usrnName = new TextBox()
            {
                Name = "UsernameBox",
                PlaceholderText = "Username",
                TextWrapping = TextWrapping.Wrap

            };

            panel.Children.Add(usrnName);
            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = "Send";

           // TODO: dodac api które po wyslaniu username zwroci mi jego id
            dialog.PrimaryButtonClick += async delegate
            {


                var respondLoginInfo =
                    (BackLogin)
                        await
                            SerwerFunction.Getfromserver<BackLogin>(
                                "Users", "GET",
                               null);
                SerwerFunction.Uid = respondLoginInfo.uid;



                };
                dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate
            {
                return;
            };


            var result = dialog.ShowAsync();

            

        }
    }
    }



