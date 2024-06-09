using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace LR4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void CalculateWithDispatcherButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateWithDispatcherButton.IsEnabled = false;
            CalculateWithBackgroundWorkerButton.IsEnabled = false;

            double lowerBound = double.Parse(LowerBoundTextBox.Text);
            double upperBound = double.Parse(UpperBoundTextBox.Text);
            int n = int.Parse(NTextBox.Text);

            Thread thread = new Thread(() =>
            {
                double result = CalculateIntegral(lowerBound, upperBound, n);
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Результат: {result}");
                    ProgressBar.Value = 100;
                    CalculateWithDispatcherButton.IsEnabled = true;
                    CalculateWithBackgroundWorkerButton.IsEnabled = true;
                });
            });
            thread.Start();
        }

        private void CalculateWithBackgroundWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateWithDispatcherButton.IsEnabled = false;
            CalculateWithBackgroundWorkerButton.IsEnabled = false;

            double lowerBound = double.Parse(LowerBoundTextBox.Text);
            double upperBound = double.Parse(UpperBoundTextBox.Text);
            int n = int.Parse(NTextBox.Text);

            backgroundWorker.RunWorkerAsync(new double[] { lowerBound, upperBound, n });
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            double[] parameters = e.Argument as double[];
            double lowerBound = parameters[0];
            double upperBound = parameters[1];
            int n = (int)parameters[2];

            double result = CalculateIntegral(lowerBound, upperBound, n);
            e.Result = result;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show($"Результат: {e.Result}");
            ProgressBar.Value = 100;
            CalculateWithDispatcherButton.IsEnabled = true;
            CalculateWithBackgroundWorkerButton.IsEnabled = true;
        }

        private double CalculateIntegral(double lowerBound, double upperBound, int n)
        {
            double width = (upperBound - lowerBound) / n;
            double sum = 0;

            for (int i = 0; i < n; i++)
            {
                double x = lowerBound + i * width;
                sum += Math.Sin(x) * width;

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = (double)i / n * 100;
                });

            }

            return sum;
        }
    }
}
