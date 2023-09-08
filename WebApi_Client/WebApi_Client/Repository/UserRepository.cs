using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApi_Client.Models;
using WebApi_Client.Responses;

namespace WebApi_Client.Repository
{
    internal class UserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _usersUrl;

        public UserRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _usersUrl = ConfigurationManager.AppSettings.Get("webApiUsersUrl");
        }

        public async Task<ClientResponse<List<User>>> GetAllUsers(int pageNumber, int numberOfUsers)
        {
            HttpResponseMessage response;
            var _responseData = new ClientResponse<List<User>>();

            try
            {
                response = await _httpClient.GetAsync($"{_usersUrl}?pn={pageNumber}&nu={numberOfUsers}");
            }
            catch (HttpRequestException)
            {
                _responseData.ServerIsRunning = false;
                _responseData.Success = false;
                _responseData.Errors.Add($"Sorry, but the server is down at the moment. {Environment.NewLine}Restoration work is underway.");

                return _responseData;
            }
            catch (Exception ex)
            {
                _responseData.Success = false;
                _responseData.Errors.Add(ex.Message);

                return _responseData;
            }

            var contentString = await response.Content.ReadAsStringAsync();
            var usersResponse = JsonConvert.DeserializeObject<APIResponse<List<User>>>(contentString);

            if(usersResponse == null)
                _responseData.Errors.Add("No more users");
            else
                _responseData.Data = usersResponse.Data;

            return _responseData;
        }
        public async Task<ClientResponse<User>> GetUserById(int userId)
        {
            HttpResponseMessage response;
            ClientResponse<User> _responseData = new ClientResponse<User>();

            try
            {
                response = await _httpClient.GetAsync($"{_usersUrl}/{userId}");
            }
            catch (HttpRequestException)
            {
                _responseData.ServerIsRunning = false;
                _responseData.Success = false;
                _responseData.Errors.Add($"Sorry, but the server is down at the moment. {Environment.NewLine}Restoration work is underway.");

                return _responseData;
            }
            catch (Exception ex)
            {
                _responseData.Success = false;
                _responseData.Errors.Add(ex.Message);

                return _responseData;
            }

            var contentString = await response.Content.ReadAsStringAsync();
            var usersResponse = JsonConvert.DeserializeObject<APIResponse<User>>(contentString);
            
            if (usersResponse == null)
                _responseData.Errors.Add("No more users");
            else
                _responseData.Data = usersResponse.Data;

            return _responseData;
        }
        public async Task<string> CreateNewUser(User user)
        {
            HttpResponseMessage response;

            var serializedContent = JsonConvert.SerializeObject(user);
            var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            
            try
            {
                response = await _httpClient.PostAsync(_usersUrl, content);
            }
            catch (HttpRequestException)
            {
                return $"Error {HttpStatusCode.ServiceUnavailable}";
            }

            if (response.StatusCode == HttpStatusCode.OK)
                return $"Successfully! User created.";
            else
                return $"Error {response.StatusCode}";
        }
        public async Task<string> UpdateUser(User user)
        {
            HttpResponseMessage response;

            var serializedContent = JsonConvert.SerializeObject(user);
            var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            
            try
            {
                response = await _httpClient.PutAsync(_usersUrl, content);
            }
            catch (HttpRequestException)
            {
                return $"Error {HttpStatusCode.ServiceUnavailable}";
            }

            if (response.StatusCode == HttpStatusCode.OK)
                return $"Successfully! User data updated.";
            else
                return $"Error {response.StatusCode}";
        }
        public async Task<string> DeleteUser(int userId)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.DeleteAsync($"{_usersUrl}/{userId}");
            }
            catch (HttpRequestException)
            {
                return $"Error {HttpStatusCode.ServiceUnavailable}";
            }

            if (response.StatusCode == HttpStatusCode.OK)
                return $"Successfully! User with id {userId} deleted.";
            else
                return $"Error {response.StatusCode}";
        }
    }
}
