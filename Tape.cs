using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SBD_siszarp
{
    class Tape
    {
        private WriteBuffer _write_buff;
        private ReadBuffer _read_buff;
        private int _meta_last;
        private bool _first_read = true;
        private int _series_counter = 0;
        private bool _end_of_serie = false;
        private bool _end_of_tape = false;
        private int _series = 0;

        public Tape(WriteBuffer m_write_buff)
        {
            this._write_buff = m_write_buff;
            this._read_buff = null;
            /*
            this._first_read = true;
            _series_counter = 0;
            _end_of_serie = false;
            _end_of_tape = false;
             * */
        }
        public Tape(ReadBuffer m_read_buff)
        {
            this._read_buff = m_read_buff;
            this._write_buff = null;
            /*
            this._first_read = true;
            _series_counter = 0;
            this._end_of_serie = false;
            _end_of_tape = false;
             */
        }
        public Tape(String m_filename,bool read_only = true)
        {
            if (read_only)
            {
                this._read_buff = new ReadBuffer(m_filename);
                this._write_buff = null; 
            }
            else
            {
                this._write_buff = new WriteBuffer(m_filename);
                this._read_buff = null;
            }
            /*
            _series_counter = 0;
            this._first_read = true;
            _end_of_serie = false;
             **/
        }
       public  char[] getRekord()
        {
            if (_end_of_tape)
                return null;
            char[] _result = null;
            _result = _read_buff.getNextRecord();
            if (_result == null)
            {
                this._end_of_serie = true;
                this._end_of_tape = true;
                this._read_buff.Close();
                return null;
            }
            if ((Tape.getMeta(_result) < _meta_last) && !this._first_read)
            {
                _end_of_serie = true;
            }
            else if (this._first_read == true)
            {
                _end_of_serie = true;
            }
               
            _meta_last = getMeta(_result);

            this._first_read = false;
            return _result;
        }
        public void writeRekord(char [] m_record)
        {
            _meta_last = getMeta(m_record);
            _write_buff.writeRecord(m_record);
        }
        public int getLastMeta()
        {
            return _meta_last;
        }
        static public int getMeta(char[] m_record)
        {
            // trzeba dopisać nową implementację. liczymy 
            int _sum = 0;
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                count = Convert.ToString(m_record[i], 2).ToCharArray().Count(c => c == '1');
                _sum += count;
            }
            return _sum;
        }
        public int getReads()
        {
            return this._read_buff.getReads();
        }
        public int getWrites()
        {
            return this._write_buff.getWrites();
        }
        public void endWrite()
        {
            this._write_buff.Close();
        }
        public bool isEndOfSerie()
        {
            return _end_of_serie;
        }
        public void unlockSerie()
        {
            if (_end_of_tape != true)
            {
                if( this._end_of_serie)
                _series++;

                this._end_of_serie = false;
            }
        }
        public bool isEndOfTape()
        {
            return this._end_of_tape;
        }
        public int getSeries()
        {
            return this._series;
        }

    }
}
