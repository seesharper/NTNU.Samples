using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NTNU.Tests
{
    public class AsyncAwaitTests
    {

        [Fact]
        public void ShouldReadLargeFile()
        {
            var stopWatch = Stopwatch.StartNew();

            ReadLargeFile();

            Console.WriteLine($"Elapsed (ms): {stopWatch.ElapsedMilliseconds}");
        }

        [Fact]
        public void ShouldReadLargeFiles()
        {
            var stopWatch = Stopwatch.StartNew();

            ReadLargeFile();
            ReadLargeFile();

            Console.WriteLine($"Elapsed (ms): {stopWatch.ElapsedMilliseconds}");
        }


        [Fact]
        public void ShouldReadLargeFilesInParallel()
        {
            var stopWatch = Stopwatch.StartNew();

            Task.WhenAll(Task.Run(ReadLargeFile), Task.Run(ReadLargeFile)).Wait();

            Console.WriteLine($"Elapsed (ms): {stopWatch.ElapsedMilliseconds}");
        }


        [Fact]
        public async Task ShouldReadLargeFilesAsyncInParallel()
        {
            var stopWatch = Stopwatch.StartNew();

            await Task.WhenAll(ReadLargeFileAsync(), ReadLargeFileAsync());

            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }



        private byte[] ReadLargeFile()
        {
            Console.WriteLine($"Current thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(2000);
            return new byte[] { 42 };
        }

        private async Task<byte[]> ReadLargeFileAsync()
        {
            Console.WriteLine($"Current thread {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(2000);
            return new byte[] { 42 };
        }
    }



}