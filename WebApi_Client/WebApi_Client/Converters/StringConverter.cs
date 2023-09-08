using System;
using System.Collections.Generic;
using System.Linq;
using WebApi_Client.Models;

namespace WebApi_Client.Converters
{
    internal class StringConverter
    {
        public string ConvertAllPostsInString(User user)
        {
            var result = string.Empty;

            if (user.Posts == null)
            {
                return $@"User has no posts.
{ Environment.NewLine}";
            }

            foreach (var post in user.Posts)
            {
                result += $@"    Post ID: {post.Id}
Title: {post.Title}
Description: {post.Description}
{Environment.NewLine}";
            }

            return result;
        }
        public string ConvertAllUsersInString(List<User> users)
        {
            var result = string.Empty;

            if (users == null)
            {
                return $@"No user data.
{ Environment.NewLine}";
            }

            foreach (var user in users)
            {
                result += $@"User ID: {user.Id}
Name: {user.Name}
Number of posts: ";
                if (user.Posts != null)
                {
                    result += $@"{user.Posts.Count}
{Environment.NewLine}";
                }
                else
                {
                    result += $@"0
{Environment.NewLine}";
                }
            }

            return result;
        }
        public string ConvertOnePostInString(User user, int postId)
        {
            if (user.Posts == null)
            {
                return "User has no posts.";
            }
            if (user.Posts.FirstOrDefault(post => post.Id == postId) == null)
            {
                return $"User does not have a {postId} message.{Environment.NewLine}";
            }

            foreach (var post in user.Posts)
            {
                return $@"Post ID: {post.Id}
Title: {post.Title}
Description: {post.Description} {Environment.NewLine}";
            }

            return "";
        }

    }
}
