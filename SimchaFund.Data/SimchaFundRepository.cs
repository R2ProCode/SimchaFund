using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class SimchaFundRepository
    {
        private string _connectionString;

        public SimchaFundRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT s.Id, s.Name, s.Date, SUM(c.Amount) AS 'Total', COUNT(c.ContributorId) AS 'AmountOfContributors'
                                    FROM Simchas s
                                    LEFT JOIN Contributions c ON c.SimchaId = s.Id
                                    GROUP BY s.Id, s.Name, s.Date
                                    ORDER BY s.Date DESC";
            connection.Open();

            var reader = command.ExecuteReader();
            var simchas = new List<Simcha>();
            while (reader.Read())
            {
                simchas.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Total = reader.GetOrNull<decimal>("Total"),
                    AmountOfContributors = reader.GetOrNull<int>("AmountOfContributors")
                });
            }
            return simchas;
        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributors";
            connection.Open();
            var reader = command.ExecuteReader();
            var contributors = new List<Contributor>();
            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    ContributorId = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                });
            }
            return contributors;
        }

        public decimal GetContributionForSimcha(int simchaId, int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT Amount FROM Contributions c
                                    WHERE c.SimchaId = @simchaId AND c.ContributorId = @contributorId";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            command.Parameters.AddWithValue("@contributorId", contributorId);
            
            connection.Open();
            var reader = command.ExecuteReader();
            decimal amount = 0;
            while (reader.Read())
            {
                amount = reader.GetOrNull<decimal>("Amount");
            }
            return amount;
        }

        public void AddSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Simchas (Name, Date) VALUES (@name, @date);
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@name", simcha.Name);
            command.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            
            simcha.Id = (int)(decimal)command.ExecuteScalar();

            foreach (var contributor in GetContributors())
            {
                if (contributor.AlwaysInclude)
                {
                    AddContributionForSimcha(simcha.Id, contributor.ContributorId, 5);
                }
            }
        }

        public void AddContributor(Contributor contributor, decimal initialDeposit)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributors (FirstName, LastName, CellNumber, Date, AlwaysInclude) 
                                    VALUES (@firstName, @lastName, @cellNumber, @date, @alwaysInclude);
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            command.Parameters.AddWithValue("@date", contributor.Date);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            var contributorId = (int)(decimal)command.ExecuteScalar();
                        
            var depositRecord = new Deposit
            {
                ContributorId = contributorId,
                Amount = initialDeposit,
                Date = contributor.Date
            };
            AddDepositForContributor(depositRecord);
        }

        public void AddDepositForContributor(Deposit deposit)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Deposits (ContributorId, Amount, Date) VALUES (@contributorId, @amount, @date)";
            command.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            command.Parameters.AddWithValue("@amount", deposit.Amount);
            command.Parameters.AddWithValue("@date", deposit.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void AddContributionForSimcha(int simchaId, int contributorId, decimal amount)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributions (SimchaId, ContributorId, Amount) VALUES (@simchaId, @contributorId, @amount)";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            command.Parameters.AddWithValue("@contributorId", contributorId);
            command.Parameters.AddWithValue("@amount", amount);
            connection.Open();
            command.ExecuteNonQuery();
        }        

        public void UpdateContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Contributors 
                                    SET FirstName = @firstName, LastName = @lastName, CellNumber = @cellNumber, AlwaysInclude = @alwaysInclude
                                    WHERE Id = @id";

            connection.Open();
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            command.Parameters.AddWithValue("@id", contributor.ContributorId);
            command.ExecuteNonQuery();
        }

        public void UpdateContributionsForSimcha(int simchaId, List<Contributor> contributors)
                {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Contributions WHERE SimchaId = @simchaId";
            
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            command.ExecuteNonQuery();

            foreach (var contributor in contributors)
            {
                AddContributionForSimcha(simchaId, contributor.ContributorId, contributor.Amount.Value);
            }
        }

        public List<History> GetHistoryForContributor(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT
                                        s.Date,
                                        c.Amount,
                                        s.Name AS Action
                                    FROM Contributions c
                                    JOIN Simchas s
                                        ON c.SimchaId = s.Id
                                    WHERE c.ContributorId = @contributorId

                                    UNION ALL

                                    SELECT
                                        d.Date,
                                        d.Amount,
                                        'Deposit' AS Action
                                    FROM Deposits d
                                    WHERE d.ContributorId = @contributorId

                                    ORDER BY Date ASC";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            var reader = command.ExecuteReader();

            var records = new List<History>();
            while (reader.Read())
            {
                records.Add(new History
                {
                    Action = (string)reader["Action"],
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]
                });
            }
            return records;
        }

        public decimal GetTotalForSimcha(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT SUM(Amount) AS 'Total' FROM Simchas s
                                    JOIN Contributions c
                                    ON c.SimchaId = s.Id
                                    WHERE s.Id = @id";
            command.Parameters.AddWithValue("@id", id);
            return (decimal)command.ExecuteScalar();
        }

        public int GetContributorCount()
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM Contributors";
            connection.Open();
            return (int)command.ExecuteScalar();
        }

        public decimal GetContributorDeposits(int contributorId)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT SUM(d.Amount)
                                    FROM Deposits d
                                    JOIN Contributors cs
                                    ON d.ContributorId = cs.Id
                                    WHERE cs.Id = @contributorId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            return (decimal)command.ExecuteScalar();
        }
        public decimal GetContributorContributions(int contributorId)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT SUM(c.Amount) AS 'Balance'
                                    FROM Contributions c
                                    JOIN Contributors cs
                                    ON c.ContributorId = cs.Id
                                    WHERE c.ContributorId = @contributorId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            var reader = command.ExecuteReader();
            decimal contributions = 0;
            while (reader.Read())
            {
                contributions = reader.GetOrNull<decimal>("Balance");
            }
            return contributions;
        }

        public decimal GetContributorBalance(int contributorId)
        {
            return GetContributorDeposits(contributorId) - GetContributorContributions(contributorId);
        }

        public string GetSimchaNameById(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Name FROM Simchas
                                    WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (string)command.ExecuteScalar();
        }

        public decimal GetTotalContributions()
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT SUM(Amount) FROM Contributions";
            connection.Open();
            return (decimal)command.ExecuteScalar();
        }

        public string GetContributorNameById(int contributorId)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT FirstName + ' ' + LastName AS Name FROM Contributors WHERE Id = @contributorId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            return (string)command.ExecuteScalar();
        }
    }
}
