using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SBD_siszarp
{
    class Generator
    {
        private WriteBuffer _write_buf;
        private int _min;
        private int _max;
        private int _writes;
        public Generator(WriteBuffer m_write_buff)
        {
            this._write_buf = m_write_buff;
        }
        public Generator(String m_filename)
        {
            this._write_buf = new WriteBuffer(m_filename);
        }
        public Generator()
        {
            this._write_buf = null;
        }
        public void generateToFile(String m_filename,int m_amount)
        {
            this._write_buf = new WriteBuffer(m_filename);

        }

        static public char[] enabledBitsToCharRecord(int _enabledBitsCount)
        {
            const int RECORD_LENGTH = 10; //no dlugosc rekordu
            char[] _new_record = new char[RECORD_LENGTH];
            char singleCharToInsert = '\0';
            int enabledBitsRemain = _enabledBitsCount;
            int howManyEnabledBitsForSingleChar = 0;
            for (int elementIter = 0; elementIter < 10; elementIter++)
            {
                if (enabledBitsRemain > 8)
                {
                    enabledBitsRemain -= 8;
                    singleCharToInsert = Convert.ToChar(255);
                }
                else
                {
                    // jesli mamy mniej niz 9 jedynek to zmiesci sie to w jednym bajcie 

                    howManyEnabledBitsForSingleChar = enabledBitsRemain;
                    
                    if (enabledBitsRemain > 0)
                    {
                        singleCharToInsert = (char)Generator.getByteFromEnabledBitsCounter(howManyEnabledBitsForSingleChar);
                    }
                    else
                    {
                        singleCharToInsert = Convert.ToChar(0);
                    }
                    enabledBitsRemain -= howManyEnabledBitsForSingleChar;

                }
                _new_record[elementIter] = singleCharToInsert;
            }

            return _new_record;

        }
        public void generateManualy(int m_amount, int [] m_keys)
        {
            this._write_buf.Close();
            this._write_buf.Open();
            char[] record = new char[10];
            for (int i = 0; i < m_amount; i++)
            {

                /* tutaj potrzebujemy takiej generacji, która będzie agresywnie pochłaniała ilość bitów == 1
                 * Np. jesli mamy rekord o wartości 12 to pierwszy znak bedzie mial 8 bitow == 1 a kolejny 4.
                 * nastepne po 0 'jedynek'  czyli beda nullbyte'ami.
                 */
                char znak = '\0';
                int enabledBitsCount = m_keys[i];

                record = Generator.enabledBitsToCharRecord( enabledBitsCount );

                this._write_buf.writeRecord(record);
            }
            this._write_buf.Close();
        }
        public void generateSorted(int m_min, int m_max)
        {
            this._write_buf.Close();
            this._write_buf.Open();
            char[] record = new char[10];
            
            if (m_min < m_max)
            {
                //rosnąco generuj
                for (int i = m_min; i < m_max; i++)
                {
                    record = Generator.enabledBitsToCharRecord(i);
                this._write_buf.writeRecord(record);
                }
            }
            else
            {
                //malejąco generuj
                for (int i = m_min; i > m_max; i--)
                {
                    record = Generator.enabledBitsToCharRecord(i);
                    this._write_buf.writeRecord(record);
                    
                }

            }
            this._write_buf.Close();
        }
        public void generateToFile(int m_amount, int m_min = -100, int m_max = 100)
        {
            this._min = m_min;
            this._max = m_max;
            this._write_buf.Close();
            this._write_buf.Open();
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            for (int x = 0; x < m_amount; x++)
            {
                char[] record = new char[10];
                for (int i = 0; i < 10; i++)
                {
                    record[i] = Convert.ToChar(rand.Next(this._min, this._max));
                }
                this._write_buf.writeRecord(record);
            }
            this._write_buf.Close();
        }
        public void setMin(int m_min)
        {
            this._min = m_min;
        }
        public void setMax(int m_max)
        {
            this._max = m_max;
        }
        public int getWrites()
        {
            return this._write_buf.getWrites();
        }


        static public byte getByteFromEnabledBitsCounter(int _enabledBits)
        {
            byte myByte = (byte)0x00;
            byte tempByte  = (byte)0x00;
            if (_enabledBits == 0)
            {
                return myByte;
            }
            
            myByte = (byte)0x01;
            /*
            for (int i = 0 ; i < _enabledBits; i++)
            {
                tempByte = myByte;
                myByte = (byte)(myByte << 1 | tempByte);
                
            }
             * */
            /*
             * Abusrd testowy ponizej:
             * */
            myByte = 0xCA;
            return myByte;
        }
        
    }
}
