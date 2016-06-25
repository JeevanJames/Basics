using System.Collections.Generic;

using Basics.Models;

using Dapper;

using Xunit;

namespace Basics.Data.Dapper.SqlServer.Tests
{
    public class QueryBuilderTests
    {
        [Theory]
        [InlineData("SELECT [[[Id], [Name]]] FROM [dbo].[People] WHERE [Name] LIKE @Name", "SELECT [Id], [Name] FROM [dbo].[People] WHERE [Name] LIKE @Name")]
        public void Basic_queries(string query, string expectedQuery)
        {
            var @params = new DynamicParameters();
            @params.Set("@Name", "%Jeevan%");
            SearchQueryBuilder builder = SearchQueryBuilder.Create(query);
            SearchQuery searchQuery = builder.Build(null, @params);

            Assert.Equal(expectedQuery, searchQuery.DataQuery);
            Assert.False(searchQuery.HasCountQuery);
            Assert.Null(searchQuery.CountQuery);
        }

        [Theory]
        [InlineData("SELECT [[*]] FROM MyTable", "SELECT * FROM MyTable")]
        [InlineData("SELECT [[Field1, Field2, Field3]] FROM MyTable", "SELECT Field1, Field2, Field3 FROM MyTable")]
        public void Basic_queries_without_criteria(string baseQuery, string expectedQuery)
        {
            SearchQueryBuilder builder = SearchQueryBuilder.Create(baseQuery, false);
            SearchQuery searchQuery = builder.Build(null);

            Assert.Equal(expectedQuery, searchQuery.DataQuery);
            Assert.False(searchQuery.HasCountQuery);
            Assert.Null(searchQuery.CountQuery);
        }

        [Theory(Skip = "Temporarily enabling appveyor CI")]
        [MemberData("WithPaginationQuery")]
        public void With_pagination(string baseQuery, string expectedDataQuery, string expectedCountQuery)
        {
            SearchQueryBuilder builder = SearchQueryBuilder.Create(baseQuery, false);
            SearchQuery searchQuery = builder.Build(new MyCriteria { StartRecord = 5, RecordCount = 10 });

            Assert.Equal(expectedDataQuery, searchQuery.DataQuery);
            Assert.True(searchQuery.HasCountQuery);
            Assert.Equal(expectedCountQuery, searchQuery.CountQuery);
        }

        public static IEnumerable<object[]> WithPaginationQuery
        {
            get
            {
                yield return new object[] {
                    "SELECT [[*]] FROM MyTable",
                    "SELECT * FROM MyTable",
                    "SELECT COUNT(*) FROM MyTable"
                };
            }
        }

        [Fact(Skip = "Temporarily enabling appveyor CI")]
        public void Query_constructed_correctly()
        {
            SearchQueryBuilder builder = SearchQueryBuilder.Create("SELECT [[*]] FROM MyTable WHERE Active IS NULL",
                true);
            var criteria = new MyCriteria()
                .Where(MyField.Name).IsEqualTo("Jeevan") as MyCriteria;
            SearchQuery searchQuery = builder.Build(criteria);
            Assert.Equal("SELECT * FROM MyTable WHERE Active IS NULL AND Name = @p0", searchQuery.DataQuery);
        }
    }

    public sealed class BasicCriteria : SearchCriteria
    {
        public string Name { get; set; }
    }

    public sealed class MyCriteria : SearchCriteria<MyField, MyField>
    {
    }

    public enum MyField
    {
        Name,
        Address
    }
}
