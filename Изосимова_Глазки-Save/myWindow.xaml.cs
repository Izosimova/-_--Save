﻿using System;
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
using System.Windows.Shapes;

namespace Изосимова_Глазки_Save
{
    /// <summary>
    /// Логика взаимодействия для myWindow.xaml
    /// </summary>
    public partial class myWindow : Window
    {
        public myWindow(int max)
        {
            InitializeComponent();
            TBPriority.Text = max.ToString();
        }

            private void SetButton_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }
        } }
