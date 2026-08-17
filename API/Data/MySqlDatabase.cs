using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using MySql.Data.MySqlClient;
using MeuProxySsl.Models;
using ConfigurationModel = MeuProxySsl.Models.Configuration;
using EmailContentModel = MeuProxySsl.Models.EmailContent;

namespace MeuProxySsl.Data
{
    public class MySqlDatabase
    {
        public string ConnectionString =>
            ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString
            ?? ConfigurationManager.AppSettings["MySql:ConnectionString"]
            ?? string.Empty;

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(ConnectionString);

        public MySqlConnection OpenConnection()
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException(
                    "A conexão com MySQL não foi configurada. Defina 'MySqlConnection' ou 'MySql:ConnectionString' no Web.config.");
            }

            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new MySqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                return command.ExecuteScalar();
            }
        }

        public int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new MySqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                return command.ExecuteNonQuery();
            }
        }

        public List<Dictionary<string, object>> Query(string sql, params MySqlParameter[] parameters)
        {
            var rows = new List<Dictionary<string, object>>();

            using (var connection = OpenConnection())
            using (var command = new MySqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var key = reader.GetName(i);
                            var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[key] = value;
                        }

                        rows.Add(row);
                    }
                }
            }

            return rows;
        }

        #region PlataformType CRUD

        public List<PlataformType> GetAllPlataformTypes()
        {
            var query = "SELECT * FROM plataformtype_backup";
            var results = Query(query);
            return results.Select(r => new PlataformType
            {
                Id = r["Id"]?.ToString(),
                Label = r["Label"]?.ToString(),
                Order = r["Order"] != null ? Convert.ToInt32(r["Order"]) : (int?)null,
                IsActive = r["Is_Active"] != null ? Convert.ToBoolean(r["Is_Active"]) : (bool?)null
            }).ToList();
        }

        public PlataformType GetPlataformTypeById(string id)
        {
            var query = "SELECT * FROM plataformtype_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new PlataformType
            {
                Id = result["Id"]?.ToString(),
                Label = result["Label"]?.ToString(),
                Order = result["Order"] != null ? Convert.ToInt32(result["Order"]) : (int?)null,
                IsActive = result["Is_Active"] != null ? Convert.ToBoolean(result["Is_Active"]) : (bool?)null
            };
        }

        public void CreatePlataformType(PlataformType model)
        {
            var query = "INSERT INTO plataformtype_backup (Id, Label, `Order`, Is_Active) VALUES (@Id, @Label, @Order, @IsActive)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id ?? ""),
                new MySqlParameter("@Label", model.Label ?? ""),
                new MySqlParameter("@Order", model.Order ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true)
            );
        }

        public void UpdatePlataformType(PlataformType model)
        {
            var query = "UPDATE plataformtype_backup SET Label = @Label, `Order` = @Order, Is_Active = @IsActive WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id ?? ""),
                new MySqlParameter("@Label", model.Label ?? ""),
                new MySqlParameter("@Order", model.Order ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true)
            );
        }

        public void DeletePlataformType(string id)
        {
            var query = "DELETE FROM plataformtype_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region Position CRUD

        public List<Position> GetAllPositions()
        {
            var query = "SELECT * FROM position_backup";
            var results = Query(query);
            return results.Select(r => new Position
            {
                Id = Convert.ToInt64(r["Id"]),
                Name = r["Name"]?.ToString(),
                Description = r["Description"]?.ToString(),
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                CreatedBy = r["CreatedBy"] != null ? Convert.ToInt32(r["CreatedBy"]) : (int?)null,
                UpdatedOn = r["UpdatedOn"] != null ? Convert.ToDateTime(r["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = r["UpdatedBy"] != null ? Convert.ToInt32(r["UpdatedBy"]) : (int?)null,
                IsActive = r["IsActive"] != null ? Convert.ToBoolean(r["IsActive"]) : (bool?)null
            }).ToList();
        }

        public Position GetPositionById(long id)
        {
            var query = "SELECT * FROM position_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new Position
            {
                Id = Convert.ToInt64(result["Id"]),
                Name = result["Name"]?.ToString(),
                Description = result["Description"]?.ToString(),
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                CreatedBy = result["CreatedBy"] != null ? Convert.ToInt32(result["CreatedBy"]) : (int?)null,
                UpdatedOn = result["UpdatedOn"] != null ? Convert.ToDateTime(result["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = result["UpdatedBy"] != null ? Convert.ToInt32(result["UpdatedBy"]) : (int?)null,
                IsActive = result["IsActive"] != null ? Convert.ToBoolean(result["IsActive"]) : (bool?)null
            };
        }

        public long CreatePosition(Position model)
        {
            var query = "INSERT INTO position_backup (Id, Name, Description, CreatedOn, CreatedBy, IsActive) VALUES (@Id, @Name, @Description, @CreatedOn, @CreatedBy, @IsActive)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Description", model.Description ?? ""),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@CreatedBy", model.CreatedBy ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true)
            );
            return model.Id;
        }

        public void UpdatePosition(Position model)
        {
            var query = "UPDATE position_backup SET Name = @Name, Description = @Description, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy, IsActive = @IsActive WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Description", model.Description ?? ""),
                new MySqlParameter("@UpdatedOn", model.UpdatedOn ?? DateTime.Now),
                new MySqlParameter("@UpdatedBy", model.UpdatedBy ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true)
            );
        }

        public void DeletePosition(long id)
        {
            var query = "DELETE FROM position_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region Role CRUD

        public List<Role> GetAllRoles()
        {
            var query = "SELECT * FROM role_backup";
            var results = Query(query);
            return results.Select(r => new Role
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = r["Name"]?.ToString(),
                Persistent = r["Persistent"] != null ? Convert.ToBoolean(r["Persistent"]) : (bool?)null,
                SS_Key = r["SS_Key"]?.ToString(),
                Espace_Id = r["Espace_Id"] != null ? Convert.ToInt32(r["Espace_Id"]) : (int?)null,
                IsActive = r["Is_Active"] != null ? Convert.ToBoolean(r["Is_Active"]) : (bool?)null,
                Description = r["Description"]?.ToString()
            }).ToList();
        }

        public Role GetRoleById(int id)
        {
            var query = "SELECT * FROM role_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new Role
            {
                Id = Convert.ToInt32(result["Id"]),
                Name = result["Name"]?.ToString(),
                Persistent = result["Persistent"] != null ? Convert.ToBoolean(result["Persistent"]) : (bool?)null,
                SS_Key = result["SS_Key"]?.ToString(),
                Espace_Id = result["Espace_Id"] != null ? Convert.ToInt32(result["Espace_Id"]) : (int?)null,
                IsActive = result["Is_Active"] != null ? Convert.ToBoolean(result["Is_Active"]) : (bool?)null,
                Description = result["Description"]?.ToString()
            };
        }

        public int CreateRole(Role model)
        {
            var query = "INSERT INTO role_backup (Id, Name, Persistent, SS_Key, Espace_Id, Is_Active, Description) VALUES (@Id, @Name, @Persistent, @SS_Key, @Espace_Id, @IsActive, @Description)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Persistent", model.Persistent ?? true),
                new MySqlParameter("@SS_Key", model.SS_Key ?? ""),
                new MySqlParameter("@Espace_Id", model.Espace_Id ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true),
                new MySqlParameter("@Description", model.Description ?? "")
            );
            return model.Id;
        }

        public void UpdateRole(Role model)
        {
            var query = "UPDATE role_backup SET Name = @Name, Persistent = @Persistent, SS_Key = @SS_Key, Espace_Id = @Espace_Id, Is_Active = @IsActive, Description = @Description WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Persistent", model.Persistent ?? true),
                new MySqlParameter("@SS_Key", model.SS_Key ?? ""),
                new MySqlParameter("@Espace_Id", model.Espace_Id ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true),
                new MySqlParameter("@Description", model.Description ?? "")
            );
        }

        public void DeleteRole(int id)
        {
            var query = "DELETE FROM role_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region User CRUD

        public List<User> GetAllUsers()
        {
            LogDebug("GetAllUsers() called");
            try
            {
                LogDebug($"ConnectionString: {(string.IsNullOrEmpty(ConnectionString) ? "EMPTY/NULL" : "***")}");
                LogDebug($"IsConfigured: {IsConfigured}");
                
                var query = "SELECT * FROM user_backup";
                LogDebug($"Query: {query}");
                
                var results = Query(query);
                LogDebug($"Query executed, got {(results?.Count ?? 0)} results");
                
                var users = results.Select(r => new User
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString(),
                    Username = r["Username"]?.ToString(),
                    Password = r["Password"]?.ToString(),
                    Email = r["Email"]?.ToString(),
                    MobilePhone = r["MobilePhone"]?.ToString(),
                    External_Id = r["External_Id"]?.ToString(),
                    Creation_Date = r["Creation_Date"] != null ? Convert.ToDateTime(r["Creation_Date"]) : (DateTime?)null,
                    Last_Login = r["Last_Login"] != null ? Convert.ToDateTime(r["Last_Login"]) : (DateTime?)null,
                    IsActive = r["Is_Active"] != null ? Convert.ToBoolean(r["Is_Active"]) : (bool?)null
                }).ToList();
                
                LogDebug($"Mapped to {users.Count} User objects");
                return users;
            }
            catch (Exception ex)
            {
                LogDebug($"ERROR in GetAllUsers: {ex.GetType().Name}: {ex.Message}\nStack: {ex.StackTrace}");
                throw;
            }
        }

        public User GetUserById(int id)
        {
            var query = "SELECT * FROM user_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new User
            {
                Id = Convert.ToInt32(result["Id"]),
                Name = result["Name"]?.ToString(),
                Username = result["Username"]?.ToString(),
                Password = result["Password"]?.ToString(),
                Email = result["Email"]?.ToString(),
                MobilePhone = result["MobilePhone"]?.ToString(),
                External_Id = result["External_Id"]?.ToString(),
                Creation_Date = result["Creation_Date"] != null ? Convert.ToDateTime(result["Creation_Date"]) : (DateTime?)null,
                Last_Login = result["Last_Login"] != null ? Convert.ToDateTime(result["Last_Login"]) : (DateTime?)null,
                IsActive = result["Is_Active"] != null ? Convert.ToBoolean(result["Is_Active"]) : (bool?)null
            };
        }

        public int CreateUser(User model)
        {
            var query = "INSERT INTO user_backup (Id, Name, Username, Password, Email, MobilePhone, External_Id, Creation_Date, Is_Active) VALUES (@Id, @Name, @Username, @Password, @Email, @MobilePhone, @External_Id, @Creation_Date, @IsActive)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Username", model.Username ?? ""),
                new MySqlParameter("@Password", model.Password ?? ""),
                new MySqlParameter("@Email", model.Email ?? ""),
                new MySqlParameter("@MobilePhone", model.MobilePhone ?? ""),
                new MySqlParameter("@External_Id", model.External_Id ?? ""),
                new MySqlParameter("@Creation_Date", model.Creation_Date ?? DateTime.Now),
                new MySqlParameter("@IsActive", model.IsActive ?? true)
            );
            return model.Id;
        }

        public void UpdateUser(User model)
        {
            var query = "UPDATE user_backup SET Name = @Name, Username = @Username, Password = @Password, Email = @Email, MobilePhone = @MobilePhone, External_Id = @External_Id, Is_Active = @IsActive WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Username", model.Username ?? ""),
                new MySqlParameter("@Password", model.Password ?? ""),
                new MySqlParameter("@Email", model.Email ?? ""),
                new MySqlParameter("@MobilePhone", model.MobilePhone ?? ""),
                new MySqlParameter("@External_Id", model.External_Id ?? ""),
                new MySqlParameter("@IsActive", model.IsActive ?? true)
            );
        }

        public void DeleteUser(int id)
        {
            var query = "DELETE FROM user_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserRole CRUD

        public List<UserRole> GetAllUserRoles()
        {
            var query = "SELECT * FROM user_role_backup";
            var results = Query(query);
            return results.Select(r => new UserRole
            {
                Id = Convert.ToInt32(r["Id"]),
                User_Id = r["User_Id"] != null ? Convert.ToInt32(r["User_Id"]) : (int?)null,
                Role_Id = r["Role_Id"] != null ? Convert.ToInt32(r["Role_Id"]) : (int?)null
            }).ToList();
        }

        public UserRole GetUserRoleById(int id)
        {
            var query = "SELECT * FROM user_role_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserRole
            {
                Id = Convert.ToInt32(result["Id"]),
                User_Id = result["User_Id"] != null ? Convert.ToInt32(result["User_Id"]) : (int?)null,
                Role_Id = result["Role_Id"] != null ? Convert.ToInt32(result["Role_Id"]) : (int?)null
            };
        }

        public int CreateUserRole(UserRole model)
        {
            var query = "INSERT INTO user_role_backup (Id, User_Id, Role_Id) VALUES (@Id, @User_Id, @Role_Id)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@User_Id", model.User_Id ?? 0),
                new MySqlParameter("@Role_Id", model.Role_Id ?? 0)
            );
            return model.Id;
        }

        public void UpdateUserRole(UserRole model)
        {
            var query = "UPDATE user_role_backup SET User_Id = @User_Id, Role_Id = @Role_Id WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@User_Id", model.User_Id ?? 0),
                new MySqlParameter("@Role_Id", model.Role_Id ?? 0)
            );
        }

        public void DeleteUserRole(int id)
        {
            var query = "DELETE FROM user_role_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserAccess CRUD

        public List<UserAccess> GetAllUserAccess()
        {
            var query = "SELECT * FROM useraccess_backup";
            var results = Query(query);
            return results.Select(r => new UserAccess
            {
                Id = Convert.ToInt64(r["Id"]),
                UserId = r["UserId"] != null ? Convert.ToInt32(r["UserId"]) : (int?)null,
                UserPerfilId = r["UserPerfilId"] != null ? Convert.ToInt64(r["UserPerfilId"]) : (long?)null,
                PlataformTypeId = r["PlataformTypeId"]?.ToString(),
                IP = r["IP"]?.ToString(),
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null
            }).ToList();
        }

        public UserAccess GetUserAccessById(long id)
        {
            var query = "SELECT * FROM useraccess_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserAccess
            {
                Id = Convert.ToInt64(result["Id"]),
                UserId = result["UserId"] != null ? Convert.ToInt32(result["UserId"]) : (int?)null,
                UserPerfilId = result["UserPerfilId"] != null ? Convert.ToInt64(result["UserPerfilId"]) : (long?)null,
                PlataformTypeId = result["PlataformTypeId"]?.ToString(),
                IP = result["IP"]?.ToString(),
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null
            };
        }

        public long CreateUserAccess(UserAccess model)
        {
            var query = "INSERT INTO useraccess_backup (Id, UserId, UserPerfilId, PlataformTypeId, IP, CreatedOn) VALUES (@Id, @UserId, @UserPerfilId, @PlataformTypeId, @IP, @CreatedOn)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@UserPerfilId", model.UserPerfilId ?? 0),
                new MySqlParameter("@PlataformTypeId", model.PlataformTypeId ?? ""),
                new MySqlParameter("@IP", model.IP ?? ""),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now)
            );
            return model.Id;
        }

        public void UpdateUserAccess(UserAccess model)
        {
            var query = "UPDATE useraccess_backup SET UserId = @UserId, UserPerfilId = @UserPerfilId, PlataformTypeId = @PlataformTypeId, IP = @IP WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@UserPerfilId", model.UserPerfilId ?? 0),
                new MySqlParameter("@PlataformTypeId", model.PlataformTypeId ?? ""),
                new MySqlParameter("@IP", model.IP ?? "")
            );
        }

        public void DeleteUserAccess(long id)
        {
            var query = "DELETE FROM useraccess_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserAvatar CRUD

        public List<UserAvatar> GetAllUserAvatars()
        {
            var query = "SELECT * FROM useravatar_backup";
            var results = Query(query);
            return results.Select(r => new UserAvatar
            {
                Id = Convert.ToInt64(r["Id"]),
                Name = r["Name"]?.ToString(),
                BinaryData = r["binaryData"] as byte[],
                IsActive = r["IsActive"] != null ? Convert.ToBoolean(r["IsActive"]) : (bool?)null,
                Description = r["Description"]?.ToString(),
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                CreatedBy = r["CreatedBy"] != null ? Convert.ToInt32(r["CreatedBy"]) : (int?)null,
                UpdatedOn = r["UpdatedOn"] != null ? Convert.ToDateTime(r["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = r["UpdatedBy"] != null ? Convert.ToInt32(r["UpdatedBy"]) : (int?)null
            }).ToList();
        }

        public UserAvatar GetUserAvatarById(long id)
        {
            var query = "SELECT * FROM useravatar_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserAvatar
            {
                Id = Convert.ToInt64(result["Id"]),
                Name = result["Name"]?.ToString(),
                BinaryData = result["binaryData"] as byte[],
                IsActive = result["IsActive"] != null ? Convert.ToBoolean(result["IsActive"]) : (bool?)null,
                Description = result["Description"]?.ToString(),
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                CreatedBy = result["CreatedBy"] != null ? Convert.ToInt32(result["CreatedBy"]) : (int?)null,
                UpdatedOn = result["UpdatedOn"] != null ? Convert.ToDateTime(result["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = result["UpdatedBy"] != null ? Convert.ToInt32(result["UpdatedBy"]) : (int?)null
            };
        }

        public long CreateUserAvatar(UserAvatar model)
        {
            var query = "INSERT INTO useravatar_backup (Id, Name, binaryData, IsActive, Description, CreatedOn, CreatedBy) VALUES (@Id, @Name, @BinaryData, @IsActive, @Description, @CreatedOn, @CreatedBy)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@BinaryData", model.BinaryData ?? new byte[0]),
                new MySqlParameter("@IsActive", model.IsActive ?? true),
                new MySqlParameter("@Description", model.Description ?? ""),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@CreatedBy", model.CreatedBy ?? 0)
            );
            return model.Id;
        }

        public void UpdateUserAvatar(UserAvatar model)
        {
            var query = "UPDATE useravatar_backup SET Name = @Name, binaryData = @BinaryData, IsActive = @IsActive, Description = @Description, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@BinaryData", model.BinaryData ?? new byte[0]),
                new MySqlParameter("@IsActive", model.IsActive ?? true),
                new MySqlParameter("@Description", model.Description ?? ""),
                new MySqlParameter("@UpdatedOn", model.UpdatedOn ?? DateTime.Now),
                new MySqlParameter("@UpdatedBy", model.UpdatedBy ?? 0)
            );
        }

        public void DeleteUserAvatar(long id)
        {
            var query = "DELETE FROM useravatar_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserDevice CRUD

        public List<UserDevice> GetAllUserDevices()
        {
            var query = "SELECT * FROM userdevice_backup";
            var results = Query(query);
            return results.Select(r => new UserDevice
            {
                Id = Convert.ToInt64(r["Id"]),
                Version = r["Version"]?.ToString(),
                UUID = r["UUID"]?.ToString(),
                Serial = r["Serial"]?.ToString(),
                Platform = r["Platform"]?.ToString(),
                Model = r["Model"]?.ToString(),
                Manufacturer = r["Manufacturer"]?.ToString(),
                IsVirtual = r["IsVirtual"] != null ? Convert.ToBoolean(r["IsVirtual"]) : (bool?)null,
                GetCordova = r["GetCordova"]?.ToString(),
                DeviceType = r["DeviceType"]?.ToString(),
                UserId = r["UserId"] != null ? Convert.ToInt32(r["UserId"]) : (int?)null,
                UserInitialRegistrationToken = r["UserInitialRegistrationToken"]?.ToString()
            }).ToList();
        }

        public UserDevice GetUserDeviceById(long id)
        {
            var query = "SELECT * FROM userdevice_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserDevice
            {
                Id = Convert.ToInt64(result["Id"]),
                Version = result["Version"]?.ToString(),
                UUID = result["UUID"]?.ToString(),
                Serial = result["Serial"]?.ToString(),
                Platform = result["Platform"]?.ToString(),
                Model = result["Model"]?.ToString(),
                Manufacturer = result["Manufacturer"]?.ToString(),
                IsVirtual = result["IsVirtual"] != null ? Convert.ToBoolean(result["IsVirtual"]) : (bool?)null,
                GetCordova = result["GetCordova"]?.ToString(),
                DeviceType = result["DeviceType"]?.ToString(),
                UserId = result["UserId"] != null ? Convert.ToInt32(result["UserId"]) : (int?)null,
                UserInitialRegistrationToken = result["UserInitialRegistrationToken"]?.ToString()
            };
        }

        public long CreateUserDevice(UserDevice model)
        {
            var query = "INSERT INTO userdevice_backup (Id, Version, UUID, Serial, Platform, Model, Manufacturer, IsVirtual, GetCordova, DeviceType, UserId, UserInitialRegistrationToken) VALUES (@Id, @Version, @UUID, @Serial, @Platform, @Model, @Manufacturer, @IsVirtual, @GetCordova, @DeviceType, @UserId, @UserInitialRegistrationToken)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Version", model.Version ?? ""),
                new MySqlParameter("@UUID", model.UUID ?? ""),
                new MySqlParameter("@Serial", model.Serial ?? ""),
                new MySqlParameter("@Platform", model.Platform ?? ""),
                new MySqlParameter("@Model", model.Model ?? ""),
                new MySqlParameter("@Manufacturer", model.Manufacturer ?? ""),
                new MySqlParameter("@IsVirtual", model.IsVirtual ?? false),
                new MySqlParameter("@GetCordova", model.GetCordova ?? ""),
                new MySqlParameter("@DeviceType", model.DeviceType ?? ""),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@UserInitialRegistrationToken", model.UserInitialRegistrationToken ?? "")
            );
            return model.Id;
        }

        public void UpdateUserDevice(UserDevice model)
        {
            var query = "UPDATE userdevice_backup SET Version = @Version, UUID = @UUID, Serial = @Serial, Platform = @Platform, Model = @Model, Manufacturer = @Manufacturer, IsVirtual = @IsVirtual, GetCordova = @GetCordova, DeviceType = @DeviceType, UserId = @UserId, UserInitialRegistrationToken = @UserInitialRegistrationToken WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Version", model.Version ?? ""),
                new MySqlParameter("@UUID", model.UUID ?? ""),
                new MySqlParameter("@Serial", model.Serial ?? ""),
                new MySqlParameter("@Platform", model.Platform ?? ""),
                new MySqlParameter("@Model", model.Model ?? ""),
                new MySqlParameter("@Manufacturer", model.Manufacturer ?? ""),
                new MySqlParameter("@IsVirtual", model.IsVirtual ?? false),
                new MySqlParameter("@GetCordova", model.GetCordova ?? ""),
                new MySqlParameter("@DeviceType", model.DeviceType ?? ""),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@UserInitialRegistrationToken", model.UserInitialRegistrationToken ?? "")
            );
        }

        public void DeleteUserDevice(long id)
        {
            var query = "DELETE FROM userdevice_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserInfo CRUD

        public List<UserInfo> GetAllUserInfos()
        {
            var query = "SELECT * FROM userinfo_backup";
            var results = Query(query);
            return results.Select(r => new UserInfo
            {
                Id = Convert.ToInt32(r["Id"]),
                Biography = r["Biography"]?.ToString(),
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                CreatedBy = r["CreatedBy"] != null ? Convert.ToInt32(r["CreatedBy"]) : (int?)null,
                UpdatedOn = r["UpdatedOn"] != null ? Convert.ToDateTime(r["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = r["UpdatedBy"] != null ? Convert.ToInt32(r["UpdatedBy"]) : (int?)null,
                IsStatusEmail = r["IsStatusEmail"] != null ? Convert.ToBoolean(r["IsStatusEmail"]) : (bool?)null,
                HasStreamingAccount = r["HasStreamingAccount"] != null ? Convert.ToBoolean(r["HasStreamingAccount"]) : (bool?)null,
                IsCollaborator = r["IsCollaborator"] != null ? Convert.ToBoolean(r["IsCollaborator"]) : (bool?)null,
                BirthDate = r["BirthDate"] != null ? Convert.ToDateTime(r["BirthDate"]) : (DateTime?)null,
                Country = r["Country"]?.ToString(),
                CountryCode = r["CountryCode"]?.ToString(),
                Address = r["Address"]?.ToString()
            }).ToList();
        }

        public UserInfo GetUserInfoById(int id)
        {
            var query = "SELECT * FROM userinfo_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserInfo
            {
                Id = Convert.ToInt32(result["Id"]),
                Biography = result["Biography"]?.ToString(),
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                CreatedBy = result["CreatedBy"] != null ? Convert.ToInt32(result["CreatedBy"]) : (int?)null,
                UpdatedOn = result["UpdatedOn"] != null ? Convert.ToDateTime(result["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = result["UpdatedBy"] != null ? Convert.ToInt32(result["UpdatedBy"]) : (int?)null,
                IsStatusEmail = result["IsStatusEmail"] != null ? Convert.ToBoolean(result["IsStatusEmail"]) : (bool?)null,
                HasStreamingAccount = result["HasStreamingAccount"] != null ? Convert.ToBoolean(result["HasStreamingAccount"]) : (bool?)null,
                IsCollaborator = result["IsCollaborator"] != null ? Convert.ToBoolean(result["IsCollaborator"]) : (bool?)null,
                BirthDate = result["BirthDate"] != null ? Convert.ToDateTime(result["BirthDate"]) : (DateTime?)null,
                Country = result["Country"]?.ToString(),
                CountryCode = result["CountryCode"]?.ToString(),
                Address = result["Address"]?.ToString()
            };
        }

        public int CreateUserInfo(UserInfo model)
        {
            var query = "INSERT INTO userinfo_backup (Id, Biography, CreatedOn, CreatedBy, IsStatusEmail, HasStreamingAccount, IsCollaborator, BirthDate, Country, CountryCode, Address) VALUES (@Id, @Biography, @CreatedOn, @CreatedBy, @IsStatusEmail, @HasStreamingAccount, @IsCollaborator, @BirthDate, @Country, @CountryCode, @Address)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Biography", model.Biography ?? ""),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@CreatedBy", model.CreatedBy ?? 0),
                new MySqlParameter("@IsStatusEmail", model.IsStatusEmail ?? false),
                new MySqlParameter("@HasStreamingAccount", model.HasStreamingAccount ?? false),
                new MySqlParameter("@IsCollaborator", model.IsCollaborator ?? false),
                new MySqlParameter("@BirthDate", model.BirthDate.HasValue ? model.BirthDate.Value : (object)DBNull.Value),
                new MySqlParameter("@Country", model.Country ?? ""),
                new MySqlParameter("@CountryCode", model.CountryCode ?? ""),
                new MySqlParameter("@Address", model.Address ?? "")
            );
            return model.Id;
        }

        public void UpdateUserInfo(UserInfo model)
        {
            var query = "UPDATE userinfo_backup SET Biography = @Biography, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy, IsStatusEmail = @IsStatusEmail, HasStreamingAccount = @HasStreamingAccount, IsCollaborator = @IsCollaborator, BirthDate = @BirthDate, Country = @Country, CountryCode = @CountryCode, Address = @Address WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Biography", model.Biography ?? ""),
                new MySqlParameter("@UpdatedOn", model.UpdatedOn ?? DateTime.Now),
                new MySqlParameter("@UpdatedBy", model.UpdatedBy ?? 0),
                new MySqlParameter("@IsStatusEmail", model.IsStatusEmail ?? false),
                new MySqlParameter("@HasStreamingAccount", model.HasStreamingAccount ?? false),
                new MySqlParameter("@IsCollaborator", model.IsCollaborator ?? false),
                new MySqlParameter("@BirthDate", model.BirthDate.HasValue ? model.BirthDate.Value : (object)DBNull.Value),
                new MySqlParameter("@Country", model.Country ?? ""),
                new MySqlParameter("@CountryCode", model.CountryCode ?? ""),
                new MySqlParameter("@Address", model.Address ?? "")
            );
        }

        public void DeleteUserInfo(int id)
        {
            var query = "DELETE FROM userinfo_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserInitialRegistration CRUD

        public List<UserInitialRegistration> GetAllUserInitialRegistrations()
        {
            var query = "SELECT * FROM userinitialregistration_backup";
            var results = Query(query);
            return results.Select(r => new UserInitialRegistration
            {
                Id = Convert.ToInt64(r["Id"]),
                Status = r["Status"] != null ? Convert.ToBoolean(r["Status"]) : (bool?)null,
                Email = r["Email"]?.ToString(),
                PlataformTypeId = r["PlataformTypeId"]?.ToString(),
                IP = r["IP"]?.ToString(),
                Token = r["Token"]?.ToString(),
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                UpdateOn = r["UpdateOn"] != null ? Convert.ToDateTime(r["UpdateOn"]) : (DateTime?)null,
                RegionName = r["RegionName"]?.ToString(),
                City = r["City"]?.ToString(),
                Country = r["Country"]?.ToString(),
                V_OS = r["v_OS"]?.ToString(),
                V_Browser = r["v_Browser"]?.ToString(),
                Deeplink = r["Deeplink"]?.ToString(),
                Password = r["Password"]?.ToString()
            }).ToList();
        }

        public UserInitialRegistration GetUserInitialRegistrationById(long id)
        {
            var query = "SELECT * FROM userinitialregistration_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserInitialRegistration
            {
                Id = Convert.ToInt64(result["Id"]),
                Status = result["Status"] != null ? Convert.ToBoolean(result["Status"]) : (bool?)null,
                Email = result["Email"]?.ToString(),
                PlataformTypeId = result["PlataformTypeId"]?.ToString(),
                IP = result["IP"]?.ToString(),
                Token = result["Token"]?.ToString(),
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                UpdateOn = result["UpdateOn"] != null ? Convert.ToDateTime(result["UpdateOn"]) : (DateTime?)null,
                RegionName = result["RegionName"]?.ToString(),
                City = result["City"]?.ToString(),
                Country = result["Country"]?.ToString(),
                V_OS = result["v_OS"]?.ToString(),
                V_Browser = result["v_Browser"]?.ToString(),
                Deeplink = result["Deeplink"]?.ToString(),
                Password = result["Password"]?.ToString()
            };
        }

        public long CreateUserInitialRegistration(UserInitialRegistration model)
        {
            var query = "INSERT INTO userinitialregistration_backup (Id, Status, Email, PlataformTypeId, IP, Token, CreatedOn, RegionName, City, Country, v_OS, v_Browser, Deeplink, Password) VALUES (@Id, @Status, @Email, @PlataformTypeId, @IP, @Token, @CreatedOn, @RegionName, @City, @Country, @V_OS, @V_Browser, @Deeplink, @Password)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Status", model.Status ?? false),
                new MySqlParameter("@Email", model.Email ?? ""),
                new MySqlParameter("@PlataformTypeId", model.PlataformTypeId ?? ""),
                new MySqlParameter("@IP", model.IP ?? ""),
                new MySqlParameter("@Token", model.Token ?? ""),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@RegionName", model.RegionName ?? ""),
                new MySqlParameter("@City", model.City ?? ""),
                new MySqlParameter("@Country", model.Country ?? ""),
                new MySqlParameter("@V_OS", model.V_OS ?? ""),
                new MySqlParameter("@V_Browser", model.V_Browser ?? ""),
                new MySqlParameter("@Deeplink", model.Deeplink ?? ""),
                new MySqlParameter("@Password", model.Password ?? "")
            );
            return model.Id;
        }

        public void UpdateUserInitialRegistration(UserInitialRegistration model)
        {
            var query = "UPDATE userinitialregistration_backup SET Status = @Status, Email = @Email, PlataformTypeId = @PlataformTypeId, IP = @IP, Token = @Token, UpdateOn = @UpdateOn, RegionName = @RegionName, City = @City, Country = @Country, v_OS = @V_OS, v_Browser = @V_Browser, Deeplink = @Deeplink, Password = @Password WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Status", model.Status ?? false),
                new MySqlParameter("@Email", model.Email ?? ""),
                new MySqlParameter("@PlataformTypeId", model.PlataformTypeId ?? ""),
                new MySqlParameter("@IP", model.IP ?? ""),
                new MySqlParameter("@Token", model.Token ?? ""),
                new MySqlParameter("@UpdateOn", model.UpdateOn ?? DateTime.Now),
                new MySqlParameter("@RegionName", model.RegionName ?? ""),
                new MySqlParameter("@City", model.City ?? ""),
                new MySqlParameter("@Country", model.Country ?? ""),
                new MySqlParameter("@V_OS", model.V_OS ?? ""),
                new MySqlParameter("@V_Browser", model.V_Browser ?? ""),
                new MySqlParameter("@Deeplink", model.Deeplink ?? ""),
                new MySqlParameter("@Password", model.Password ?? "")
            );
        }

        public void DeleteUserInitialRegistration(long id)
        {
            var query = "DELETE FROM userinitialregistration_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserPasswordRecovery CRUD

        public List<UserPasswordRecovery> GetAllUserPasswordRecoveries()
        {
            var query = "SELECT * FROM userpasswordrecovery_backup";
            var results = Query(query);
            return results.Select(r => new UserPasswordRecovery
            {
                Id = Convert.ToInt64(r["Id"]),
                UserId = r["UserId"] != null ? Convert.ToInt32(r["UserId"]) : (int?)null,
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                IsValid = r["IsValid"] != null ? Convert.ToBoolean(r["IsValid"]) : (bool?)null
            }).ToList();
        }

        public UserPasswordRecovery GetUserPasswordRecoveryById(long id)
        {
            var query = "SELECT * FROM userpasswordrecovery_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserPasswordRecovery
            {
                Id = Convert.ToInt64(result["Id"]),
                UserId = result["UserId"] != null ? Convert.ToInt32(result["UserId"]) : (int?)null,
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                IsValid = result["IsValid"] != null ? Convert.ToBoolean(result["IsValid"]) : (bool?)null
            };
        }

        public long CreateUserPasswordRecovery(UserPasswordRecovery model)
        {
            var query = "INSERT INTO userpasswordrecovery_backup (Id, UserId, CreatedOn, IsValid) VALUES (@Id, @UserId, @CreatedOn, @IsValid)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@IsValid", model.IsValid ?? true)
            );
            return model.Id;
        }

        public void UpdateUserPasswordRecovery(UserPasswordRecovery model)
        {
            var query = "UPDATE userpasswordrecovery_backup SET UserId = @UserId, IsValid = @IsValid WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@IsValid", model.IsValid ?? true)
            );
        }

        public void DeleteUserPasswordRecovery(long id)
        {
            var query = "DELETE FROM userpasswordrecovery_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserPerfil CRUD

        public List<UserPerfil> GetAllUserPerfis()
        {
            var query = "SELECT * FROM userperfil_backup";
            var results = Query(query);
            return results.Select(r => new UserPerfil
            {
                Id = Convert.ToInt64(r["Id"]),
                UserId = r["UserId"] != null ? Convert.ToInt32(r["UserId"]) : (int?)null,
                IsActive = r["IsActive"] != null ? Convert.ToBoolean(r["IsActive"]) : (bool?)null,
                Name = r["Name"]?.ToString(),
                UserAvatarId = r["UserAvatarId"] != null ? Convert.ToInt64(r["UserAvatarId"]) : (long?)null,
                IsChild = r["IsChild"] != null ? Convert.ToBoolean(r["IsChild"]) : (bool?)null,
                IsMain = r["IsMain"] != null ? Convert.ToBoolean(r["IsMain"]) : (bool?)null,
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                DeletedOn = r["DeletedOn"] != null ? Convert.ToDateTime(r["DeletedOn"]) : (DateTime?)null
            }).ToList();
        }

        public UserPerfil GetUserPerfilById(long id)
        {
            var query = "SELECT * FROM userperfil_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserPerfil
            {
                Id = Convert.ToInt64(result["Id"]),
                UserId = result["UserId"] != null ? Convert.ToInt32(result["UserId"]) : (int?)null,
                IsActive = result["IsActive"] != null ? Convert.ToBoolean(result["IsActive"]) : (bool?)null,
                Name = result["Name"]?.ToString(),
                UserAvatarId = result["UserAvatarId"] != null ? Convert.ToInt64(result["UserAvatarId"]) : (long?)null,
                IsChild = result["IsChild"] != null ? Convert.ToBoolean(result["IsChild"]) : (bool?)null,
                IsMain = result["IsMain"] != null ? Convert.ToBoolean(result["IsMain"]) : (bool?)null,
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                DeletedOn = result["DeletedOn"] != null ? Convert.ToDateTime(result["DeletedOn"]) : (DateTime?)null
            };
        }

        public long CreateUserPerfil(UserPerfil model)
        {
            var query = "INSERT INTO userperfil_backup (Id, UserId, IsActive, Name, UserAvatarId, IsChild, IsMain, CreatedOn) VALUES (@Id, @UserId, @IsActive, @Name, @UserAvatarId, @IsChild, @IsMain, @CreatedOn)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@UserAvatarId", model.UserAvatarId ?? 0),
                new MySqlParameter("@IsChild", model.IsChild ?? false),
                new MySqlParameter("@IsMain", model.IsMain ?? false),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now)
            );
            return model.Id;
        }

        public void UpdateUserPerfil(UserPerfil model)
        {
            var query = "UPDATE userperfil_backup SET UserId = @UserId, IsActive = @IsActive, Name = @Name, UserAvatarId = @UserAvatarId, IsChild = @IsChild, IsMain = @IsMain WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@IsActive", model.IsActive ?? true),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@UserAvatarId", model.UserAvatarId ?? 0),
                new MySqlParameter("@IsChild", model.IsChild ?? false),
                new MySqlParameter("@IsMain", model.IsMain ?? false)
            );
        }

        public void DeleteUserPerfil(long id)
        {
            var query = "DELETE FROM userperfil_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserPicture CRUD

        public List<UserPicture> GetAllUserPictures()
        {
            var query = "SELECT * FROM userpicture_backup";
            var results = Query(query);
            return results.Select(r => new UserPicture
            {
                Id = Convert.ToInt32(r["Id"]),
                BinaryData = r["binaryData"] as byte[],
                Name = r["Name"]?.ToString()
            }).ToList();
        }

        public UserPicture GetUserPictureById(int id)
        {
            var query = "SELECT * FROM userpicture_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserPicture
            {
                Id = Convert.ToInt32(result["Id"]),
                BinaryData = result["binaryData"] as byte[],
                Name = result["Name"]?.ToString()
            };
        }

        public int CreateUserPicture(UserPicture model)
        {
            var query = "INSERT INTO userpicture_backup (Id, binaryData, Name) VALUES (@Id, @BinaryData, @Name)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@BinaryData", model.BinaryData ?? new byte[0]),
                new MySqlParameter("@Name", model.Name ?? "")
            );
            return model.Id;
        }

        public void UpdateUserPicture(UserPicture model)
        {
            var query = "UPDATE userpicture_backup SET binaryData = @BinaryData, Name = @Name WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@BinaryData", model.BinaryData ?? new byte[0]),
                new MySqlParameter("@Name", model.Name ?? "")
            );
        }

        public void DeleteUserPicture(int id)
        {
            var query = "DELETE FROM userpicture_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserPosition CRUD

        public List<UserPosition> GetAllUserPositions()
        {
            var query = "SELECT * FROM userposition_backup";
            var results = Query(query);
            return results.Select(r => new UserPosition
            {
                Id = Convert.ToInt64(r["Id"]),
                UserId = r["UserId"] != null ? Convert.ToInt32(r["UserId"]) : (int?)null,
                PositionId = r["PositionId"] != null ? Convert.ToInt64(r["PositionId"]) : (long?)null,
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                CreatedBy = r["CreatedBy"] != null ? Convert.ToInt32(r["CreatedBy"]) : (int?)null,
                UpdatedOn = r["UpdatedOn"] != null ? Convert.ToDateTime(r["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = r["UpdatedBy"] != null ? Convert.ToInt32(r["UpdatedBy"]) : (int?)null
            }).ToList();
        }

        public UserPosition GetUserPositionById(long id)
        {
            var query = "SELECT * FROM userposition_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserPosition
            {
                Id = Convert.ToInt64(result["Id"]),
                UserId = result["UserId"] != null ? Convert.ToInt32(result["UserId"]) : (int?)null,
                PositionId = result["PositionId"] != null ? Convert.ToInt64(result["PositionId"]) : (long?)null,
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                CreatedBy = result["CreatedBy"] != null ? Convert.ToInt32(result["CreatedBy"]) : (int?)null,
                UpdatedOn = result["UpdatedOn"] != null ? Convert.ToDateTime(result["UpdatedOn"]) : (DateTime?)null,
                UpdatedBy = result["UpdatedBy"] != null ? Convert.ToInt32(result["UpdatedBy"]) : (int?)null
            };
        }

        public long CreateUserPosition(UserPosition model)
        {
            var query = "INSERT INTO userposition_backup (Id, UserId, PositionId, CreatedOn, CreatedBy) VALUES (@Id, @UserId, @PositionId, @CreatedOn, @CreatedBy)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@PositionId", model.PositionId ?? 0),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@CreatedBy", model.CreatedBy ?? 0)
            );
            return model.Id;
        }

        public void UpdateUserPosition(UserPosition model)
        {
            var query = "UPDATE userposition_backup SET UserId = @UserId, PositionId = @PositionId, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@UserId", model.UserId ?? 0),
                new MySqlParameter("@PositionId", model.PositionId ?? 0),
                new MySqlParameter("@UpdatedOn", model.UpdatedOn ?? DateTime.Now),
                new MySqlParameter("@UpdatedBy", model.UpdatedBy ?? 0)
            );
        }

        public void DeleteUserPosition(long id)
        {
            var query = "DELETE FROM userposition_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region UserStatus CRUD

        public List<UserStatus> GetAllUserStatuses()
        {
            var query = "SELECT * FROM userstatus_backup";
            var results = Query(query);
            return results.Select(r => new UserStatus
            {
                Id = Convert.ToInt32(r["Id"]),
                IsOnLine = r["IsOnLine"] != null ? Convert.ToBoolean(r["IsOnLine"]) : (bool?)null,
                UpdateOn = r["UpdateOn"] != null ? Convert.ToDateTime(r["UpdateOn"]) : (DateTime?)null
            }).ToList();
        }

        public UserStatus GetUserStatusById(int id)
        {
            var query = "SELECT * FROM userstatus_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new UserStatus
            {
                Id = Convert.ToInt32(result["Id"]),
                IsOnLine = result["IsOnLine"] != null ? Convert.ToBoolean(result["IsOnLine"]) : (bool?)null,
                UpdateOn = result["UpdateOn"] != null ? Convert.ToDateTime(result["UpdateOn"]) : (DateTime?)null
            };
        }

        public int CreateUserStatus(UserStatus model)
        {
            var query = "INSERT INTO userstatus_backup (Id, IsOnLine, UpdateOn) VALUES (@Id, @IsOnLine, @UpdateOn)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@IsOnLine", model.IsOnLine ?? false),
                new MySqlParameter("@UpdateOn", model.UpdateOn ?? DateTime.Now)
            );
            return model.Id;
        }

        public void UpdateUserStatus(UserStatus model)
        {
            var query = "UPDATE userstatus_backup SET IsOnLine = @IsOnLine, UpdateOn = @UpdateOn WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@IsOnLine", model.IsOnLine ?? false),
                new MySqlParameter("@UpdateOn", model.UpdateOn ?? DateTime.Now)
            );
        }

        public void DeleteUserStatus(int id)
        {
            var query = "DELETE FROM userstatus_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region Configuration CRUD

        public List<ConfigurationModel> GetAllConfigurations()
        {
            var query = "SELECT * FROM configurations_backup";
            var results = Query(query);
            return results.Select(r => new ConfigurationModel
            {
                Id = Convert.ToInt64(r["Id"]),
                Name = r["Name"]?.ToString(),
                Description = r["Description"]?.ToString(),
                Value = r["Value"]?.ToString(),
                CreatedOn = r["CreatedOn"] != null ? Convert.ToDateTime(r["CreatedOn"]) : (DateTime?)null,
                CreatedBy = r["CreatedBy"] != null ? Convert.ToInt32(r["CreatedBy"]) : (int?)null,
                UpdateOn = r["UpdateOn"] != null ? Convert.ToDateTime(r["UpdateOn"]) : (DateTime?)null,
                UpdateBy = r["UpdateBy"] != null ? Convert.ToInt32(r["UpdateBy"]) : (int?)null
            }).ToList();
        }

        public ConfigurationModel GetConfigurationById(long id)
        {
            var query = "SELECT * FROM configurations_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new ConfigurationModel
            {
                Id = Convert.ToInt64(result["Id"]),
                Name = result["Name"]?.ToString(),
                Description = result["Description"]?.ToString(),
                Value = result["Value"]?.ToString(),
                CreatedOn = result["CreatedOn"] != null ? Convert.ToDateTime(result["CreatedOn"]) : (DateTime?)null,
                CreatedBy = result["CreatedBy"] != null ? Convert.ToInt32(result["CreatedBy"]) : (int?)null,
                UpdateOn = result["UpdateOn"] != null ? Convert.ToDateTime(result["UpdateOn"]) : (DateTime?)null,
                UpdateBy = result["UpdateBy"] != null ? Convert.ToInt32(result["UpdateBy"]) : (int?)null
            };
        }

        public long CreateConfiguration(ConfigurationModel model)
        {
            var query = "INSERT INTO configurations_backup (Id, Name, Description, Value, CreatedOn, CreatedBy) VALUES (@Id, @Name, @Description, @Value, @CreatedOn, @CreatedBy)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Description", model.Description ?? ""),
                new MySqlParameter("@Value", model.Value ?? ""),
                new MySqlParameter("@CreatedOn", model.CreatedOn ?? DateTime.Now),
                new MySqlParameter("@CreatedBy", model.CreatedBy ?? 0)
            );
            return model.Id;
        }

        public void UpdateConfiguration(ConfigurationModel model)
        {
            var query = "UPDATE configurations_backup SET Name = @Name, Description = @Description, Value = @Value, UpdateOn = @UpdateOn, UpdateBy = @UpdateBy WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Description", model.Description ?? ""),
                new MySqlParameter("@Value", model.Value ?? ""),
                new MySqlParameter("@UpdateOn", model.UpdateOn ?? DateTime.Now),
                new MySqlParameter("@UpdateBy", model.UpdateBy ?? 0)
            );
        }

        public void DeleteConfiguration(long id)
        {
            var query = "DELETE FROM configurations_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        #region EmailContent CRUD

        public List<EmailContentModel> GetAllEmailContents()
        {
            var query = "SELECT * FROM emailcontent_backup";
            var results = Query(query);
            return results.Select(r => new EmailContentModel
            {
                Id = Convert.ToInt64(r["Id"]),
                Name = r["Name"]?.ToString(),
                Tittle = r["Tittle"]?.ToString(),
                Greetings = r["Greetings"]?.ToString(),
                MainText = r["MainText"]?.ToString(),
                SecondaryText = r["SecondaryText"]?.ToString(),
                AuxiliarText = r["AuxiliarText"]?.ToString(),
                ButtonText = r["ButtonText"]?.ToString(),
                Link = r["Link"]?.ToString(),
                UpdateBy = r["UpdateBy"] != null ? Convert.ToInt32(r["UpdateBy"]) : (int?)null,
                UpdateOn = r["UpdateOn"] != null ? Convert.ToDateTime(r["UpdateOn"]) : (DateTime?)null
            }).ToList();
        }

        public EmailContentModel GetEmailContentById(long id)
        {
            var query = "SELECT * FROM emailcontent_backup WHERE Id = @Id";
            var result = Query(query, new MySqlParameter("@Id", id)).FirstOrDefault();
            if (result == null) return null;

            return new EmailContentModel
            {
                Id = Convert.ToInt64(result["Id"]),
                Name = result["Name"]?.ToString(),
                Tittle = result["Tittle"]?.ToString(),
                Greetings = result["Greetings"]?.ToString(),
                MainText = result["MainText"]?.ToString(),
                SecondaryText = result["SecondaryText"]?.ToString(),
                AuxiliarText = result["AuxiliarText"]?.ToString(),
                ButtonText = result["ButtonText"]?.ToString(),
                Link = result["Link"]?.ToString(),
                UpdateBy = result["UpdateBy"] != null ? Convert.ToInt32(result["UpdateBy"]) : (int?)null,
                UpdateOn = result["UpdateOn"] != null ? Convert.ToDateTime(result["UpdateOn"]) : (DateTime?)null
            };
        }

        public long CreateEmailContent(EmailContentModel model)
        {
            var query = "INSERT INTO emailcontent_backup (Id, Name, Tittle, Greetings, MainText, SecondaryText, AuxiliarText, ButtonText, Link) VALUES (@Id, @Name, @Tittle, @Greetings, @MainText, @SecondaryText, @AuxiliarText, @ButtonText, @Link)";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Tittle", model.Tittle ?? ""),
                new MySqlParameter("@Greetings", model.Greetings ?? ""),
                new MySqlParameter("@MainText", model.MainText ?? ""),
                new MySqlParameter("@SecondaryText", model.SecondaryText ?? ""),
                new MySqlParameter("@AuxiliarText", model.AuxiliarText ?? ""),
                new MySqlParameter("@ButtonText", model.ButtonText ?? ""),
                new MySqlParameter("@Link", model.Link ?? "")
            );
            return model.Id;
        }

        public void UpdateEmailContent(EmailContentModel model)
        {
            var query = "UPDATE emailcontent_backup SET Name = @Name, Tittle = @Tittle, Greetings = @Greetings, MainText = @MainText, SecondaryText = @SecondaryText, AuxiliarText = @AuxiliarText, ButtonText = @ButtonText, Link = @Link, UpdateBy = @UpdateBy, UpdateOn = @UpdateOn WHERE Id = @Id";
            ExecuteNonQuery(query,
                new MySqlParameter("@Id", model.Id),
                new MySqlParameter("@Name", model.Name ?? ""),
                new MySqlParameter("@Tittle", model.Tittle ?? ""),
                new MySqlParameter("@Greetings", model.Greetings ?? ""),
                new MySqlParameter("@MainText", model.MainText ?? ""),
                new MySqlParameter("@SecondaryText", model.SecondaryText ?? ""),
                new MySqlParameter("@AuxiliarText", model.AuxiliarText ?? ""),
                new MySqlParameter("@ButtonText", model.ButtonText ?? ""),
                new MySqlParameter("@Link", model.Link ?? ""),
                new MySqlParameter("@UpdateBy", model.UpdateBy ?? 0),
                new MySqlParameter("@UpdateOn", model.UpdateOn ?? DateTime.Now)
            );
        }

        public void DeleteEmailContent(long id)
        {
            var query = "DELETE FROM emailcontent_backup WHERE Id = @Id";
            ExecuteNonQuery(query, new MySqlParameter("@Id", id));
        }

        #endregion

        private static void LogDebug(string message)
        {
            try
            {
                string[] logDirCandidates = new string[]
                {
                    SafeMapPath("~/logs"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "logs"),
                    Path.Combine(Path.GetTempPath(), "SatoPlusLogs")
                };

                foreach (var logDir in logDirCandidates)
                {
                    if (string.IsNullOrEmpty(logDir)) continue;
                    try
                    {
                        if (!Directory.Exists(logDir))
                            Directory.CreateDirectory(logDir);
                        File.AppendAllText(Path.Combine(logDir, "database_debug.log"), $"[{DateTime.UtcNow:o}] {message}\n");
                        return;
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static string SafeMapPath(string path)
        {
            try
            {
                return HostingEnvironment.MapPath(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
