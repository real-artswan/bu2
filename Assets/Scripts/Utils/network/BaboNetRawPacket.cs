using System;
using System.Runtime.InteropServices;

namespace BaboNetwork
{
    public class BaboRawPacket
    {
        public static ushort HEADER_SIZE = 4;

        public byte[] header = new byte[4]; //(UInt16 Size, UInt16 TypeID)
        public byte[] data = null;     //un array dynamique qui tien le data a envoyer

        public UInt16 dataSize
        {
            get { return BitConverter.ToUInt16(header, 0); }
        }

        public UInt16 typeID
        {
            get { return BitConverter.ToUInt16(header, 2); }
        }

        public object packetToStruct() {
            var type = Type.GetType(typeof(BaboPacketTypeID).Namespace + "." + ((BaboPacketTypeID)typeID).ToString().ToLower(),
                throwOnError: false);

            if (type == null) {
                return null;
                //throw new InvalidOperationException(dtoSelection.ToString() + " is not a known dto type");
            }
            object obj;
            //int size = Marshal.SizeOf(type);
            int size = dataSize;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try {
                Marshal.Copy(data, 0, ptr, size);

                obj = Marshal.PtrToStructure(ptr, type);
            }
            finally {
                Marshal.FreeHGlobal(ptr);
            }

            return obj;
        }

        public BaboRawPacket() { }

        public BaboRawPacket(object dataStruct) {
            string typeIDName = dataStruct.GetType().Name.ToUpper();

            BaboPacketTypeID id = (BaboPacketTypeID)Enum.Parse(typeof(BaboPacketTypeID), typeIDName);

            UInt16 size = (UInt16)Marshal.SizeOf(dataStruct);
            data = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try {
                Marshal.StructureToPtr(dataStruct, ptr, true);
                Marshal.Copy(ptr, data, 0, size);
            }
            finally {
                Marshal.FreeHGlobal(ptr);
            }
            //fill the header
            Array.Copy(BitConverter.GetBytes(size), header, 2);
            Array.Copy(BitConverter.GetBytes((UInt16)id), 0, header, 2, 2);
            /*header[0] = (byte)(size >> 8);
            header[1] = (byte)(size << 8);
            UInt16 typeIDValue = (UInt16)id;
            header[2] = (byte)(typeIDValue >> 8);
            header[3] = (byte)(typeIDValue << 8);*/
        }
    }
}
