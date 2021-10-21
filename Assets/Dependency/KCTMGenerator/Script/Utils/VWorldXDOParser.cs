using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ARRC_DigitalTwin_Generator
{
    public static class VWorldXDOParsor
    {
        public static void writeNailData_fake(FileStream bis, string imageName, int nailSize)
        {
            byte[] b = new byte[nailSize];
            int readByteNo = bis.Read(b, 0, nailSize);
            return;
        }

        //xdo 에 기본적으로 포함된 최하위 해상도 텍스쳐 파일을 꺼낸다.
        public static void writeNailData(FileStream bis, string fileName, int nailSize)
        {
            //byte[] b = new byte[nailSize];
            //int readByteNo = bis.read(b);

            ////BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(new File(storageFolder + "xdo_Files\\" + fileName)));
            ////BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(new File(storageFolder + "xdo_Files\\" + fileName)));
            //FileStream bos;
            //bos.write(b);
            //bos.close();
            return;
        }


        //바이너리 파일 파싱
        public static string pVersion(FileStream bis)
        {
            byte[] b = new byte[1];
            int readByteNo = bis.Read(b, 0, 1);

            return null;
        }

        //바이너리 파일 파싱
        public static float pFloat(FileStream bis)
        {

            byte[] b = new byte[4];
            int readByteNo = bis.Read(b, 0, 4);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return BitConverter.ToSingle(b, 0);

        }

        //바이너리 파일 파싱
        public static double pDouble(FileStream bis)
        {

            byte[] b = new byte[8];
            int readByteNo = bis.Read(b, 0, 8);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return BitConverter.ToDouble(b, 0);

        }

        //바이너리 파일 파싱
        public static string pChar(FileStream bis, int r_keylen)
        {
            byte[] b = new byte[r_keylen];
            int readByteNo = bis.Read(b, 0, r_keylen);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);

            return Encoding.Default.GetString(b);

        }

        //바이너리 파일 파싱
        public static int pU8(FileStream bis)
        {

            byte[] b = new byte[1];
            int readByteNo = bis.Read(b, 0, 1);
            int number = b[0];
            return number;

        }

        //바이너리 파일 파싱
        public static short pU16(FileStream bis)
        {

            byte[] b = new byte[2];
            int readByteNo = bis.Read(b, 0, 2);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return BitConverter.ToInt16(b, 0);

        }

        //바이너리 파일 파싱
        public static int pU32(FileStream bis)
        {
            byte[] b = new byte[4];

            int readByteNo = bis.Read(b, 0, 4);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);

            int i32 = BitConverter.ToInt32(b, 0);  // 내 시스템에서는 reverse하지 않아야 된다?
            return i32;
        }
    }
}
