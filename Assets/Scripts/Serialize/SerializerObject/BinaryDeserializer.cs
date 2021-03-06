﻿using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A binary serializer used for deserializing
    /// </summary>
    public class BinaryDeserializer : SerializerObject, IDisposable {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The serializing context</param>
        public BinaryDeserializer(Context context) : base(context) {}

        public override Pointer CurrentPointer {
            get {
                if (currentFile == null) {
                    return null;
                }
                uint curPos = (uint)reader.BaseStream.Position;
                return new Pointer((uint)(curPos + currentFile.baseAddress), currentFile);
            }
        }

        public override uint CurrentLength => (uint)reader.BaseStream.Length;
        private string LogPrefix => Settings.Log ? ("(READ) " + CurrentPointer + ":" + new string(' ', (Depth + 1) * 2)) : null;

        protected Reader reader;
        protected BinaryFile currentFile;
        protected Dictionary<BinaryFile, Reader> readers = new Dictionary<BinaryFile, Reader>();
        protected void SwitchToFile(BinaryFile newFile) {
            if (newFile == null) return;
            if (!readers.ContainsKey(newFile)) {
                readers.Add(newFile, newFile.CreateReader());
                newFile.InitFileReadMap(readers[newFile].BaseStream.Length);
            }
            reader = readers[newFile];
            currentFile = newFile;
        }

        // Helper method which returns an object so we can cast it
        protected object ReadAsObject<T>(string name = null) {
            // Get the type
            var type = typeof(T);

            TypeCode typeCode = Type.GetTypeCode(type);
            /*if (type.IsEnum)
                typeCode = TypeCode.Byte;*/

            switch (typeCode) {
                case TypeCode.Boolean:
                    var b = reader.ReadByte();

                    if (b != 0 && b != 1) {
                        Debug.LogWarning($"Binary boolean '{name}' ({b}) was not correctly formatted");

                        if (Settings.Log) {
                            Context.Log.Log(LogPrefix + "(" + typeof(T) + "): Binary boolean was not correctly formatted (" + b + ")");
                        }
                    }

                    return b == 1;

                case TypeCode.SByte:
                    return reader.ReadSByte();

                case TypeCode.Byte:
                    return reader.ReadByte();

                case TypeCode.Int16:
                    return reader.ReadInt16();

                case TypeCode.UInt16:
                    return reader.ReadUInt16();

                case TypeCode.Int32:
                    return reader.ReadInt32();

                case TypeCode.UInt32:
                    return reader.ReadUInt32();

                case TypeCode.Int64:
                    return reader.ReadInt64();

                case TypeCode.UInt64:
                    return reader.ReadUInt64();

                case TypeCode.Single:
                    return reader.ReadSingle();

                case TypeCode.Double:
                    return reader.ReadDouble();
                case TypeCode.String:
                    return reader.ReadNullDelimitedString();

                case TypeCode.Decimal:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Object:
                    if (type == typeof(UInt24)) {
                        return reader.ReadUInt24();
                    } else if(type == typeof(byte?)) {
                        byte nullableByte = reader.ReadByte();
                        if(nullableByte == 0xFF) return (byte?)null;
                        return nullableByte;
                    } else {
                        throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
                    }
                default:
                    throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
            }
        }

        /// <summary>
        /// Reads the remaining bytes from the stream
        /// </summary>
        /// <returns>The remaining bytes</returns>
        public byte[] ReadRemainingBytes()
        {
            return reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
        }

        /// <summary>
        /// Reads a string
        /// </summary>
        /// <param name="encoding">The encoding to use, or null for the default one</param>
        /// <returns>The string</returns>
        public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null) {
            string logString = LogPrefix;
            string t;
            if (length.HasValue) {
                t = reader.ReadString(length.Value, encoding: encoding);
            } else {
                t = reader.ReadNullDelimitedString(encoding: encoding);
            }
            if (Settings.Log) {
                Context.Log.Log(logString + "(string) " + (name ?? "<no name>") + ": " + t);
            }
            return t;
        }

        public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null)
        {
            if (Settings.Log)
            {
                string logString = LogPrefix;
                Context.Log.Log(logString + "(String[" + count + "]) " + (name ?? "<no name>"));
            }
            string[] buffer;
            if (obj != null) {
                buffer = obj;
                if (buffer.Length != count) {
                    Array.Resize(ref buffer, (int)count);
                }
            } else {
                buffer = new string[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeString(default, length, encoding, name: name == null ? null : name + "[" + i + "]");

            return buffer;
        }

        /// <summary>
        /// Begins calculating byte checksum for all decrypted bytes read from the stream
        /// </summary>
        /// <param name="checksumCalculator">The checksum calculator to use</param>
        public override void BeginCalculateChecksum(IChecksumCalculator checksumCalculator)
        {
            reader.BeginCalculateChecksum(checksumCalculator);
        }

        /// <summary>
        /// Ends calculating the checksum and return the value
        /// </summary>
        /// <typeparam name="T">The type of checksum value</typeparam>
        /// <returns>The checksum value</returns>
        public override T EndCalculateChecksum<T>()
        {
            return reader.EndCalculateChecksum<T>();
        }


        public override void BeginXOR(byte xorKey) {
            reader.BeginXOR(xorKey);
        }
        public override void EndXOR() {
            reader.EndXOR();
        }

        public override void DoXOR(byte xorKey, Action action)
        {
            var prevKey = reader.GetXOR();
            BeginXOR(xorKey);
            action();

            if (prevKey == null)
                EndXOR();
            else
                BeginXOR(prevKey.Value);
        }

        public override void Goto(Pointer offset) {
            if (offset == null) return;
            if (offset.file != currentFile) {
                SwitchToFile(offset.file);
            }
            reader.BaseStream.Position = offset.FileOffset;
        }

        public override T Serialize<T>(T obj, string name = null) {
            string logString = LogPrefix;

            var start = reader.BaseStream.Position;

            T t = (T)ReadAsObject<T>(name);

            currentFile.UpdateReadMap(start, reader.BaseStream.Position - start);

            if (Settings.Log) {
                Context.Log.Log(logString + "(" + typeof(T) + ") " + (name ?? "<no name>") + ": " + (t?.ToString() ?? "null"));
            }
            return t;
        }

        public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) {
            string logString = LogPrefix;

            var start = reader.BaseStream.Position;

            T checksum = (T)ReadAsObject<T>(name);

            currentFile.UpdateReadMap(start, reader.BaseStream.Position - start);

            if (!checksum.Equals(calculatedChecksum)) {
                Debug.LogWarning("Checksum " + name + " did not match!");
            }
            if (Settings.Log) {
                Context.Log.Log(logString + "(" + typeof(T) + ") " + (name ?? "<no name>") + ": " + checksum.ToString()
                    + " - Checksum to match: " + calculatedChecksum.ToString() + " - Matched? " + checksum.Equals(calculatedChecksum));
            }
            return checksum;
        }

        public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) {
            Pointer current = CurrentPointer;
            T instance = Context.Cache.FromOffset<T>(current);
            if (instance == null || currentFile is ProcessMemoryStreamFile) {
                bool newInstance = false;
                if (instance == null) {
                    newInstance = true;
                    instance = new T();
                }
                instance.Init(current);

                // Do not cache already created objects
                if (newInstance)
                    Context.Cache.Add<T>(instance);
                
                if (Settings.Log) {
                    string logString = LogPrefix;
                    Context.Log.Log(logString + "(Object: " + typeof(T) + ") " + (name ?? "<no name>"));
                }
                Depth++;
                onPreSerialize?.Invoke(instance);
                instance.Serialize(this);
                Depth--;
            } else {
                Goto(current + instance.Size);
            }
            return instance;
        }

        public override Pointer SerializePointer(Pointer obj, Pointer anchor = null, bool allowInvalid = false, string name = null) {
            string logString = LogPrefix;
            Pointer current = CurrentPointer;
            uint value = reader.ReadUInt32();
            Pointer ptr = currentFile.GetPreDefinedPointer(current.AbsoluteOffset);
            if (ptr != null) {
                ptr = ptr.SetAnchor(anchor);
            }
            if(ptr == null) ptr = currentFile.GetPointer(value, anchor: anchor);
            if (ptr == null && value != 0 && !allowInvalid && !currentFile.AllowInvalidPointer(value, anchor: anchor)) {
                if (Settings.Log) {
                    Context.Log.Log(logString + "(Pointer) " + (name ?? "<no name>") + ": InvalidPointerException - " + string.Format("{0:X8}", value));
                }
                throw new PointerException("Not a valid pointer at " + (current) + ": " + string.Format("{0:X8}", value), "SerializePointer");
            }
            if (Settings.Log) {
                Context.Log.Log(logString + "(Pointer) " + (name ?? "<no name>") + ": " + ptr?.ToString());
            }
            return ptr;
        }

        public override Pointer<T> SerializePointer<T>(Pointer<T> obj, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                string logString = LogPrefix;
                Context.Log.Log(logString + "(Pointer<T>: " + typeof(T) + ") " + (name ?? "<no name>"));
            }
            Depth++;
            Pointer<T> p = new Pointer<T>(this, anchor: anchor, resolve: resolve, onPreSerialize: onPreSerialize, allowInvalid: allowInvalid);
            Depth--;
            return p;
        }

        public override T[] SerializeArray<T>(T[] obj, long count, string name = null) {
            // Use byte reading method if requested
            if (typeof(T) == typeof(byte)) {
                currentFile.UpdateReadMap(reader.BaseStream.Position, count);
                if (Settings.Log) {
                    string normalLog = LogPrefix + "(" + typeof(T) + "[" + count + "]) " + (name ?? "<no name>") + ": ";
                    byte[] bytes = reader.ReadBytes((int)count);
                    Context.Log.Log(normalLog
                        + Util.ByteArrayToHexString(bytes, Align: 16, NewLinePrefix: new string(' ', normalLog.Length), MaxLines: 10));
                    return (T[])(object)bytes;
                } else {
                    return (T[])(object)reader.ReadBytes((int)count);
                }
            }
            if (Settings.Log) {
                string logString = LogPrefix;
                Context.Log.Log(logString + "(" + typeof(T) + "[" + count + "]) " + (name ?? "<no name>"));
            }
            T[] buffer;
            if (obj != null) {
                buffer = obj;
                if (buffer.Length != count) {
                    Array.Resize(ref buffer, (int)count);
                }
            } else {
                buffer = new T[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = Serialize<T>(buffer[i], name: name == null ? null : name + "[" + i + "]");

            return buffer;
        }

        public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null) {
            if (Settings.Log) {
                string logString = LogPrefix;
                Context.Log.Log(logString + "(Object[]: " + typeof(T) + "[" + count + "]) " + (name ?? "<no name>"));
            }
            T[] buffer;
            if (obj != null) {
                buffer = obj;
                if (buffer.Length != count) {
                    Array.Resize(ref buffer, (int)count);
                }
            } else {
                buffer = new T[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializeObject<T>(buffer[i], onPreSerialize: onPreSerialize, name: name == null ? null : name + "[" + i + "]");

            return buffer;
        }

        public override Pointer[] SerializePointerArray(Pointer[] obj, long count, Pointer anchor = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                string logString = LogPrefix;
                Context.Log.Log(logString + "(Pointer[" + count + "]) " + (name ?? "<no name>"));
            }
            Pointer[] buffer;
            if (obj != null) {
                buffer = obj;
                if (buffer.Length != count) {
                    Array.Resize(ref buffer, (int)count);
                }
            } else {
                buffer = new Pointer[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializePointer(buffer[i], anchor: anchor, allowInvalid: allowInvalid, name: name == null ? null : name + "[" + i + "]");

            return buffer;
        }

        public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                string logString = LogPrefix;
                Context.Log.Log(logString + "(Pointer<" + typeof(T) + ">[" + count + "]) " + (name ?? "<no name>"));
            }
            Pointer<T>[] buffer;
            if (obj != null) {
                buffer = obj;
                if (buffer.Length != count) {
                    Array.Resize(ref buffer, (int)count);
                }
            } else {
                buffer = new Pointer<T>[(int)count];
            }

            for (int i = 0; i < count; i++)
                // Read the value
                buffer[i] = SerializePointer<T>(buffer[i], anchor: anchor, resolve: resolve, onPreSerialize: onPreSerialize, allowInvalid: allowInvalid, name: name == null ? null : name + "[" + i + "]");

            return buffer;
        }

        public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) {
            //U Size = (U)Convert.ChangeType((obj?.Length) ?? 0, typeof(U));
            U Size = default; // For performance reasons, don't supply this argument
            Size = Serialize<U>(Size, name: name + ".Length");
            // Convert size to int, slow
            int intSize = (int)Convert.ChangeType(Size, typeof(int));
            if (obj == null) {
                obj = new T[intSize];
            } else if (obj.Length != intSize) {
                Array.Resize(ref obj, intSize);
            }
            return obj;
        }

        public override void SerializeBitValues<T>(Action<SerializeBits> serializeFunc) {
            string logPrefix = LogPrefix;
            // Convert to int so we can work with it
            var valueInt = Convert.ToInt32(Serialize<T>(default, name: "Value"));

            // Extract bits
            int pos = 0;
            serializeFunc((v, length, name) => {
                var bitValue = BitHelpers.ExtractBits(valueInt, length, pos);

                if (Settings.Log) {
                    Context.Log.Log(logPrefix + $"  ({typeof(T)}) {name ?? "<no name>"}: {bitValue}");
                }

                pos += length;
                return bitValue;
            });
        }

        public void Dispose() {
            foreach (KeyValuePair<BinaryFile, Reader> r in readers) {
                r.Key.EndRead(r.Value);
            }
            readers.Clear();
            reader = null;
        }

        public void DisposeFile(BinaryFile file) {
            if (readers.ContainsKey(file)) {
                Reader r = readers[file];
                file.EndRead(r);
                readers.Remove(file);
            }
        }
        public override void DoEncoded(IStreamEncoder encoder, Action action, BinaryFile.Endian? endianness = null) {
            // Stream key
            string key = CurrentPointer.ToString() + "_decoded";
            // Decode the data into a stream
            using (var memStream = encoder.DecodeStream(reader.BaseStream)) {

                // Add the stream
                StreamFile sf = new StreamFile(key, memStream, Context)
                {
                    Endianness = endianness ?? currentFile.Endianness
                };
                Context.AddFile(sf);

                DoAt(sf.StartPointer, () => {
                    action();
                    if (CurrentPointer != sf.StartPointer + sf.length) {
                        UnityEngine.Debug.LogWarning($"Encoded block {key} was not fully deserialized: Serialized size: {CurrentPointer - sf.StartPointer} != Total size: {sf.length}");
                    }
                });

                Context.RemoveFile(sf);

            }
        }
        public override void DoEndian(BinaryFile.Endian endianness, Action action) {
            Reader r = reader;
            bool isLittleEndian = r.isLittleEndian;
            if (isLittleEndian != (endianness == BinaryFile.Endian.Little)) {
                r.isLittleEndian = (endianness == BinaryFile.Endian.Little);
                action();
                r.isLittleEndian = isLittleEndian;
            } else {
                action();
            }
        }

        public override void Log(string logString) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + logString);
            }
		}

        public override async UniTask FillCacheForRead(int length)
        {
            if (reader.BaseStream is PartialHttpStream httpStream)
                await httpStream.FillCacheForRead(length);
        }
	}
}