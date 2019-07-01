using UnityEngine;

public class Morton
{

    public ulong mortonCode;

    public Morton()
    {
        mortonCode = 1;
    }

    public Morton(ulong index)
    {
        mortonCode = index;
    }

    public static ulong Insert(ulong mortonCode, ulong subIndex, int k)
    {
        return mortonCode |= subIndex << (63 - k * 3);
    }

    public void EncodePos(Vector3Int pos)
    {
        mortonCode = (ulong)1 << 63;
        mortonCode |= Encode(pos.z) | Encode(pos.x) << 1 | Encode(pos.y) << 2;
    }

    ulong Encode(int a)
    {
        ulong x = (ulong)a & 0x1fffff; // we only look at the first 21 bits
        x = (x | x << 32) & 0x1f00000000ffff; // shift left 32 bits, OR with self, and 00011111000000000000000000000000000000001111111111111111
        x = (x | x << 16) & 0x1f0000ff0000ff; // shift left 32 bits, OR with self, and 00011111000000000000000011111111000000000000000011111111
        x = (x | x << 8) & 0x100f00f00f00f00f; // shift left 32 bits, OR with self, and 0001000000001111000000001111000000001111000000001111000000000000
        x = (x | x << 4) & 0x10c30c30c30c30c3; // shift left 32 bits, OR with self, and 0001000011000011000011000011000011000011000011000011000100000000
        x = (x | x << 2) & 0x1249249249249249;
        return x;
    }

    public Vector3Int DecodeIndex()
    {
        int x = Decode(mortonCode >> 1);
        int y = Decode(mortonCode >> 2);
        int z = Decode(mortonCode >> 0);
        return new Vector3Int(x, y, z);
    }

    int Decode(ulong a)
    {
        ulong x = a & 0x1249249249249249; // we only look at the first 21 bits
        x = (x | x >> 2) & 0x10c30c30c30c30c3;
        x = (x | x >> 4) & 0x100f00f00f00f00f; // shift left 32 bits, OR with self, and 0001000011000011000011000011000011000011000011000011000100000000
        x = (x | x >> 8) & 0x1f0000ff0000ff; // shift left 32 bits, OR with self, and 0001000000001111000000001111000000001111000000001111000000000000
        x = (x | x >> 16) & 0x1f00000000ffff; // shift left 32 bits, OR with self, and 00011111000000000000000011111111000000000000000011111111
        x = (x | x >> 32) & 0x1fffff; // shift left 32 bits, OR with self, and 00011111000000000000000000000000000000001111111111111111

        return (int)x;
    }

    public ulong GetChildIndex(int k)
    {

        ulong mask = GetMask(k);
        ulong mortonChild = mortonCode & mask;
        return mortonChild >>= 63 - k * 3;
    }

    ulong GetMask(int k)
    {
        ulong mask = 0;
        for (int i = 1; i < 64; i++)
        {
            if (i >= k * 3 - 2 && i <= k * 3)
            {
                mask |= ((ulong)1 << (63 - i));
            }
        }
        return mask;
    }

    public static Vector3Int WorldToMortonIntPos(Vector3 worldPos, OctreeTransform nodeTransform)
    {
        return WorldToMortonIntPos(worldPos, nodeTransform, 21);
    }

    public static Vector3Int WorldToMortonIntPos(Vector3 worldPos, OctreeTransform nodeTransform, int depth)
    {
        int maxCoord = (int) Mathf.Pow(2, depth)-1;
        return new Vector3Int(
            (int)Mathf.Lerp(0, maxCoord, Mathf.InverseLerp(-nodeTransform.worldScale.x / 2, nodeTransform.worldScale.x / 2, worldPos.x)),
            (int)Mathf.Lerp(0, maxCoord, Mathf.InverseLerp(-nodeTransform.worldScale.y / 2, nodeTransform.worldScale.y / 2, worldPos.y)),
            (int)Mathf.Lerp(0, maxCoord, Mathf.InverseLerp(-nodeTransform.worldScale.z / 2, nodeTransform.worldScale.z / 2, worldPos.z))
        );
    }

    public static Morton MortonPosToCode(Vector3Int pos)
    {
        Morton morton = new Morton();
        morton.EncodePos(pos);
        return morton;
    }
}
