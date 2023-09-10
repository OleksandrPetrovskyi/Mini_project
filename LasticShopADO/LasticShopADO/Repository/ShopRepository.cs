using LasticShopAdo.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace LasticShopAdo.Repository
{
    internal class ShopRepository : IShopRepository
    {
        private readonly string connectionString;
        public ShopRepository()
        {
            connectionString = @$"Data Source={ConfigurationManager.AppSettings.Get("dataSource")}; Initial Catalog={ConfigurationManager.AppSettings.Get("initialCatalog")};Integrated Security=true";
        }

        public async Task<BaseResponse<User>> GetUserById(int userId)
        {
            var user = new User
            {
                Products = new List<Product?>()
            };

            var queryString = @"Select Users.Id, Users.FirstName, Users.SecondName, Users.Email, 
                Products.Id, Products.Title, Products.Price
                From Users
                Inner Join Products On Users.Id = Products.UserId
                Where Users.Id = @userId";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userId", userId);

                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        if (user.Id == 0)
                        {
                            user.Id = (int)reader[0];
                            user.FirstName = (string)reader[1];
                            user.SecondName = (string)reader[2];
                            user.Email = (string)reader[3];
                        }

                        if (reader[4] != DBNull.Value)
                        {
                            user.Products.Add(new Product
                            {
                                Id = (int)reader[4],
                                Title = (string)reader[5],
                                Price = (decimal)reader[6],
                            });
                        }
                    }

                    await reader.CloseAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<User>(false, new List<string> { ex.Message });
                }
            }

            return new BaseResponse<User>(user);
        }

        public async Task<BaseResponse<User>> GetUserByEmail(string email)
        {
            var user = new User();

            var queryString = @"Select Id, Email, Password From Users
        Where Email=@email";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@email", email);

                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        user.Id = (int)reader[0];
                        user.Email = (string)reader[1];
                        user.Password = (string)reader[2];
                    }

                    await reader.CloseAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<User>(false, new List<string> { ex.Message });
                }
            }

            return new BaseResponse<User>(user);
        }

        public async Task<BaseResponse<User>> AccountLogin(string email)
        {
            var user = new User
            {
                Products = new List<Product?>()
            };

            var userTempData = await GetUserByEmail(email);

            if (userTempData.IsSuccess == false)
                return userTempData;

            var queryString = @"Select Users.Id, Users.FirstName, Users.SecondName, Users.Email, 
                        Products.Id, Products.Title
                        From Users
                        Left Join Products On Users.Id = Products.UserId
                        Where Users.Id = @userId";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userId", userTempData.Data.Id);

                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        user.Id = (int)reader[0];
                        user.FirstName = (string)reader[1];
                        user.SecondName = ConvertFromDbValue<string>(reader[2]);
                        user.Email = (string)reader[3];

                        if (reader[4] != DBNull.Value)
                        {
                            user.Products.Add(new Product
                            {
                                Id = ConvertFromDbValue<int>(reader[4]),
                                Title = ConvertFromDbValue<string>(reader[5]),
                            });
                        }
                    }

                    await reader.CloseAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<User>(false, new List<string> { ex.Message });
                }
            }

            return new BaseResponse<User>(user);
        }

        public async Task<BaseResponse<Product>> GetProductById(int productId)
        {
            var product = new Product
            {
                Reviews = new List<Review?>()
            };

            var queryString = @"Select Users.Id as SellerId,
Products.Id, Products.Title, Products.Description, Products.Price,  Products.Valuta, Products.Balance, Products.Category,
Reviews.UserId as Reviewer, Users.FirstName, Users.SecondName, Reviews.Evalution, Reviews.Description
From (Products
Left Join Users On Products.UserId = Users.Id)
Left Join Reviews On Products.Id = Reviews.ProductId
Left Join Users as Reviewers On Reviewers.Id = Reviews.UserId
Where Products.Id=@productId;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        if (product.Id == 0)
                        {
                            product.UserId = (int)reader[0];
                            product.Id = (int)reader[1];
                            product.Title = (string)reader[2];
                            product.Description = ConvertFromDbValue<string>(reader[3]);
                            product.Price = (decimal)reader[4];
                            product.Valuta = (string)reader[5];
                            product.Balance = (int)reader[6];
                            product.Category = (string)reader[7];
                        }

                        if (reader[8] != DBNull.Value)
                        {
                            product.Reviews.Add(new Review
                            {
                                UserId = (int)reader[8],
                                FirsName = (string)reader[9],
                                SecondName = ConvertFromDbValue<string>(reader[10]),
                                Evalution = (int)reader[11],
                                Description = (string)reader[12]
                            });
                        }
                    }
                    await reader.CloseAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<Product>(false, new List<string> { ex.Message });
                }
            }
            return new BaseResponse<Product>(product);
        }

        public async Task<BaseResponse<List<Product>>> GetProductsByCategory(string category)
        {
            var product = new List<Product>();

            var queryString = @"Select UserId, Id, Title, Price, Valuta From Products
Where Category=@category;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@category", category);

                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        product.Add(new Product()
                        {
                            UserId = (int)reader[0],
                            Id = (int)reader[1],
                            Title = (string)reader[2],
                            Price = (decimal)reader[3],
                            Valuta = (string)reader[4]
                        });
                    }
                    await reader.CloseAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<List<Product>>(false, new List<string> { ex.Message });
                }
            }
            return new BaseResponse<List<Product>>(product);
        }

        public async Task<BaseResponse<List<string>>> GetProductCategories()
        {
            var categories = new List<string>();

            var queryString = @"Select Distinct Category From Products;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);

                try
                {
                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        categories.Add((string)reader[0]);
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse<List<string>>(false, new List<string> { ex.Message });
                }
            }
            return new BaseResponse<List<string>>(categories);
        }

        public async Task<BaseResponse<bool>> BuyProductById(Product product, int quantity)
        {
            if (product.Balance < quantity)
            {
                return new BaseResponse<bool>(false,
                    new List<string>() { "Sorry, but the product is over." });
            }
            else
                product.Balance -= quantity;

            var queryString = @"UPDATE Products
SET Balance = @balance
WHERE Id = @productId;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@productId", product.Id);
                command.Parameters.AddWithValue("@balance", product.Balance);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteReaderAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>(false, new List<string> { ex.Message });
                }
            }
            return new BaseResponse<bool>(true);
        }


        public async Task<BaseResponse<int>> CreateNewProduct(Product product)
        {
            var productId = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                var queryString = @"INSERT INTO Products(Title, Description, Price, Valuta, Balance, Category, UserId)
VALUES
(@title, @description, @price, @valuta, @balance, @category, @userId);";

                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@title", product.Title);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@valuta", product.Valuta);
                command.Parameters.AddWithValue("@balance", product.Balance);
                command.Parameters.AddWithValue("@category", product.Category);
                command.Parameters.AddWithValue("@userId", product.UserId);
                if (string.IsNullOrEmpty(product.Description))
                    command.Parameters.AddWithValue("@description", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@description", product.Description);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteReaderAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<int>(false, new List<string>() { ex.Message });
                }
            }

            return new BaseResponse<int>(productId);
        }

        public async Task<BaseResponse<bool>> CreatNewReview(Review review)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var queryString = @"Insert Into Reviews(UserId, ProductId, Evalution, Description)
values (@userId, @productId, @Evalution, @Description);";

                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userId", review.UserId);
                command.Parameters.AddWithValue("@productId", review.ProductId);
                command.Parameters.AddWithValue("@Evalution", review.Evalution);

                if (string.IsNullOrEmpty(review.Description))
                    command.Parameters.AddWithValue("@Description", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Description", review.Description);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteReaderAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>(false, new List<string>() { ex.Message });
                }
            }

            return new BaseResponse<bool>(true);
        }

        public async Task<BaseResponse<int>> CreateNewUser(User user)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var queryString = @"INSERT INTO Users(FirstName, SecondName, Email, Password)
VALUES (@firstName, @secondName, @email, @password);";

                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@firstName", user.FirstName);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);

                if (string.IsNullOrEmpty(user.SecondName))
                    command.Parameters.AddWithValue("@SecondName", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@SecondName", user.SecondName);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteReaderAsync();
                }
                catch (Exception ex)
                {
                    return new BaseResponse<int>(false, new List<string>() { ex.Message });
                }
            }

            var userByEmail = await GetUserByEmail(user.Email);

            if (userByEmail.IsSuccess == false)
                return new BaseResponse<int>(false, userByEmail.Errors);
            else
                return new BaseResponse<int>(userByEmail.Data.Id);
        }

        private T ConvertFromDbValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
                return default(T);
            else
                return (T)value;
        }
    }
}

