using System;

namespace UExtension
{
    public interface IRandom
    {
        uint Seed { get; set; }
        uint Next();   
    }

    public class Xorshift128 : IRandom
    {
        uint IRandom.Seed
        {
            get { return this.x;  }
            set { SetSeed(value); }
        }
        uint IRandom.Next()
        {
            uint t = this.x ^ (this.x << 11);
            this.x = this.y; this.y = this.z; this.z = this.w;
            this.w = (this.w ^ (this.w >> 19)) ^ (t ^ (t >> 8));
            return this.w;
        }

        protected void SetSeed(uint nSeed)
        {
            this.x = nSeed;
            this.y = this.x * 1812433253U + 1;
            this.z = this.y * 1812433253U + 1;
            this.w = this.z * 1812433253U + 1;
        }

        protected uint x, y, z, w;
    }

    public class TRandom<T>
        where T : IRandom
    {
        public uint Seed
        {
            get { return this.Impl.Seed;  }
            set { this.Impl.Seed = value; }
        }
        public TRandom() {}
        public TRandom(uint nSeed)
        {
            this.Impl.Seed = nSeed;
        }

        public int Next()
        {
            return (int)this.Impl.Next();
        }
        public float NextFloat()
        {
            return UInt2Float(this.Impl.Next());
        }
        public byte NextByte()
        {
            return UInt2Byte(this.Impl.Next());
        }
        public float Range(float min, float max)
        {
            var t = this.NextFloat();
            return min * t + (1.0f - t) * max;
        }
        public int   Range(int min, int max)
        {
            int diff;
            if (min < max)
            {
                diff = max - min;
                var t = this.Next() % diff;
                t += min;
                return t;
            }
            else if (min > max)
            {
                diff = min - max;
                var t = this.Next() % diff;
                t = min - t;
                return t;
            }
            else
            {
                return min;
            }
        }

        static float UInt2Float(uint value)
        {
            return ((float)(value & 0x007FFFFF)) * (1.0f / 8388607.0f);
        }
        static byte UInt2Byte(uint value)
        {
            return (byte)(value >> (23 - 8));
        }

        protected IRandom Impl = ReflectExtension.TConstruct<IRandom>(typeof(T));
    }

    public class ERandom : TRandom<Xorshift128>
    {
        public ERandom() {}
        public ERandom(uint nSeed)
            : base(nSeed)
        {}
    }
}