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
using System.Windows.Threading;
using Calculator.ViewModels;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Width = 480;
            this.Height = 640;
            InitializeButtonMap();
        }

        private Dictionary<string, Button> _buttonMap = new Dictionary<string, Button>();
        private void InitializeButtonMap()
        {
            _buttonMap = new Dictionary<string, Button>
            {
                { "0", zeroBtn },
                { "1", oneBtn },
                { "2", twoBtn },
                { "3", threeBtn },
                { "4", fourBtn },
                { "5", fiveBtn },
                { "6", sixBtn },
                { "7", sevenBtn },
                { "8", eightBtn },
                { "9", nineBtn },
                { "C", clearBtn },
                { "Remove", eraseBtn },
                { "%", percentBtn },
                { "÷", divideBtn },
                { "×", multiplyBtn },
                { "-", subtractionBtn },
                { "+", additionBtn },
                { "=", equalBtn },
                { ".", commaBtn },
                { "(+/-)", plusMinusBtn }
            };
        }
        private void ButtonPressed(Button button) 
        {
            var originalBackground = button.Background;
            var originalForeground = button.Foreground;
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FE5D9F")); 
            button.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4BBD3"));

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            timer.Tick += (s, e) =>
            {
                button.ClearValue(Button.BackgroundProperty); // Revert to styled background
                button.ClearValue(Button.ForegroundProperty);
                timer.Stop();
            };
            timer.Start();
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            Button button;
            var vm = DataContext as CalculatorViewModel;
            if (vm == null) return;

            
            // Operators
            if (e.Key == Key.D5 && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                vm.ButtonCommand.Execute("%");
                if (_buttonMap.TryGetValue("%", out button))
                {
                    ButtonPressed(button);
                }
                e.Handled = true;
                return;
            }
            else if(e.Key == Key.D8 && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                vm.ButtonCommand.Execute("×");
                if (_buttonMap.TryGetValue("×", out button))
                {
                    ButtonPressed(button);
                }
                e.Handled= true;
                return;
            }
            else if (e.Key == Key.OemPlus && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                vm.ButtonCommand.Execute("=");
                if (_buttonMap.TryGetValue("=", out button))
                {
                    ButtonPressed(button);
                }
                e.Handled = true;
                return;
            }

            // Check if a digit was pressed
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string key = (e.Key - Key.D0).ToString();
                vm.ButtonCommand.Execute(key);
                if (_buttonMap.TryGetValue(key, out button))
                {
                    ButtonPressed(button);
                }
                e.Handled = true;
                return;
            }

            // Numpad keys
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string key = (e.Key - Key.NumPad0).ToString();
                vm.ButtonCommand.Execute(key);
                if (_buttonMap.TryGetValue(key, out button))
                {
                    ButtonPressed(button);
                }
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Delete:
                case Key.Escape:
                    vm.ButtonCommand.Execute("C");
                    if (_buttonMap.TryGetValue("C", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.Back:
                    vm.ButtonCommand.Execute("Remove");
                    if (_buttonMap.TryGetValue("Remove", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.Divide:
                case Key.OemQuestion:
                    vm.ButtonCommand.Execute("÷");
                    if (_buttonMap.TryGetValue("÷", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.Multiply:
                    vm.ButtonCommand.Execute("×");
                    if (_buttonMap.TryGetValue("×", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;
                
                case Key.Subtract:
                case Key.OemMinus:
                    vm.ButtonCommand.Execute("-");
                    if (_buttonMap.TryGetValue("-", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.Enter:
                    vm.ButtonCommand.Execute("=");
                    if (_buttonMap.TryGetValue("=", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.Add:
                case Key.OemPlus:
                    vm.ButtonCommand.Execute("+");
                    if (_buttonMap.TryGetValue("+", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.F9:
                    vm.ButtonCommand.Execute("(+/-)");
                    if (_buttonMap.TryGetValue("(+/-)", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;

                case Key.OemPeriod:
                case Key.Decimal:
                    vm.ButtonCommand.Execute(".");
                    if (_buttonMap.TryGetValue(".", out button))
                    {
                        ButtonPressed(button);
                    }
                    break;
            }

            
        }
    }
}