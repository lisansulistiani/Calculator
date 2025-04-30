using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Calculator.Helpers;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _formula = string.Empty;
        public string Formula
        {
            get { return _formula; }
            set {
                if (_formula != value)
                {
                    _formula = value;
                    OnPropertyChanged(nameof(Formula));
                }
            }
        }
        private string _display = "0";
        public string Display
        {
            get { return _display; }
            set {
                if (_display != value)
                {
                    _display = value;
                    OnPropertyChanged(nameof(Display));
                }
            }
        }
        public ICommand ButtonCommand { get; }
        public ICommand WindowCommandMinimize { get; }
        public ICommand WindowCommandMaximize { get; }
        public ICommand WindowCommandClose { get; }
        public ICommand DragMoveCommand { get; }
        public CalculatorViewModel()
        {
            ButtonCommand = new RelayCommand(param => ProcessInput(param.ToString()!));
            WindowCommandMinimize = new RelayCommand(param =>
            {
                if (param is Window window)
                    window.WindowState = WindowState.Minimized;
            });
            WindowCommandMaximize = new RelayCommand(param =>
            {
                if (param is Window window)
                {
                    window.WindowState = window.WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
                }
            });
            WindowCommandClose = new RelayCommand(param =>
            {
                if (param is Window window)
                    window.Close();
            });

            DragMoveCommand = new RelayCommand(param =>
            {
                if (param is Window window)
                {
                    try { window.DragMove(); } catch { /* drag cancelled */ }
                }
            });
        }
        
        private List<string> _symbol = new List<string>{ ".", "+", "÷", "%", "C", "Remove", "×", "-", "(+/-)", "="};
        private bool start = true;
        
        private string _tempDisplay = "0";
        
        private bool _justProcessPercent = false;

        private string _lastOperator = string.Empty;
        
        private void ProcessInput(string input)
        {
            if (!_symbol.Contains(input))
            {
                // Handle number input
                if (start)
                {
                    Formula = string.Empty;
                    Display = "0";
                    _tempDisplay = input;
                    start = false;
                    _justProcessPercent = false;
                }
                else
                {
                    //if(Convert.ToDouble(_tempDisplay)!=0)
                    if (Convert.ToDouble(_tempDisplay) + Convert.ToDouble(input) != 0)
                    {
                        if (_tempDisplay == "0")
                            _tempDisplay = input;
                        else
                            _tempDisplay += input;
                    }
                }
                Display = _tempDisplay;
                _lastOperator = string.Empty ;
                return;
            }
            else
            {
                switch (input)
                {
                    case "C":
                        start = true;
                        _justProcessPercent = false;
                        Formula = string.Empty;
                        Display = "0";
                        _tempDisplay = "0";
                        _lastOperator = string.Empty;
                        return;
                    case "Remove":
                        if (start)
                        {
                            Formula = string.Empty;
                        }
                        else if (start && Formula == string.Empty && _tempDisplay != string.Empty)
                        {
                            Display = "0";
                            _tempDisplay = string.Empty;
                            _lastOperator = string.Empty;
                        }
                        else
                        {
                            if (_tempDisplay.Length > 1 && Display.Length>1)
                                _tempDisplay = _tempDisplay.Substring(0, _tempDisplay.Length - 1);
                            else
                            {
                                _tempDisplay = "0";
                                _lastOperator= string.Empty;
                            }
                            
                            Display = _tempDisplay;
                        }
                        return;
                    case "%":
                        string temp_tempDisplay = _tempDisplay;
                        string percent = Convert.ToString(Convert.ToDouble(_tempDisplay) / 100);
                        _tempDisplay = percent;
                        if (!start)
                        {
                            if (Formula != string.Empty)
                            {
                                if (_lastOperator == string.Empty)
                                {
                                    if (_justProcessPercent)
                                    //if (Formula.Substring(Formula.Length - Display.Length, Formula.Length) == temp_tempDisplay)
                                    {
                                        Formula = Formula.Substring(0, Formula.Length - Display.Length) + _tempDisplay;
                                    }
                                    else
                                    {
                                        Formula += _tempDisplay;
                                    }
                                }
                                else
                                {
                                    percent = Convert.ToString(Convert.ToDouble(Display) / 100);
                                    _tempDisplay = percent;
                                    Formula += _tempDisplay;
                                    _lastOperator = string.Empty;
                                }
                            }
                            else
                            {
                                Formula = _tempDisplay;
                            }
                        }
                        else
                        {
                            Formula = _tempDisplay;
                            start = false;
                        }

                        Display = _tempDisplay;
                        _justProcessPercent = true;
                        return;
                    case "+":
                    case "-":
                    case "×":
                    case "÷":
                        if (!start)
                        {
                            if (_justProcessPercent)
                            {
                                Formula += input;
                                _justProcessPercent = false;
                                _lastOperator = input;
                            }
                            else
                            {
                                if (_lastOperator == string.Empty)
                                {
                                    Formula += _tempDisplay + input;
                                    _tempDisplay = "0";
                                    _lastOperator = input;
                                }
                                else
                                {
                                    if (_lastOperator != input)
                                    {
                                        Formula = Formula.Substring(0, Formula.Length - 1)+input;
                                        _lastOperator = input;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Formula = _tempDisplay + input;
                            start = false;
                            _lastOperator = input;
                        }
                        _tempDisplay = "0";
                        return;
                    case "=":
                        if (!start)
                        {
                            Formula += _tempDisplay+"=";
                        }
                        string result = executeFormula(Formula);
                        _tempDisplay = result;
                        Display = _tempDisplay;
                        start = true;
                        return;
                    case ".":
                        if (!start)
                        {
                            if (Display == "0" || !_tempDisplay.Contains('.'))
                                _tempDisplay += '.';
                        }
                        else
                        {
                            _tempDisplay = "0.";
                            start = false;
                        }
                        Display = _tempDisplay;
                        return;
                    case "(+/-)":
                        if (!start)
                        {
                            if (_tempDisplay != "0")
                            {
                                if (_tempDisplay.StartsWith("-"))
                                    _tempDisplay = _tempDisplay.Substring(1);
                                else
                                    _tempDisplay = "-"+_tempDisplay;
                            }
                        }
                        else
                        {
                            if (_tempDisplay != "0")
                            {
                                if (_tempDisplay.StartsWith("-"))
                                    _tempDisplay = _tempDisplay.Substring(1);
                                else
                                    _tempDisplay = "-" + _tempDisplay;
                            }
                            Formula = string.Empty;
                            start = false;
                        }
                        Display = _tempDisplay;
                        return;
                }
            }
        }

        private string executeFormula(string Formula)
        {
            if (Formula != null)
            {
                Formula = Formula.Replace("÷", "/");
                Formula = Formula.Replace("×", "*");
                Formula = Formula.Replace("=", "");
                
                var result = new DataTable().Compute(Formula, null);
                return Convert.ToString(result);
            }
            else
            {
                return "0";
            }
        } 
        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}   