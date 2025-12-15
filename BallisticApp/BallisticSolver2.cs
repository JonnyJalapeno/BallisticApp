using System;

namespace BallisticApp
{
    public static class BallisticsSolver2
    {
        const double g = 9.80665;
        const double speedOfSound = 343.0;
        const double rho = 1.225;

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

        static double Cd(double v, BallisticSettings.DragModelEnum model)
        {
            if (model == BallisticSettings.DragModelEnum.None)
                return 0;

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

        struct State
        {
            public double x, y, z;
            public double vx, vy, vz;
        }

        static State Deriv(State s, BallisticSettings b, double wx, double wz)
        {
            double rvx = s.vx - wx;
            double rvy = s.vy;
            double rvz = s.vz - wz;

            double v = Math.Sqrt(rvx * rvx + rvy * rvy + rvz * rvz);
            double drag = Cd(v, b.DragModel) / Math.Max(b.BallisticCoefficient, 1e-6);

            return new State
            {
                x = s.vx,
                y = s.vy,
                z = s.vz,

                vx = -drag * v * rvx,
                vy = -g - drag * v * rvy,
                vz = -drag * v * rvz
            };
        }

        static State RK4(State s, BallisticSettings b, double dt, double wx, double wz)
        {
            var k1 = Deriv(s, b, wx, wz);
            var k2 = Deriv(Add(s, k1, dt * 0.5), b, wx, wz);
            var k3 = Deriv(Add(s, k2, dt * 0.5), b, wx, wz);
            var k4 = Deriv(Add(s, k3, dt), b, wx, wz);

            return Add(s, Combine(k1, k2, k3, k4), dt / 6.0);
        }

        static State Add(State s, State k, double dt) =>
            new State
            {
                x = s.x + k.x * dt,
                y = s.y + k.y * dt,
                z = s.z + k.z * dt,
                vx = s.vx + k.vx * dt,
                vy = s.vy + k.vy * dt,
                vz = s.vz + k.vz * dt
            };

        static State Combine(State a, State b, State c, State d) =>
            new State
            {
                x = a.x + 2 * b.x + 2 * c.x + d.x,
                y = a.y + 2 * b.y + 2 * c.y + d.y,
                z = a.z + 2 * b.z + 2 * c.z + d.z,
                vx = a.vx + 2 * b.vx + 2 * c.vx + d.vx,
                vy = a.vy + 2 * b.vy + 2 * c.vy + d.vy,
                vz = a.vz + 2 * b.vz + 2 * c.vz + d.vz
            };

        static double FindZeroAngle(BallisticSettings b)
        {
            double lo = -0.05, hi = 0.05;

            for (int i = 0; i < 30; i++)
            {
                double mid = (lo + hi) * 0.5;
                if (HeightAtZero(b, mid) > 0) hi = mid;
                else lo = mid;
            }
            return (lo + hi) * 0.5;
        }

        static double HeightAtZero(BallisticSettings b, double angle)
        {
            double dt = 0.01;
            double t = 0;

            State s = new State
            {
                y = -b.ScopeHeight,
                vy = b.BulletVelocity * Math.Sin(angle),
                vz = b.BulletVelocity * Math.Cos(angle)
            };

            while (t < 20 && s.z < b.ZeroDistance)
            {
                s = RK4(s, b, dt, 0, 0);
                t += dt;
            }
            return s.y;
        }

        public static (double vertical, double horizontal) Solve(BallisticSettings b)
        {
            double dt = 0.01;
            double t = 0;

            double wRad = b.WindDirection * Math.PI / 180.0;
            double wx = b.WindSpeed * Math.Sin(wRad);
            double wz = b.WindSpeed * Math.Cos(wRad);

            double angle = FindZeroAngle(b);

            State s = new State
            {
                y = -b.ScopeHeight,
                vy = b.BulletVelocity * Math.Sin(angle),
                vz = b.BulletVelocity * Math.Cos(angle)
            };

            while (t < 20 && s.z < b.Distance && s.vz > 0)
            {
                s = RK4(s, b, dt, wx, wz);
                t += dt;
            }

            return (s.y, s.x);
        }
    }
}
