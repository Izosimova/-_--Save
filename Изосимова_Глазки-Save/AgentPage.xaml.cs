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
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;
        public AgentPage()
        {
            InitializeComponent();
            var currentAgents = ИзосимоваГлазкиSaveEntities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgents;
            ComboType.SelectedIndex = 0;
            ComboAgentType.SelectedIndex = 0;
            UpdateAgents();
        }
        private void UpdateAgents()
        {
            var currentAgents = ИзосимоваГлазкиSaveEntities.GetContext().Agent.ToList();
            string searchAgent = TBoxSearch.Text.ToLower().Replace("+", " ").Replace("(", " ").Replace(")", " ").Replace(" ", "");
            currentAgents = currentAgents.Where(p => p.Title.ToLower().Contains(searchAgent) || p.Phone.Replace("+", " ").Replace("(", " ").Replace(")", " ").Replace(" ", "").Replace("-", "").Contains(searchAgent) || p.Email.ToLower().Contains(searchAgent)).ToList();
            AgentListView.ItemsSource = currentAgents.ToList();
            if (ComboAgentType.SelectedIndex == 0) { 
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("МФО") || p.AgentType.Title.Contains("ООО") || p.AgentType.Title.Contains("ЗАО") || p.AgentType.Title.Contains("МКК") || p.AgentType.Title.Contains("ОАО") || p.AgentType.Title.Contains("ПАО"))).ToList();
            }
            if (ComboAgentType.SelectedIndex == 1)
            {
                currentAgents = currentAgents.Where(p =>(p.AgentType.Title.Contains("МФО"))).ToList();
            }

            if (ComboAgentType.SelectedIndex == 2)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ООО"))).ToList();
            }

            if (ComboAgentType.SelectedIndex == 3)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ЗАО"))).ToList();
            }

            if (ComboAgentType.SelectedIndex == 4)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("МКК"))).ToList();
            }

            if (ComboAgentType.SelectedIndex == 5)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ОАО"))).ToList();
            }

            if (ComboAgentType.SelectedIndex == 6)
            {
                currentAgents = currentAgents.Where(p =>(p.AgentType.Title.Contains("ПАО"))).ToList();
            }
            AgentListView.ItemsSource = currentAgents;

            if (ComboType.SelectedIndex == 1)
            {
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            }

            if (ComboType.SelectedIndex == 2)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
            }

            if (ComboType.SelectedIndex == 3)
            {
                currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
            }

            if (ComboType.SelectedIndex == 4)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
            }
            if (ComboType.SelectedIndex == 5)
                currentAgents = currentAgents.OrderBy(p => p.Discount).ToList();
            if (ComboType.SelectedIndex == 6)
                currentAgents = currentAgents.OrderByDescending(p => p.Discount).ToList();
            TableList = currentAgents;
            ChangePage(0, 0);

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void ComboAgentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }
        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBALLRecords.Text = " из " + CountRecords.ToString();
                AgentListView.ItemsSource = CurrentPageList;
                AgentListView.Items.Refresh();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                ИзосимоваГлазкиSaveEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AgentListView.ItemsSource = ИзосимоваГлазкиSaveEntities.GetContext().Agent.ToList();
                UpdateAgents();
            }
        }

        private void AgentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AgentListView.SelectedItems.Count > 1)
                ChangePriorityButton.Visibility = Visibility.Visible;
            else
                ChangePriorityButton.Visibility= Visibility.Hidden;
        }

        private void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
        {

            int maxPriority = 0;
            foreach (Agent agent in AgentListView.SelectedItems)
            {
                if (agent.Priority > maxPriority)
                    maxPriority = agent.Priority;
            }

            myWindow Window = new myWindow(maxPriority);
            Window.ShowDialog();

            if (string.IsNullOrEmpty(Window.TBPriority.Text))
            {
                MessageBox.Show("Изменение не произошло");
            }
            else
            {
                // Проверяем, является ли введенное значение числом
                if (int.TryParse(Window.TBPriority.Text, out int newPriority))
                {
                    // Проверяем, является ли новое значение приоритета отрицательным
                    if (newPriority < 0)
                    {
                        MessageBox.Show("Приоритет не может быть отрицательным.");
                        return; // Выход из метода, если приоритет отрицательный
                    }

                    foreach (Agent agent in AgentListView.SelectedItems)
                    {
                        agent.Priority = newPriority;
                    }

                    try
                    {
                        ИзосимоваГлазкиSaveEntities.GetContext().SaveChanges();
                        MessageBox.Show("Информация сохранена");
                        UpdateAgents();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное числовое значение для приоритета.");
                }
            }

        }
    }
}
