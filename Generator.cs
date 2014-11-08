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
        public void generateManualy(int m_amount, char [] m_keys)
        {
            this._write_buf.Close();
            this._write_buf.Open();
            char[] record = new char[10];
            for (int i = 0; i < m_amount; i++)
            {
                record[0] = m_keys[i];
                for (int x = 1; x < 10; x++)
                {
                    record[x] = '\0';
                }
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
                    record[0] = Convert.ToChar(i);
                    for (int x = 1; x < 10; x++)
                    {
                        record[x] = '\0';
                    }
                this._write_buf.writeRecord(record);
                }
            }
            else
            {
                //malejąco generuj
                for (int i = m_min; i > m_max; i--)
                {
                    record[0] = Convert.ToChar(i);
                    for (int x = 1; x < 10; x++)
                    {
                        record[x] = '\0';
                    }
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
        
    }
}
