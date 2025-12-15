using System;
using System.Collections.Generic;

namespace BallisticApp
{
    public static class BallisticsSolver
    {
        // Constants
        private const double g = 9.80665;           // gravitational acceleration (m/s²)
        private const double rho = 1.225;           // air density at sea level (kg/m³)
        private const double speedOfSound = 343.0;  // m/s at sea level
        private const double TimeStep = 0.001;        // time step for integration (seconds)

        // G1 Drag Coefficient Table (Mach vs Cd)
        private static readonly (double Mach, double Cd)[] G1Table =
        {
            (0.00, 0.2629),
            (0.50, 0.2629),
            (0.70, 0.2558),
            (0.90, 0.2487),
            (1.00, 0.3000),
            (1.20, 0.3800),
            (1.50, 0.3000),
            (2.00, 0.2500),
            (3.00, 0.2100),
            (5.00, 0.1900)
        };

        // G7 Drag Coefficient Table (Mach vs Cd)
        private static readonly (double Mach, double Cd)[] G7Table =
        {
            (0.00, 0.1198),
            (0.50, 0.1198),
            (0.70, 0.1187),
            (0.90, 0.1200),
            (1.00, 0.1600),
            (1.20, 0.2000),
            (1.50, 0.1700),
            (2.00, 0.1500),
            (3.00, 0.1300),
            (5.00, 0.1200)
        };

        /// <summary>
        /// Interpolates drag coefficient for given velocity and drag model
        /// </summary>
        private static double InterpolateCd(double velocity, BallisticSettings.DragModelEnum model)
        {
            if (model == BallisticSettings.DragModelEnum.None)
                return 0.0;

            double mach = Math.Abs(velocity) / speedOfSound;
            var table = model == BallisticSettings.DragModelEnum.G7 ? G7Table : G1Table;

            // Handle extreme values
            if (mach <= table[0].Mach)
                return table[0].Cd;

            if (mach >= table[table.Length - 1].Mach)
                return table[table.Length - 1].Cd;

            // Linear interpolation
            for (int i = 0; i < table.Length - 1; i++)
            {
                if (mach >= table[i].Mach && mach <= table[i + 1].Mach)
                {
                    double fraction = (mach - table[i].Mach) / (table[i + 1].Mach - table[i].Mach);
                    return table[i].Cd + fraction * (table[i + 1].Cd - table[i].Cd);
                }
            }

            return table[table.Length - 1].Cd;
        }

        /// <summary>
        /// Calculates bullet drop and windage at specified distance
        /// </summary>
        /// <returns>(verticalDropInMeters, horizontalWindageInMeters, timeOfFlightInSeconds)</returns>
        public static (double drop, double windage, double time) Solve(BallisticSettings settings)
        {
            double dt = TimeStep;

            // 1. Find zero angle (elevation needed to hit zero distance)
            double zeroAngle = FindZeroAngle(settings, dt);

            // 2. Calculate trajectory to target distance
            return CalculateTrajectory(settings, zeroAngle, dt);
        }

        /// <summary>
        /// Finds the elevation angle needed to zero at the specified distance
        /// </summary>
        private static double FindZeroAngle(BallisticSettings settings, double dt)
        {
            // Binary search for correct angle
            double angleLow = -0.1;   // radians (~-5.7 degrees)
            double angleHigh = 0.1;   // radians (~5.7 degrees)
            double tolerance = 1e-6;

            // Target final height (bullet should end at line of sight height)
            double targetHeight = -settings.ScopeHeight;

            for (int i = 0; i < 50; i++)  // Max 50 iterations
            {
                double midAngle = (angleLow + angleHigh) * 0.5;
                double finalHeight = SimulateToDistance(settings, midAngle, settings.ZeroDistance, dt, false);

                if (Math.Abs(finalHeight - targetHeight) < tolerance)
                    return midAngle;

                if (finalHeight < targetHeight)
                    angleLow = midAngle;  // Need higher angle
                else
                    angleHigh = midAngle; // Need lower angle
            }

            return (angleLow + angleHigh) * 0.5;
        }

        /// <summary>
        /// Simulates trajectory to specified distance (no wind for zeroing)
        /// </summary>
        private static double SimulateToDistance(BallisticSettings settings, double angle,
                                                double targetDistance, double dt, bool includeWind)
        {
            // Initial conditions
            double x = 0.0;      // Windage (horizontal)
            double y = -settings.ScopeHeight;  // Vertical (negative = below line of sight)
            double z = 0.0;      // Downrange distance

            // Initial velocity components
            double vx = 0.0;
            double vy = settings.BulletVelocity * Math.Sin(angle);
            double vz = settings.BulletVelocity * Math.Cos(angle);

            // Wind components (if applicable)
            double wx = 0.0, wz = 0.0;
            if (includeWind && settings.WindSpeed > 0)
            {
                double windRad = settings.WindDirection * Math.PI / 180.0;
                wx = settings.WindSpeed * Math.Sin(windRad);
                wz = settings.WindSpeed * Math.Cos(windRad);
            }

            // Integration loop
            while (z < targetDistance && vz > 0)
            {
                // Relative velocity to air (accounting for wind)
                double rvx = vx - wx;
                double rvy = vy;
                double rvz = vz - wz;

                double velocity = Math.Sqrt(rvx * rvx + rvy * rvy + rvz * rvz);

                // Avoid division by zero
                if (velocity < 0.001)
                    break;

                // Get drag coefficient and calculate drag force
                double cd = InterpolateCd(velocity, settings.DragModel);

                // FIXED: Correct drag calculation using BC
                // Drag deceleration = (ρ * v * Cd * v) / (2 * BC) = (ρ * v² * Cd) / (2 * BC)
                // BC already incorporates mass, diameter, and form factor
                double dragFactor = (rho * velocity * cd) / (2.0 * Math.Max(settings.BallisticCoefficient, 0.001));

                // Drag acceleration components (opposite to velocity direction)
                // Normalize by velocity to get direction
                double ax = -dragFactor * (rvx / velocity);
                double ay = -g - dragFactor * (rvy / velocity);
                double az = -dragFactor * (rvz / velocity);

                // Euler integration
                vx += ax * dt;
                vy += ay * dt;
                vz += az * dt;

                x += vx * dt;
                y += vy * dt;
                z += vz * dt;
            }

            return y;  // Return vertical position
        }

        /// <summary>
        /// Calculates full trajectory with wind
        /// </summary>
        private static (double drop, double windage, double time) CalculateTrajectory(
            BallisticSettings settings, double zeroAngle, double dt)
        {
            // Initial conditions
            double x = 0.0;      // Windage
            double y = -settings.ScopeHeight;  // Vertical
            double z = 0.0;      // Downrange

            double vx = 0.0;
            double vy = settings.BulletVelocity * Math.Sin(zeroAngle);
            double vz = settings.BulletVelocity * Math.Cos(zeroAngle);

            // Wind components
            double wx = 0.0, wz = 0.0;
            if (settings.WindSpeed > 0)
            {
                double windRad = settings.WindDirection * Math.PI / 180.0;
                wx = settings.WindSpeed * Math.Sin(windRad);
                wz = settings.WindSpeed * Math.Cos(windRad);
            }

            double time = 0.0;
            double lastZ = 0.0;
            double lastY = y;
            double lastX = x;

            // Integration loop
            while (z < settings.Distance && vz > 0)
            {
                // Store previous values for interpolation
                lastZ = z;
                lastY = y;
                lastX = x;

                // Relative velocity to air
                double rvx = vx - wx;
                double rvy = vy;
                double rvz = vz - wz;

                double velocity = Math.Sqrt(rvx * rvx + rvy * rvy + rvz * rvz);

                if (velocity < 0.001)
                    break;

                // Drag calculation
                double cd = InterpolateCd(velocity, settings.DragModel);
                double dragFactor = (rho * velocity * cd) / (2.0 * Math.Max(settings.BallisticCoefficient, 0.001));

                // Acceleration components (normalized by velocity)
                double ax = -dragFactor * (rvx / velocity);
                double ay = -g - dragFactor * (rvy / velocity);
                double az = -dragFactor * (rvz / velocity);

                // Integration
                vx += ax * dt;
                vy += ay * dt;
                vz += az * dt;

                x += vx * dt;
                y += vy * dt;
                z += vz * dt;
                time += dt;

                // Break if we've passed the target
                if (z >= settings.Distance)
                {
                    // Linear interpolation to exact distance
                    double fraction = (settings.Distance - lastZ) / (z - lastZ);
                    x = lastX + (x - lastX) * fraction;
                    y = lastY + (y - lastY) * fraction;
                    // Time doesn't need interpolation for final result
                    break;
                }
            }

            return (y, x, time);
        }

        /// <summary>
        /// Generates a trajectory table at specified intervals
        /// </summary>
        public static List<TrajectoryPoint> GenerateTrajectoryTable(BallisticSettings settings, double interval = 50.0)
        {
            var table = new List<TrajectoryPoint>();
            double dt = TimeStep;

            // Find zero angle
            double zeroAngle = FindZeroAngle(settings, dt);

            // Initial conditions
            double x = 0.0;
            double y = -settings.ScopeHeight;
            double z = 0.0;

            double vx = 0.0;
            double vy = settings.BulletVelocity * Math.Sin(zeroAngle);
            double vz = settings.BulletVelocity * Math.Cos(zeroAngle);

            // Wind
            double wx = 0.0, wz = 0.0;
            if (settings.WindSpeed > 0)
            {
                double windRad = settings.WindDirection * Math.PI / 180.0;
                wx = settings.WindSpeed * Math.Sin(windRad);
                wz = settings.WindSpeed * Math.Cos(windRad);
            }

            double time = 0.0;
            double nextRange = 0.0;

            // Add muzzle data
            table.Add(new TrajectoryPoint
            {
                Range = 0,
                Drop = -settings.ScopeHeight,
                Windage = 0,
                Velocity = settings.BulletVelocity,
                Time = 0
            });

            nextRange += interval;

            // Integration loop
            while (z < settings.Distance && vz > 0)
            {
                // Relative velocity
                double rvx = vx - wx;
                double rvy = vy;
                double rvz = vz - wz;

                double velocity = Math.Sqrt(rvx * rvx + rvy * rvy + rvz * rvz);

                if (velocity < 0.001)
                    break;

                // Drag
                double cd = InterpolateCd(velocity, settings.DragModel);
                double dragFactor = (rho * velocity * cd) / (2.0 * Math.Max(settings.BallisticCoefficient, 0.001));

                // Acceleration
                double ax = -dragFactor * (rvx / velocity);
                double ay = -g - dragFactor * (rvy / velocity);
                double az = -dragFactor * (rvz / velocity);

                // Integration
                vx += ax * dt;
                vy += ay * dt;
                vz += az * dt;

                x += vx * dt;
                y += vy * dt;
                z += vz * dt;
                time += dt;

                // Check if we've passed an interval point
                if (z >= nextRange)
                {
                    table.Add(new TrajectoryPoint
                    {
                        Range = nextRange,
                        Drop = y,
                        Windage = x,
                        Velocity = velocity,
                        Time = time
                    });
                    nextRange += interval;
                }
            }

            // Add final point if not already included
            if (table.Count > 0)
            {
                var lastPoint = table[table.Count - 1];
                if (lastPoint.Range < settings.Distance && z >= settings.Distance)
                {
                    double finalVelocity = Math.Sqrt(vx * vx + vy * vy + vz * vz);
                    table.Add(new TrajectoryPoint
                    {
                        Range = settings.Distance,
                        Drop = y,
                        Windage = x,
                        Velocity = finalVelocity,
                        Time = time
                    });
                }
            }

            return table;
        }

        /// <summary>
        /// Converts radians to minutes of angle (MOA)
        /// </summary>
        public static double RadiansToMOA(double radians)
        {
            // 1 MOA = 1/60 degree = (π/180)/60 radians at 100 yards
            // At 100 yards: 1 MOA ≈ 1.047 inches ≈ 0.0266 meters
            return radians * (180.0 / Math.PI) * 60.0; // radians to degrees to MOA
        }

        /// <summary>
        /// Converts meters to inches
        /// </summary>
        public static double MetersToInches(double meters)
        {
            return meters * 39.3701;
        }

        /// <summary>
        /// Converts meters to feet
        /// </summary>
        public static double MetersToFeet(double meters)
        {
            return meters * 3.28084;
        }

        /// <summary>
        /// Example usage/test method
        /// </summary>
        public static void RunExample()
        {
            var settings = new BallisticSettings
            {
                BulletVelocity = 850.0,           // m/s (~2780 fps)
                BallisticCoefficient = 0.5,       // Typical G1 BC for .308
                ZeroDistance = 100.0,             // Zero at 100m
                Distance = 300.0,                 // Target at 300m
                ScopeHeight = 0.05,               // 5cm scope height
                DragModel = BallisticSettings.DragModelEnum.G1,
                WindSpeed = 5.0,                  // 5 m/s wind
                WindDirection = 90.0              // 90° = full crosswind from right
            };

            Console.WriteLine("Ballistic Solver Example");
            Console.WriteLine("========================");
            Console.WriteLine($"Muzzle Velocity: {settings.BulletVelocity} m/s ({settings.BulletVelocity * 3.28084:F0} fps)");
            Console.WriteLine($"BC (G1): {settings.BallisticCoefficient}");
            Console.WriteLine($"Zero Range: {settings.ZeroDistance} m ({MetersToFeet(settings.ZeroDistance):F0} ft)");
            Console.WriteLine($"Target Range: {settings.Distance} m ({MetersToFeet(settings.Distance):F0} ft)");
            Console.WriteLine($"Wind: {settings.WindSpeed} m/s ({settings.WindSpeed * 2.23694:F1} mph) at {settings.WindDirection}°");
            Console.WriteLine();

            // Calculate solution
            var result = Solve(settings);

            Console.WriteLine($"Results at {settings.Distance}m:");
            Console.WriteLine($"  Drop: {result.drop:F3} m ({MetersToInches(result.drop):F2} in)");
            Console.WriteLine($"  Windage: {result.windage:F3} m ({MetersToInches(result.windage):F2} in)");
            Console.WriteLine($"  Time of Flight: {result.time:F3} s");

            // Calculate drop in MOA
            double dropMOA = RadiansToMOA(Math.Atan2(result.drop, settings.Distance));
            Console.WriteLine($"  Drop Adjustment: {dropMOA:F2} MOA");
            Console.WriteLine();

            // Generate trajectory table
            Console.WriteLine("Trajectory Table:");
            Console.WriteLine("Range(m)  Drop(m)  Windage(m)  Velocity(m/s)  Time(s)");
            Console.WriteLine("------------------------------------------------------");

            var table = GenerateTrajectoryTable(settings, 50.0);
            foreach (var point in table)
            {
                Console.WriteLine($"{point.Range,6:F0}  {point.Drop,7:F3}  {point.Windage,9:F3}  {point.Velocity,12:F1}  {point.Time,7:F3}");
            }
        }
    }

    /// <summary>
    /// Represents a point on the bullet trajectory
    /// </summary>
    public class TrajectoryPoint
    {
        public double Range { get; set; }     // meters
        public double Drop { get; set; }      // meters (negative = below line of sight)
        public double Windage { get; set; }   // meters (positive = right)
        public double Velocity { get; set; }  // m/s
        public double Time { get; set; }      // seconds
    }
}