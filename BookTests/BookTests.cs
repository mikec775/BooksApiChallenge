using BookApiChallenge;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;


namespace BookTests
{
    public class BookTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BookTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        //Check if we set things up correctly

        [Fact]
        public async Task GetBooks_ShouldReturnOK()
        {

            var response = await _client.GetAsync("/api/Books");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        //Check if a book is created correctly
        [Fact]
        public async Task CreateBook_ShouldReturnCreatedAndNewBook()
        {

            var title = "test book";
            var response = await _client.PostAsJsonAsync("/api/Books", title);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdBook = await response.Content.ReadFromJsonAsync<Book>();

            //assess if data assigned correctly
            Assert.NotNull(createdBook);

            Assert.Equal(title, createdBook?.BookTitle);

            Assert.True(createdBook?.BookID > 0);

            Assert.Equal("Planned", createdBook?.PublicationStatus);

            Assert.Null(createdBook?.PublicationDate);

        }

        //Check if a book can be published correctly
        [Fact]
        public async Task PublishBook_ShouldReturnOkAndUpdatedStatus()
        {

            var createResponse = await _client.PostAsJsonAsync("/api/Books", "Published");
            createResponse.EnsureSuccessStatusCode();

            var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(createdBook);

            var bookID = createdBook.BookID;

            var ifPublished = await _client.PostAsync($"/api/Books/{bookID}/publish", null);

            Assert.Equal(HttpStatusCode.OK, ifPublished.StatusCode);

            var publishedBook = await ifPublished.Content.ReadFromJsonAsync<Book>();

            Assert.NotNull(publishedBook.PublicationDate);

            Assert.Equal("Published", publishedBook.BookTitle);

        }


        //Check if a fake book cannot be published
        [Fact]
        public async Task PublishBook_ShouldReturnNotFoundForExistingBook()
        {

            var doesntexist = 888;
            var response = await _client.PostAsync($"/api/Books/{doesntexist}/publish", null);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }

        //Check if a book cannot be published twice
        [Fact]
        public async Task PublishBook_ShouldReturnBadRequestForDuplicateBook()
        {

            var createResponse = await _client.PostAsJsonAsync("/api/Books", "Published");
            createResponse.EnsureSuccessStatusCode();

            var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(createdBook);
            var bookID = createdBook.BookID;

            var publishResponse = await _client.PostAsync($"/api/Books/{bookID}/publish", null);
            Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);

            //attempt a 2nd time
            var publishResponse2 = await _client.PostAsync($"/api/Books/{bookID}/publish", null);
            Assert.Equal(HttpStatusCode.BadRequest, publishResponse2.StatusCode);

            var errorMsg = await publishResponse2.Content.ReadAsStringAsync();
            Assert.Contains("The book is already published.", errorMsg);


        }

        //Check empty title input validation
        [Fact]
        public async Task CreateEmptyBook_ShouldReturnBadRequest()
        {

            var title = "";
            var response = await _client.PostAsJsonAsync("/api/Books", title);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);


        }

        //Check if correct data returned from getting books
        public async Task GetBooks_ShouldReturnCorrectData()
        {

            await _client.PostAsJsonAsync("/api/Books", "Test Book");
            await _client.PostAsync("/api/Books/1/publish", null);

            var response = await _client.GetAsync("/api/Books");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var booksAsList = await response.Content.ReadFromJsonAsync<List<Book>>();
            Assert.NotNull(booksAsList);
            Assert.True(booksAsList.Count >= 1);

            var book = booksAsList[0];
            Assert.NotNull(book);
            Assert.Equal("Test Book", book.BookTitle);
            Assert.Equal("Published", book.PublicationStatus);


        }
    }
}