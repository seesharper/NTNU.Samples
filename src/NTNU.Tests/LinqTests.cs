using System;
using FluentAssertions;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace NTNU.Tests
{
    public class LinqTests
    {
        private Person[] persons = new[]
        {
            new Person("Ida", "Richter"),
            new Person("Astrid", "Richter"),
            new Person("Ingrid", "Richter")
        };


        [Fact]
        public void SimpleFilter()
        {
            var filteredPersons = persons.Where(person => person.FirstName.StartsWith("I"));
            filteredPersons.Count().Should().Be(2);
        }

        [Fact]
        public void ShouldFilterNamesByFirstLetter()
        {
            var filteredPersons = FilterPersons(persons, person => person.FirstName.StartsWith("I"));

            filteredPersons.Count().Should().Be(2);
        }


        [Fact]
        public void ShouldFilterNamesByLength()
        {
            var filteredPersons = FilterPersons(persons, person => person.FirstName.Length == 3);
            filteredPersons.Count().Should().Be(1);
        }


        public IEnumerable<Person> FilterPersons(IEnumerable<Person> persons, Func<Person, bool> filterFunction)
        {
            return persons.Where(person => filterFunction(person));
        }

        public class Person
        {
            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}
