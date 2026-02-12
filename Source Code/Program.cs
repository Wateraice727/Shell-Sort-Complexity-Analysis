using System.Diagnostics;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Runtime.Intrinsics.X86;

namespace DSA
{
    public class STiming
    {
        private TimeSpan _startingTime;
        private TimeSpan _duration;
        public STiming()
        {
            _startingTime = new TimeSpan(0);
            _duration = new TimeSpan(0);
        }
        public void StartTime()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _startingTime=Process.GetCurrentProcess().Threads[0].UserProcessorTime;
        }
        public void StopTime()
        {
            _duration=Process.GetCurrentProcess().Threads[0].UserProcessorTime.Subtract(_startingTime);
        }
        public TimeSpan Result()
        {
            return _duration;
        }
    }
    public class Timing
    {
        private Stopwatch _stopwatch;
        public Timing()
        {
            _stopwatch = new Stopwatch();
        }
        public void StartTime()
        {
            _stopwatch.Restart();
        }
        public void StopTime()
        {
            _stopwatch.Stop();
        }
        public TimeSpan Result()
        {
            return _stopwatch.Elapsed;
        }
    }
    internal class Program
    {
        static Random rand = new Random();
        static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
        static void InsertionSort<T>(T[] arr, int left, int right)
        where T : IComparable<T>
        {
            for (int i = left + 1; i < right; ++i) 
            {
                T key = arr[i];
                int j = i - 1;
                while (j >= left && arr[j].CompareTo(key) > 0) 
                // arr[j] > key tương đương với arr[j].CompareTo(key) > 0
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
        }
        static List<int> Shell(int size)
        {
            List<int> gaps = new List<int>();
            while ((size /= 2) > 0) 
                gaps.Add(size);
            return gaps;
        }
        static List<int> Hibbard(int size)
        {
            List<int> gaps = new List<int>();
            gaps.Add(1);
            while (gaps[gaps.Count - 1] * 2 + 1 < size)
                gaps.Add(gaps[gaps.Count - 1] * 2 + 1);
            gaps.Reverse();
            return gaps;
        }
        static List<int> Pratt(int size)
        {
            List<int> gaps = new List<int>();
            for (int i = 1; i < size; i *= 2)
                for (int j = i; j < size; j *= 3)
                    gaps.Add(j);
            gaps.Sort((a, b) => b.CompareTo(a));
            // sắp xếp ngược dãy số
            return gaps;
        }
        static List<int> Sedgewick(int size)
        {
            List<int> gaps = new List<int>();
            gaps.Add(1);
            for (int k = 1; (1 << 2 * k) + 3 * (1 << k - 1) + 1 < size; k++)
                gaps.Add((1 << 2 * k) + 3 * (1 << k - 1) + 1);
            gaps.Reverse();
            return gaps;
        }
        static List<int> CiuraEx(int size)
        {
            List<int> gaps = [1, 4, 10, 23, 57, 132, 301, 701, 1750];
            
            // phần mở rộng
            double gap = 1750;
            while ((gap *= 2.25) < size)
            {
                int floor = (int)Math.Round(gap);
                gaps.Add(floor);
            }
            
            gaps.Reverse();
            return gaps;
        }
        static int[] GenerateValue(int size)
        {
            int[] arr = new int[size];
            for (int i = 0; i < size; i++) 
                arr[i] = rand.Next(1, 1 << 30);
            return arr;
        }
        static int[] BestGenerateValue(int size)
        {
            int[] arr = new int[size];
            arr[0] = 1;
            for (int i = 1; i < size; i++)
                arr[i] = rand.Next(arr[i - 1] + 1, arr[i - 1] + 100);
            return arr;
        }
        static int[] ReverseGenerateValue(int size)
        {
            int[] arr = new int[size];
            arr[0] = 1 << 30;
            for (int i = 1; i < size; i++)
                arr[i] = rand.Next(arr[i - 1] - 100, arr[i - 1] - 1);
            return arr;
        }
        static int[] WorstShellGenerate(int size)
        {
            int[] arr = new int[size];
            int index = 0;
            for (int i = size / 2; i > 0; i--)
            {
                arr[index] = i;
                index += 2;
            }
            index = 1;
            for (int i = size; i > size / 2; i--)
            {
                arr[index] = i;
                index += 2;
            }
            return arr;
        }
        // static long count = 0;
        static void ShellSort (int [] arr, int left, int right, List<int> gaps) 
        {    
            // nếu left > right thì sắp xếp giảm dần
            bool greater = left > right;
            if (greater) Swap(ref left, ref right);
            foreach (int h in gaps) //Duyệt gap ngược
            {
                if (h >= right - left + 1) continue;
                for (int i = left + h; i <= right; i++) 
                {
                    // count++;
                    int key = arr[i];
                    int j = i;
                    while (j >= left + h && (greater ? arr[j - h] < key : arr[j - h] > key))
                    {
                        arr[j] = arr[j - h];    
                        j -= h;
                        // count++;
                    }
                    arr[j] = key;
                }
            }
        }
        static void StableShellSort((int, char)[] arr, int left, int right, List<int> gaps)
        {
            foreach (int h in gaps)
            {
                if (h >= right - left + 1) continue;
                for (int i = left + h; i <= right; i++)
                {
                    (int, char) key = arr[i];
                    int j = i;
                    while (j >= left + h && arr[j - h].Item1 > key.Item1)
                    {
                        arr[j] = arr[j - h];
                        j -= h;
                    }
                    arr[j] = key;
                }
            }
        }
        static void StandardOutput()
        {
            int[] arr = GenerateValue(30);
            List<int> gaps = CiuraEx(arr.Length);
            foreach (int x in arr) Console.Write(x + " ");
            Console.Write("\n");
            ShellSort(arr, 0, arr.Length - 1, gaps);
            foreach (int x in arr) Console.Write(x + " ");
            Console.Write("\n");
            ShellSort(arr, arr.Length - 1, 0, gaps);
            foreach (int x in arr) Console.Write(x + " ");
            Console.Write("\n");
        }
        static void StableCheck()
        {
            (int value, char position)[] arr = { (5, 'a'), (3, ' '), (2, ' '), (5, 'b'), (1, ' ')};
            foreach ((int, char) p in arr)
            {
                Console.Write(p.Item1);
                if (p.Item2 != ' ') Console.Write(p.Item2);
                Console.Write(" ");
            }
            Console.Write("\n");
            List<int> gaps = CiuraEx(arr.Length - 1);
            StableShellSort(arr, 0, arr.Length - 1, gaps);
            foreach ((int, char) p in arr)
            {
                Console.Write(p.Item1);
                if (p.Item2 != ' ') Console.Write(p.Item2);
                Console.Write(" ");
            }
            Console.Write("\n");
        }
        static void GapCheck()
        {
            int size = 1000000;
            List<int> gap1 = Shell(size);
            List<int> gap2 = Hibbard(size);
            List<int> gap3 = Pratt(size);
            List<int> gap4 = Sedgewick(size);
            List<int> gap5 = CiuraEx(size);
            //foreach (int h in gap1)
            //    Console.Write(h + " ");
            //Console.Write('\n');
            //foreach (int h in gap2)
            //    Console.Write(h + " ");
            //Console.Write('\n');
            //foreach (int h in gap3)
            //    Console.Write(h + " ");
            //Console.Write('\n');
            //foreach (int h in gap4)
            //    Console.Write(h + " ");
            //Console.Write('\n');
            //foreach (int h in gap5)
            //    Console.Write(h + " ");
            //Console.Write('\n');
        }
        static void Process()
        {
            Timing maintimer = new Timing(), copytimer = new Timing();
            int time = 1;
            int size = 10000000;
            List<int> gaps = Shell(size);

            copytimer.StartTime();
            int[] arr = GenerateValue(size);
            copytimer.StopTime();

            maintimer.StartTime();
            for (int i = 0; i < time; i++)
            {
                arr = GenerateValue(size);
                ShellSort(arr, 0, size - 1, gaps);
            }
            maintimer.StopTime();

            // Console.Write(count/time + "\n");
            Console.Write("Average Estimated Time: {0} ms\n", maintimer.Result().TotalMilliseconds/time - copytimer.Result().TotalMilliseconds);
        }
        static void Main(string[] args)
        {
            Console.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            //StandardOutput();
            //GapCheck();
            //StableCheck();
            Process();
        }
    }
}