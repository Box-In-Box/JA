using ExternPropertyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureConversion : MonoBehaviour
{
    public (int, bool[]) GetProgress(int value)
    {
        return BinaryCheck(IntToBinaryString(value));
    }

    public string IntToBinaryString(int value)
    {
        return string.Format("{0:D5}", IntToBinary(value));
    }

    public int IntToBinary(int value)
    {
        return int.Parse(Convert.ToString(value, 2));
    }

    public int StringToBinary(string value)
    {
        return Convert.ToInt32(value, 2);
    }

    public string UpdateBinary(string value, int index)
    {
        char[] binary = value.ToCharArray();
        binary[index] = '1';
        return string.Concat(binary);
    }

    public (int, bool[]) BinaryCheck(string value)
    {
        int count = 0;
        bool[] bools = new bool[value.Length];
        
        for (int i = 0; i < value.Length; ++i)
        {
            bools[i] = value[i] == '1' ? true : false;
            count = value[i] == '1' ? ++count : count;
        }
        return (count, bools);
    }

    public string GetBinary(bool[] value)
    {
        string binaryString = "";

        for (int i = 0; i < value.Length; ++i)
        {
            binaryString += value[i] == true ? "1" : "0";
        }
        return binaryString;
    }
}