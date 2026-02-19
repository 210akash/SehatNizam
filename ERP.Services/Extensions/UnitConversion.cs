using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Services.Extensions
{
    public class TemperatureConversion
    {
        //Fahrenheit to Celsius
        public double FtoC(double fahrenheit)
        {
            double celsius = 0.0;
            celsius = (fahrenheit - 32) * 5 / 9;
            return celsius;
        }
        //Celsius to Fahrenheit
        public double CtoF(double celsius)
        {
            double fahrenheit = 0.0;
            fahrenheit = (celsius * 9 / 5) + 32;
            return fahrenheit;
        }
        //Kelvin to Fahrenheit
        public double KtoF(double kelvin)
        {
            double celsius = kelvin - 273.15;
            double fahrenheit = CtoF(celsius);
            return fahrenheit;
        }
        //Fahrenheit to Kelvin
        public double FtoK(double Fahrenheit)
        {
            double celsius = FtoC(Fahrenheit);
            double kelvin = celsius + 273.15;
            return kelvin;
        }
        //Celsius to Kelvin
        public double CtoK(double celsius)
        {
            double kelvin = celsius + 273.15;
            return kelvin;
        }
        //Kelvin to Celsius
        public double KtoC(double kelvin)
        {
            double celsius = kelvin - 273.15;
            return celsius;
        }

        //-------------------------------SensorType: Relative Humidity------------------------
        // Only unit for relative humidity is '%'.  So incoming and user side unit will be same



    }

    public static class PressureConversion
    {
        //-------------------------------SensorType: Pressure Conversions-----------------------
        //Bar: bar
        //KiloPascal: kPa
        //PSI: psi
        //ATM: atm
        //Torr: Torr

        //Bar to KPascal
        public static double BarToKPascal(double bar)
        {
            double Kpascal = bar * 100;
            return Kpascal;
        }
        //KPascal to Bar
        public static double KPascalToBar(double Kpascal)
        {
            double bar = Kpascal / 100;
            return bar;
        }
        //Bar to PSI
        public static double BarToPSI(double bar)
        {
            double PSI = bar * 14.5038;
            return PSI;
        }
        //PSI to Bar
        public static double PSIToBar(double PSI)
        {
            double bar = PSI / 14.5038;
            return bar;
        }
        //Bar to ATM
        public static double BarToATM(double bar)
        {
            double ATM = bar / 1.013;
            return ATM;
        }
        //ATM to Bar
        public static double ATMToBar(double ATM)
        {
            double bar = ATM * 1.013;
            return bar;
        }
        //Bar to Torr
        public static double BarToTorr(double bar)
        {
            double Torr = bar * 750.062;
            return Torr;
        }
        //Torr to Bar
        public static double TorrToBar(double Torr)
        {
            double bar = Torr / 750.062;
            return bar;
        }
        //KPascal to PSI
        public static double KPascalToPSI(double Kpascal)
        {
            double PSI = Kpascal / 6.89476;
            return PSI;
        }
        //PSI to KPascal
        public static double PSIToKPascal(double PSI)
        {
            double Kpascal = PSI * 6.89476;
            return Kpascal;
        }
        //KPascal to ATM
        public static double KPascalToATM(double Kpascal)
        {
            double ATM = Kpascal / 101.325;
            return ATM;
        }
        //ATM to KPascal
        public static double ATMToKPascal(double ATM)
        {
            double Kpascal = ATM * 101.325;
            return Kpascal;
        }
        //KPascal to Torr
        public static double KPascalToTorr(double Kpascal)
        {
            double Torr = Kpascal * 7.501;
            return Torr;
        }
        //Torr to KPascal
        public static double TorrToKPascal(double Torr)
        {
            double Kpascal = Torr / 7.501;
            return Kpascal;
        }
        //PSI to ATM
        public static double PSIToATM(double PSI)
        {
            double ATM = PSI / 14.696;
            return ATM;
        }
        //ATM to PSI
        public static double ATMToPSI(double ATM)
        {
            double PSI = ATM * 14.696;
            return PSI;
        }
        //PSI to Torr
        public static double PSIToTorr(double PSI)
        {
            double Torr = PSI * 51.7149;
            return Torr;
        }
        //Torr to PSI
        public static double TorrToPSI(double Torr)
        {
            double PSI = Torr / 51.7149;
            return PSI;
        }
        //ATM to Torr
        public static double ATMToTorr(double ATM)
        {
            double Torr = ATM * 760;
            return Torr;
        }
        //Torr to ATM
        public static double TorrToATM(double Torr)
        {
            double ATM = Torr / 760;
            return ATM;
        }
    }
}
