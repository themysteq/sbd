
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace SBD_siszarp
{
    class ReadBuffer
    {
        private String _filename;
        private FileStream _file_stream;
        private byte[] _bytes; 
        private int _elements;
        private bool _end_of_file;
      //  private int[][] _internal_buffer ;
        private int _pointer;
        private int _reads;
        private string p;
        
        public ReadBuffer(String m_filename)
        {
            this._filename = m_filename;
            this._file_stream = new FileStream(_filename, FileMode.OpenOrCreate);
            this._pointer = 0;
            this._end_of_file = false;
            this._elements = 0;
         //   this._internal_buffer = new int[20][];
            this._bytes = new byte[sizeof(char)*10*20];
            this._reads = 0;
        }
        public void Close()
        {
            this._file_stream.Close();
            this._file_stream.Dispose();
            this._file_stream = null;
        }
        private int loadToBuffer()
        {
            
            int n = _file_stream.Read(_bytes,0,sizeof(char)*10*20);
            this._reads++;
            _pointer = 0;
             return n;
        }
        public char[] getNextRecord()
        {
            int n = 0;

            if (_pointer == 0 && _end_of_file)
            {
                this._file_stream.Close();
                this._file_stream.Dispose();
                return null;

            } if (_pointer == 0 && !_end_of_file)
            {
                n = loadToBuffer();
                _elements = n / (10 * sizeof(char)); // liczba załadowanych rekordów

                if (_elements < 20)
                    _end_of_file = true;

                    
            }

            if (_pointer == 20) //musimy załadować nowy szajs
            {
                n = loadToBuffer();
                _elements = n / (10 * sizeof(char)); // liczba załadowanych rekordów
                if (n == 0)
                {
                    this._end_of_file = true;
                    this._file_stream.Close();
                    this._file_stream.Dispose();
                    return null;
                }

            }
            if (_pointer == _elements)
                return null;

            char[] output = new char[10];
            for (int i = 0; i < 10; i++)
            {
                output[i] = System.BitConverter.ToChar(_bytes, (_pointer*10+i)* sizeof(char));
            }
            _pointer++;
            return output;

        }
        public int getReads()
        {
            return this._reads;
        }
    }

}
