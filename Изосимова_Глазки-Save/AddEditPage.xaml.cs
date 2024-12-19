using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace Изосимова_Глазки_Save
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();
        private List<Product> Items;
        private ProductSale currentProductSale = new ProductSale();
        public AddEditPage(Agent SelectedAgent)
        {
           
            InitializeComponent();
            ComboType.SelectedIndex = 0;
            if (SelectedAgent != null) {
                _currentAgent = SelectedAgent;
            }
            var allProducts = ИзосимоваГлазкиSaveEntities.GetContext().Product.ToList();

            var currentProductSales = ИзосимоваГлазкиSaveEntities.GetContext().ProductSale.ToList();
            currentProductSales = currentProductSales.Where(p => p.AgentID == _currentAgent.ID).ToList();

            History.ItemsSource = currentProductSales;


            DataContext = currentProductSale;

            DataContext = _currentAgent;

            LoadItems();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentAgent == null)
            {
                MessageBox.Show("Текущий агент не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Выход из метода, если _currentAgent равен null
            }

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentAgent.Logo = myOpenFileDialog.FileName;
                Image.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            _currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;
            if (string.IsNullOrWhiteSpace(_currentAgent.Title)) { 
                errors.AppendLine("Укажите наименование агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Address)) { 
                errors.AppendLine("Укажите адрес агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName)) { 
                errors.AppendLine("Укажите ФИО директора");
            }
            if (ComboType.SelectedItem == null) { 
                errors.AppendLine("Укажите тип агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString())) { 
                errors.AppendLine("Укажите приоритет агента");
            }
            if (_currentAgent.Priority <= 0) { 
                errors.AppendLine("Укажите положительный приоритет агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
            {
                errors.AppendLine("Укажите ИНН агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.KPP)) { 
                errors.AppendLine("Укажите КПП агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Phone)) { 
                errors.AppendLine("Укажите телефон агента");
            }
            else
            {
                string ph = _currentAgent.Phone.Replace("(", "").Replace("-", "").Replace("+", "-");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || ph[1] == '3' && ph.Length != 12)
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Email)) { 
                errors.AppendLine("Укажите почту агента");
            }
            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return; 
            }
            
            if (_currentAgent.ID == 0)
                ИзосимоваГлазкиSaveEntities.GetContext().Agent.Add(_currentAgent);
            try
            {
                ИзосимоваГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentAgent = (sender as Button).DataContext as Agent;

            var currentProductSale= ИзосимоваГлазкиSaveEntities.GetContext().ProductSale.ToList();
            currentProductSale = currentProductSale.Where(p =>( p.AgentID == currentAgent.ID)).ToList();
            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
            else
            {
                if(MessageBox.Show("Вы точно хотите выполнить удаление?","Вниманние!", MessageBoxButton.YesNo, MessageBoxImage.Question)== MessageBoxResult.Yes)
                {
                    try {
                        ИзосимоваГлазкиSaveEntities.GetContext().Agent.Remove(currentAgent);
                        ИзосимоваГлазкиSaveEntities.GetContext().SaveChanges();
                    }
                    catch(Exception ex) {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }

        }

        private void LoadItems()
        {
            // Здесь вы загружаете элементы из базы данных
            Items = ИзосимоваГлазкиSaveEntities.GetContext().Product.ToList();
            ComboTitle.ItemsSource = Items; // Устанавливаем источник данных для ComboBox
        }

        private void SearchBoxForComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBoxForComboBox.Text.ToLower();
            var filteredItems = Items.Where(p => p.Title.ToLower().Contains(searchText)).ToList();
            ComboTitle.ItemsSource = filteredItems;

            // Если ничего не найдено, показываем все элементы
            if (string.IsNullOrEmpty(searchText))
            {
                ComboTitle.ItemsSource = Items;
            }
        }

        private void ComboTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboTitle.SelectedItem is Product selectedProduct)
            {
                // Подставляем значение Title в TextBox
                SearchBoxForComboBox.Text = selectedProduct.Title; // Убедитесь, что у вас есть TextBox с именем SearchBoxForComboBox
            }
        }

        private void AddHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ComboTitle.SelectedItem as Product;

            // Получаем количество из TextBox (например, для количества продукта)
            int productCount;
            if (!int.TryParse(CountProductTB.Text, out productCount) || productCount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (selectedProduct != null)
            {
                // Создаем новый объект ProductSale
                var newSale = new ProductSale
                {
                    AgentID = _currentAgent.ID, // Указываем ID агента
                    ProductID = selectedProduct.ID, // Указываем ID выбранного продукта
                    SaleDate = StartDate.SelectedDate ?? DateTime.Now, // Указываем дату продажи (если выбрана)
                    ProductCount = productCount // Указываем количество, введенное пользователем
                };

                // Добавляем новый объект в контекст и сохраняем изменения
                ИзосимоваГлазкиSaveEntities.GetContext().ProductSale.Add(newSale);
                ИзосимоваГлазкиSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("информация сохранена");

                // Обновляем список продаж
                LoadProductSalesForCurrentAgent();

                // Очистка полей ввода (по желанию)
                ComboTitle.SelectedItem = null;
                CountProductTB.Clear();
                StartDate.SelectedDate = null;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для добавления.");
            }

        }
        private void LoadProductSalesForCurrentAgent()
        {
            var currentProductSales = ИзосимоваГлазкиSaveEntities.GetContext().ProductSale
                .Where(ps => ps.AgentID == _currentAgent.ID) // Предполагается, что у ProductSale есть поле AgentID
                .ToList();

            // Устанавливаем источник данных для списка продаж
            History.ItemsSource = currentProductSales;
        }


        private void DelHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = History.SelectedItems.Cast<ProductSale>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один элемент для удаления.");
                return;
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление? ", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = ИзосимоваГлазкиSaveEntities.GetContext();

                        // Удаляем каждый выбранный элемент из контекста
                        foreach (var item in selectedItems)
                        {
                            context.ProductSale.Remove(item);
                        }

                        // Сохраняем изменения в базе данных
                        context.SaveChanges();

                        LoadProductSalesForCurrentAgent();

                        MessageBox.Show("Элементы успешно удалены.");
                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }

        }
    }
}
