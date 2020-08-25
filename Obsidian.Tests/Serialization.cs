using Obsidian.Chat;
using Obsidian.Net;
using Obsidian.Util.DataTypes;
using Obsidian.Net.Packets;
using Obsidian.Net.Packets.Login;
using Obsidian.Net.Packets.Play;
using Obsidian.Serializer;
using System.Threading.Tasks;
using Xunit;
using System;
using Obsidian.Entities;

namespace Obsidian.Tests
{
    public class Serialization
    {
        [Fact]
        public async Task Disconnect()
        {
            var reason = ChatMessage.Simple("text");
            var packet = new Disconnect(reason, ClientState.Status);

            using var stream = new MinecraftStream(debug: true);
            await PacketSerializer.FastSerializeAsync(packet, stream);
            stream.Position = 0;

            Assert.Equal(reason.ToString(), (await stream.ReadChatAsync()).ToString());
        }

        [Fact]
        public async Task KeepAlive()
        {
            var packet = new KeepAlive(long.MaxValue);

            using var stream = new MinecraftStream(debug: true);
            await PacketSerializer.FastSerializeAsync(packet, stream);
            stream.Position = 0;

            Assert.Equal(packet.KeepAliveId, await stream.ReadLongAsync());
        }

        [Fact]
        public async Task PlayerPositionLook()
        {
            var pitch = new Angle(byte.MinValue);
            var yaw = new Angle(byte.MaxValue);
            var transform = new Transform(1.0, 2.0, 3.0, pitch, yaw);
            var flags = PositionFlags.X | PositionFlags.Y_ROT;
            var tpId = int.MaxValue;
            var packet = new PlayerPositionLook(transform, flags, tpId);

            using var stream = new MinecraftStream(debug: true);
            await PacketSerializer.FastSerializeAsync(packet, stream);
            stream.Position = 0;

            Assert.Equal(packet.Transform.X, await stream.ReadDoubleAsync());
            Assert.Equal(packet.Transform.Y, await stream.ReadDoubleAsync());
            Assert.Equal(packet.Transform.Z, await stream.ReadDoubleAsync());
            Assert.Equal(packet.Transform.Yaw.Degrees, await stream.ReadFloatAsync());
            Assert.Equal(packet.Transform.Pitch.Degrees, await stream.ReadFloatAsync());
            Assert.Equal((sbyte)packet.Flags, await stream.ReadByteAsync());
            Assert.Equal(tpId, await stream.ReadVarIntAsync());
        }

        //[Fact]
        //public async Task SpawnMob()
        //{
        //    var id = int.MaxValue;
        //    var uuid = Guid.NewGuid();
        //    var type = int.MinValue;
        //    var pitch = new Angle(byte.MinValue);
        //    var yaw = new Angle(byte.MaxValue);
        //    var transform = new Transform(1.0, 2.0, 3.0, pitch, yaw);
        //    var headPitch = float.MaxValue;
        //    var velocity = new Velocity(short.MaxValue, short.MinValue, short.MaxValue);
        //    var entity = new Bat();
        //    var packet = new SpawnMob(id, uuid, type, transform, headPitch, velocity, entity);

        //    using var stream = new MinecraftStream(debug: true);
        //    await PacketSerializer.FastSerializeAsync(packet, stream);
        //    stream.Position = 0;

        //    Assert.Equal(id, await stream.ReadIntAsync());
        //    Assert.Equal(uuid.ToString(), await stream.ReadStringAsync());
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //    //Assert.Equal();
        //}

        //[Fact]
        //public async Task PlayerInfo()
        //{

        //}

        //[Fact]
        //public async Task PlayerListHeaderFooter()
        //{

        //}

        //[Fact]
        //public async Task RequestResponse()
        //{

        //}

        //[Fact]
        //public async Task PingPong()
        //{

        //}

        //[Fact]
        //public async Task EncryptionRequest()
        //{

        //}

        //[Fact]
        //public async Task SetCompression()
        //{

        //}
        //[Fact]

        //public async Task LoginSuccess()
        //{

        //}

        //[Fact]
        //public async Task JoinGame()
        //{

        //}

        //[Fact]
        //public async Task SpawnPosition()
        //{

        //}

        //[Fact]
        //public async Task BlockChange()
        //{

        //}

        //[Fact]
        //public async Task EntityPacket()
        //{

        //}

        //[Fact]
        //public async Task DeclareCommands()
        //{

        //}

        //[Fact]
        //public async Task ChatMessagePacket()
        //{

        //}

        //[Fact]
        //public async Task SoundEffect()
        //{

        //}

        //[Fact]
        //public async Task NamedSoundEffect()
        //{

        //}

        //[Fact]
        //public async Task BossBar()
        //{

        //}
    }
}
