namespace LearningSystem.Web.Areas.Trainer.Controllers
{
    using Data.Models;
    using Data.Models.Enums;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Courses;
    using Services.Contracts;
    using Services.Models.Courses;
    using Services.Trainer.Contracts;
    using Services.Trainer.Models.Courses;
    using Services.Trainer.Models.Users;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Web.Models;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(TrainerArea)]
    [Authorize(Roles = TrainerRole)]
    public class TrainerController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ITrainerService trainerService;
        private readonly ICourseService courseService;

        public TrainerController(UserManager<User> userManager, ITrainerService trainerService, ICourseService courseService)
        {
            this.userManager = userManager;
            this.trainerService = trainerService;
            this.courseService = courseService;
        }

        public async Task<IActionResult> Index(string searchToken = null, int page = 1)
        {
            string trainerId = this.userManager.GetUserId(User);

            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagesViewModel<CourseBasicServiceModel> model = new PagesViewModel<CourseBasicServiceModel>
            {
                Elements = await this.trainerService.GetAllListingByTrainerIdAsync(trainerId, searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.trainerService.TotalCountAsync(trainerId, searchToken),
                    PageSize = CoursePageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            return View(model);
        }

        public async Task<IActionResult> Course(int id)
        {
            string userId = this.userManager.GetUserId(User);

            bool isUserTrainer = await this.trainerService.IsUserTrainerAsync(id, userId);

            if (!isUserTrainer)
            {
                return BadRequest();
            }

            IEnumerable<StudentInCourseServiceModel> students = await this.trainerService.GetStudentsInCourseByCourseIdAsync(id);
            CourseBasicServiceModel course = await this.courseService.GetByIdAsync<CourseBasicServiceModel>(id);

            CourseWithStudentsViewModel model = new CourseWithStudentsViewModel
            {
                Students = students,
                Course = course
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssessStudent(int id, string studentId, Grade grade)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest();
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserTrainer = await this.trainerService.IsUserTrainerAsync(id, userId);

            if (!isUserTrainer)
            {
                return BadRequest();
            }

            bool assessResult = await this.trainerService.AssessStudentAsync(id, studentId, grade);

            if (!assessResult)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Course), new { id });
        }

        public async Task<IActionResult> DownloadExamSubmission(int id, string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest();
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserTrainer = await this.trainerService.IsUserTrainerAsync(id, userId);

            if (!isUserTrainer)
            {
                return BadRequest();
            }

            byte[] examSubmissionContents = await this.trainerService.DownloadExamSubmission(id, studentId);

            if (examSubmissionContents == null)
            {
                this.TempData.AddErrorMessage("This student did not upload exam submission yet.");

                return RedirectToAction(nameof(Course), new { id });
            }

            CourseNameWithStudentNameServiceModel model = await this.trainerService.GetCourseNameAndStudentName(id, studentId);

            if (model == null)
            {
                return BadRequest();
            }

            return File(examSubmissionContents, "application/zip", $"{model.CourseName} - {model.StudentName} - {DateTime.UtcNow.ToString("DD-MM-YYYY")}.zip");
        }
    }
}