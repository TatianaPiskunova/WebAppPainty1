using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAppPainty1.Models;

namespace WebAppPainty1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment Web;





        public UserController(ApiContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment web)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            Web = web;
        }

        /// <summary>
        /// Получение списка зарегистрированных пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>

        [Route("GetAllUser")]
        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(_context.Users);
        }

        /// <summary>
        /// Регистрация пользователей. !!!!!!!!!!! пароль (длина не менее 6 знаков, должен включать в себя спец символы, буквы разного регистра, цифры)
        /// </summary>
        /// <param name="model">логин, пароль, email</param>
        /// <returns>Сообщенее об ошибке или успешной регистрации</returns>

        [Route("Register")]
        [HttpPost]
        public async Task<JsonResult> RegistrationUserAsync(RegisterUserModel model)
        {
            var tmpUserName = _context.Users.FirstOrDefault(i => i.UserName == model.UserName);
            if (tmpUserName == null)
            {
                var newUser = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,

                };
                IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(newUser, "user");
                    return new JsonResult("Регистрация прошла успешна");
                 

                }
                else
                {
                    return new JsonResult("Ошибка");
                }

            }
            return new JsonResult("Пользователь с данным именем существует");

        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        /// <param name="model">логин, пароль</param>
        /// <returns>Сообщенее об ошибке или успешной авторизации</returns>

        [Route("Login")]
        [HttpPost]
        public async Task<JsonResult> LoginAsync(LoginUserModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

                if (isPasswordValid)
                {
                    await _signInManager.SignInAsync(user, false);
                    return new JsonResult("Вход осуществлен");
                }
            }
           
                return new JsonResult("Неправильный логин и (или) пароль");
          
        }
        /// <summary>
        /// Выход пользователя из системы
        /// </summary>
        /// <returns>Сообщенее о выходе из системы</returns>
        [Route("LogOut")]
        [HttpGet]
        public async Task<JsonResult> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return new JsonResult("Вы вышли");
        }
        /// <summary>
        /// Отправка запроса на "добавление в друзья". Может осуществить только авторизированный пользователь
        /// </summary>
        /// <param name="model">Логин пользователя</param>
        /// <returns>Сообщение о необходимости регистрации или пользователь не найден или успешной отправке</returns>
        [Route("AddFriend")]
        [HttpPost]
        public JsonResult AddFriend(EnterNameFriendModel model)
        {
            var tmpName = _context.Users.FirstOrDefault(i => i.UserName == model.Name);

            if (User.Identity.Name != null)
            {

                if (tmpName != null)

                {
                    var newFriend = new Friend
                    {
                        NameFirstFriend = User.Identity.Name,
                        NameSecondFriend = model.Name
                    };

                    _context.Friends.Add(newFriend);
                    _context.SaveChanges();

                    return new JsonResult("Запрос в друзья отправлен");



                }
                else return new JsonResult("Пользователь с данным именем НЕ существует");


            }
            return new JsonResult("Необходимо авторизироваться");
        }
        /// <summary>
        /// Список друзей (которым он отправил запросы) авторизированного пользователя
        /// </summary>
        /// <returns>Список друзей или сообщение о необходимости авторизироватся</returns>

        [Route("GetFriendForUser")]
        [HttpGet]
        public JsonResult GetFriendForUser()
        {
            if (User.Identity.Name != null)
            {
                var tmpFriend = _context.Friends.Where(i => i.NameFirstFriend == User.Identity.Name).ToList();

                return new JsonResult(tmpFriend);

            }
            return new JsonResult("Необходимо авторизироватися");


        }
        /// <summary>
        /// Загрузка изображений
        /// </summary>
        /// <param name="model"></param>
        /// <returns>>Cообщение о необходимости авторизироватся или успешной загрузки</returns>
        [Route("AddImage")]
        [HttpPost]
        public JsonResult AddImage([FromForm]UploadFile model)
        {
          
            if (User.Identity.Name != null)
            {

                string fileName = null;
                if (model.Image is not null)
                {
                    string uploadDir = Path.Combine(Web.WebRootPath, "images");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    fileName = Guid.NewGuid().ToString() + "-" + model.Image.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }
                }

                var tmpUser = _context.Users.FirstOrDefault(i => i.UserName == User.Identity.Name);

                var tmpImage = new ImageFile
                {
                    FileName = fileName,
                };
                tmpUser.Images.Add(tmpImage);
                _context.SaveChanges();


                return new JsonResult("Успешно загружено");


            }
            return new JsonResult("Необходимо авторизироваться");
        }
        /// <summary>
        /// Отображение картинок авторизированного пользователя
        /// </summary>
        /// <returns>Список картинок или сообщение о необходимости авторизироваться</returns>
        [Route("ShowImageForUser")]
        [HttpGet]
        public JsonResult ShowImageForUser()
        {
            if (User.Identity.Name != null)
            {
                var tmpImage = _context.Users.Where(i => i.UserName == User.Identity.Name).Select(i => i.Images);

                return new JsonResult(tmpImage);

            }
            return new JsonResult("Необходимо авторизироваться");


        }
        /// <summary>
        /// Отображение картинок друзей авторизированного пользователя, которые приняли запрос "дружбы"
        /// </summary>
        /// <param name="model">Логин пользователя</param>
        /// <returns>Список картинок или сообщение о необходимости авторизироваться или сообщение об ошибке</returns>
        [Route("ShowImageForFriend")]
        [HttpPost]
        public JsonResult ShowImageForFriend(EnterNameFriendModel model)
        {
            if (User.Identity.Name != null)
            {

                var tmpFriendFirst = _context.Friends.FirstOrDefault(i => i.NameFirstFriend == User.Identity.Name && i.NameSecondFriend == model.Name);
                var tmpFriendSecond = _context.Friends.FirstOrDefault(i => i.NameFirstFriend == model.Name && i.NameSecondFriend == User.Identity.Name);

                if (tmpFriendFirst !=null && tmpFriendSecond !=null)
                {
                    var tmpImage = _context.Users.Where(i => i.UserName == model.Name).Select(i => i.Images);
                    return new JsonResult(tmpImage);
                }
                else return new JsonResult("Пользователь с данным именем не принял запрос на дружбу ");

            }
            return new JsonResult("Необходимо авторизироваться");


        }

    }
}
