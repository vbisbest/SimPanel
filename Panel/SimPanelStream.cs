using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace Panel
{
    public class SimPanelStream
    {
        public delegate void SimDataEventHandler(SimData data);
        public event SimDataEventHandler OnSimData;

        //private Stream stream;
        TcpClient clientSocket = new TcpClient();
        NetworkStream stream = default(NetworkStream);


        private int timeout = 0;
        Stopwatch sw = new Stopwatch();

        public SimPanelStream()
        {
        }

        public SimPanelStream(int timeout)
        {
            //tream = new MemoryStream();
            this.timeout = timeout;
        }

        public async void StartSocket()
        {
            await clientSocket.ConnectAsync("192.168.1.105", 8888);
            this.stream = clientSocket.GetStream();

            while (true)
            {
                if (stream.DataAvailable)
                {
                    var data = ReadString();

                    SimData simData = Newtonsoft.Json.JsonConvert.DeserializeObject<SimData>(data);
                    simData.Heading = simData.Heading * -1;
                    simData.AttitudePitch = simData.AttitudePitch * -1;
                    
                    OnSimData(simData);
                }
            }
        }

        public double ReadDouble()
        {
            byte[] buffer = new byte[8];
            this.Read(buffer, 0, buffer.Length);
            return BitConverter.ToDouble(buffer, 0);
        }

        public float ReadFloat()
        {
            byte[] buffer = new byte[4];
            this.Read(buffer, 0, buffer.Length);
            return BitConverter.ToSingle(buffer, 0);
        }

        public short ReadShort()
        {
            byte[] buffer = new byte[2];
            this.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt16(buffer, 0);
        }

        public byte[] ReadByteArray(int length)
        {
            byte[] buffer = new byte[length];
            if (length == 0)
                return buffer;
            this.Read(buffer, 0, length);
            return buffer;
        }

        public byte ReadByte()
        {
            byte[] buffer = new byte[1];
            this.Read(buffer, 0, 1);
            return buffer[0];
        }

        public int Read(byte[] buffer, int offset, int length)
        {
            int count = length;

            sw.Reset();
            sw.Start();

            try
            {
                while (count != 0)
                {
                    count -= stream.Read(buffer, length - count, count);
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                throw new Exception("Read failed");
            }

            sw.Stop();
            return length;
        }

        public string ReadString()
        {
            byte[] numArray = new byte[this.ReadShort()];

            this.Read(numArray, 0, numArray.Length);
            return Encoding.UTF8.GetString(numArray, 0, numArray.Length);
        }

        public string ReadString(int length)
        {
            byte[] numArray = new byte[length];

            this.Read(numArray, 0, numArray.Length);
            return Encoding.UTF8.GetString(numArray, 0, numArray.Length);
        }

        public string ReadUUID()
        {
            byte[] buffer = new byte[16];
            this.Read(buffer, 0, buffer.Length);
            Array.Reverse((Array)buffer);
            return new Guid(buffer).ToString();
        }

        public int ReadInt()
        {
            byte[] buffer = new byte[4];
            this.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        public uint ReadUnsignedInt()
        {
            byte[] buffer = new byte[4];
            this.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public long ReadLong()
        {
            byte[] buffer = new byte[8];
            this.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt64(buffer, 0);
        }

        public sbyte ReadUnsignedByte()
        {
            return (sbyte)this.ReadByte();
        }

        public long ReadUnsignedByteArray(NetworkStream stream)
        {
            return 0L;
        }

        public bool ReadBool()
        {
            return (int)this.ReadByte() == (int)Convert.ToByte(1);
        }

        public ushort ReadUnsingedShort()
        {
            byte[] buffer = new byte[2];
            this.Read(buffer, 0, buffer.Length);
            //Array.Reverse((Array)buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public void ReadArrayOfInt()
        {
            byte num = this.ReadByte();
            for (int index = 0; index < (int)num; ++index)
                this.ReadInt();
        }
    }

    public class SimData
    {
        public double Altitude;
        public double Heading;
        public double AttitudeBank;
        public double AttitudePitch;
        public double Airspeed;
    }

}
