using System;
using System.ComponentModel;
using System.Windows.Input;
using Application_Bac3_WPF.Models;

namespace Application_Bac3_WPF.Vue_Model
{
    public class VM_Chronometre : INotifyPropertyChanged
    {
        private readonly M_Chronometre _model;
        private double _secondHandAngle;
        private double _minuteHandAngle;
        private bool _isRunning;

        public double SecondHandAngle
        {
            get => _secondHandAngle;
            set { _secondHandAngle = value; OnPropertyChanged(nameof(SecondHandAngle)); }
        }

        public double MinuteHandAngle
        {
            get => _minuteHandAngle;
            set { _minuteHandAngle = value; OnPropertyChanged(nameof(MinuteHandAngle)); }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ResetCommand { get; }

        public VM_Chronometre()
        {
            _model = new M_Chronometre();
            _model.TempsChange += OnTempsChange;

            StartCommand = new RelayCommand(_ => Start(), _ => !_isRunning);
            StopCommand = new RelayCommand(_ => Stop(), _ => _isRunning);
            ResetCommand = new RelayCommand(_ => Reset(), _ => true);
        }

        private void OnTempsChange(TimeSpan temps)
        {
            
            SecondHandAngle = (temps.TotalSeconds % 60) * 6; 
            MinuteHandAngle = (temps.TotalMinutes % 60) * 6; 
        }

        private void Start()
        {
            _isRunning = true;
            UpdateCommandStates();
            _model.Start();
        }

        private void Stop()
        {
            _isRunning = false;
            UpdateCommandStates();
            _model.Stop();
        }

        private void Reset()
        {
            _isRunning = false;
            UpdateCommandStates();
            _model.Reset();
        }

        private void UpdateCommandStates()
        {
            (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ResetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
