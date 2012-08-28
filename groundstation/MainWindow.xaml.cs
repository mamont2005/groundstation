using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;


namespace groundstation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort port;
        int currentX = 0;
        long lastUpdate = 0;
        bool dirty = false;


        public MainWindow()
        {
            InitializeComponent();
            
            port = new SerialPort("COM8", 115200, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Port");
                Application.Current.Shutdown();
                return;
            }

            Thread readThread = new Thread(Read);
            readThread.IsBackground = true;
            readThread.Start();
        }

        public void Read()
        {
            for (; ; )
            {
                string message = port.ReadLine();
                string[] split = message.Split(null);
                if (split[0] == "g")
                {
                    var point = new Graph.Point();
                    point.x = currentX;
                    currentX++;

                    for (int idx = 1; idx < split.Count(); idx++)
                    {
                        double yval;
                        if (double.TryParse(split[idx], out yval))
                            point.y.Add(yval);
                    }
                    this.Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        graph1.points.Add(point);
                        if (System.DateTime.Now.Ticks > lastUpdate + 1000000)    // 10 msec
                        {
                            graph1.InvalidateVisual();
                            lastUpdate = System.DateTime.Now.Ticks;
                            dirty = false;
                        }
                        else
                            dirty = true;
                        
                    });
                }
                else
                {
                    this.Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
//                        textBox1.Text += message/* + System.Environment.NewLine*/;
//                        textBox1.ScrollToEnd();
                    });
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                port.WriteLine(comboBox1.Text + System.Environment.NewLine);
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(comboBox1);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            graph1.points.Clear();
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void checkBoxZero_Click(object sender, RoutedEventArgs e)
        {
            graph1.zero = checkBoxZero.IsChecked ?? true;
        }

        private void checkCustom_Click(object sender, RoutedEventArgs e)
        {
            graph1.custom = checkBoxCustom.IsChecked ?? false;
            double.TryParse(textBoxFrom.Text, out graph1.from);
            double.TryParse(textBoxTo.Text, out graph1.to);
        }
    }
}
