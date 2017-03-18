namespace UnityTestTools.IntegrationTestsFramework.TestRunner
{

    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    using UnityTestTools.Common;

    public class DTOFormatter {
    
        private interface ITransferInterface
        {
            void Transfer(ref ResultDTO.MessageType val);
            void Transfer(ref TestResultState val);
            void Transfer(ref byte val);
            void Transfer(ref bool val);
            void Transfer(ref int val);
            void Transfer(ref float val);
            void Transfer(ref double val);
            void Transfer(ref string val);
        }
        
        private class Writer : ITransferInterface
        {
            private readonly Stream _stream;
            public Writer(Stream stream) { this._stream = stream; }
        
            private void WriteConvertedNumber(byte[] bytes)
            {
                if(BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                this._stream.Write(bytes, 0, bytes.Length);
            }
        
            public void Transfer(ref ResultDTO.MessageType val) { this._stream.WriteByte((byte)val); }
            public void Transfer(ref TestResultState val) { this._stream.WriteByte((byte)val); }
            public void Transfer(ref byte val) { this._stream.WriteByte(val); }
            public void Transfer(ref bool val) { this._stream.WriteByte((byte)(val ? 0x01 : 0x00)); }
            public void Transfer(ref int val) { this.WriteConvertedNumber(BitConverter.GetBytes(val)); }
            public void Transfer(ref float val) { this.WriteConvertedNumber(BitConverter.GetBytes(val)); }
            public void Transfer(ref double val) { this.WriteConvertedNumber(BitConverter.GetBytes(val)); }
            
            public void Transfer(ref string val) 
            {
                var bytes = Encoding.BigEndianUnicode.GetBytes(val);
                int length = bytes.Length;
                this.Transfer(ref length);
                this._stream.Write(bytes, 0, bytes.Length);
            }
        }
    
        private class Reader : ITransferInterface
        {
            private readonly Stream _stream;
            public Reader(Stream stream) { this._stream = stream; }
            
            private byte[] ReadConvertedNumber(int size)
            {
                byte[] buffer = new byte[size];
                this._stream.Read (buffer, 0, buffer.Length);
                if(BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                return buffer;
            }
            
            public void Transfer(ref ResultDTO.MessageType val) { val = (ResultDTO.MessageType)this._stream.ReadByte(); }
            public void Transfer(ref TestResultState val) { val = (TestResultState)this._stream.ReadByte(); }
            public void Transfer(ref byte val) { val = (byte)this._stream.ReadByte(); }
            public void Transfer(ref bool val) { val = (this._stream.ReadByte() != 0); }
            public void Transfer(ref int val) { val = BitConverter.ToInt32(this.ReadConvertedNumber(4), 0); }
            public void Transfer(ref float val) { val = BitConverter.ToSingle(this.ReadConvertedNumber(4), 0); }
            public void Transfer(ref double val) { val = BitConverter.ToDouble(this.ReadConvertedNumber(8), 0); }
            
            public void Transfer(ref string val) 
            {
                int length = 0;
                this.Transfer (ref length);
                var bytes = new byte[length];
                int remain = length;
                int index = 0;
                do {
                    int bytesRead = this._stream.Read(bytes, index, remain);
                    remain -= bytesRead;
                    index += bytesRead;
                } while (remain > 0);
#if !UNITY_WSA
                val = Encoding.BigEndianUnicode.GetString(bytes);
#endif
            }
        }
        
        private void Transfer(ResultDTO dto, ITransferInterface transfer)
        {
            transfer.Transfer(ref dto.messageType);
            
            transfer.Transfer(ref dto.levelCount);
            transfer.Transfer(ref dto.loadedLevel);
            transfer.Transfer(ref dto.loadedLevelName);
            
            if(dto.messageType == ResultDTO.MessageType.Ping
               || dto.messageType == ResultDTO.MessageType.RunStarted
               || dto.messageType == ResultDTO.MessageType.RunFinished
               || dto.messageType == ResultDTO.MessageType.RunInterrupted
               || dto.messageType == ResultDTO.MessageType.AllScenesFinished)
                return;
                
            transfer.Transfer(ref dto.testName);
            transfer.Transfer(ref dto.testTimeout);
            
            if(dto.messageType == ResultDTO.MessageType.TestStarted)
                return;
            
            if(transfer is Reader)
                dto.testResult = new SerializableTestResult();
            SerializableTestResult str = (SerializableTestResult)dto.testResult;
            
            transfer.Transfer(ref str.resultState);
            transfer.Transfer(ref str.message);
            transfer.Transfer(ref str.executed);
            transfer.Transfer(ref str.name);
            transfer.Transfer(ref str.fullName);
            transfer.Transfer(ref str.id);
            transfer.Transfer(ref str.isSuccess);
            transfer.Transfer(ref str.duration);
            transfer.Transfer(ref str.stackTrace);
        }
    
        public void Serialize (Stream stream, ResultDTO dto)
        {
            this.Transfer(dto, new Writer(stream));
        }
        
        public object Deserialize (Stream stream)
        {
#if !UNITY_WSA
            var result = (ResultDTO)FormatterServices.GetSafeUninitializedObject(typeof(ResultDTO));
            this.Transfer (result, new Reader(stream));
            return result;
#else
            return null;
#endif
        }
    }

}