using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TestTask
{
    class Triplet
    {
        public string Name { get; set; }
        public int NubmerOccurrences { get; set; }

        public void Print() => Console.WriteLine($"Triplet: {Name} \nNumber of repetitions: {NubmerOccurrences}"); 
    }
    
    internal class Program
    {
        static List<Triplet> Tripets = new List<Triplet>();
        static List<Triplet> promTripets = new List<Triplet>();

        static void Main(string[] args)
        {
            DateTime StartTime = DateTime.Now;
            //Console.WriteLine("Enter the path to the file \n Example: D:/.../_.txt");
            //string path = Console.ReadLine();

            string path = @"D:\Andrey\one.txt";

            try
            {
                if ((File.Exists(path) != false) && (File.ReadAllLines(path).Length != 0))
                {
                    string[] lines =  File.ReadAllLines(path);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == lines.Length-1)
                        {
                            ThreadSearch thread = new ThreadSearch(i.ToString(), lines[i].ToLower(), Tripets, ThreadPriority.Lowest);
                        }
                        else
                        {
                            ThreadSearch thread = new ThreadSearch(i.ToString(), lines[i].ToLower(), Tripets, ThreadPriority.Highest);
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("Not found or the file is empty");
                }

            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }

            DateTime EndTime = DateTime.Now;
            
            Console.Read();
            Console.WriteLine($" Milliseconds: {(EndTime - StartTime).Milliseconds}");
            Console.ReadKey();
        }
    }

    internal class ThreadSearch
    {
        Thread thread;

        public List<Triplet> triplets;

        public ArrayList tripArr = new ArrayList();
        
        public ThreadSearch(string name , string line_text, List<Triplet> tripletOn, ThreadPriority level)
        {
            this.triplets = tripletOn;

            if (level == ThreadPriority.Lowest)
            {
                Thread.Sleep(10);
            }

            thread = new Thread(this.SearchTriplets);
            thread.Name = name;
            thread.Start(line_text);
            thread.Priority = level;
        }

        void SearchTriplets(object inText)
        {
            tripArr = ArrayList.Adapter(triplets);
            tripArr = ArrayList.Synchronized(tripArr);

            char[] a_b_c = inText.ToString().ToCharArray();

            for (int i = 0; i < a_b_c.Length - 2; i++)
            {
                if (Char.IsLetter(a_b_c[i]) && Char.IsLetter(a_b_c[i + 1]) && Char.IsLetter(a_b_c[i + 2]))
                {
                    char[] temporary = new char[] { a_b_c[i], a_b_c[i + 1], a_b_c[i + 2] };
                    string LineValid = String.Join("", temporary);

                    lock (triplets)
                    {
                        if (triplets.Any(u => u.Name == LineValid))
                        {
                            triplets.FirstOrDefault(u => u.Name == LineValid).NubmerOccurrences++;

                        }
                        else
                        {

                            Triplet goosy = new Triplet()
                            {
                                Name = LineValid,
                                NubmerOccurrences = 1,
                            };
                            triplets.Add(goosy);
                        }
                    }
                }
                else continue;
            }

            lock (triplets)
            {
                if (thread.Priority == ThreadPriority.Lowest)
                {
                    foreach (var triplet in triplets.OrderByDescending(u => u.NubmerOccurrences).Take(10))
                    {
                        Console.WriteLine("/-------------------------------------------------------------/");
                        triplet.Print();
                    }
                    Console.WriteLine($"{thread.Name} закончила работу");
                }
            }
        }
    }
}
