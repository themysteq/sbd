using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace SBD_siszarp
{
    class WriteBuffer
    {
        private String _filename;
        private FileStream _file_stream;
        private byte[] _bytes;
        private int _elements;
        private char[][] _internal_buffer;
        private int _pointer;
        private int _writes;

       public WriteBuffer(String m_filename)
        {
            this._filename = m_filename;
            this._file_stream = new FileStream(_filename,FileMode.Create);
            this._pointer = 0;
            this._internal_buffer = new char[20][];
            this._bytes = new byte[sizeof(char) * 10 * 20];
            this._writes = 0;
        }
       public void Close()
       {
           flushBuffer();
           this._file_stream.Close();
           this._file_stream.Dispose();
           this._file_stream = null;
          // this._file_stream.Close();
       }
        private void flushBuffer()
        {
            if (_pointer > 0)
            {
                this._file_stream.Write(_bytes, 0, sizeof(char) * 10 * _pointer);
                this._writes++;
                Array.Clear(_bytes, 0, sizeof(char) * 10 * 20);
                _pointer = 0;
            }
        }
        public void writeRecord(char[] _record)
        {
            if (_pointer == 20)
            {
                flushBuffer();
            }
            System.Buffer.BlockCopy(_record, 0, _bytes, sizeof(char) * (_pointer * 10), sizeof(char) * 10);
            _pointer++;
            
        }
        public int getWrites()
        {
            return this._writes;
        }
        public void Open()
        {
            this._file_stream = new FileStream(this._filename,FileMode.Create);
        }
    }
}
