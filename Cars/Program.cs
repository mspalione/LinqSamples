using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();


            //CreateXml();
            //QueryXml();

            //var cars = ProcessCars("fuel.csv");
            //var manufacturers = ProcessManufacturers("manufacturers.csv");

            ////aggregate data
            ////query syntax
            //var query = from car in cars
            //            group car by car.Manufacturer into carGroup
            //            select new
            //            {
            //                Name = carGroup.Key,
            //                Max = carGroup.Max(c => c.Combined),
            //                Min = carGroup.Min(c => c.Combined),
            //                Avg = carGroup.Average(c => c.Combined)
            //            } into result
            //            orderby result.Max descending
            //            select result;

            ////method syntax
            //var query2 = cars.GroupBy(c => c.Manufacturer)
            //                .Select(g =>
            //                {
            //                    var results = g.Aggregate(new CarStatistics(),
            //                                    (acc, c) => acc.Accumulate(c),
            //                                    acc => acc.Compute());
            //                    return new
            //                    {
            //                        Name = g.Key,
            //                        Avg = results.Average,
            //                        Min = results.Min,
            //                        Max = results.Max
            //                    };
            //                })
            //                .OrderByDescending(r => r.Max);

            //foreach (var result in query2)    
            //{
            //    Console.WriteLine($"{result.Name}");
            //    Console.WriteLine($"\t Max: {result.Max}");
            //    Console.WriteLine($"\t Min: {result.Min}");
            //    Console.WriteLine($"\t Avg: {result.Avg}");
            //}

            //// query syntax for groupjoin
            //var query = from manufacturer in manufacturers
            //            join car in cars on manufacturer.Name equals car.Manufacturer
            //                into carGroup
            //            orderby manufacturer.Name
            //            select new
            //            {
            //                Manufacturer = manufacturer,
            //                Cars = carGroup
            //            } into result 
            //            group result by result.Manufacturer.Headquarters;


            // method syntax for groupjoin
            //var query2 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
            //            (m, g) =>
            //                new
            //                {
            //                    Manufacturer = m,
            //                    Cars = g
            //                })
            //            .GroupBy(m => m.Manufacturer.Headquarters);
            //            //.OrderBy(m => m.Manufacturer.Headquarters);

            //foreach (var group in query2)
            //{
            //    Console.WriteLine($"{group.Key}");
            //    foreach (var car in group.SelectMany(g => g.Cars)
            //                            .OrderByDescending(c => c.Combined)
            //                            .Take(3))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined}");
            //    }
            //}




            ////group query syntax
            //var query =
            //    from car in cars
            //    group car by car.Manufacturer.ToUpper() into manufacturer
            //    orderby manufacturer.Key
            //    select manufacturer;

            ////group method syntax
            //var query2 =
            //    cars.GroupBy(c => c.Manufacturer.ToUpper())
            //        .OrderBy(g => g.Key);

            //foreach (var group in query)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined}");
            //    }
            //}

            ////join query syntax
            //var query =
            //    from car in cars
            //    join manufacturer in manufacturers
            //        on new { car.Manufacturer, car.Year } 
            //            equals 
            //            new { Manufacturer = manufacturer.Name, manufacturer.Year }
            //    orderby car.Combined descending, car.Name ascending
            //    select new
            //    {
            //        manufacturer.Headquarters,
            //        car.Name,
            //        car.Combined
            //    };

            ////join method syntax
            //var query2 =
            //    cars.Join(manufacturers,
            //                c => new { c.Manufacturer, c.Year },
            //                m => new { Manufacturer = m.Name, m.Year },
            //                (c, m) => new
            //                {
            //                    m.Headquarters,
            //                    c.Name,
            //                    c.Combined
            //                })
            //        .OrderByDescending(c => c.Combined)
            //        .ThenBy(c => c.Name);


            //foreach (var car in query2.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            //}

            //var top = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
            //                .OrderByDescending(c => c.Combined)
            //                .ThenBy(c => c.Name)
            //                .First();

            //Console.WriteLine(top.Name);

            //var result = cars.SelectMany(c => c.Name)
            //                    .OrderBy(c => c);

            //foreach (var name in result)
            //{
            //    Console.WriteLine(name);
            //}

            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Name} : {car.Combined}");
            //}
        }

        private static void QueryData()
        {
            var db = new CarDb();
            //query syntax
            var query = from car in db.Cars
                        orderby car.Combined descending, car.Name ascending
                        select car;

            //method syntax
            var query2 =
                db.Cars.OrderByDescending(c => c.Combined).ThenBy(c => c.Name).Take(10);

            foreach (var car in query2) //query.Take(10) for query syntax
            {
                Console.WriteLine($"{car.Name}: {car.Combined}");
            }
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void QueryXml()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel.xml");

            var query =
                from element in document.Element(ns + "Cars")?.Elements(ex + "Car") ?? Enumerable.Empty<XElement>()
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;

            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXml()
        {
            var records = ProcessCars("fuel.csv");

            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = new XDocument();
            var cars = new XElement(ns + "Cars",

                from record in records
                select new XElement(ex + "Car",
                            new XAttribute("Name", record.Name),
                            new XAttribute("Combined", record.Combined),
                            new XAttribute("Manufacturer", record.Manufacturer)));

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));

            document.Add(cars);
            document.Save("fuel.xml");
        }

        public class CarStatistics
        {
            public CarStatistics()
            {
                Max = Int32.MinValue;
                Min = Int32.MaxValue;
            }

            public CarStatistics Accumulate(Car car)
            {
                Count += 1;
                Total += car.Combined;
                Max = Math.Max(Max, car.Combined);
                Min = Math.Min(Min, car.Combined);

                return this;
            }

            public CarStatistics Compute()
            {
                Average = Total / Count;
                return this;
            }

            public int Max { get; set; }
            public int Min { get; set; }
            public int Total { get; set; }
            public int Count { get; set; }
            public double Average { get; set; }
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .Select(l =>
                    {
                        var columns = l.Split(',');
                        return new Manufacturer
                        {
                            Name = columns[0],
                            Headquarters = columns[1],
                            Year = int.Parse(columns[2])
                        };
                    });

            return query.ToList();
        }

        private static List<Car> ProcessCars(string path)
        {
            var query =
                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToCar();

            return query.ToList();
        }
    }
    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
