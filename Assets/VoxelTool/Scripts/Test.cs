using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    ulong morton = 1;
    public Vector3Int pos;
    public int k;
    public int insertSubindex;
    public int insertK;
    //uint x, y, z;

    public void Insert()
    {
        insertSubindex = 0b101;
        morton |= (ulong)insertSubindex << (63 - insertK *3);
        print("MORTON (inserted): " + System.Convert.ToString((long)morton, 2));
    }

    public void EncodePos()
    {
        morton = (ulong)1 << 63;
        morton |= Encode(pos.z) | Encode(pos.x) << 1 | Encode(pos.y) << 2;
        print("MORTON: " + System.Convert.ToString((long)morton, 2));
    }

    public ulong Encode(int a)
    {
        ulong x = (ulong)a & 0x1fffff; // we only look at the first 21 bits
        x = (x | x << 32) & 0x1f00000000ffff; // shift left 32 bits, OR with self, and 00011111000000000000000000000000000000001111111111111111
        x = (x | x << 16) & 0x1f0000ff0000ff; // shift left 32 bits, OR with self, and 00011111000000000000000011111111000000000000000011111111
        x = (x | x << 8) & 0x100f00f00f00f00f; // shift left 32 bits, OR with self, and 0001000000001111000000001111000000001111000000001111000000000000
        x = (x | x << 4) & 0x10c30c30c30c30c3; // shift left 32 bits, OR with self, and 0001000011000011000011000011000011000011000011000011000100000000
        x = (x | x << 2) & 0x1249249249249249;
        return x;
    }

    public void DecodeIndex()
    {
        int x = Decode(morton >> 1);
        int y = Decode(morton >> 2);
        int z = Decode(morton >> 0);
        print("Vector: " + new Vector3Int(x, y, z));
    }

    public int Decode(ulong a)
    {
        ulong x = a & 0x1249249249249249; // we only look at the first 21 bits
        x = (x | x >> 2) & 0x10c30c30c30c30c3;
        x = (x | x >> 4) & 0x100f00f00f00f00f; // shift left 32 bits, OR with self, and 0001000011000011000011000011000011000011000011000011000100000000
        x = (x | x >> 8) & 0x1f0000ff0000ff; // shift left 32 bits, OR with self, and 0001000000001111000000001111000000001111000000001111000000000000
        x = (x | x >> 16) & 0x1f00000000ffff; // shift left 32 bits, OR with self, and 00011111000000000000000011111111000000000000000011111111
        x = (x | x >> 32) & 0x1fffff; // shift left 32 bits, OR with self, and 00011111000000000000000000000000000000001111111111111111
        
        return (int)x;
    }

    public void GetChildIndex()
    {
        
        ulong mask = GetMask(k);
        ulong mortonChild = morton & mask;
        mortonChild >>= 63 - k*3;
        print("MortonChild: " + System.Convert.ToString((long) mortonChild, 2));
    }

    ulong GetMask(int k)
    {
        ulong mask = 0;
        for (int i = 1; i < 64; i++)
        {
            if(i >= k*3-2 && i <= k * 3)
            {
                mask |= ((ulong)1 << (63 - i));
            }
        }
        print("Mask: " + System.Convert.ToString((long)mask, 2));
        return mask;
    }
}
