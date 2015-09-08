namespace News.Services.Controllers
{
    using System.Linq;
    using System.Web.Http;

    using News.Data;
    using News.Data.Contracts;
    using News.Models;
    using News.Services.Models;

    [RoutePrefix("api/news")]
    public class NewsController : BaseApiController
    {
        public NewsController()
            : base(new NewsData(new NewsContext()))
        {
        }

        public NewsController(INewsData data)
            : base(data)
        {
        }

        [HttpGet]
        public IHttpActionResult GetNews()
        {
            var news = this.Data.News.All()
                .OrderBy(n => n.PublishDate);

            return this.Ok(news);
        }

        [HttpPost]
        public IHttpActionResult PostNews(NewsBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("No news to post.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newNews = new News
            {
                Title = model.Title,
                Content = model.Content,
                PublishDate = model.PublishDate
            };

            this.Data.News.Add(newNews);
            this.Data.SaveChanges();

            return this.Created("api/news/" + newNews.Id, newNews);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult ChangeNews([FromUri]int id, [FromBody]NewsBindingModel model)
        {
            var newsInDb = this.Data.News.All()
                .FirstOrDefault(n => n.Id == id);

            if (newsInDb == null)
            {
                return this.BadRequest("There is no news with given id.");
            }

            if (model == null)
            {
                return this.BadRequest("No parameters to update news.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            newsInDb.Title = model.Title;
            newsInDb.Content = model.Content;
            newsInDb.PublishDate = model.PublishDate;

            this.Data.News.Update(newsInDb);
            this.Data.SaveChanges();

            return this.Ok(newsInDb);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteNews(int id)
        {
            var newsInDb = this.Data.News.All()
                .FirstOrDefault(n => n.Id == id);

            if (newsInDb == null)
            {
                return this.BadRequest("There is no news with given id.");
            }
            
            this.Data.News.Delete(newsInDb);
            this.Data.SaveChanges();

            return this.Ok(string.Format("News with id '{0}'", id));
        }
    }
}