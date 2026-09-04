using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AeroSpaceEFB.ViewModels;

public class CalculationsViewModel : INotifyPropertyChanged
{
    // --- WEIGHT & BALANCE (TAKEOFF CG) ---
    private double _emptyWeight = 1650; // lbs or kg
    public double EmptyWeight
    {
        get => _emptyWeight;
        set { _emptyWeight = value; OnPropertyChanged(); CalculateCg(); }
    }

    private double _emptyArm = 38.5; // inches or cm
    public double EmptyArm
    {
        get => _emptyArm;
        set { _emptyArm = value; OnPropertyChanged(); }
    }

    private double _pilotPaxWeight = 340;
    public double PilotPaxWeight
    {
        get => _pilotPaxWeight;
        set { _pilotPaxWeight = value; OnPropertyChanged(); }
    }

    private double _pilotPaxArm = 37.0;
    public double PilotPaxArm
    {
        get => _pilotPaxArm;
        set { _pilotPaxArm = value; OnPropertyChanged(); }
    }

    private double _fuelWeight = 240; // e.g. 40 gal * 6 lbs
    public double FuelWeight
    {
        get => _fuelWeight;
        set { _fuelWeight = value; OnPropertyChanged(); }
    }

    private double _fuelArm = 48.0;
    public double FuelArm
    {
        get => _fuelArm;
        set { _fuelArm = value; OnPropertyChanged(); }
    }

    private double _baggageWeight = 50;
    public double BaggageWeight
    {
        get => _baggageWeight;
        set { _baggageWeight = value; OnPropertyChanged(); }
    }

    private double _baggageArm = 95.0;
    public double BaggageArm
    {
        get => _baggageArm;
        set { _baggageArm = value; OnPropertyChanged(); }
    }

    private string _cgResultText = "Total Weight: -- | Takeoff CG: --";
    public string CgResultText
    {
        get => _cgResultText;
        set { _cgResultText = value; OnPropertyChanged(); }
    }

    // --- WIND CALCULATOR ---
    public string RunwayHeading { get; set; } = string.Empty;
    public string WindDirection { get; set; } = string.Empty;
    public string WindSpeed { get; set; } = string.Empty;

    private string _windResult = "Headwind: -- kt | Crosswind: -- kt";
    public string WindResult
    {
        get => _windResult;
        set { _windResult = value; OnPropertyChanged(); }
    }

    // --- TOD CALCULATOR ---
    public string CurrentAlt { get; set; } = string.Empty;
    public string TargetAlt { get; set; } = string.Empty;
    public string GroundSpeed { get; set; } = string.Empty;

    private string _todResult = "TOD: -- NM | Des. Rate: -- fpm";
    public string TodResult
    {
        get => _todResult;
        set { _todResult = value; OnPropertyChanged(); }
    }

    // --- FUEL CALCULATOR ---
    public string FuelGallons { get; set; } = string.Empty;

    private string _fuelResult = "0 L | 0 kg (AVGAS) / 0 kg (JET-A)";
    public string FuelResult
    {
        get => _fuelResult;
        set { _fuelResult = value; OnPropertyChanged(); }
    }

    // COMMANDS
    public ICommand CalculateWindCommand { get; }
    public ICommand CalculateTodCommand { get; }
    public ICommand CalculateFuelCommand { get; }

    public CalculationsViewModel()
    {
        CalculateWindCommand = new Command(CalculateWind);
        CalculateTodCommand = new Command(CalculateTod);
        CalculateFuelCommand = new Command(CalculateFuel);

        CalculateCg(); // Startberechnung für CG
    }

    private void CalculateCg()
    {
        double totalWeight = EmptyWeight + PilotPaxWeight + FuelWeight + BaggageWeight;

        if (totalWeight <= 0)
        {
            CgResultText = "Ungültige Gewichtsangaben";
            return;
        }

        double totalMoment = (EmptyWeight * EmptyArm) +
                             (PilotPaxWeight * PilotPaxArm) +
                             (FuelWeight * FuelArm) +
                             (BaggageWeight * BaggageArm);

        double takeoffCg = totalMoment / totalWeight;

        CgResultText = $"Takeoff Weight: {Math.Round(totalWeight, 1)}  |  Takeoff CG: {Math.Round(takeoffCg, 2)}";
    }

    private void CalculateWind()
    {
        if (double.TryParse(RunwayHeading, out double rwy) &&
            double.TryParse(WindDirection, out double windDir) &&
            double.TryParse(WindSpeed, out double windSpd))
        {
            double angleRad = (windDir - rwy) * (Math.PI / 180.0);
            double headwind = Math.Cos(angleRad) * windSpd;
            double crosswind = Math.Sin(angleRad) * windSpd;

            string hwText = headwind >= 0
                ? $"Headwind: {Math.Round(headwind, 1)} kt"
                : $"Tailwind: {Math.Round(Math.Abs(headwind), 1)} kt";

            string cwText = $"Crosswind: {Math.Round(Math.Abs(crosswind), 1)} kt " +
                           (crosswind > 0 ? "(Right)" : crosswind < 0 ? "(Left)" : "");

            WindResult = $"{hwText} | {cwText}";
        }
        else
        {
            WindResult = "Ungültige Wind-Eingabe";
        }
    }

    private void CalculateTod()
    {
        if (double.TryParse(CurrentAlt, out double cAlt) &&
            double.TryParse(TargetAlt, out double tAlt) &&
            double.TryParse(GroundSpeed, out double gs))
        {
            double altDiff = Math.Max(0, cAlt - tAlt);
            double todDistanceNm = (altDiff / 1000.0) * 3.0;
            double vsFpm = gs * 5.0;

            TodResult = $"TOD: {Math.Round(todDistanceNm, 1)} NM | Vertical Speed: ~{Math.Round(vsFpm)} fpm";
        }
        else
        {
            TodResult = "Ungültige Höhen-/GS-Eingabe";
        }
    }

    private void CalculateFuel()
    {
        if (double.TryParse(FuelGallons, out double gal))
        {
            double liters = gal * 3.78541;
            double kgAvgas = liters * 0.72;
            double kgJetA = liters * 0.80;

            FuelResult = $"{Math.Round(liters, 1)} L  |  AVGAS: {Math.Round(kgAvgas, 1)} kg  |  JET-A: {Math.Round(kgJetA, 1)} kg";
        }
        else
        {
            FuelResult = "Ungültige Gallonen-Eingabe";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}