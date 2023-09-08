using WebAPI.Models;

namespace WebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly List<UserViewModel> _users;

        public UserRepository()
        {
            _users = new List<UserViewModel>();

            _users.Add(new UserViewModel
            {
                Id = 1,
                Name = "Oleksandr",
                Posts = new List<Posts>
                {
                    new Posts
                    {
                        Id = 1,
                        Title = "First title",
                        Description = "First description"
                    },
                    new Posts
                    {
                        Id = 2,
                        Title = "Second title",
                        Description = "Second description"
                    }
                }
            });
            _users.Add(new UserViewModel
            {
                Id = 2,
                Name = "Sergey"
            });
        }

        public List<UserViewModel> GetCertainNumberOfUsers(int pageNumber, int numberOfUsers)
        {
            List<UserViewModel> users = new List<UserViewModel>();
            if(pageNumber >= _users.Count)
            {
                return null;
            }

            for (int i = pageNumber; i < numberOfUsers && i < _users.Count; i++)
            {
                users.Add(_users[i]);
            }

            return users;
        }
        public UserViewModel GetById(int id)
        {
            var user = _users.FirstOrDefault(user => user.Id == id);

            return user;
        }
        public int CreateNewUser(UserViewModel user)
        {
            var newId = GetNewId();
            user.Id = newId;

            _users.Add(user);

            return newId;
        }
        public void Update(UserViewModel user)
        {
            var oldUser = GetById(user.Id);

            if (oldUser == null)
            {
                throw new Exception($"No user with this ID: {user.Id}");
            }

            if (user.Name != oldUser.Name)
            {
                oldUser.Name = user.Name;
            }

            if (user.Posts != null)
            {
                foreach (var postUser in user.Posts)
                {
                    var postOldUser = oldUser.Posts.FirstOrDefault(post => post.Id == postUser.Id);
                    if (postOldUser == null)
                        oldUser.Posts.Add(postUser);

                    postOldUser.Title = postUser.Title;
                    postOldUser.Description = postUser.Description;
                }
            }
        }
        public void Delete(int id)
        {
            var user = GetById(id);
            _users.Remove(user);
        }

        private int GetNewId()
        {
            var lastId = _users.Max(user => user.Id);
            return ++lastId;
        }
    }
}