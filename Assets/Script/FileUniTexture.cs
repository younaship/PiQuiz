using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileUniTexture {

    public static Texture PNGToUniTex(string path)
    {
        Debug.Log("PNG to UniTex. " + path);
        byte[] readBinary = ReadPngFile(path);
        int x, y;
        GetSizePNG(readBinary, out x, out y);

        Texture2D texture = new Texture2D(x, y);
        texture.LoadImage(readBinary);

        return texture;
    }

    private static byte[] ReadPngFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    private static void GetSizePNG(byte[] binary, out int width, out int height)
    {
        int p = 16; // PNG Config Start 16Byte ~

        width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + binary[p++];
        }

        height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + binary[p++];
        }
    }
}
