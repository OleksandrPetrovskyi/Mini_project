using LasticShopAdo.Models;
using LasticShopAdo.Repository;
using System.Text.RegularExpressions;

namespace LasticShopAdo.Validation
{
    internal class DataValidation : IDataValidation
    {
        IShopRepository repository;
        private Regex emailRegex;

        public DataValidation(IShopRepository repository)
        {
            this.repository = repository;

            emailRegex = new Regex(@"(?<localPart>((\.)?[a-zA-Z0-9]){5,30})(?<domainPart>@(?!\.)((\.)?[a-zA-Z0-9]){2,63})");
        }

        public async Task<BaseResponse<bool>> EmailValidation(string email)
        {
            var baseResponse = new BaseResponse<bool>(true);

            if (!emailRegex.IsMatch(email))
            {
                baseResponse.Data = false;
                await baseResponse.AddError("Error. Input correct E-mail.");
            }
            else if (email[0] == '.')
            {
                baseResponse.Data = false;
                await baseResponse.AddError("You can't make the first character a dot");
            }

            if (baseResponse.IsSuccess == true)
            {
                var user = await repository.GetUserByEmail(email);

                /*if (user.Data == null)
                    await baseResponse.AddError("There is no user with this email.");*/
                if (user.IsSuccess == false)
                {
                    baseResponse.Data = false;
                    await baseResponse.AddErrors(user.Errors);
                }
                else if (user.Data.Email != null)
                {
                    baseResponse.Data = false;
                    await baseResponse.AddError("User with this email already exists.");
                    baseResponse.IsSuccess = true;
                }
                else if (user.Data.Email == null)
                {
                    await baseResponse.AddError("There is no user with this email.");
                }
            }

            return baseResponse;
        }

        public async Task<BaseResponse<bool>> PasswordValidation(string email, string password)
        {
            var baseResponse = new BaseResponse<bool>();

            if (password.Length < 5)
                await baseResponse.AddError("Password is too short");
            else if (password.Length > 30)
                await baseResponse.AddError("Password is too long");

            if (baseResponse.IsSuccess == true)
            {
                var userData = await repository.GetUserByEmail(email);
                baseResponse.Data = true;

                if (userData.IsSuccess == false)
                    baseResponse.Errors = userData.Errors;
                else if (userData.Data.Password != null && password != userData.Data.Password)
                    await baseResponse.AddError("Wrong password");
            }

            return baseResponse;
        }
    }
}

