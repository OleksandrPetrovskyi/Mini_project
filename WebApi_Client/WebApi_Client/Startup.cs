using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi_Client.Converters;
using WebApi_Client.Logger;
using WebApi_Client.Models;
using WebApi_Client.Repository;
using WebApi_Client.Responses;

namespace WebApi_Client
{
    internal class Startup
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;
        private readonly StringConverter _stringConverter;

        public Startup(IUserRepository userRepository, ILogger logger, StringConverter converter)
        {
            _userRepository = userRepository;
            _logger = logger;
            _stringConverter = converter;
        }
        
        public async Task Run()
        {
            _logger.DisplayMessage($"        Welcome!");

            do
            {   
                _logger.DisplayMessage($@"
Press 1 to get a list of all users.
Press 2 to get User by ID.
Press 3 to create a new user.
Press 0 to exit.{Environment.NewLine}
Your choice: ");

                var userResponse = _logger.InputMessage();

                if (userResponse.Contains("1"))
                {
                    ClientResponse<List<User>> users;
                    var index = 0;
                    var amoung = 5;

                    do
                    {
                        users = await _userRepository.GetAllUsers(index, amoung);
                        index += amoung;

                        if(users.Success == true)
                        {
                            _logger.DisplayMessage(_stringConverter.ConvertAllUsersInString(users.Data));
                        }

                        else
                        {
                            _logger.DisplayMessage(Environment.NewLine);

                            foreach (var error in users.Errors)
                            {
                                _logger.DisplayMessage($"{error}{Environment.NewLine}");
                            }
                        }
                    }
                    while (users.Errors == null);
                }

                else if (userResponse.Contains("2"))
                {
                    _logger.DisplayMessage($"Write the user ID you need: ");
                    Int32.TryParse(_logger.InputMessage(), out int userId);

                    var response = _userRepository.GetUserById(userId).Result;
                    if(response.Success == false)
                    {
                        foreach (var error in response.Errors)
                        {
                            _logger.DisplayMessage($"{error}{Environment.NewLine}");
                        }

                        continue;
                    }

                    var user = response.Data;

                    if (user == null)
                    {
                        _logger.DisplayMessage($"There is no user with this ID.{Environment.NewLine}");

                        continue;
                    }

                    _logger.DisplayMessage($@"    User ID: {user.Id}{Environment.NewLine}Name: {user.Name}{Environment.NewLine}");

                    do
                    {
                        _logger.DisplayMessage($@"Press 1 to view all user posts.
Press 2 to view one user post.
Press 3 to update user data.
Press 4 to delete user.
Press 5 to return to main menu.
Your choice: ");

                        userResponse = _logger.InputMessage();

                        if (userResponse.Contains("1"))
                        {
                            _logger.DisplayMessage(_stringConverter.ConvertAllPostsInString(user));
                        }

                        else if (userResponse.Contains("2"))
                        {
                            do
                            {
                                _logger.DisplayMessage("Enter post number: ");
                                int.TryParse(_logger.InputMessage(), out int postId);

                                if (user.Posts.FirstOrDefault(post => post.Id == postId) == null)
                                {
                                    _logger.DisplayMessage($"Error. User does not have a post with {postId} ID.{Environment.NewLine}");
                                }
                                else
                                {
                                    _logger.DisplayMessage($@"{_stringConverter.ConvertOnePostInString(user, postId)}{Environment.NewLine}");
                                }

                                userResponse = UserChoice("1", "2", $@"Press 1 to see another post.
Press 2 to go back.
Your choice: ");
                            }
                            while (userResponse.Contains("1"));
                        }

                        else if (userResponse.Contains("3"))
                        {
                            UpdateUserData(user);
                        }

                        else if (userResponse.Contains("4"))
                        {
                            _logger.DisplayMessage($"{_userRepository.DeleteUser(userId)}{Environment.NewLine}");
                        }
                    } 
                    while (!userResponse.Contains("5"));
                }

                else if(userResponse.Contains("3"))
                {
                    var newUser = new User();

                    InputName(newUser);
                    newUser.Posts = new List<Post>();

                    do
                    {
                        userResponse = UserChoice("1", "2", $@"Press 1 to create a new post.
Press 2 to not create new posts.
Your choice: ");

                        if (userResponse.Contains("2"))
                            break;

                        AddNewPost(newUser);
                    }
                    while (true);

                    _logger.DisplayMessage($"{_userRepository.CreateNewUser(newUser)}{Environment.NewLine}");
                }

                else if(userResponse.Contains("0"))
                {
                    return;
                }
            }
            while (true);
        }

        void InputName(User user)
        {
            var minNameLength = 6;

            do
            {
                _logger.DisplayMessage($@"Username must be longer than {minNameLength} characters and contain no spaces
Enter a new username: ");
                user.Name = _logger.InputMessage();
            }
            while (user.Name.Length < minNameLength || user.Name.Contains(" "));
        }
        void AddNewPost(User user)
        {
            user.Posts.Add(new Post
            {
                Id = user.Posts.Count
            });

            _logger.DisplayMessage("Enter post title: ");
            user.Posts[user.Posts.Count - 1].Title = _logger.InputMessage();

            _logger.DisplayMessage("Enter post description: ");
            user.Posts[user.Posts.Count - 1].Description = _logger.InputMessage();
        }
        string UserChoice(string agreement, string disagreement, string choose)
        {
            if (agreement == null || disagreement == null)
                throw new Exception("Parameter agreement or disagree disagreement");

            do
            {
                _logger.DisplayMessage($"{choose}");
                var response = _logger.InputMessage();

                if (response.Contains(agreement) || response.Contains(disagreement))
                    return response;
            }
            while(true);
        }
        void UpdateUserData(User user)
        {
            var userResponse = string.Empty;

            User updateUser = new User();
            updateUser.Id = user.Id;
            updateUser.Name = user.Name;
            updateUser.Posts = new List<Post>();
            
            do
            {
                _logger.DisplayMessage($@"Press 1 to change username
Press 2 to edit post
Press 3 to create a new post
Press 4 to go back
Your choice: ");
                userResponse = _logger.InputMessage();

                if (userResponse.Contains("1"))
                {
                    InputName(updateUser);

                    _logger.DisplayMessage($"{_userRepository.UpdateUser(updateUser)}{Environment.NewLine}");
                }

                else if (userResponse.Contains("2"))
                {
                    int postId;

                    do
                    {
                        userResponse = String.Empty;

                        _logger.DisplayMessage("Enter post ID:");
                        int.TryParse(_logger.InputMessage(), out postId);

                        if (user.Posts == null || user.Posts.FirstOrDefault(post => post.Id == postId) == null)
                        {
                            userResponse = UserChoice("1", "2", $@"User does not have this post.
Press 1 to re-enter post ID
Press 2 to go back
Your choice: ");

                            if (userResponse.Contains("1"))
                                continue;
                        }
                        break;
                    }
                    while (true);

                    if (userResponse.Contains("2"))
                        break;

                    _logger.DisplayMessage($"You are editing {postId} post{Environment.NewLine}");

                    _logger.DisplayMessage("Enter post title: ");
                    var title = _logger.InputMessage();

                    _logger.DisplayMessage("Enter post description: ");
                    var description = _logger.InputMessage();

                    updateUser.Posts.Add(new Post { Id = postId, Title = title, Description = description});
                    _logger.DisplayMessage($"{_userRepository.UpdateUser(updateUser)}{Environment.NewLine}");
                }

                else if (userResponse.Contains("3"))
                {
                    do
                    {
                        AddNewPost(updateUser);

                        userResponse = UserChoice("1", "2", $@"Press 1 to create another post
Press 2 to stop creating new messages
Your choice: ");

                        _logger.DisplayMessage($"{_userRepository.UpdateUser(updateUser)}{Environment.NewLine}");
                    }
                    while (userResponse.Contains("1"));
                }
            }
            while (!userResponse.Contains("4"));
        }
    }
}
