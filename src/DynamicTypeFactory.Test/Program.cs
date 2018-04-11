using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTypeFactory.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var lst = new List<ClassA>()
            {
                new ClassA()
                {
                    Prop1 = "1",
                    Prop2 = "2",
                    Prop3 = "3",
                    Prop4 = "4",
                    Prop5 = "5",
                    Prop6 = 6
                },
                new ClassA()
                {
                    Prop1 = "1",
                    Prop2 = "2",
                    Prop3 = "3",
                    Prop4 = "4",
                    Prop5 = "5",
                    Prop6 = 6
                },
                new ClassA()
                {
                    Prop1 = "1",
                    Prop2 = "2",
                    Prop3 = "3",
                    Prop4 = "4",
                    Prop5 = "5",
                    Prop6 = 6
                },
                new ClassA()
                {
                    Prop1 = "1",
                    Prop2 = "2",
                    Prop3 = "3",
                    Prop4 = "4",
                    Prop5 = "5",
                    Prop6 = 6
                },
                new ClassA()
                {
                    Prop1 = "1",
                    Prop2 = "2",
                    Prop3 = "3",
                    Prop4 = "4",
                    Prop5 = "5",
                    Prop6 = 6
                }
            };
            var result = lst.Map<ClassA, ClassB>();
            var props = result.FirstOrDefault()?.GetType()?.GetProperties();
            foreach (var item in result)
            {
                Console.WriteLine("{\n\r");

                foreach (var prop in props)
                {
                    Console.WriteLine($"\t{prop.Name}:\t{prop.GetValue(item)}\n\r");
                }

                Console.WriteLine("},");
            }

            
            Console.ReadLine();
        }
    }

    public class ClassA
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
        public string Prop3 { get; set; }
        public string Prop4 { get; set; }
        public string Prop5 { get; set; }
        public int Prop6 { get; set; }
    }

    public class ClassB
    {
        public string Prop2 { get; set; }
        public string Prop4 { get; set; }
        public string Prop6 { get; set; }
        public string Prop8 { get; set; }
    }
}
