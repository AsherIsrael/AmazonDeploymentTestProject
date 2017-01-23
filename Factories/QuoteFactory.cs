using System.Data;
using Dapper;
using QuotingDojoRedux.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace QuotingDojoRedux.Factories
{
    public class QuoteFactory : IFactory<Quote>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        
        public QuoteFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;
        }

        internal IDbConnection Connection
        {
            get { return new MySqlConnection(MySqlConfig.Value.ConnectionString); }
        }

        public void Add(Quote Item, int UserId)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = $"INSERT into quotes (Userid, QuoteContent, CreatedAt, UpdatedAt) VALUES ({UserId}, @QuoteContent, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(Query, Item);
            }
        }

        public IEnumerable<Quote> FindAll()
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = "SELECT * From quotes JOIN users ON quotes.UserId WHERE users.UserId = quotes.UserId";
                dbConnection.Open();
                IEnumerable<Quote> AllQuotes = dbConnection.Query<Quote, User, Quote>(Query, (quote, user) => { quote.Quoter = user; return quote;}, splitOn: "UserId");
                return AllQuotes;
            }
        }

        public void Delete(int QuoteId)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = $"DELETE FROM quotes WHERE QuoteId = {QuoteId}";
                dbConnection.Open();
                dbConnection.Execute(Query);
                return;
            }
        }

        public Quote GetQuoteById(int QuoteId)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = $"SELECT * FROM quotes WHERE QuoteId = {QuoteId}";
                dbConnection.Open();
                return dbConnection.QuerySingleOrDefault<Quote>(Query);
            }
        }

        public void Update(Quote Item)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = "UPDATE quotes SET QuoteContent = @QuoteContent WHERE QuoteId = @QuoteId";
                dbConnection.Open();
                dbConnection.Execute(Query, Item);
                return;
            }
        }
    }
}