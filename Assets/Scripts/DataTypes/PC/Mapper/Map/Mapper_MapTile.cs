﻿namespace R1Engine
{
    /// <summary>
    /// Map tile data for the Mapper
    /// </summary>
    public class Mapper_MapTile : R1Serializable
    {
        /// <summary>
        /// The tile texture index
        /// </summary>
        public ushort TileIndex { get; set; }

        /// <summary>
        /// The tile collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            TileIndex = s.Serialize(TileIndex, name: "TileIndex");

            CollisionType = (TileCollisionType)s.Serialize((ushort)CollisionType, name: "CollisionType");
        }
    }
}