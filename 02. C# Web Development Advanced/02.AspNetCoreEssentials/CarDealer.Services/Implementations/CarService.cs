namespace CarDealer.Services.Implementations
{
    using CarDealer.Data.Models;
    using Contracts;
    using Data;
    using Models.Cars;
    using Models.Parts;
    using System.Collections.Generic;
    using System.Linq;

    public class CarService : ICarService
    {
        private readonly CarDealerDbContext database;

        public CarService(CarDealerDbContext database)
        {
            this.database = database;
        }

        public IEnumerable<CarServiceModel> GetAllListing(int page = 1, int pageSize = 10)
        {
            return this.database
                .Cars
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CarServiceModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();
        }

        public IEnumerable<CarServiceModel> GetAllByMake(string make)
        {
            return this.database
                .Cars
                .Where(c => c.Make.ToLower() == make.ToLower())
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new CarServiceModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();
        }

        public IEnumerable<CarBasicServiceModel> GetAllBasicCars()
        {
            return this.database
                .Cars
                .Select(c => new CarBasicServiceModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model
                })
                .ToList();
        }

        public CarWithPartsServiceModel GetCarWithPartsById(int id)
        {
            return this.database
                 .Cars
                 .Where(c => c.Id == id)
                 .Select(c => new CarWithPartsServiceModel
                 {
                     Make = c.Make,
                     Model = c.Model,
                     TravelledDistance = c.TravelledDistance,
                     Parts = c.Parts.Select(p => new PartServiceModel
                     {
                         Name = p.Part.Name,
                         Price = p.Part.Price
                     })
                 })
                 .FirstOrDefault();
        }

        public CarBasicServiceModel GetBasicCarById(int id)
        {
            return this.database
                 .Cars
                 .Where(c => c.Id == id)
                 .Select(c => new CarBasicServiceModel
                 {
                     Id = c.Id,
                     Make = c.Make,
                     Model = c.Model,
                 })
                 .FirstOrDefault();
        }

        public void Create(string make, string model, long travelledDistance, IEnumerable<int> partsIds)
        {
            IEnumerable<int> existingPartsIds = this.database
                .Parts
                .Where(p => partsIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToList();

            Car car = new Car
            {
                Make = make,
                Model = model,
                TravelledDistance = travelledDistance,
            };

            IList<PartCar> parts = new List<PartCar>();

            foreach (int partId in existingPartsIds)
            {
                PartCar part = new PartCar
                {
                    PartId = partId,
                    CarId = car.Id
                };

                parts.Add(part);
            }

            car.Parts = parts;

            this.database.Cars.Add(car);
            this.database.SaveChanges();
        }

        public bool IsCarExisting(int id)
        {
            return this.database.Cars.Any(c => c.Id == id);
        }

        public double TotalCarsCount()
        {
            return this.database.Cars.Count();
        }
    }
}