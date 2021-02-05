﻿using Obsidian.Entities;
using Obsidian.Serialization.Attributes;
using System.Threading.Tasks;

namespace Obsidian.Net.Packets.Play.Clientbound
{
    /// <summary>
    /// https://wiki.vg/index.php?title=Protocol#Update_View_Position
    /// </summary>
    public partial class UpdateViewPosition : IPacket
    {
        [Field(0), VarLength]
        public int ChunkX;

        [Field(1), VarLength]
        public int ChunkZ;

        private UpdateViewPosition()
        {
        }

        public UpdateViewPosition(int chunkx, int chunkz)
        {
            this.ChunkX = chunkx;
            this.ChunkZ = chunkz;
        }

        public int Id => 0x40;

        public Task HandleAsync(Server server, Player player) => Task.CompletedTask;

        public Task ReadAsync(MinecraftStream stream) => Task.CompletedTask;

        public Task WriteAsync(MinecraftStream stream) => Task.CompletedTask;
    }
}