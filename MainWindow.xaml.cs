using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoGeistModel;

namespace Lazar_Sorin_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState { New, Edit, Delete, Nothing }

    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoGeistEntitiesModel ctx = new AutoGeistEntitiesModel();
        CollectionViewSource carViewSource;
        CollectionViewSource customerViewSource;
        CollectionViewSource carOrdersViewSource;
        Binding bodyStyleTextBoxBinding = new Binding();
        Binding modelTextBoxBinding = new Binding();
        Binding makeTextBoxBinding = new Binding();
        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();
        Binding purchaseDatePickerBinding = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            modelTextBoxBinding.Path = new PropertyPath("Model");
            makeTextBoxBinding.Path = new PropertyPath("Make");
            bodyStyleTextBoxBinding.Path = new PropertyPath("BodyStyle");
            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            purchaseDatePickerBinding.Path = new PropertyPath("PurchaseDate");
            modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
            bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            purchaseDatePicker.SetBinding(TextBox.TextProperty, purchaseDatePickerBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // System.Windows.Data.CollectionViewSource carViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource")));
            // System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            this.carViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carViewSource")));
            this.carViewSource.Source = ctx.Cars.Local;
            ctx.Cars.Load();
            this.customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            this.customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            carOrdersViewSource =((System.Windows.Data.CollectionViewSource)(this.FindResource("carOrdersViewSource")));
            // carOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();
            BindDataGrid();
            cbCars.ItemsSource = ctx.Cars.Local;
            // cbCars.DisplayMemberPath = "Make";
            cbCars.SelectedValuePath = "CarId";
            cbCustomers.ItemsSource = ctx.Customers.Local;
            // cbCustomers.DisplayMemberPath = "FirstName";
            cbCustomers.SelectedValuePath = "CustId";
            
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            TabItem ti = tbCtrlAutoGeist.SelectedItem as TabItem;
            switch (ti.Header)
            {
                case "Cars":
                    BindingOperations.ClearBinding(bodyStyleTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(modelTextBox, TextBox.TextProperty);
                    bodyStyleTextBox.Text = "";
                    makeTextBox.Text = "";
                    modelTextBox.Text = "";
                    Keyboard.Focus(bodyStyleTextBox);
                    break;
                case "Customers":
                    BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(purchaseDatePicker, DatePicker.SelectedDateProperty);
                    firstNameTextBox.Text = "";
                    lastNameTextBox.Text = "";
                    purchaseDatePicker.SelectedDate = DateTime.Now;
                    Keyboard.Focus(firstNameTextBox);
                    SetValidationBinding();
                    break;
                case "Orders":
                    break;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            SetValidationBinding();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            this.carViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            this.carViewSource.View.MoveCurrentToNext();
        }

        private void SaveCars()
        {
            Car car = null;
            if (action == ActionState.New)
            {
                try
                {
                    car = new Car()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Model = modelTextBox.Text.Trim(),
                        BodyStyle = bodyStyleTextBox.Text.Trim(),
                    };
                    ctx.Cars.Add(car);
                    ctx.SaveChanges();

                    this.carViewSource.View.Refresh();
                    this.carViewSource.View.MoveCurrentTo(car);
                }
                // using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty,
               bodyStyleTextBoxBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    car = (Car)carDataGrid.SelectedItem;
                    car.Make = makeTextBox.Text.Trim();
                    car.Model = modelTextBox.Text.Trim();
                    car.BodyStyle = bodyStyleTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.carViewSource.View.Refresh();
                this.carViewSource.View.MoveCurrentTo(car);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty,
               bodyStyleTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    car = (Car)carDataGrid.SelectedItem;
                    ctx.Cars.Remove(car);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.carViewSource.View.Refresh();
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, modelTextBoxBinding);
                bodyStyleTextBox.SetBinding(TextBox.TextProperty, bodyStyleTextBoxBinding);
            }
        }

        private void btnPrevCust_Click(object sender, RoutedEventArgs e)
        {
            this.customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextCust_Click(object sender, RoutedEventArgs e)
        {
            this.customerViewSource.View.MoveCurrentToNext();
        }

        private void SaveCust()
        {
            Customer cust = null;
            if (action == ActionState.New)
            {
                try
                {
                    cust = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),
                        PurchaseDate = purchaseDatePicker.SelectedDate,
                    };
                    ctx.Customers.Add(cust);
                    this.customerViewSource.View.Refresh();
                    this.customerViewSource.View.MoveCurrentTo(cust);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                purchaseDatePicker.SetBinding(TextBox.TextProperty, purchaseDatePickerBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    cust = (Customer)customerDataGrid.SelectedItem;
                    cust.FirstName = firstNameTextBox.Text.Trim();
                    cust.LastName = lastNameTextBox.Text.Trim();
                    cust.PurchaseDate = purchaseDatePicker.SelectedDate;
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.customerViewSource.View.Refresh();
                this.customerViewSource.View.MoveCurrentTo(cust);

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                purchaseDatePicker.SetBinding(TextBox.TextProperty, purchaseDatePickerBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    cust = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(cust);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.customerViewSource.View.Refresh();

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
                purchaseDatePicker.SetBinding(TextBox.TextProperty, purchaseDatePickerBinding);
            }
        }

        private void gbOperations_Click(object sender, RoutedEventArgs e)
        {
            Button SelectedButton = (Button)e.OriginalSource;
            Panel panel = (Panel)SelectedButton.Parent;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                if (B != SelectedButton)
                    B.IsEnabled = false;
            }
            gbActions.IsEnabled = true;
        }

        private void ReInitialize()
        {
            Panel panel = gbOperations.Content as Panel;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                B.IsEnabled = true;
            }
            gbActions.IsEnabled = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReInitialize();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tbCtrlAutoGeist.SelectedItem as TabItem;
            switch (ti.Header)
            {
                case "Cars":
                    SaveCars();
                    break;
                case "Customers":
                    SaveCust();
                    break;
                case "Orders":
                    break;
            }
            ReInitialize();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join car in ctx.Cars on ord.CarId equals car.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 car.Make,
                                 car.Model
                             };
            carOrdersViewSource.Source = queryOrder.ToList();
        }

        private void SaveOrders()
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Car car = (Car)cbCars.SelectedItem;
                    Customer customer = (Customer)cbCustomers.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {
                        CarId = car.CarId,
                        CustId = customer.CustId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId ==
                    curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CarId =
                       Convert.ToInt32(cbCars.SelectedValue.ToString());
                        editedOrder.CustId =
                       Int32.Parse(cbCustomers.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                carOrdersViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId ==
                   curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void SetValidationBinding()
        {
            // System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));

            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = this.customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());

            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = this.customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLength());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }

    }
}
