﻿using System;

namespace Obsidian.API
{
    public class ParticleData
    {
        public static readonly ParticleData None = new ParticleData();

        private object data;
        internal ParticleType ParticleType { get; set; }

        private ParticleData()
        {
            data = null;
            ParticleType = (ParticleType)(-1);
        }

        private ParticleData(object data, ParticleType targetType)
        {
            this.data = data;
            ParticleType = targetType;
        }

        internal T GetDataAs<T>()
        {
            if (data is T t)
            {
                return t;
            }
            throw new InvalidOperationException();
        }

        public static ParticleData ForBlock(int blockState) => new ParticleData(blockState, ParticleType.Block);
        public static ParticleData ForDust(float red, float green, float blue, float scale) => new ParticleData((red, green, blue, scale), ParticleType.Dust);
        public static ParticleData ForFallingDust(int blockState) => new ParticleData(blockState, ParticleType.FallingDust);
        public static ParticleData ForItem(ItemStack item) => new ParticleData(item, ParticleType.Item);
    }
}
