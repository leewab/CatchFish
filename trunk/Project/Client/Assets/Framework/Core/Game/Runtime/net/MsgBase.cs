using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Utils
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgbaseWriter")]
    public class MsgbaseWriter : msg_base
    {
        private int MsgId { get; set; }
        public MsgbaseWriter(int msgid)
        {
            MsgId = msgid;
        }
        public override int _msgid() { return MsgId; }
        public void WriteString(int key, string value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteInt(int key, int value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteLong(int key, long value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteFloat(int key, float value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteDouble(int key, double value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteBool(int key, bool value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteObject(int key,MsgbaseWriter obj)
        {
            ProtoBuf.Extensible.AppendValue(this, key, obj);
        }

        //array
        public void WriteStringArray(int key, string[] value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteIntArray(int key, int[] value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteLongArray(int key, long[] value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteFloatArray(int key, float[] value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteDoubleArray(int key, double[] value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteBoolArray(int key, bool[] value)
        {
            ProtoBuf.Extensible.AppendValue(this, key, value);
        }
        public void WriteObjectArray(int key, MsgbaseWriter[] obj)
        {
            ProtoBuf.Extensible.AppendValue(this, key, obj);
        }
    }
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgbaseReader")]
    public class MsgbaseReader : msg_base
    {
        public MsgbaseReader()
        {
        }
        public string ReadString(int key)
        {
            return read<string>(key);
        }
        public string[] ReadStringArray(int key)
        {
            return read<string[]>(key);
        }
        public int ReadInt(int key)
        {
            return read<int>(key);
        }
        public int[] ReadIntArray(int key)
        {
            return read<int[]>(key);
        }
        public long ReadLong(int key)
        {
            return read<long>(key);
        }
        public long[] ReadLongArray(int key)
        {
            return read<long[]>(key);
        }
        public float ReadFloat(int key)
        {
            return read<float>(key);
        }
        public float[] ReadFloatArray(int key)
        {
            return read<float[]>(key);
        }
        public double ReadDouble(int key)
        {
            return read<double>(key);
        }
        public double[] ReadDoubleArray(int key)
        {
            return read<double[]>(key);
        }
        public bool ReadBool(int key)
        {
            return read<bool>(key);
        }
        public bool[] ReadBoolArray(int key)
        {
            return read<bool[]>(key);
        }
        public MsgbaseReader ReadObject(int key)
        {
            return read<MsgbaseReader>(key);
        }
        public MsgbaseReader[] ReadObjectArray(int key)
        {
            return read<MsgbaseReader[]>(key);
        }
        private T read<T>(int key)
        {
            T value = default(T);
            ProtoBuf.Extensible.TryGetValue(this, key, out value);
            return value;
        }

/*
        private void testMsgbase()
        {
            var w = new Utils.MsgbaseWriter(0);
            w.WriteBool(1, true);
            w.WriteBoolArray(2, new bool[] { false, true });
            w.WriteDouble(3, 1.01);
            w.WriteDoubleArray(4, new double[] { 1.02, 1.03 });
            w.WriteFloat(5, 2.01f);
            w.WriteFloatArray(6, new float[] { 2.02f, 2.03f });
            w.WriteInt(7, 301);
            w.WriteIntArray(8, new int[] { 302, 203 });
            w.WriteLong(9, 99999999991);
            w.WriteLongArray(10, new long[] { 99999999992, 99999999992 });
            w.WriteString(11, "Hello World1");
            w.WriteStringArray(12, new string[] { "hw2", "hw3" });
            var w2 = new Utils.MsgbaseWriter(0);
            w2.WriteString(1, "w2");
            w2.WriteInt(2, 2001);
            w.WriteObject(13, w2);
            w.WriteObjectArray(14, new Utils.MsgbaseWriter[] { w2, w2 });

            Utils.msg_base msg = w;
            System.IO.Stream s = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize(s, msg);
            s.Seek(0, System.IO.SeekOrigin.Begin);

            var r = ProtoBuf.Serializer.Deserialize<Utils.MsgbaseReader>(s);
            var r1 = r.ReadBool(1);
            var r2 = r.ReadBoolArray(2);
            var r3 = r.ReadDouble(3);
            var r4 = r.ReadDoubleArray(4);
            var r5 = r.ReadFloat(5);
            var r6 = r.ReadFloatArray(6);
            var r7 = r.ReadInt(7);
            var r8 = r.ReadIntArray(8);
            var r9 = r.ReadLong(9);
            var r10 = r.ReadLongArray(10);
            var r11 = r.ReadString(11);
            var r12 = r.ReadStringArray(12);
            var r13 = r.ReadObject(13);
            var r13_1 = r13.ReadString(1);
            var r13_2 = r13.ReadInt(2);
            var r14 = r.ReadObjectArray(14);
            var r14_1 = r14[1].ReadString(1);
            var r14_2 = r14[1].ReadInt(2);
        }
 * */
    }
}
