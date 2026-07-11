using Rocket.Core.Logging;
using System;
using MySqlConnector;

namespace fr34kyn01535.Uconomy
{
    /// <summary>
    /// Manages all MySQL database operations for Uconomy,
    /// including schema validation, balance retrieval, and balance updates.
    /// </summary>
    public class DatabaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class
        /// and ensures the database schema exists.
        /// </summary>
        internal DatabaseManager()
        {
            _ = new I18N.West.CP1250(); //Workaround for database encoding issues with mono
            CheckSchema();
        }

        /// <summary>
        /// Verifies that the required database table exists,
        /// creating it with default values if it does not.
        /// </summary>
        internal void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SHOW TABLES LIKE '" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId` varchar(32) NOT NULL,`balance` decimal(15,2) NOT NULL DEFAULT '25.00',`lastUpdated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`steamId`)) ";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /// <summary>
        /// Creates and returns a new MySQL connection using the configured database settings.
        /// </summary>
        /// <returns>A configured <see cref="MySqlConnection"/> instance, or null if creation failed.</returns>
        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (Uconomy.Instance.Configuration.Instance.DatabasePort == 0) Uconomy.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(
                    $"SERVER={Uconomy.Instance.Configuration.Instance.DatabaseAddress};DATABASE={Uconomy.Instance.Configuration.Instance.DatabaseName};UID={Uconomy.Instance.Configuration.Instance.DatabaseUsername};PASSWORD={Uconomy.Instance.Configuration.Instance.DatabasePassword};PORT={Uconomy.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        /// <summary>
        /// Retrieves the current balance for the specified account.
        /// </summary>
        /// <param name="steamId">The Steam ID of the account owner.</param>
        /// <returns>The current balance, or 0 if the account does not exist.</returns>
        public decimal GetBalance(string steamId)
        {
            decimal output = 0;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT `balance` FROM `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` = '" + steamId + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) decimal.TryParse(result.ToString(), out output);
                connection.Close();
                Uconomy.Instance.OnBalanceChecked(steamId, output);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }

        /// <summary>
        /// Increases the balance of the specified account by the given amount.
        /// The amount can be negative to decrease the balance.
        /// </summary>
        /// <param name="steamId">The Steam ID of the account owner.</param>
        /// <param name="increaseBy">The amount to add to the balance (negative to subtract).</param>
        /// <returns>The updated balance after the change.</returns>
        public decimal IncreaseBalance(string steamId, decimal increaseBy)
        {
            decimal output = 0;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` SET `balance` = balance + (" + increaseBy + ") WHERE `steamId` = '" + steamId + "'; SELECT `balance` FROM `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` = '" + steamId + "'";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) decimal.TryParse(result.ToString(), out output);
                connection.Close();
                Uconomy.Instance.BalanceUpdated(steamId, increaseBy);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }
        
        /// <summary>
        /// Sets the balance of the specified account to the given value.
        /// </summary>
        /// <param name="steamId">The Steam ID of the account owner.</param>
        /// <param name="newValue">The new balance amount to set.</param>
        /// <returns>True if the balance was successfully updated; otherwise, false.</returns>
        public bool SetBalance(string steamId, decimal newValue)
        {
            int result = 0;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` SET `balance` = '" + newValue + "' WHERE `steamId` = '" + steamId + "';";
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
                Uconomy.Instance.BalanceUpdated(steamId, newValue);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return result > 0;
        }

        /// <summary>
        /// Ensures that an account exists for the given Steam ID.
        /// If no account exists, one is created with the configured initial balance.
        /// </summary>
        /// <param name="id">The Steam ID of the player to check or create an account for.</param>
        public void CheckSetupAccount(Steamworks.CSteamID id)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                int exists = 0;
                command.CommandText = "SELECT EXISTS(SELECT 1 FROM `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` ='" + id + "' LIMIT 1);";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) int.TryParse(result.ToString(), out exists);
                connection.Close();

                if (exists == 0)
                {
                    command.CommandText = "INSERT IGNORE INTO `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` (balance,steamId,lastUpdated) VALUES(" + Uconomy.Instance.Configuration.Instance.InitialBalance + ",'" + id.ToString() + "',now())";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

        }
    }
}
