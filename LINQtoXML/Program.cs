using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LINQtoXML
{
    class Program
    {
        static string path = @"C:\Users\Marusya\Downloads\LINQtoXML (1)\LINQtoXML\bin\Debug\netcoreapp3.1\";

        static void Task1()
        {
            List<string> lines = ReadFile(path + "input1.txt");

            var xml = new XDocument(new XDeclaration("1.0", "windows-1251", null), new XElement("root", 
                lines.Select((item, i) => new XElement("line", new XAttribute("num", (i += 1)),
                item.Split(' ').Select((item, j) => new XElement("word", item, new XAttribute("num", j += 1)))))));

            xml.Save(Path.Combine(Environment.CurrentDirectory, "output1.xml"));

            PrintLines(ReadFile(path + "input1.txt"));
            PrintLines(ReadFile(path + "output1.xml"));
        }

        static void Task2()
        {
            XDocument document = XDocument.Load(path + "input2.xml");

            var groupElem = document.Root.Elements()
                .Select(ans => new { ans.Name, Count = ans.Elements().Count(second => second.Attributes().Count() >= 2) })
                .OrderBy(x => x.Name.ToString()).ThenBy(y => y.Count);

            PrintLines(ReadFile(path + "input2.xml"));

            foreach (var x in groupElem) Console.WriteLine("Name: " + x.Name + " Count: " + x.Count);
            Console.WriteLine();
        }

        static void Task3()
        {
            XDocument document = XDocument.Load(path + "input3.xml");

            document.Root.Elements().Where(element => element.Attributes().Count() > 1).Attributes().Remove();
            document.Root.Elements().Elements().Where(element => element.Attributes().Count() > 1).Attributes().Remove();

            document.Save(path + "output3.xml");

            PrintLines(ReadFile(path + "input3.xml"));
            PrintLines(ReadFile(path + "output3.xml"));
        }

        static void Task4()
        {
            XDocument document = XDocument.Load(path + "input4.xml");

            document.Root.ReplaceAll(document.Root.Elements().Select(first =>
            {
                first.ReplaceAll(first.Elements().Select(second =>
                {
                    second.SetAttributeValue("child-count", second.Elements().Count());
                    return second;
                }));
                return first;
            }));

            document.Save(path + "output4.xml");

            PrintLines(ReadFile(path + "input4.xml"));
            PrintLines(ReadFile(path + "output4.xml"));
        }

        static void Task5()
        {
            XDocument document = XDocument.Load(path + "input5.xml");

            document.Root.ReplaceAll(document.Root.Descendants().Select(elems =>
            {
                if (elems.HasAttributes)
                {
                    int count = elems.DescendantsAndSelf().Sum(desc => desc.Attributes().Count());
                    elems.AddFirst(new XElement("odd-attr-count", count % 2 == 1));
                }
                return elems;
            }));

            document.Save(path + "output5.xml");

            PrintLines(ReadFile(path + "input5.xml"));
            PrintLines(ReadFile(path + "output5.xml"));
        }

        static void Task6()
        {
            XDocument document = XDocument.Load(path + "input6.xml");

            document.Root.Elements().Elements().Attributes().Where(e => e.IsNamespaceDeclaration).Remove();

            document.Save(path + "output6.xml");

            PrintLines(ReadFile(path + "input6.xml"));
            PrintLines(ReadFile(path + "output6.xml"));
        }

        class PersonTime
        {
            public int time;
            public int id;

            public PersonTime(int time, int id)
            {
                this.time = time;
                this.id = id;
            }
        }

        static void Task7()
        {
            XDocument document = XDocument.Load(path + "input7.xml");
            var years = new Dictionary<string, List<PersonTime>>();

            document.Root.Elements().ToList().ForEach(first =>
            {
                string id = first.Name.ToString().Replace("id", "");
                string curYear = "";
                int time = -1;
                first.Elements().Elements().ToList().ForEach(second =>
                {
                    if (second.Name == "date")
                    {
                        curYear = second.Value.Split('-')[0];
                    }
                    else if (second.Name == "time")
                    {
                        string str = second.Value.Replace("PT", "");
                        int hour = int.Parse(str.Split('H')[0]);
                        int min = int.Parse(str.Split('H')[1].Split('M')[0]);
                        time = hour * 60 + min;
                    }
                    if (!(curYear == "" || time == -1))
                    {
                        if (!years.ContainsKey(curYear))
                            years.Add(curYear, new List<PersonTime>());
                        years[curYear].Add(new PersonTime(time, int.Parse(id)));
                        curYear = "";
                        time = -1;
                    }
                });
            });

            XDocument documentOut = new XDocument(new XDeclaration("1.0", "windows-1251", null));
            XElement root = new XElement("root");
            root.Add(years.OrderBy(q => q.Key).Select(year =>
            {
                var yearX = new XElement("year", new XAttribute("value", year.Key));
                yearX.Add(year.Value.OrderBy(q => q.id).Select(personTime =>
                {
                    var total = new XElement("total-time", new XAttribute("id", personTime.id))
                    {
                        Value = personTime.time.ToString()
                    };
                    return total;
                }));
                return yearX;
            }));
            documentOut.Add(root);
            documentOut.Save(path + "output7.xml");

            PrintLines(ReadFile(path + "input7.xml"));
            PrintLines(ReadFile(path + "output7.xml"));
        }

        class BrandPrice
        {
            public int brand;
            public int price;

            public BrandPrice(int brand, int price)
            {
                this.brand = brand;
                this.price = price;
            }
        }

        static void Task8()
        {
            XDocument document = XDocument.Load(path + "input8.xml");
            var streets = new Dictionary<string, List<BrandPrice>>();
            document.Root.Elements().ToList().ForEach(first =>
            {
                string street = first.Name.ToString().Split('_')[1];
                int brand = -1, price = -1;
                first.Elements().ToList().ForEach(second =>
                {
                    if (second.Name == "brand")
                    {
                        brand = int.Parse(second.Value);
                    }
                    else if (second.Name == "price")
                    {
                        price = int.Parse(second.Value);
                    }
                    if (!(price == -1 || brand == -1))
                    {
                        if (!streets.ContainsKey(street))
                            streets.Add(street, new List<BrandPrice>());
                        streets[street].Add(new BrandPrice(brand, price));
                        brand = -1;
                        price = -1;
                    }
                });
            });

            XDocument documentOut = new XDocument(new XDeclaration("1.0", "windows-1251", null));
            XElement root = new XElement("root");
            root.Add(streets.OrderBy(q => q.Key).Select(street =>
            {
                var streetX = new XElement(street.Key);
                streetX.Add(street.Value.GroupBy(group => group.brand)
                    .Select(arr => new { brand = arr.Key, price = arr.Average(p => p.price) })
                    .OrderByDescending(brand => brand.brand).Select(brandPrice =>
                    {
                        int count = street.Value.Count(brand => brand.brand == brandPrice.brand);
                        double average = street.Value.Where(brand => brand.brand == brandPrice.brand)
                            .Average(avg => avg.price);
                        var total = new XElement($"brand{brandPrice.brand}", new XAttribute("station-count", count));
                        total.Value = Convert.ToInt64(average).ToString();
                        return total;
                    }));
                return streetX;
            }));
            documentOut.Add(root);
            documentOut.Save(path + "output8.xml");

            PrintLines(ReadFile(path + "input8.xml"));
            PrintLines(ReadFile(path + "output8.xml"));
        }

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int input;
            do
            {
                Console.WriteLine("0 - Выход \n1 - Задание 05\n2 - Задание 15\n3 - Задание 25\n4 - Задание 35\n5 - Задание 45\n6 - Задание 55\n7 - Задание 65\n8 - Задание 75");
                input = InputInt(0, 8);
                switch (input)
                {
                    case 1:
                        Task1();
                        break;
                    case 2:
                        Task2();
                        break;
                    case 3:
                        Task3();
                        break;
                    case 4:
                        Task4();
                        break;
                    case 5:
                        Task5();
                        break;
                    case 6:
                        Task6();
                        break;
                    case 7:
                        Task7();
                        break;
                    case 8:
                        Task8();
                        break;
                }
            } while (input > 0);
        }

        static int InputInt(int min, int max)
        {
            int number;
            bool inputCheck;
            do
            {
                Console.Write("\nВвод: ");
                inputCheck = int.TryParse(Console.ReadLine(), out number) && number >= min && number <= max;
                if (!inputCheck) Console.WriteLine("Ошибка ввода! Введите целое число в пределах от {0} до {1} (включительно)", min, max);
            } while (!inputCheck);
            return number;
        }

        static List<string> ReadFile(string path)
        {
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        static void PrintLines(List<string> lines)
        {
            foreach (string line in lines) Console.WriteLine(line);
            Console.WriteLine();
        }
    }
}
