using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallisticApp
{
    public static class BallisticsSolver
    {
        const double g = 9.80665;     // m/s²
        const double rho = 1.225;     // kg/m³
        const double speedOfSound = 343.0; // m/s

        // McCoy-style reduced drag tables (Mach, Cd)
        static readonly (double M, double Cd)[] G1 =
        {
            (0.0,0.2629),(0.5,0.2629),(0.7,0.2558),(0.9,0.2487),
            (1.0,0.3000),(1.2,0.3800),(1.5,0.3000),(2.0,0.2500),
            (3.0,0.2100),(5.0,0.1900)
        };

        static readonly (double M, double Cd)[] G7 =
        {
            (0.0,0.1198),(0.5,0.1198),(0.7,0.1187),(0.9,0.1200),
            (1.0,0.1600),(1.2,0.2000),(1.5,0.1700),(2.0,0.1500),
            (3.0,0.1300),(5.0,0.1200)
        };

        static double InterpCd(double v, BallisticSettings.DragModelEnum model)
        {
            if (model == BallisticSettings.DragModelEnum.None)
                return 0.0;

            double mach = v / speedOfSound;
            var t = model == BallisticSettings.DragModelEnum.G7 ? G7 : G1;

            for (int i = 0; i < t.Length - 1; i++)
            {
                if (mach >= t[i].M && mach <= t[i + 1].M)
                {
                    double u = (mach - t[i].M) / (t[i + 1].M - t[i].M);
                    return t[i].Cd + u * (t[i + 1].Cd - t[i].Cd);
                }
            }
            return t[^1].Cd;
        }

        public static (double vertical, double horizontal) Solve(BallisticSettings s)
        {
            double dt = 0.001;

            // Wind
            double wRad = s.WindDirection * Math.PI / 180.0;
            double wx = s.WindSpeed * Math.Sin(wRad);
            double wz = s.WindSpeed * Math.Cos(wRad);

            // Zeroing
            double zeroAngle = FindZeroAngle(s, dt);

            double x = 0;
            double y = -s.ScopeHeight;
            double z = 0;

            double vx = 0;
            double vy = s.BulletVelocity * Math.Sin(zeroAngle);
            double vz = s.BulletVelocity * Math.Cos(zeroAngle);

            while (z < s.Distance && vz > 0)
            {
                double rvx = vx - wx;
                double rvy = vy;
                double rvz = vz - wz;

                double v = Math.Sqrt(rvx * rvx + rvy * rvy + rvz * rvz);
                double cd = InterpCd(v, s.DragModel);
                double drag = cd / Math.Max(s.BallisticCoefficient, 1e-6);

                double ax = -drag * v * rvx;
                double ay = -g - drag * v * rvy;
                double az = -drag * v * rvz;

                vx += ax * dt;
                vy += ay * dt;
                vz += az * dt;

                x += vx * dt;
                y += vy * dt;
                z += vz * dt;
            }

            return (y, x);
        }

        static double FindZeroAngle(BallisticSettings s, double dt)
        {
            double lo = -0.05, hi = 0.05;

            for (int i = 0; i < 32; i++)
            {
                double mid = (lo + hi) * 0.5;
                if (ZeroHeight(s, mid, dt) > 0)
                    hi = mid;
                else
                    lo = mid;
            }
            return (lo + hi) * 0.5;
        }

        static double ZeroHeight(BallisticSettings s, double angle, double dt)
        {
            double y = -s.ScopeHeight;
            double z = 0;

            double vy = s.BulletVelocity * Math.Sin(angle);
            double vz = s.BulletVelocity * Math.Cos(angle);

            while (z < s.ZeroDistance && vz > 0)
            {
                double v = Math.Sqrt(vy * vy + vz * vz);
                double cd = InterpCd(v, s.DragModel);
                double drag = cd / Math.Max(s.BallisticCoefficient, 1e-6);

                double ay = -g - drag * v * vy;
                double az = -drag * v * vz;

                vy += ay * dt;
                vz += az * dt;

                y += vy * dt;
                z += vz * dt;
            }
            return y;
        }
    }
}
