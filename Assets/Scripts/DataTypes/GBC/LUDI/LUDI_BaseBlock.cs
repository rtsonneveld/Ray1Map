﻿namespace R1Engine
{
    public abstract class LUDI_BaseBlock : R1Serializable 
    {
        public LUDI_BlockIdentifier LUDI_Header { get; set; }

        public Pointer BlockStartPointer {
            get {
                return Offset + 4;
            }
        }
        private uint? _cachedBlockLength { get; set; }
        public uint BlockSize {
            get {
                if (!_cachedBlockLength.HasValue) {
                    var offTable = Context.GetStoredObject<LUDI_GlobalOffsetTable>(GBC_BaseManager.GlobalOffsetTableKey);
                    uint? size = offTable?.GetBlockLength(LUDI_Header);
                    if (size.HasValue) {
                        _cachedBlockLength = size.Value - 4;
                    } else {
                        _cachedBlockLength = 0;
                    }
                }
                return _cachedBlockLength.Value;
            }
        }

        public override void SerializeImpl(SerializerObject s) 
        {
            LUDI_Header = s.SerializeObject<LUDI_BlockIdentifier>(LUDI_Header, name: nameof(LUDI_Header));

            s.Goto(BlockStartPointer);
            SerializeBlock(s);

            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm) {
                s.Align(baseOffset: BlockStartPointer);
            }
            CheckBlockSize(s);
        }

        public abstract void SerializeBlock(SerializerObject s);

        private void CheckBlockSize(SerializerObject s) {
             if (BlockStartPointer + BlockSize != s.CurrentPointer) {
                UnityEngine.Debug.LogWarning($"{GetType()} @ {Offset}: Serialized size: {(s.CurrentPointer - BlockStartPointer)} != BlockSize: {BlockSize}");
            }
        }
    }
}