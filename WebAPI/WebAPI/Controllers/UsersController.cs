using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult GetCertainNumberOfUsers(int pn, int nu)
        {
            var users = _userRepository.GetCertainNumberOfUsers(pn, nu);

            if(users == null)
                return CreateNotFoundResponse<UserViewModel>();

            return CreateOkResponse(users);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult GetUsersById(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return CreateNotFoundResponse<UserViewModel>();
            }

            return CreateOkResponse(user);
        }

        [HttpGet("{userId:int}/posts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult GetUserPosts(int userId)
        {
            var posts = _userRepository.GetById(userId).Posts;

            return CreateOkResponse(posts);
        }

        [HttpGet("{userId:int}/posts/{postId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult GetUserPostById(int userId, int postId)
        {
            var post = _userRepository.GetById(userId).Posts.FirstOrDefault(post => post.Id == postId);

            return CreateOkResponse(post);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult CreateUser(UserViewModel user)
        {
            var newUserId = _userRepository.CreateNewUser(user);

            return CreateOkResponse(newUserId);
        }

        [HttpDelete("{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult DeleteUser(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                return CreateNotFoundResponse<UserViewModel>();
            }

            _userRepository.Delete(userId);
            return CreateOkResponse(user);
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseBase<UserViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseBase<UserViewModel>))]
        public IActionResult UpdateUser(UserViewModel user)
        {
            var userId = _userRepository.GetById(user.Id);
            if (userId == null)
            {
                return CreateNotFoundResponse<UserViewModel>();
            }

            try
            {
                _userRepository.Update(user);
            }
            catch(Exception)
            {
                return CreateNotFoundResponse<UserViewModel>();
            }

            return CreateOkResponse(user);
        }
    }
}
