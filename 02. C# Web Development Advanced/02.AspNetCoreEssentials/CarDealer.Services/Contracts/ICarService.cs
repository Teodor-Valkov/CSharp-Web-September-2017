namespace CarDealer.Services.Contracts
{
    using Models.Cars;
    using System.Collections.Generic;

    public interface ICarService
    {
        IEnumerable<CarServiceModel> GetAllListing(int page, int pageSize);

        IEnumerable<CarServiceModel> GetAllByMake(string make);

        IEnumerable<CarBasicServiceModel> GetAllBasicCars();

        CarWithPartsServiceModel GetCarWithPartsById(int id);

        CarBasicServiceModel GetBasicCarById(int id);

        void Create(string make, string model, long travelledDistance, IEnumerable<int> partsIds);

        bool IsCarExisting(int id);

        double TotalCarsCount();
    }
}