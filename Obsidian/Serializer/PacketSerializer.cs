using Obsidian.Net;
using Obsidian.Net.Packets;
using Obsidian.Serializer.Dynamic;
using Obsidian.Serializer.Enums;
using Obsidian.Util.DataTypes;
using Obsidian.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Obsidian.Serializer
{
    public static class PacketSerializer
    {
        internal delegate void SerializeDelegate(MinecraftStream minecraftStream, Packet packet);
        public delegate Packet DeserializeDelegate(MinecraftStream minecraftStream);

        private static readonly Dictionary<Type, SerializeDelegate> serializationMethodsCache = new Dictionary<Type, SerializeDelegate>();
        public static readonly Dictionary<Type, DeserializeDelegate> deserializationMethodsCache = new Dictionary<Type, DeserializeDelegate>();

        public static async Task SerializeAsync(Packet packet, MinecraftStream stream)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            await stream.Lock.WaitAsync();

            var valueDict = (await packet.GetAllObjectsAsync()).OrderBy(x => x.Key.Order);

            await using var dataStream = new MinecraftStream(true);

            foreach (var (key, value) in valueDict)
            {
                var dataType = key.Type;

                if (dataType == DataType.Auto)
                    dataType = value.GetType().ToDataType();

                await dataStream.WriteAsync(dataType, key, value);
            }

            var packetLength = packet.id.GetVarIntLength() + (int)dataStream.Length;

            await stream.WriteVarIntAsync(packetLength);
            await stream.WriteVarIntAsync(packet.id);

            dataStream.Position = 0;
            await dataStream.DumpAsync();

            await dataStream.CopyToAsync(stream);

            stream.Lock.Release();
        }

        public static async Task<T> DeserializeAsync<T>(byte[] data) where T : Packet
        {
            await using var stream = new MinecraftStream(data);
            var packet = (T)Activator.CreateInstance(typeof(T));
            if (packet == null)
                throw new NullReferenceException(nameof(packet));

            var valueDict = (await packet.GetAllMemberNamesAsync()).OrderBy(x => x.Key.Order);
            var members = packet.GetType().GetMembers(PacketExtensions.Flags);

            foreach (var (key, value) in valueDict)
            {
                foreach (var member in members)
                {
                    if (member.Name != value)
                        continue;

                    if (member is FieldInfo field)
                    {
                        var dataType = key.Type;

                        if (dataType == DataType.Auto)
                            dataType = field.FieldType.ToDataType();

                        var val = await stream.ReadAsync(field.FieldType, dataType, key);

                        await Program.PacketLogger.LogDebugAsync($"Setting val {val}");

                        field.SetValue(packet, val);
                    }
                    else if (member is PropertyInfo property)
                    {
                        var dataType = key.Type;

                        if (dataType == DataType.Auto)
                            dataType = property.PropertyType.ToDataType();

                        var val = await stream.ReadAsync(property.PropertyType, dataType, key);

                        await Program.PacketLogger.LogDebugAsync($"Setting val {val}");

                        if (property.PropertyType.IsEnum && property.PropertyType == typeof(BlockFace))
                            val = (BlockFace)val;

                        property.SetValue(packet, val);
                    }
                }
            }

            return packet;
        }

        public static async Task FastSerializeAsync<T>(T packet, MinecraftStream minecraftStream) where T : Packet
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (minecraftStream == null)
                throw new ArgumentNullException(nameof(minecraftStream));

            if (!serializationMethodsCache.TryGetValue(typeof(T), out var serializeMethod))
                serializationMethodsCache.Add(typeof(T), serializeMethod = SerializationMethodBuilder.BuildSerializationMethod<T>());

            serializeMethod(minecraftStream, packet);
        }

        public static async Task<T> FastDeserializeAsync<T>(byte[] data) where T : Packet
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            await using var stream = new MinecraftStream(data);

            if (!deserializationMethodsCache.TryGetValue(typeof(T), out var deserializeMethod))
                deserializationMethodsCache.Add(typeof(T), deserializeMethod = SerializationMethodBuilder.BuildDeserializationMethod<T>());

            return (T)deserializeMethod(stream);
        }

        public static T FastDeserialize<T>(MinecraftStream minecraftStream) where T : Packet
        {
            if (minecraftStream == null)
                throw new ArgumentNullException(nameof(minecraftStream));

            if (!deserializationMethodsCache.TryGetValue(typeof(T), out var deserializeMethod))
                deserializationMethodsCache.Add(typeof(T), deserializeMethod = SerializationMethodBuilder.BuildDeserializationMethod<T>());

            return (T)deserializeMethod(minecraftStream);
        }

        public static void PrewarmPacketSerialization<T>() where T : Packet
        {
            try
            {
                if (!serializationMethodsCache.ContainsKey(typeof(T)))
                    serializationMethodsCache.Add(typeof(T), SerializationMethodBuilder.BuildSerializationMethod<T>());
            }
            catch
            {

            }

            try
            {
                if (!deserializationMethodsCache.ContainsKey(typeof(T)))
                    deserializationMethodsCache.Add(typeof(T), SerializationMethodBuilder.BuildDeserializationMethod<T>());
            }
            catch
            {

            }
        }

#nullable enable
        public static void PrewarmAssemblyPackets(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            var prewarmMethod = typeof(PacketSerializer).GetMethod(nameof(PrewarmPacketSerialization));
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsSubclassOf(typeof(Packet)))
                {
                    var genericPrewarmMethod = prewarmMethod?.MakeGenericMethod(type);
                    genericPrewarmMethod?.Invoke(null, Array.Empty<object>());
                }
            }
        }
#nullable disable
    }
}