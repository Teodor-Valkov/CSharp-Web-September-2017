namespace LearningSystem.Web.Controllers
{
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Courses;
    using Services.Contracts;
    using Services.Models.Courses;
    using Services.Trainer.Contracts;
    using System.Threading.Tasks;
    using Web.Models;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class CoursesController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ICourseService courseService;
        private readonly ITrainerService trainerService;

        public CoursesController(UserManager<User> userManager, ICourseService courseService, ITrainerService trainerService)
        {
            this.userManager = userManager;
            this.courseService = courseService;
            this.trainerService = trainerService;
        }

        public async Task<IActionResult> Index(string searchToken = null, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagesViewModel<CourseBasicServiceModel> model = new PagesViewModel<CourseBasicServiceModel>
            {
                Elements = await this.courseService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.courseService.TotalCountAsync(searchToken),
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

        public async Task<IActionResult> Details(int id, string name)
        {
            CourseDetailsViewModel model = new CourseDetailsViewModel
            {
                Course = await this.courseService.GetByIdAsync<CourseDetailsServiceModel>(id)
            };

            if (model.Course == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                string userId = this.userManager.GetUserId(User);

                model.IsStudentEnrolledInCourse = await this.courseService.IsUserEnrolledInCourseAsync(userId, id);
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitExamSubmission(int id, IFormFile examSubmission)
        {
            CourseBasicServiceModel course = await this.courseService.GetByIdAsync<CourseBasicServiceModel>(id);

            if (!examSubmission.FileName.EndsWith(".zip") || examSubmission.Length > ExamSubmissionFileLength)
            {
                TempData.AddErrorMessage("Your submission should be a '.zip' file with no more than 2 MB in size.");

                return RedirectToAction(nameof(Details), new { id, course.Name });
            }

            byte[] examSubmisionContents = await examSubmission.ToByteArray();

            string userId = this.userManager.GetUserId(User);

            bool submitResult = await this.courseService.SaveExamSubmissionAsync(id, userId, examSubmisionContents);

            if (!submitResult)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage("Exam submission uploaded successfully.");

            return RedirectToAction(nameof(Details), new { id, name = course.Name });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SignUp(int id)
        {
            CourseBasicServiceModel course = await this.courseService.GetByIdAsync<CourseBasicServiceModel>(id);

            string userId = this.userManager.GetUserId(User);

            bool isUserTrainer = await this.trainerService.IsUserTrainerAsync(id, userId);

            if (isUserTrainer)
            {
                TempData.AddErrorMessage("You cannot sign up for course where you are a trainer.");

                return RedirectToAction(nameof(Details), new { id, name = course.Name });
            }

            bool signUpResult = await this.courseService.SignUpStudentAsync(userId, id);

            if (!signUpResult)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage($"You have successfully signed up for '{course.Name}'.");

            return RedirectToAction(nameof(Details), new { id, name = course.Name });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SignOut(int id)
        {
            string userId = this.userManager.GetUserId(User);

            bool signOutResult = await this.courseService.SignOutStudentAsync(userId, id);

            if (!signOutResult)
            {
                return BadRequest();
            }

            CourseBasicServiceModel course = await this.courseService.GetByIdAsync<CourseBasicServiceModel>(id);
            TempData.AddSuccessMessage($"You have successfully signed out from '{course.Name}'.");

            return RedirectToAction(nameof(Details), new { id, name = course.Name });
        }
    }
}