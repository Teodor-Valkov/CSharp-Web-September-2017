using AutoMapper;
using LearningSystem.Common.Mapping;
using LearningSystem.Data.Models;
using LearningSystem.Data.Models.Enums;
using System;
using System.Linq;

namespace LearningSystem.Services.Models.Courses
{
    public class CourseCertificateServiceModel : IMapFrom<Course>, IHaveCustomMapping
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Student { get; set; }

        public Grade Grade { get; set; }

        public string Trainer { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            string studentId = null;

            mapper
             .CreateMap<Course, CourseCertificateServiceModel>()
                .ForMember(dest => dest.Trainer, opt => opt.MapFrom(src => src.Trainer.Name))
            .ForMember(dest => dest.Student, opt => opt
                .MapFrom(src => src
                    .Students
                    .Where(s => s.StudentId == studentId)
                    .Select(s => s.Student.Name)
                    .FirstOrDefault()))
            .ForMember(dest => dest.Grade, opt => opt
                .MapFrom(src => src
                    .Students
                    .Where(s => s.StudentId == studentId)
                    .Select(s => s.Grade)
                    .FirstOrDefault()));
        }
    }
}