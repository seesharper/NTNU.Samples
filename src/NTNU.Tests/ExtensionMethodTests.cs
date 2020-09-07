using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace NTNU.Tests
{
    public class ExtensionMethodTests
    {
        private Person[] persons = new[]
        {
            new Person("Ida", "Richter"),
            new Person("Astrid", "Richter"),
            new Person("Ingrid", "Richter")
        };

        [Fact]
        public void ShouldFilterNamesUsingExtensionMethod()
        {
            var personsWithShortNames = persons.WithFirstNameLength(3);

            personsWithShortNames.Count().Should().Be(1);
        }
    }

    public static class PersonListExtensions
    {
        public static IEnumerable<Person> WithFirstNameLength(this IEnumerable<Person> persons, int length)
        {
            return persons.Where(person => person.FirstName.Length <= length);
        }
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