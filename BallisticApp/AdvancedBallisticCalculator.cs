using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace BallisticApp
{
    public class AdvancedBallisticsCalculator
    {
        public static double CalculateImpact(BallisticSettings settings)
        {
            
            double temp = settings.Temperature + 273.15; // Convert to Kelvin
            double pressure = settings.Pressure * 100; // Convert hPa to Pa
            double humidity = settings.Humidity / 100.0;
            double area = CalculateBulletArea(settings.BulletDiameter);
            (double rho, double airSpeed) = CalculateRHOandAirspeed(temp, pressure, humidity);
            string tableChoice = "Table" + settings.DragModel; // e.g., "TableG1"
            List<DragTablePoint> table = DragTables.Tables[tableChoice];
            double bulletMass = ConvertGrainsToKg(settings.BulletWeight);
            double zeroingAngle = FindZeroingAngle(settings, airSpeed, table, rho, area, bulletMass);
            double impactY = SimulateTrajectory(
                zeroingAngle,
                settings.Distance,
                settings.BulletVelocity,
                rho,
                airSpeed,
                area,
                bulletMass,
                table
            );
            return impactY;
        }

        public static double FindZeroingAngle(BallisticSettings settings, double airSpeed, List<DragTablePoint> table,
        double rho, double area, double bulletMass)
        {
            double dt = 0.0001;
            double angleLow = 0; // 0 degrees in radians
            double angleHigh = 10 * (Math.PI / 180.0); // 10 degrees in radians
            double angleTolerance = 0.00001;
            while ((angleHigh - angleLow) > angleTolerance)
            {
                double midAngle = (angleLow + angleHigh) / 2.0;

                double x = 0;
                double y = -settings.SightHeight/100;
                double vx = settings.BulletVelocity * Math.Cos(midAngle);
                double vy = settings.BulletVelocity * Math.Sin(midAngle);
                double v = settings.BulletVelocity;
                double mach = v / airSpeed;
                double cd = GetDragCoefficient(table, mach);

                while (x < settings.ZeroDistance)
                {
                    double dragForce = 0.5 * rho * v * v * area * cd / bulletMass;
                    double ax = -dragForce * (vx / v);
                    double ay = -9.81 - dragForce * (vy / v);
                    vx = vx + ax * dt;
                    vy = vy + ay * dt;
                    x = x + vx * dt;
                    y = y + vy * dt;
                    v = Math.Sqrt(vx * vx + vy * vy);
                    mach = v / airSpeed;
                    cd = GetDragCoefficient(table, mach);
                }
                if (y > 0)
                    angleHigh = midAngle;
                else
                    angleLow = midAngle;
            }
            double zeroingAngle = (angleLow + angleHigh) / 2.0;
            return zeroingAngle;
        }


        public static double  SimulateTrajectory(
        double angle, double distance, double v0, double rho,
        double airSpeed, double area, double bulletMass,
        List<DragTablePoint> table)
            {
                double dt = 0.0001;
                double x = 0;
                double y = 0;
                double vx = v0 * Math.Cos(angle);
                double vy = v0 * Math.Sin(angle);
                double v = v0;
                double mach = v / airSpeed;
                double cd = GetDragCoefficient(table, mach);

                while (x < distance)
                {
                    double dragForce = 0.5 * rho * v * v * area * cd / bulletMass;
                    double ax = -dragForce * (vx / v);
                    double ay = -9.81 - dragForce * (vy / v);
                    vx += ax * dt;
                    vy += ay * dt;
                    x += vx * dt;
                    y += vy * dt;
                    v = Math.Sqrt(vx * vx + vy * vy);
                    mach = v / airSpeed;
                    cd = GetDragCoefficient(table, mach);
                }
                return (y);
        }

        public static (double rho, double airSpeed) CalculateRHOandAirspeed(
            double T,   // Temperature in K
            double P,   // Absolute pressure in Pa
            double RH   // Relative humidity 0–1
        )
        {
            // Constants
            const double Rd = 287.05;  // J/(kg·K) dry air
            const double Rv = 461.5;   // J/(kg·K) water vapor
            const double Cpd = 1005.0; // J/(kg·K) specific heat dry air
            const double Cpv = 1850.0; // J/(kg·K) specific heat water vapor

            // Saturation vapor pressure (Pa) using Magnus formula
            double Tc = T - 273.15;
            double Psat = 6.112 * Math.Exp((17.67 * Tc) / (Tc + 243.5)) * 100.0;

            // Partial pressures
            double Pv = RH * Psat;
            double Pd = P - Pv;

            // Density of mixture
            double rho = (Pd / (Rd * T)) + (Pv / (Rv * T));

            // Mass fractions
            double md = Pd / (Rd * T);
            double mv = Pv / (Rv * T);
            double mTot = md + mv;
            double wd = md / mTot;
            double wv = mv / mTot;

            // Mixture properties
            double Rmix = wd * Rd + wv * Rv;
            double Cp = wd * Cpd + wv * Cpv;
            double Cv = Cp - Rmix;
            double gamma = Cp / Cv;

            // Speed of sound
            double airSpeed = Math.Sqrt(gamma * Rmix * T);

            return (rho, airSpeed);
        }


        public static double CalculateBulletArea(double diameterMm)
        {
            double diameterM = diameterMm / 1000.0; // Convert mm to m
            return Math.PI * Math.Pow(diameterM / 2.0, 2);
        }

        public static double ConvertGrainsToKg(double grains)
        {
            return grains * 0.00006479891;
        }

        public static double GetDragCoefficient(List<DragTablePoint> table, double mach)
        {
            if (mach <= table.First().Mach) return table.First().CD;
            if (mach >= table.Last().Mach) return table.Last().CD;

            int low = 0;
            int high = table.Count - 1;

            while (low <= high)
            {
                int mid = (low + high) / 2;

                if (mach < table[mid].Mach)
                {
                    high = mid - 1;
                }
                else if (mach > table[mid].Mach)
                {
                    low = mid + 1;
                }
                else
                {
                    return table[mid].CD; // exact match
                }
            }

            // Interpolation between high and low
            DragTablePoint lowerPoint = table[high];
            DragTablePoint upperPoint = table[low];
            double t = (mach - lowerPoint.Mach) / (upperPoint.Mach - lowerPoint.Mach);
            return lowerPoint.CD + t * (upperPoint.CD - lowerPoint.CD);
        }

    }
}