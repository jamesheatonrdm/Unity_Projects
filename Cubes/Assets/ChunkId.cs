using System;
using System.Diagnostics;
using UnityEngine;

public struct ChunkId : IEquatable<ChunkId>
{
    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public ChunkId(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static ChunkId FromWorldPos(float x, float y, float z)
    {
        UnityEngine.Debug.Log("Int val: " + (int)(x / 4));
        return new ChunkId((int)(x / 4), (int)(y / 4), (int)z / 4);
    }

    #region Equality members

    public bool Equals(ChunkId other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is ChunkId other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = X;
            hashCode = (hashCode * 397) ^ Y;
            hashCode = (hashCode * 397) ^ Z;
            return hashCode;
        }
    }

    public static bool operator ==(ChunkId left, ChunkId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ChunkId left, ChunkId right)
    {
        return !left.Equals(right);
    }

    #endregion
}
