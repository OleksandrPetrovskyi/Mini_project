using LasticShopAdo.Interfaces;
using LasticShopAdo.Models;
using LasticShopAdo.Repository;
using LasticShopAdo.Validation;
using System.ComponentModel;

namespace LasticShopAdo
{
    internal class Startup
    {
        IUserInterface userInterface;
        IShopRepository repository;
        StringСonverter stringСonverter;
        IDataValidation validation;

        public Startup(IUserInterface userInterface, IShopRepository repository, StringСonverter stringСonverter, IDataValidation validation)
        {
            this.userInterface = userInterface;
            this.repository = repository;
            this.stringСonverter = stringСonverter;
            this.validation = validation;
        }

        public async Task Run()
        {
            var mainUser = new User();
            string userAnswer;

            userInterface.Message("\t\tWelcom");

            do
            {
                userInterface.Message("Do you have account? Yes(1), No(2)");
                userAnswer = userInterface.Input();

                if (userAnswer.Contains('1'))
                {
                    var userData = await AccountLogin();

                    if (userData.IsSuccess == false)
                        userInterface.Error(stringСonverter.ListToString(userData.Errors));
                    else
                        mainUser = userData.Data;
                }
                else if (userAnswer.Contains('2'))
                {
                    var userData = await CreateNewAccount();

                    if (userData.IsSuccess == false)
                        userInterface.Error(stringСonverter.ListToString(userData.Errors));
                    else
                        mainUser = userData.Data;
                }
            }
            while (mainUser.Email == null);

            do
            {
                userInterface.Message("Do you want to create a new lot (1) or watch any product (2)?");
                userAnswer = userInterface.Input();

                if (userAnswer.Contains('1'))
                    await CreateNewProduct(mainUser);
                else if (userAnswer.Contains('2'))
                    await SelectProduct(mainUser);
            }
            while (true);
        }


        async Task<BaseResponse<User>> AccountLogin()
        {
            string password;
            string email = string.Empty;
            var check = new BaseResponse<bool>() { IsSuccess = false };

            while (check.IsSuccess == false)
            {
                userInterface.Message($"Enter your email.");
                email = userInterface.Input();
                check = await validation.EmailValidation(email);

                if (check.IsSuccess == false)
                {
                    userInterface.Error(stringСonverter.ListToString(check.Errors));

                    userInterface.Message("Want to create a new account? Yes(1) No(2)");
                    if (userInterface.Input().Contains('1'))
                        return new BaseResponse<User> { IsSuccess = false };
                }
            }

            check.IsSuccess = false;

            while (check.IsSuccess == false)
            {
                userInterface.Message($"Enter your password.");
                password = userInterface.Input();
                check = await validation.PasswordValidation(email, password);

                userInterface.Error(stringСonverter.ListToString(check.Errors));
            }

            var user = await repository.AccountLogin(email);

            return new BaseResponse<User>(user.Data);
        }

        async Task<BaseResponse<User>> CreateNewAccount()
        {
            var baseResponse = new BaseResponse<User>();
            var userData = new User();
            var check = new BaseResponse<bool>();

            while (check.Data == false || userData.Email.Length > 30)
            {
                userInterface.Message("Input your email.");
                userData.Email = userInterface.Input();
                check = await validation.EmailValidation(userData.Email);

                if (check.Data == false)
                {
                    userInterface.Error(stringСonverter.ListToString(check.Errors));

                    userInterface.Message("Want to login to your existing account?? Yes(1) No(2)");
                    if (userInterface.Input().Contains('1'))
                        return new BaseResponse<User> { IsSuccess = false };
                }
            }


            check.IsSuccess = false;

            while (check.IsSuccess == false || userData.Password.Length > 30)
            {
                userInterface.Message("Input your password.");
                userData.Password = userInterface.Input();
                check = await validation.PasswordValidation(userData.Email, userData.Password);

                if (check.Data == false)
                    userInterface.Error(stringСonverter.ListToString(check.Errors));
            }

            userData.FirstName = FillInRequiredStringFields("Input your first name", 15);

            userInterface.Message("Input your second name");
            userData.SecondName = CheckMaxStrLength(15);

            var responseWithUserId = await repository.CreateNewUser(userData);

            if (responseWithUserId.IsSuccess == false)
                await baseResponse.AddErrors(responseWithUserId.Errors);
            else
            {
                baseResponse.Data = userData;
                baseResponse.Data.Id = responseWithUserId.Data;
            }

            return baseResponse;
        }

        async Task SelectProduct(User mainUser)
        {
            const string productPage = "Go to product page (1)";
            const string buyProduct = "Buy product (2)";
            const string goToSellerPage = "Go to seller page (3)";
            const string writeReview = "Write a review (4)";
            const string anotherProducts = "See another products from this category? (5) ";
            const string anotherCategory = "Choose another category (6)";
            const string mainMenu = "Exit to main menu (7)";

            int productId = 0;
            string userAnswer = string.Empty;
            var productIDs = new List<int>();
            var product = new BaseResponse<Product>();

            do
            {
                userInterface.Message("Which product category are you interested in?");
                var categoryResult = await repository.GetProductCategories();

                if (categoryResult.IsSuccess == false)
                {
                    userInterface.Error(stringСonverter.ListToString(categoryResult.Errors));
                    continue;
                }

                do
                {
                    string categories;

                    do
                    {
                        categories = stringСonverter.ListToString(categoryResult.Data);
                        userInterface.Message(categories);
                        userInterface.Message("Choose one category from the available");
                        userAnswer = userInterface.Input();
                    }
                    while (!categories.Contains(userAnswer));

                    var productsInSelectCategory = await repository.GetProductsByCategory(userAnswer);

                    if (productsInSelectCategory.IsSuccess == false)
                    {
                        userInterface.Error(stringСonverter.ListToString(productsInSelectCategory.Errors));
                        break;
                    }

                    productIDs.Clear();
                    foreach (var productInCategory in productsInSelectCategory.Data)
                    {
                        productIDs.Add(productInCategory.Id);
                    }

                    do
                    {
                        foreach (var productInCategory in productsInSelectCategory.Data)
                        {
                            userInterface.Message($"{stringСonverter.ShortProductInfo(productInCategory)}{Environment.NewLine}");
                        }

                        userInterface.Message(@$"Do you want to ...
{productPage}
{buyProduct}
{anotherCategory}
{mainMenu}");
                        userAnswer = userInterface.Input();

                        if (userAnswer.Contains('1'))
                        {
                            product = await GoToProductPage(productIDs);

                            if (product.IsSuccess == false)
                            {
                                userInterface.Error(stringСonverter.ListToString(product.Errors));
                                continue;
                            }

                            product.Data = product.Data;
                            userInterface.Message($"{Environment.NewLine}{stringСonverter.FullProductInfo(product.Data)}");


                            userInterface.Message(@$"Do you want to ...
{buyProduct}
{goToSellerPage}
{writeReview}
{anotherProducts} 
{anotherCategory}
{mainMenu}");
                            userAnswer = userInterface.Input();
                        }

                        if (userAnswer.Contains('2'))
                        {
                            await BuyProduct(productIDs);

                            userInterface.Message(@$"Do you want to ...
{goToSellerPage}
{writeReview}
{anotherProducts} 
{anotherCategory}
{mainMenu}");
                            userAnswer = userInterface.Input();
                        }

                        if (userAnswer.Contains('3'))
                        {
                            var userResult = await repository.GetUserById(product.Data.UserId);

                            if (userResult.IsSuccess == false)
                                userInterface.Error(stringСonverter.ListToString(userResult.Errors));
                            else
                                userInterface.Message($"{Environment.NewLine}{stringСonverter.FullUserToString(userResult.Data)}{Environment.NewLine}");

                            userInterface.Message(@$"Do you want to ...
{writeReview}
{anotherProducts} 
{anotherCategory}
{mainMenu}");
                            userAnswer = userInterface.Input();
                        }

                        if (userAnswer.Contains('4'))
                        {
                            await CreateNewReview(mainUser.Id, product.Data.Id);

                            userInterface.Message(@$"Do you want to ...
{anotherProducts} 
{anotherCategory}
{mainMenu}");
                            userAnswer = userInterface.Input();
                        }
                    }
                    while (userAnswer.Contains('5'));
                }
                while (userAnswer.Contains('6'));
            }
            while (userAnswer.Contains('7'));
        }

        async Task CreateNewProduct(User seller)
        {
            var product = new Product();
            product.UserId = seller.Id;

            product.Category = FillInRequiredStringFields("Input product's category.", 30);
            product.Title = FillInRequiredStringFields("Input product's title.", 50);
            product.Price = FillInRequiredDecimalField("Input product's price.");
            product.Valuta = FillInRequiredStringFields("Input valuta.", 4);
            product.Balance = FillInRequiredIntField("Input product's balance.");

            userInterface.Message("Input product's description.");
            product.Description = CheckMaxStrLength(300);

            var productResult = await repository.CreateNewProduct(product);

            if (productResult.IsSuccess == false)
                userInterface.Error("Your lot could not be created.");
            else
                userInterface.Message("Your lot has been successfully created.");
        }

        async Task CreateNewReview(int userId, int productId)
        {
            var review = new Review
            {
                ProductId = productId,
                UserId = userId
            };

            do
            {
                userInterface.Message("Specify the rating of the product");
                review.Evalution = userInterface.InputIntData();
            }
            while (review.Evalution < 0 || review.Evalution > 5);

            userInterface.Message("Justify your rating (optional) for the product");
            review.Description = CheckMaxStrLength(200);

            var reviewInformation = await repository.CreatNewReview(review);

            if (reviewInformation.IsSuccess == false)
                userInterface.Error("Your review could not be created.");
            else
                userInterface.Message("Your review has been successfully created");
        }

        async Task<BaseResponse<Product>> GoToProductPage(List<int> productIDs)
        {
            BaseResponse<Product> productResult;
            int productId;

            do
            {
                userInterface.Message(@"
What item do you want to see?
To go to the product page, write its ID.");

                productId = userInterface.InputIntData();
                productResult = await repository.GetProductById(productId);
            }
            while (!productIDs.Contains(productId));

            return productResult;
        }

        async Task BuyProduct(List<int> productIDs)
        {
            BaseResponse<Product> product;
            int productId;
            string userAnswer = "1";

            do
            {
                do
                {
                    userInterface.Message("Write the id of the product you want to buy");
                    productId = userInterface.InputIntData();
                }
                while (!productIDs.Contains(productId));

                userInterface.Message("Input the quantity of products you want to buy");
                int quantityOfProducts = 0;

                for (var check = false; check == false;)
                    check = int.TryParse(userInterface.Input(), out quantityOfProducts);

                product = await repository.GetProductById(productId);

                if (product.IsSuccess == false)
                {
                    userInterface.Error(stringСonverter.ListToString(product.Errors));
                    continue;
                }

                var result = await repository.BuyProductById(product.Data, quantityOfProducts);

                if (result.IsSuccess == false)
                    userInterface.Error(stringСonverter.ListToString(result.Errors));
                else
                    userInterface.Message("Conjuration! You did it.");

                userInterface.Message("Do you want to buy more products from this category? yes(1) or no(2)");
                userAnswer = userInterface.Input();
            }
            while (userAnswer.Contains('1'));
        }

        string CheckMaxStrLength(int maxLength)
        {
            string value;
            var check = false;

            do
            {
                value = userInterface.Input();

                if (string.IsNullOrEmpty(value))
                    check = false;
                else if (!string.IsNullOrEmpty(value) && value.Length >= maxLength)
                    check = true;
            }
            while (check);

            return value;
        }
        string FillInRequiredStringFields(string message, int maxLength)
        {
            var value = string.Empty;

            do
            {
                userInterface.Message(message);
                value = userInterface.Input();
            }
            while (string.IsNullOrEmpty(value) || value.Length >= maxLength);

            return value;
        }
        int FillInRequiredIntField(string message)
        {
            var value = 0;
            var check = false;
            do
            {
                check = int.TryParse(FillInRequiredStringFields(message, 10), out value);
            }
            while (check == false);

            return value;
        }
        decimal FillInRequiredDecimalField(string message)
        {
            var value = 0m;
            var check = false;
            do
            {
                check = decimal.TryParse(FillInRequiredStringFields(message, 10), out value);
            }
            while (check == false);

            return value;
        }
    }
}
