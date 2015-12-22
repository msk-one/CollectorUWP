using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
            if (SerwerFunction.Uid != -1) return;

            var dialog = new ContentDialog()
            {
                Title = "Log in:",
                RequestedTheme = ElementTheme.Dark,

                MaxWidth = this.ActualWidth
            };

            // Setup Content
           

            var usrnName = new TextBox()
            {
                Name = "UsernameBox",
                PlaceholderText = "Username",
                TextWrapping = TextWrapping.Wrap

            };



            
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


            var cb = new CheckBox { Name = "registercheck" };

            var dockTop = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children = { new TextBlock { Text = "I want to register \t" }, cb }
            };

            var panel = new StackPanel
            {
                Children = { usrnName, passwd, dockTop }

            };

            dialog.Content = panel;

            
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
                    dialog.FontSize = 5;
                }


            };
            dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate
            {
                Application.Current.Exit();
            };


            var result = dialog.ShowAsync();


        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(SerwerFunction.Uid != -1)
                TitleBlock.Text = "COLLECTOR - welcome: " + SerwerFunction.login;
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





        private async void moneyButton_Click(object sender, RoutedEventArgs e)
        {



            ButtonsVisibility(Visibility.Collapsed);

            Category_choosen.Visibility = Visibility.Collapsed;
            objectBorrowButton.IsEnabled = true;

            var temp2 = (string) ((PivotItem) general_pivot.SelectedItem).Header;


            if (temp2 == "Borrow")
            {

                if (Borrow_list.Visibility == Visibility.Visible)
                {
                    Borrow_list.Visibility = Visibility.Collapsed;
                    grid_categories.Visibility = Visibility.Collapsed;
                    return;
                }

            }
            else
            {
                if (Lend_list.Visibility == Visibility.Visible)
                {
                    Lend_list.Visibility = Visibility.Collapsed;
                    grid_categories.Visibility = Visibility.Collapsed;
                    return;
                }
              
            }


            ProgressUpload.Visibility = Visibility.Visible;
            ProgressUpload.IsIndeterminate = true;
            Borrow_list.ItemsSource = ((List<ProjectClasses.Entry>) await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesMoneyForUser/" + SerwerFunction.Uid + "/AndType/1", "GET", null)).ToList()
                .Where(o => o.archived == 0);

            Lend_list.ItemsSource = ((List<ProjectClasses.Entry>) await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesMoneyForUser/" + SerwerFunction.Uid + "/AndType/2", "GET", null)).ToList()
                .Where(o => o.archived == 0);
            


            ProgressUpload.IsIndeterminate = false;
            ProgressUpload.Visibility = Visibility.Collapsed;


            if (temp2 == "Borrow")
            {

                if (Borrow_list.Visibility == Visibility.Collapsed)
                {
                    Borrow_list.Visibility = Visibility.Visible;
                    grid_categories.Visibility = Visibility.Collapsed;
                }

            }
            else
            {
                if(Lend_list.Visibility == Visibility.Collapsed)
                {
                    Lend_list.Visibility = Visibility.Visible;
                    grid_categories.Visibility = Visibility.Collapsed;
                }
            }

        }


        private async void objectButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonsVisibility(Visibility.Collapsed);


            if (grid_categories.Visibility == Visibility.Visible)
                {
                    Borrow_list.Visibility = Visibility.Collapsed;
                    Borrow_list.Visibility = Visibility.Collapsed;
                    grid_categories.Visibility = Visibility.Collapsed;
                    return;
                }

            ProgressUpload.Visibility = Visibility.Visible;
            ProgressUpload.IsIndeterminate = true;
            ProjectClasses.AllCategories = ((List<ProjectClasses.Category>)await
               SerwerFunction.Getfromserver<List<ProjectClasses.Category>>(
                   "Categories/", "GET", null)).ToList();

            
            ProgressUpload.IsIndeterminate = false;
            ProgressUpload.Visibility = Visibility.Collapsed;


            grid_categories.Children.Clear();

            for (var i = 0; i < ProjectClasses.AllCategories.Count; i++)
            {
                var gridRow1 = new RowDefinition { Height = new GridLength(35) };

                grid_categories.RowDefinitions.Add(gridRow1);


                var category = new Button
                {
                    Width = 300,
                    Height = 30,
                    Content = ProjectClasses.AllCategories.ElementAt(i).cname
                };
                category.Click += fill_categories;


                _pCategoryButtons.Add(category);
                Grid.SetRow(category, i);
                grid_categories.Children.Add(category);



            }


 


            

                if (grid_categories.Visibility == Visibility.Collapsed)
                {
                    Lend_list.Visibility = Visibility.Collapsed;
                    Borrow_list.Visibility = Visibility.Collapsed;
                    grid_categories.Visibility = Visibility.Visible;
                }

            
            




        }


        private async void fill_categories(object sender, RoutedEventArgs e)
        {
            var itemInCategory = sender as Button;


            ProgressUpload.Visibility = Visibility.Visible;
            ProgressUpload.IsIndeterminate = true;


            int catid =
                           (int)
                               await
                                   SerwerFunction.Getfromserver<int>(
                                       "GetCategoryIdFromName/" + (string)itemInCategory.Content, "GET",
                                      null);


            Borrow_list.ItemsSource =

                ((List<ProjectClasses.Entry>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesForUser/" + SerwerFunction.Uid + "/AndType/1/AndObjCategory/" + catid, "GET", null)).ToList().Where(o => o.archived == 0);

            Lend_list.ItemsSource =

                ((List<ProjectClasses.Entry>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesForUser/" + SerwerFunction.Uid + "/AndType/2/AndObjCategory/" + catid, "GET", null)).ToList().Where(o => o.archived == 0);


            ProgressUpload.IsIndeterminate = false;
            ProgressUpload.Visibility = Visibility.Collapsed;

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


        private async void EditButton(object sender, RoutedEventArgs e)
        {
            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;
           
               
                var selectedItemLend = (ProjectClasses.Entry)Lend_list.SelectedItem;
                var selectedItemBorrow = (ProjectClasses.Entry)Borrow_list.SelectedItem;
                    if (selectedItemBorrow == null && selectedItemLend == null) return;
                  
                   
                    var id = (temp2 == "Borrow") ? selectedItemBorrow.id : selectedItemLend.id;

            var entryGet =
                          (ProjectClasses.Entry)
                              await
                                  SerwerFunction.Getfromserver<ProjectClasses.Entry>(
                                      "Entries/" + id, "GET",
                                     null);


             Frame.Navigate(typeof(AddDebt), entryGet);

        }


            
        

        private void Lend_list_GotFocus(object sender, RoutedEventArgs e)
        {
            ButtonsVisibility(Visibility.Visible);
        }

        private void Borrow_list_GotFocus(object sender, RoutedEventArgs e)
        {

            ButtonsVisibility(Visibility.Visible);
        }



        private async void remove_debt(object sender, RoutedEventArgs e)
        {
           

            var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

            var selectedItemLend = (ProjectClasses.Entry)Lend_list.SelectedItem;
                var selectedItemBorrow = (ProjectClasses.Entry)Borrow_list.SelectedItem;
                if (selectedItemBorrow == null && selectedItemLend == null) return;

                var id = (temp2 == "Borrow")? selectedItemBorrow.id: selectedItemLend.id;



            var entryGet =
                          (ProjectClasses.Entry)
                              await
                                  SerwerFunction.Getfromserver<ProjectClasses.Entry>(
                                      "Entries/" + id, "GET",
                                     null);


            entryGet.archived = 1;


            var typeid = entryGet.Type.tid;

            var entryPost =
                          (ProjectClasses.Entry)
                              await
                                  SerwerFunction.Getfromserver<ProjectClasses.Entry>(
                                      "Entries/" + id, "PUT",
                                     entryGet);








            if (typeid == 1)
            {
                if (entryGet.Object != null)
                {
                    Borrow_list.ItemsSource = ((List<ProjectClasses.Entry>) await
                        SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                            "GetEntriesForUser/" + SerwerFunction.Uid + "/AndType/1/AndObjCategory/" +
                            entryGet.Object.catid, "GET", null)).ToList().Where(o => o.archived == 0);
                }
                else
                {
                    Borrow_list.ItemsSource = ((List<ProjectClasses.Entry>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesMoneyForUser/" + SerwerFunction.Uid + "/AndType/1", "GET", null)).ToList().Where(o => o.archived == 0);
                }


              
            }
            else
            {
                if (entryGet.Object != null)
                {
                    Lend_list.ItemsSource = ((List<ProjectClasses.Entry>)await
                        SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                            "GetEntriesForUser/" + SerwerFunction.Uid + "/AndType/2/AndObjCategory/" +
                            entryGet.Object.catid, "GET", null)).ToList().Where(o => o.archived == 0);
                }
                else
                {
                    Lend_list.ItemsSource = ((List<ProjectClasses.Entry>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesMoneyForUser/" + SerwerFunction.Uid + "/AndType/2", "GET", null)).ToList().Where(o => o.archived == 0);
                }

            }
            ButtonsVisibility(Visibility.Collapsed);
        }

        private async void send_debt(object sender, RoutedEventArgs e)
        {
            ButtonsVisibility(Visibility.Collapsed);

            ProjectClasses.AllUsers = ((List<ProjectClasses.User>)await
               SerwerFunction.Getfromserver<List<ProjectClasses.User>>(
                   "Users", "GET", null)).ToList();
            var dialog = new ContentDialog()
            {
                Title = "Send this to:",
                RequestedTheme = ElementTheme.Dark,
        
                MaxWidth = ActualWidth
            };

            // Setup Content
            var panel = new StackPanel();

            var usrnName = new AutoSuggestBox
            {
                Name = "UsernameBox",
                
                
              

            };

            usrnName.TextChanged += (sender2, args) =>
            {
                if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
                if (sender2.Text.Length > 0)
                {
                    sender2.ItemsSource = GetSuggestions(sender2.Text);
                    dialog.IsPrimaryButtonEnabled = true;
                }
                else
                {
                    sender2.ItemsSource = new[] { "No suggestions..." };
                    dialog.IsPrimaryButtonEnabled = false;
                }
            };
            panel.Children.Add(usrnName);
            dialog.Content = panel;
            dialog.IsPrimaryButtonEnabled = false;
            // Add Buttons
            dialog.PrimaryButtonText = "Send";

           
            dialog.PrimaryButtonClick += async delegate
            {

                var temp2 = (string)((PivotItem)general_pivot.SelectedItem).Header;

                var selectedItemLend = (ProjectClasses.Entry)Lend_list.SelectedItem;
                var selectedItemBorrow = (ProjectClasses.Entry)Borrow_list.SelectedItem;
                if (selectedItemBorrow == null && selectedItemLend == null) return;

                var id = (temp2 == "Borrow") ? selectedItemBorrow.id : selectedItemLend.id;

                try
                {
                    int userSendId =
                        (int)
                            await
                                SerwerFunction.Getfromserver<int>(
                                    "GetUserIdFromName/" + usrnName.Text, "GET",
                                    null);


                    var entryGet =
                        (ProjectClasses.Entry)
                            await
                                SerwerFunction.Getfromserver<ProjectClasses.Entry>(
                                    "Entries/" + id, "GET",
                                    null);

                    if(entryGet == null)
                        dialog.ShowAsync();

                    entryGet.typeid = (temp2 == "Borrow") ? 2 : 1;
                    entryGet.User = null;
                    entryGet.Object = null;
                    entryGet.Currency = null;
                    entryGet.Type = null;
                    entryGet.userid = userSendId;

                    entryGet.who = SerwerFunction.login;
                    var entryPost =
                        (ProjectClasses.Entry)
                            await
                                SerwerFunction.Getfromserver<ProjectClasses.Entry>(
                                    "Entries", "POST",
                                    entryGet);
                }
                catch
                {
                    dialog.Title = "This name is already taken or password is wrong";
                    
                }


            };
                dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate
            { };


            var result = dialog.ShowAsync();

            

        }

       


        private static string[] GetSuggestions(string text)
        {
             return  ProjectClasses.AllUsers.Where(x => x.login.Contains(text) && x.login != SerwerFunction.login).Select(s => s.login).ToArray().Distinct().ToArray();
             
        }


        private void Logout_click(object sender, PointerRoutedEventArgs e)
        {

            ButtonsVisibility(Visibility.Collapsed);

            var dialog = new ContentDialog()
            {
                Title = "Log in:",
                RequestedTheme = ElementTheme.Dark,

                MaxWidth = ActualWidth
            };

            var usrnName = new TextBox()
            {
                Name = "UsernameBox",
                PlaceholderText = "Username",
                TextWrapping = TextWrapping.Wrap

            };

            
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

           


            var cb = new CheckBox { Name = "registercheck" };

            var dockTop = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children = { new TextBlock { Text = "I want to register \t" }, cb }
            };
            var panel = new StackPanel
            {
                Children = { usrnName, passwd, dockTop }
                
            };

            dialog.Content = panel;

            dialog.PrimaryButtonText = "Log";
            dialog.IsPrimaryButtonEnabled = false;

            dialog.PrimaryButtonClick += async delegate
            {
                var namepath = cb.IsChecked.HasValue && cb.IsChecked.Value
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



                    SerwerFunction.password = respondLoginInfo?.token;
                    SerwerFunction.login = respondLoginInfo?.login;
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

        private void StatsClick(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Charts));
        }


        private void ButtonsVisibility(Visibility vis)
        {
            
            send_button.Visibility = vis;
            remove_button.Visibility = vis;
            edit_button.Visibility = vis;
        }
    }
}



