using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace BallisticApp
{
    public class AdvancedBallisticsCalculatorImperial
    {
        public static (double y, double x) CalculateImpact(BallisticSettings settings)
        {

            double temp = settings.Temperature + 273.15;
            double pressure = settings.Pressure * 100;
            double humidity = settings.Humidity / 100.0;
            double area = CalculateBulletArea(settings.BulletDiameter);
            (double rho, double airSpeed) = CalculateRHOandAirspeed(temp, pressure, humidity);
            double rhoLBFT3 = rho * 0.062428; // Convert kg/m^3 to lb/ft^3
            double airSpeedFTS = airSpeed * 3.28084; // Convert m/s to ft/s
            string tableChoice = "Table" + settings.DragModel; // e.g., "TableG1"
            List<DragTablePoint> table = DragTables.Tables[tableChoice];
            double bulletMass = settings.BulletWeight;
            double zeroingAngle = FindZeroingAngle(settings, airSpeedFTS, table, rhoLBFT3, area, bulletMass);
            (double impactY, double impactX) = SimulateTrajectory(
                settings,
                zeroingAngle,
                rhoLBFT3,
                airSpeedFTS,
                area,
                bulletMass,
                table
            );
            return (impactY*30.48, impactX*30.48);
        }

        public static double FindZeroingAngle(BallisticSettings settings, double airSpeed, List<DragTablePoint> table,
        double rho, double area, double bulletMass)
        {
            double dt = 0.00001;
            double angleLow = 0; // 0 degrees in radians
            double angleHigh = 10 * (Math.PI / 180.0); // 10 degrees in radians
            double angleTolerance = 0.000001;
            double BC = settings.BallisticCoefficient;
            double rho_o = 0.076474; // lb/ft^3 at sea level
            while ((angleHigh - angleLow) > angleTolerance)
            {
                double midAngle = (angleLow + angleHigh) / 2.0;

                double x = 0;
                double y = -settings.SightHeight/ 2.54 / 12; //feet
                double vx = settings.BulletVelocity * 3.28084 * Math.Cos(midAngle); //fps
                double vy = settings.BulletVelocity * 3.28084 * Math.Sin(midAngle); //fps
                double v = settings.BulletVelocity * 3.28084; //feet
                double mach = v / airSpeed;
                double cd = GetDragCoefficient(table, mach);

                while (x < settings.ZeroDistance * 3.28084)
                {
                    
                    double dragForce = v * v * (rho/rho_o) * (2.08551e-04)*cd/BC;
                    double ax = -dragForce * (vx / v);
                    double ay = -32.17 - dragForce * (vy / v);
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


        public static (double y, double z) SimulateTrajectory(BallisticSettings settings,
        double angle, double rho,
        double airSpeed, double area, double bulletMass,
        List<DragTablePoint> table)
        {
            double dt = 0.00001;
            double x = 0;
            double y = -settings.SightHeight / 2.54 / 12;
            double z = 0;
            double vx = settings.BulletVelocity * 3.28084 * Math.Cos(angle);
            double vy = settings.BulletVelocity * 3.28084 * Math.Sin(angle);
            double vz = 0;
            double v = settings.BulletVelocity * 3.28084;
            double mach = v / airSpeed;
            double cd = 0;
            double windSpeed = settings.WindSpeed * 3.28084;
            double windDirection = settings.WindDirection * (Math.PI / 180.0); // Convert to radians

            double v_wind_x = -windSpeed * Math.Cos(windDirection);
            double v_wind_z = windSpeed * Math.Sin(windDirection);

            double BC = settings.BallisticCoefficient;
            double rho_o = 0.076474; // lb/ft^3 at sea level

            while (x < settings.Distance * 3.28084)
            {
                double v_rel_x = vx - v_wind_x;  // bullet relative to air
                double v_rel_y = vy;             // vertical wind usually zero
                double v_rel_z = vz - v_wind_z;  // lateral component

                double v_rel = Math.Sqrt(v_rel_x * v_rel_x + v_rel_y * v_rel_y + v_rel_z * v_rel_z);
                mach = v_rel / airSpeed;
                cd = GetDragCoefficient(table, mach);

                double dragForce = v_rel * v_rel * (rho / rho_o) * (2.08551e-04) * cd / BC;
                double ax = -dragForce * (v_rel_x / v_rel);
                double ay = -32.17 - dragForce * (v_rel_y / v_rel);
                double az = -dragForce * (v_rel_z / v_rel);
                vx += ax * dt;
                vy += ay * dt;
                vz += az * dt;
                x += vx * dt;
                y += vy * dt;
                z += vz * dt;
            }
            return (y, z);
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