using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NTNU.Tests
{
    public class GenericTests
    {
        [Fact]
        public void NonGenericList()
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(42);
            arrayList.Add("SomeValue");
            var value = (int)arrayList[1];
        }

        [Fact]
        public void GenericList()
        {
            List<int> list = new List<int>();
            list.Add(42);

            var value = list[0];
        }
    }


    public class Generic<T>
    {
        public T Value { get; set; }
    }
}