using System.Diagnostics;


namespace HW8_Multithreading;

class Program
{
    static void Main(string[] args)
    {
        
        Console.WriteLine($"ОС : Windows 10 Pro");
        Console.WriteLine($"Процессор : Intel(R) Core(TM) i5-8250U CPU @ 1.60GHz   1.80 GHz");
        Console.WriteLine($"Количество ядер : {Environment.ProcessorCount}");
        Console.WriteLine($"Оперативная память : 12,0 ГБ");
        
        // Создаем массив чисел из случайных значений
        var maxElement = 10_000_000;
        int[] numbers = Enumerable.Range(1, maxElement).Select(x=>new Random().Next(100)).ToArray();
        
        // Последовательное вычисление
        Console.WriteLine($"-------------------------------");
        Console.WriteLine($"Последовательное вычисление");
        long sum = 0;
        var sw = new Stopwatch();
        sw.Start();
        // считаем сумму в одном потоке
        for (var i = 0; i < maxElement; i++)
        {
            sum += numbers[i]; 
        }
        sw.Stop();
        
        Console.WriteLine($"Количество элементов: {maxElement} сумма {sum}" );
        Console.WriteLine($"Время выполнения {sw.Elapsed}" );
        
        sw.Reset();
        
        // Параллельное вычисление Thread
        Console.WriteLine($"-------------------------------");
        Console.WriteLine($"Параллельное вычисление Thread");
        

        // Создаем список потоков
        List<Thread> threads = new List<Thread>();

        // Создаем список для хранения результатов суммы в каждом потоке
        List<long> sums = new List<long>();
        
        var lockobj = new object();

        // Разбиваем массив на части и запускаем вычисление суммы в каждом потоке
        sw.Start();
        int chunkSize = numbers.Length / Environment.ProcessorCount;
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            int startIndex = i * chunkSize;
            int endIndex = (i == Environment.ProcessorCount - 1) ? numbers.Length : startIndex + chunkSize;

            Thread thread = new Thread(() =>
            {
                lock(lockobj)
                {
                    long sumIn = 0;
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        sumIn += numbers[j];
                    }
                    sums.Add(sumIn);
                }
            });

            thread.Start();
            threads.Add(thread);
        }


        // Ожидаем завершения всех потоков
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        // Вычисляем общую сумму из результатов каждого потока
        long totalSum = sums.Sum();
        sw.Stop();
        
        Console.WriteLine($"Количество элементов: {maxElement} сумма {totalSum}" );
        Console.WriteLine($"Время выполнения {sw.Elapsed}" );
        sw.Reset(); 
        
           
        // Параллельное вычисление LINQ
        Console.WriteLine($"-------------------------------");
        Console.WriteLine($"Параллельное вычисление LINQ");
        
        long sumLinq = 0;
        sw.Start();
        // считаем сумму 
        sumLinq = numbers.AsParallel().Sum();
        sw.Stop();
        
        Console.WriteLine($"Количество элементов: {maxElement} сумма {sumLinq}" );
        Console.WriteLine($"Время выполнения {sw.Elapsed}" );
        
    }
}