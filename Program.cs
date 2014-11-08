using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace SBD_siszarp
{
   
    class Program
    {
        static public String dir = @"D:\SBD\";
        static public String tape_A = @"tape_A_";
        static public String tape_B = @"tape_B_";
        static public String tape_C = @"tape_C_";
        static public String extens = @".bin";
        static public void goSort(String _filename,bool print = false,bool stats_only = false)
        {
            String[] _tapes_init = new String[3];
            int reads = 0;
            int writes = 0;
            _tapes_init[0] = Path.Combine(Program.dir, _filename);
            //dwie taśmy do zapisu
            bool is_sorted = false;
            int phase = 1;

            while (!is_sorted)
            {
                if (print || stats_only)
                Console.WriteLine("\t ==== Faza " + phase +" ==== ");

                _tapes_init[1] = Path.Combine(Program.dir, Program.tape_A + phase + extens);
                _tapes_init[2] = Path.Combine(Program.dir, Program.extens + phase + extens);

                /*** faza dystrybucji, jako parametry referencje do liczników ***/
                goDistribute(_tapes_init, ref reads , ref writes, print );
                if(print)
                    Console.WriteLine("\t -- koniec etapu dystrybucji --");

                if (print && !stats_only)
                {
                    Console.WriteLine("Tape A:");
                    goPrint(_tapes_init[1]);
                    Console.WriteLine("Tape B:");
                    goPrint(_tapes_init[2]);
                }
                _tapes_init[0] = Path.Combine(dir,Program.tape_C+phase+Program.extens);

                /*** faza złączania, jako parametry przekazujemy referencje do liczników ***/
                is_sorted = goMerge(_tapes_init, ref reads, ref writes,print);
               
                if (print && !stats_only)
                {
                    Console.WriteLine("Tape C:");
                    goPrint(_tapes_init[0]);
                }

                    if (print || stats_only)
                    {
                        Console.WriteLine("\t -- koniec etapu złączania --");
                        Console.WriteLine("\t ++++ Koniec fazy " + phase + " ==== ");
                    }
                phase++;
            }
            if (print)
            {
                Console.WriteLine("Łącznie zapisów: " + writes);
                Console.WriteLine("Łącznie odczytów: " + reads);
                Console.WriteLine("Faz sortowania: " + (phase - 1));
            }
            Console.WriteLine("Tape A: tape1_" + (phase - 1) + ".bin");
            Console.WriteLine("Tape B: tape2_" + (phase - 1) + ".bin");
            Console.WriteLine("Tape C: tape_main_" + (phase - 1) + ".bin");
        }
        static public void goGenerate(String _filename,int _amount = 35, int _min = 0, int _max = 10,bool manual = false,String input_text = null,bool sorted = false)
        {

            int[] _keys = null;

            if(manual)
            {
                StreamReader file_ss = null;
                if (input_text != null)
                {
                    file_ss = new StreamReader(Path.Combine(Program.dir, input_text));
                    Int32.TryParse(file_ss.ReadLine(), out _amount);
                }
                _keys = new int[_amount];
                if (input_text == null)
                {
                    Console.WriteLine("Wpisuj po kolei kolejne wartości klucza.");
                }
                for (int i = 0; i < _amount; i++)
                {
                    if (input_text == null)
                    {
                        Console.Write("Klucz[" + i + "]: ");
                        Int32.TryParse(Console.ReadLine(), out _keys[i]);
                    }
                    else
                    {
                        Int32.TryParse(file_ss.ReadLine(), out _keys[i]);
                    }
                }
                if (file_ss != null)
                {
                    file_ss.Close();
                    file_ss.Dispose();
                }
                
                
               
            }
            String path = System.IO.Path.Combine(Program.dir, _filename);
                Generator gener = new Generator(path);
                if (!manual && !sorted)
                {
                    gener.generateToFile(_amount, _min, _max);
                }
                else if(manual)
                {
                    gener.generateManualy(_amount, _keys);
                }
                else if (sorted)
                {
                    gener.generateSorted(_min, _max);
                }
                return;
        }
      static  public void goPrint(String _filename)
        {
          
                Tape read_tape = new Tape(_filename);
                int x = 0;
                int[] rekord = read_tape.getRekord();
                while (rekord != null)
                {
                    x++;
                    System.Console.Write(x + ".\t (" + Tape.getMeta(rekord) + ")\t ");
                    for (int i = 0; i < 10; i++)
                        System.Console.Write(rekord[i] + " ");
                    System.Console.WriteLine();
                    rekord = read_tape.getRekord();
                }
        }
      static public void goDistribute(String[] _tapes_string, ref int _reads ,ref int _writes,bool print = false )
      {
          Tape read_tape = new Tape(_tapes_string[0]);
          //Tape[] write_tapes = null;
          Tape[] write_tapes = new Tape[2];
          write_tapes[0] = new Tape(_tapes_string[1], false);
          write_tapes[1] = new Tape(_tapes_string[2], false);

          //pobierz pierwszy rekord
          int[] rekord = read_tape.getRekord();

          //jeśli nic nie pobrałeś to taśma pusta, zakończ
          if (rekord == null)
              return;
          write_tapes[0].writeRekord(rekord);//zapisz na pierwszą taśmę od razu bo na pierwszej taśmie lądują mniejsze
          rekord = read_tape.getRekord(); // pobierz kolejny rekord
          int tape = 0;//przełącznik taśm
          while (rekord != null)
          {
              //jeśli ostatnio zapisany rekord na danej taśmie 
              //jest większy od tego w holderze to zamień taśmy bo skończyła się seria
              if (write_tapes[tape].getLastMeta() > Tape.getMeta(rekord))
              {
                  tape = (tape + 1) % 2; // zmien taśmę
              }
              write_tapes[tape].writeRekord(rekord);
              rekord = read_tape.getRekord();
              
          }
          
          for (int i = 0; i < 2; i++)
          {
              write_tapes[i].endWrite(); //zatrzymaj taśmy
             
              _writes += write_tapes[i].getWrites(); //aktualizuj licznik zapisow
              
          }
          _reads += read_tape.getReads(); //aktualizuj licznik odczytów
          if (print)
          {
              Console.WriteLine("Zapisów A: " + write_tapes[0].getWrites());
              Console.WriteLine("Zapisów B: " + write_tapes[1].getWrites());
              Console.WriteLine("Odczytów C: " + read_tape.getReads());
          }

      }
        // złączanie taśm
      static public bool goMerge(String[] _tapes_strings, ref int _reads, ref int _writes,bool print = false)
      {
          
          Tape[] read_tapes = new Tape[2];
          // podpinamy pliki do taśm
          read_tapes[0] = new Tape(_tapes_strings[1], true);
          read_tapes[1] = new Tape(_tapes_strings[2], true);
          Tape write_tape = new Tape(_tapes_strings[0], false);

          /*** holder dla każdej z dwóch taśm wejściowych
           * Przetrzymuje jeden pobrany rekord po bezpośrednim odczycie z taśmy
           * ***/
          int[][] temp_record = new int[2][];
          int current_tape = 0;

          //póki obydwie taśmy wejściowe się nie skończyły to możemy odczytywać i złączać
          while (!read_tapes[0].isEndOfTape() || !read_tapes[1].isEndOfTape() ) 
          {
              //---czy holder jest pusty, jeśli tak to pobierz rekord---//
              if (temp_record[current_tape] == null)
                  temp_record[current_tape] = read_tapes[current_tape].getRekord();

              if (temp_record[(current_tape + 1) % 2] == null)
                  temp_record[(current_tape + 1) % 2] = read_tapes[(current_tape + 1) % 2].getRekord();
              //--------------------------------------------------------//
              //Sprawdź czy seria skończyła się na dwóch taśmach jednocześnie
              if (read_tapes[0].isEndOfSerie() && read_tapes[1].isEndOfSerie()) 
              {
                  //spróbuj rozpocząć kolejne serie
                  read_tapes[0].unlockSerie();
                  read_tapes[1].unlockSerie();
              }
              /*** po tej operacji obydwie serie (jeśli dalej coś na taśmie jest) powinny zostać odczytane
                    Jeśli jest inaczej to na taśme z której już nie da się czytać - seria nie zostanie odblokowana ***/
              
              //czy nie skończyła się w żadnej z taśm (otwarte serie na obydwóch taśmach)
              if (!read_tapes[0].isEndOfSerie() && !read_tapes[1].isEndOfSerie())
              {
                  //zobacz, który z następnych elementów będzie mniejszy, wybierz ten o mniejszym kluczu
                  if (Tape.getMeta(temp_record[current_tape]) <= Tape.getMeta(temp_record[(current_tape + 1) % 2]))
                  {
                      // weź z holdera dla aktualnej taśmy i zapisz na wyjściową
                      write_tape.writeRekord(temp_record[current_tape]);
                      temp_record[current_tape] = null; //wyczyść holder
                  }
                  else
                  {
                      //weź z holdera dla drugiej taśmy i zapisz na wyjście
                      write_tape.writeRekord(temp_record[(current_tape + 1) % 2]);
                      temp_record[(current_tape + 1) % 2] = null; //wyczyść holder
                  }
              }
                  //czy seria NIE skończyła się na A ale skończyła się na B
              else if ((!read_tapes[0].isEndOfSerie()) && (read_tapes[1].isEndOfSerie())) // seria skonczyla sie na drugim ale nie na pierwszym
              {

                  current_tape = 0; //bierz z z holdera dla pierwszej taśmy i zapisz na wyjście
                  write_tape.writeRekord(temp_record[current_tape]);
                  temp_record[current_tape] = null; //wyczyść holder
              }
                  //czy seria skończyła się na A ale NIE skończyła się na B
              else if ((read_tapes[0].isEndOfSerie()) && (!read_tapes[1].isEndOfSerie())) //seria skonczyla sie na pierwszym
              {
                  current_tape = 1;//weź z holdera dla drugiej taśmy i zapisz na wyjście
                  write_tape.writeRekord(temp_record[current_tape]);
                  temp_record[current_tape] = null; //wyczyść holder
              }





          }
          //zatrzymaj taśmę po fazie zapisu
          write_tape.endWrite();
         
          if(print)
          {
         
          Console.WriteLine("Zapisów C: " + write_tape.getWrites());
          Console.WriteLine("Odczytów A: " + read_tapes[0].getReads());
          Console.WriteLine("Odczytów B: " + read_tapes[1].getReads());
          Console.WriteLine("Serii w A: "+ read_tapes[0].getSeries());
          Console.WriteLine("Serii w B: " + read_tapes[1].getSeries());
          
          }
           
          for (int i = 0; i < 2; i++)
          {
              _reads += read_tapes[i].getReads();
          }
          _writes += write_tape.getWrites();

          /***jeśli po łączeniu na każdej z taśm było po jednej serii to jest pewne,
           * że wyjściowa będzie już posortowana*/
          if (read_tapes[0].getSeries() == 1 && read_tapes[1].getSeries() == 1)
          {
              return true;
          }
          if (read_tapes[0].getSeries() == 1 && read_tapes[1].getSeries() == 0)
          {
              return true;
          }

          else return false;

      }
      static void Main(string[] args)
      {
          String help =
          @"
SBD_siszarp.exe [-sort] [[-print]] [[-stats]] [nazwa_pliku.bin]       Sortowanie i wypisywanie (jeśli użyty -print) taśmy na ekran etapami lub tylko statystyk [-stats]
SBD_siszarp.exe [-print] [nazwa_pliku.bin]      Wypisanie na ekran zawartości taśmy 
SBD_siszarp.exe [-print] [A|B|C] [faza]                    Wypisanie taśmy A,B lub C z danej fazy

SBD_siszarp.exe [-gener] [ilość] [min] [max] [[nazwa_pliku_wyjściowego]|generate.bin]         
Generowanie rekordów w zakresie min/max dla każdego elementuw rekordzie

SBD_siszarp.exe [-sgener] [min] [max] [[nazwa_pliku_wyjściowego]|generate.bin]         
Generowanie rekordów w zakresie min/max dla każdego elementuw rekordzie posortowanych

SBD_siszarp.exe [-gener] [-manual] [ilość] [[nazwa_pliku_wyjściowego]|generate.bin] 

SBD_siszarp.exe [-gener] [nazwa_pliku.txt] [[nazwa_pliku_wyjściowego]|generate.bin]
Generowanie rekordów z pliku tekstowego
";

          String command = null;
          String przelacznik = null;
          int _min = 0;
          int _max = 0;
          int _amount = 0;
          String filename = @"generate.bin";
          if (args.Length == 0)
          {
              Console.WriteLine(help);
              return;
          }

          try
          {  command = args[0]; }
          catch (IndexOutOfRangeException e){ }

          command = command.ToLower();
          switch (command)
          {
              
              case "-gener":
                  {
                      
                      if (args[1] != "-manual" && args.Length < 4)
                      {

                          if (args.Length == 3)
                              filename = args[2];

                          goGenerate(filename, 0, 0, 0, true, args[1]);
                          break;
                      }
                      if(args[1] =="-manual")
                      {
                          Int32.TryParse(args[2], out _amount);
                          if(args.Length == 4)
                              filename = args[3];

                          goGenerate(filename, _amount, _min, _max, true);
                          break;
                      }
                      else
                      if (args.Length >= 4)
                      {
                          Int32.TryParse(args[1], out _amount);
                          Int32.TryParse(args[2], out _min);
                          Int32.TryParse(args[3], out _max);

                      }
                      if (args.Length == 5)
                          filename = args[4];

                      goGenerate(filename,_amount, _min, _max, false);
                      break;
                  }
              case "-sgener":
                  {
                      Int32.TryParse(args[1], out _min);
                      Int32.TryParse(args[2], out _max);
                      if (args.Length == 4)
                          filename = args[3];

                      goGenerate(filename, _amount, _min, _max, false,null,true);
                      break;
                  }
              case "-print":
                  {
                      if (args.Length == 2)
                      {
                          przelacznik = System.IO.Path.Combine(Program.dir, args[1]);
                          goPrint(przelacznik);
                      }
                      if (args.Length == 3)
                      {

                          przelacznik = @"tape_" + args[1] + "_" + args[2] + Program.extens;
                          przelacznik = System.IO.Path.Combine(Program.dir, przelacznik);
                          goPrint(przelacznik);
                         
                      }
                      break;
                  }
              case "-sort":
                  {
                      if (args.Length == 3)
                          goSort(args[2], true);
                      else if (args.Length == 2)
                          goSort(args[1]);
                      else if (args.Length == 4)
                          goSort(args[3], true, true);
                      break;
                  }
              default:
                  {
                      Console.WriteLine(help);
                      return;
                  }
          }



      } 
    }
}
